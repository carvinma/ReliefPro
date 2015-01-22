using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.IO;
using NHibernate;

using ReliefProMain.Commands;
using ReliefProMain.Models;
using ReliefProMain.View;
using ReliefProModel.Drums;
using UOMLib;
using ProII;
using ReliefProDAL;
using ReliefProModel;
using ReliefProCommon.CommonLib;
using ReliefProMain.View.DrumFires;
using ReliefProMain.View.StorageTanks;
using ReliefProMain.View.HXs;
using ReliefProMain.ViewModel.HXs;
using ReliefProModel.HXs;
using ReliefProDAL.HXs;
using ReliefProBLL;
using ReliefProDAL.Drums;

namespace ReliefProMain.ViewModel.Drums
{
    public class DrumFireVM : ViewModelBase
    {
        public ICommand InputDataCMD { get; set; }
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        public ICommand DrumSizeCMD { get; set; }
        public SourceFile SourceFile { get; set; }
        public string FileFullPath { get; set; }
        private string selectedHeatInputModel = "API 521";
        public string SelectedHeatInputModel
        {
            get { return selectedHeatInputModel; }
            set
            {
                selectedHeatInputModel = value;
                if (value == "API 521")
                    EnabledEquipmentExist = true;
                else
                    EnabledEquipmentExist = false;

                OnPropertyChanged("SelectedHeatInputModel");
            }
        }
        private bool enabledEquipmentExist = true;
        public bool EnabledEquipmentExist
        {
            get { return enabledEquipmentExist; }
            set
            {
                enabledEquipmentExist = value;
                OnPropertyChanged("EnabledEquipmentExist");
            }
        }
        public List<string> lstHeatInputModel { get; set; }
        public DrumFireModel model { get; set; }
        private ISession SessionProtectedSystem;
        private ISession SessionPlant;
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }

        private double reliefPressure;
        private double fireCriticalPressure;
        private double fireCriticalTemperature;
        private int ScenarioID;
        private DrumFireBLL fireBLL;
        private DrumFireFluid fireFluidModel;
        private DrumSize sizeModel;
        private DrumSizeVM tmpVM;

        private HXFireSizeVM tmpHxFireSizeVM;
        private HXFireSize hxFireSize;
        private AirCooledHXFireSizeVM tmpAirCooledHXFireSizeVM;
        private AirCooledHXFireSize airCooledHXFireSize;
        private int FireType;
        private SourceFile SourceFileInfo;
        private UOMLib.UOMEnum uomEnum;

        private CustomStream normalVapor;
        private Drum drum=new Drum();
        private HeatExchanger hx=new HeatExchanger();


        private CustomStream molVapor;
        private double molLatent;

        private PSV psv = new PSV();
        CustomStream minVFStream = new CustomStream();
        List<CustomStream> lstFeeds = new List<CustomStream>();
        CustomStream HXProduct = new CustomStream();
        CustomStream mixFeedStream = new CustomStream();
        List<string> mixFeedStreamName = new List<string>();
        string EqName = string.Empty;
        string HeatMethod = string.Empty;
        int ErrorType = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ScenarioID"></param>
        /// <param name="przFile"></param>
        /// <param name="version"></param>
        /// <param name="SessionPS"></param>
        /// <param name="SessionPF"></param>
        /// <param name="dirPlant"></param>
        /// <param name="dirProtectedSystem"></param>
        /// <param name="FireType">0-DrumSize,1-TankSize 2 HX  Shell-Tube  3 Air Cooled</param>
        public DrumFireVM(int ScenarioID, SourceFile sourceFileInfo, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem, int FireType = 0)
        {
            this.FireType = FireType;
            this.ScenarioID = ScenarioID;
            this.SessionProtectedSystem = SessionPS;
            this.SessionPlant = SessionPF;
            SourceFileInfo = sourceFileInfo;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            FileFullPath = DirPlant + @"\" + sourceFileInfo.FileNameNoExt + @"\" + sourceFileInfo.FileName;
            InputDataCMD = new DelegateCommand<object>(OpenFluidWin);
            CalcCMD = new DelegateCommand<object>(Calc);
            OKCMD = new DelegateCommand<object>(Save);
            DrumSizeCMD = new DelegateCommand<object>(OpenDrumSize);
            PSVDAL psvdal = new PSVDAL();
            psv = psvdal.GetModel(SessionProtectedSystem);
            //如果是drum的话，获得当前drum的信息
            
            lstHeatInputModel = new List<string>();
            lstHeatInputModel.Add("API 521");
            if (FireType == 1)
            {
                lstHeatInputModel.Add("API 2000");
                SelectedHeatInputModel = "API 2000";
            }

            //获得drumfirecalc的数据
            fireBLL = new DrumFireBLL(SessionPS, SessionPF);
            var fireModel = fireBLL.GetDrumFireModel(ScenarioID);
            fireModel = fireBLL.ReadConvertModel(fireModel);
            if (!string.IsNullOrEmpty(fireModel.HeatInputModel))
                SelectedHeatInputModel = fireModel.HeatInputModel;

            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            model = new DrumFireModel(fireModel);
            fireModel.ScenarioID = ScenarioID;
            model.WettedAreaUnit = uomEnum.UserArea;
            model.LatentHeatUnit = uomEnum.UserSpecificEnthalpy;
            model.LatentHeat2Unit = uomEnum.UserSpecificEnthalpy;
            model.CrackingHeatUnit = uomEnum.UserSpecificEnthalpy;

            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefPressureUnit = uomEnum.UserPressure;
            model.DesignPressureUnit = uomEnum.UserPressure;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
            model.NoneAllGas = !model.AllGas;

            reliefPressure = ScenarioFireReliefPressure(SessionPS);

            CustomStreamDAL csdal = new CustomStreamDAL();
            if (FireType == 0)
            {
                DrumDAL drumdal = new DrumDAL();
                drum = drumdal.GetModel(SessionProtectedSystem);
                EqName = drum.DrumName;
                DrumFireFluidBLL fluidBll = new DrumFireFluidBLL(SessionPS, SessionPF);
                fireFluidModel = fluidBll.GetFireFluidModel(model.dbmodel.ID,FireType);
                                
                IList<CustomStream> products = csdal.GetAllList(SessionPS, true);
                foreach (CustomStream cs in products)
                {
                    if (cs.VaporFraction == 1)
                    {
                        normalVapor = cs;
                        break;
                    }
                }
                if (model.FluidType == 0)
                {
                    model.FluidType = GetDrumFluidType();
                }
                else if (model.FluidType == 3 || model.FluidType == 4)
                {
                    molLatent = model.LatentHeat;
                }

                if (model.dbmodel.ID == 0 && model.FluidType == 1)
                {
                    fireFluidModel.NormalCpCv = normalVapor.BulkCPCVRatio;
                    fireFluidModel.GasVaporMW = normalVapor.BulkMwOfPhase;
                    fireFluidModel.TW = UnitConvert.Convert(UOMEnum.Temperature, uomEnum.UserTemperature, 593);
                }

                if (sizeModel == null)
                {
                    DrumSizeBLL sizeBll = new DrumSizeBLL(SessionProtectedSystem, SessionPlant);
                    sizeModel = sizeBll.GetSizeModel(model.dbmodel.ID);
                }
                if (sizeModel != null)
                {                    
                    double Area = Algorithm.GetDrumArea(sizeModel.Orientation, sizeModel.HeadType, sizeModel.Elevation, sizeModel.Diameter, sizeModel.Length, sizeModel.NormalLiquidLevel, sizeModel.BootHeight, sizeModel.BootDiameter);
                    model.WettedArea = Area;
                }
                    
            }
            else if (FireType == 2)
            {
                HeatExchangerDAL hxdal = new HeatExchangerDAL();
                hx = hxdal.GetModel(SessionProtectedSystem);
                EqName = hx.HXName;
                DrumFireFluidBLL fluidBll = new DrumFireFluidBLL(SessionPS, SessionPF);
                fireFluidModel = fluidBll.GetFireFluidModel(model.dbmodel.ID,FireType);               
                if (psv != null)
                {
                    if (psv.LocationDescription == "Shell")
                    {
                        if (hx.ShellFeedStreams == hx.ColdInlet)
                        {
                            normalVapor = csdal.GetModel(SessionPS, hx.ColdOutlet);
                        }
                        else
                        {
                            normalVapor = csdal.GetModel(SessionPS, hx.HotOutlet);
                        }
                    }
                    else
                    {
                        if (hx.TubeFeedStreams == hx.ColdInlet)
                        {
                            normalVapor = csdal.GetModel(SessionPS, hx.ColdOutlet);
                        }
                        else
                        {
                            normalVapor = csdal.GetModel(SessionPS, hx.HotOutlet);
                        }
                    }
                }
                GetMix();
                if (model.FluidType == 0)
                {
                    model.FluidType = GetHXFluidType();
                }
                else if (model.FluidType == 3 || model.FluidType == 4)
                {
                    molLatent = model.LatentHeat;
                }

                if (model.dbmodel.ID == 0 && model.FluidType == 1)
                {
                    fireFluidModel.NormalCpCv = normalVapor.BulkCPCVRatio;
                    fireFluidModel.GasVaporMW = normalVapor.BulkMwOfPhase;
                    fireFluidModel.TW = UnitConvert.Convert(UOMEnum.Temperature, uomEnum.UserTemperature, 593);
                }

                if (hxFireSize == null)
                {
                    HXBLL hxbll = new HXBLL(SessionPS,SessionPF);
                    hxFireSize = hxbll.GetHXFireSizeModel(model.dbmodel.ScenarioID);

                }
                if (hxFireSize != null)
                {
                    double Area = Algorithm.GetHXArea(hxFireSize.ExposedToFire, hxFireSize.Type, hxFireSize.Length, hxFireSize.Elevation, hxFireSize.OD);
                    model.WettedArea = Area;
                    fireFluidModel.ExposedVesse = Area;
                    
                }

                
            }


        }
        private void WriteConvertModel()
        {
            if (model.ReliefLoad < 0)
                model.ReliefLoad = 0;
            model.dbmodel.WettedArea = UnitConvert.Convert(model.WettedAreaUnit, UOMLib.UOMEnum.Area.ToString(), model.WettedArea);
            model.dbmodel.LatentHeat = UnitConvert.Convert(model.LatentHeatUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.LatentHeat);
            model.dbmodel.LatentHeat2 = UnitConvert.Convert(model.LatentHeat2Unit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.LatentHeat2);
            model.dbmodel.CrackingHeat = UnitConvert.Convert(model.CrackingHeatUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.CrackingHeat);
            model.dbmodel.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.DesignPressure = UnitConvert.Convert(model.DesignPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.DesignPressure);

            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefCpCv = model.ReliefCpCv;
            model.dbmodel.ReliefZ = model.ReliefZ;
            model.dbmodel.HeavyOilFluid = model.HeavyOilFluid;
            model.dbmodel.AllGas = model.AllGas;
            model.dbmodel.EquipmentExist = model.EquipmentExist;
            model.dbmodel.HeatInputModel = selectedHeatInputModel;
            model.dbmodel.IsCalc = model.IsCalc;
            model.dbmodel.FluidType = model.FluidType;

        }
        private void OpenDrumSize(object obj)
        {
            if (FireType == 0)
            {
                DrumSizeView win = new DrumSizeView();
                win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                DrumSizeVM vm = new DrumSizeVM(model.dbmodel.ID, SessionProtectedSystem, SessionPlant);
                if (tmpVM != null) vm = tmpVM;
                win.DataContext = vm;
                if (win.ShowDialog() == true)
                {
                    tmpVM = vm;
                    sizeModel = vm.model.dbmodel;
                    double Area = 0;
                    if (sizeModel == null)
                    {
                        DrumSizeBLL sizeBll = new DrumSizeBLL(SessionProtectedSystem, SessionPlant);
                        sizeModel = sizeBll.GetSizeModel(model.dbmodel.ID);
                    }
                    if (sizeModel != null)
                        Area = Algorithm.GetDrumArea(sizeModel.Orientation, sizeModel.HeadType, sizeModel.Elevation, sizeModel.Diameter, sizeModel.Length, sizeModel.NormalLiquidLevel, sizeModel.BootHeight, sizeModel.BootDiameter);
                    if (model.AllGas)
                    {
                        double NLL = 0;
                        if (sizeModel.Orientation == "Vertical")
                        {
                            NLL = sizeModel.Length;
                        }
                        else if (sizeModel.Orientation == "Horizontal")
                        {
                            NLL = sizeModel.Diameter;
                        }
                        else
                        {
                            NLL = sizeModel.Diameter;
                        }
                        Area = Algorithm.GetDrumArea(sizeModel.Orientation, sizeModel.HeadType, sizeModel.Elevation, sizeModel.Diameter, sizeModel.Length, NLL, sizeModel.BootHeight, sizeModel.BootDiameter);            
                    }
                    model.WettedArea = Area;
                    if (fireFluidModel != null)
                        fireFluidModel.ExposedVesse = Area;
                }
            }
            else if (FireType == 1)
            {
                StorageTankSizeView win = new StorageTankSizeView();
                win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                DrumSizeVM vm = new DrumSizeVM(model.dbmodel.ID, SessionProtectedSystem, SessionPlant);
                if (tmpVM != null) vm = tmpVM;
                win.DataContext = vm;
                if (win.ShowDialog() == true)
                {
                    tmpVM = vm;
                    sizeModel = vm.model.dbmodel;
                    double Area = 0;
                    if (sizeModel == null)
                    {
                        DrumSizeBLL sizeBll = new DrumSizeBLL(SessionProtectedSystem, SessionPlant);
                        sizeModel = sizeBll.GetSizeModel(model.dbmodel.ID);
                    }
                    if (sizeModel != null)
                        Area = Algorithm.GetDrumArea(sizeModel.Orientation, sizeModel.HeadType, sizeModel.Elevation, sizeModel.Diameter, sizeModel.Length, sizeModel.NormalLiquidLevel, sizeModel.BootHeight, sizeModel.BootDiameter);
                    model.WettedArea = Area;
                    if (fireFluidModel != null)
                        fireFluidModel.ExposedVesse = Area;
                }
            }
            else if (FireType == 2)
            {
                HXFireSizeView win = new HXFireSizeView();
                win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                HXFireSizeVM vm = new HXFireSizeVM(model.dbmodel.ScenarioID, SessionProtectedSystem, SessionPlant);
                if (tmpHxFireSizeVM != null) vm = tmpHxFireSizeVM;
                win.DataContext = vm;
                if (win.ShowDialog() == true)
                {
                    tmpHxFireSizeVM = vm;
                    hxFireSize = vm.model.dbmodel;
                    double Area = 0;
                    if (hxFireSize == null)
                    {
                        HXFireSizeDAL hxFireSizeDAL = new HXFireSizeDAL();
                        hxFireSize = hxFireSizeDAL.GetModel(model.dbmodel.ID, SessionProtectedSystem);

                    }
                    if (hxFireSize != null)
                        Area = Algorithm.GetHXArea(hxFireSize.ExposedToFire, hxFireSize.Type, hxFireSize.Length,hxFireSize.Elevation, hxFireSize.OD);
                    model.WettedArea = Area;
                    if (fireFluidModel != null)
                        fireFluidModel.ExposedVesse = Area;
                }
            }
            else if (FireType == 3)
            {
                AirCooledHXFireSizeView win = new AirCooledHXFireSizeView();
                win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                AirCooledHXFireSizeVM vm = new AirCooledHXFireSizeVM(model.dbmodel.ScenarioID, SourceFileInfo, SessionProtectedSystem, SessionPlant, DirPlant, DirProtectedSystem);
                if (tmpAirCooledHXFireSizeVM != null) vm = tmpAirCooledHXFireSizeVM;
                win.DataContext = vm;
                if (win.ShowDialog() == true)
                {
                    tmpAirCooledHXFireSizeVM = vm;
                    airCooledHXFireSize = vm.model.dbmodel;
                    double Area = 0;
                    if (airCooledHXFireSize == null)
                    {
                        AirCooledHXFireSizeDAL airCooledHXFireSizeDAL = new AirCooledHXFireSizeDAL();
                        airCooledHXFireSize = airCooledHXFireSizeDAL.GetModel(model.dbmodel.ID, SessionProtectedSystem);

                    }
                    if (airCooledHXFireSize != null)
                    {
                        Area = airCooledHXFireSize.WettedBundle * (1 + airCooledHXFireSize.PipingContingency / 100);
                    }
                    model.WettedArea = Area;
                    if (fireFluidModel != null)
                        fireFluidModel.ExposedVesse = Area;
                }
            }
        }

        /// <summary>
        /// 全气相的操作
        /// </summary>
        /// <param name="obj"></param>
        private void OpenFluidWin(object obj)
        {
            if (model.FluidType==1||model.FluidType==2)
            {                
                OpenGasWin(obj);
            }
            else 
            {
                CheckLatent();
                OpenLiquidWin(obj);
            }
            
            
        }
        private void CheckLatent()
        {
            if (model.dbmodel.ID == 0 && model.IsCalc)
            {
                if (model.FluidType == 3)
                {
                    double pr = reliefPressure / fireCriticalPressure;
                    if (pr > 0.9)
                    {
                        model.LatentHeat = molLatent;
                        //MessageBox.Show("Relief Condition is near critical , Latent heat is set to default or user defined value.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        model.LatentHeat2 = UnitConvert.Convert(UOMEnum.SpecificEnthalpy, model.LatentHeatUnit, 115);
                        model.IsCalc = false;
                    }
                    else
                    {
                        if (molLatent < 115)
                        {
                            //MessageBox.Show("Calculated latent heat is less than bound value and default  or user defined value is used.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                            model.LatentHeat2 = UnitConvert.Convert(UOMEnum.SpecificEnthalpy, model.LatentHeatUnit, 115);
                            model.IsCalc = false;
                        } 
                        else
                        {
                            model.LatentHeat = molLatent;
                            model.IsCalc = true;
                        }

                    }
                    
                }
                else if (model.FluidType == 4)
                {
                    if (molLatent < 115)
                    {
                        //MessageBox.Show("Calculated latent heat is less than bound value and default  or user defined value is used.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        model.LatentHeat2 = UnitConvert.Convert(UOMEnum.SpecificEnthalpy, model.LatentHeatUnit, 115);
                        model.IsCalc = false;
                    }
                    model.LatentHeat = molLatent;
                    if (molLatent >= 115)
                    {
                        model.IsCalc = true;
                    }

                }
                else if (model.FluidType == 5)
                {
                    //MessageBox.Show("Flash for latent heat failed and default 46 KJ/Kg is used as latent heat.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    model.LatentHeat2 = UnitConvert.Convert(UOMEnum.SpecificEnthalpy, model.LatentHeatUnit, 46);
                    model.IsCalc = false;
                }
                

            }
        }

        /// <summary>
        /// 全气相的操作
        /// </summary>
        /// <param name="obj"></param>
        private void OpenGasWin(object obj)
        {
            UnitConvert uc = new UnitConvert();
            DrumFireFluidView win = new DrumFireFluidView();
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            DrumFireFluidVM vm = new DrumFireFluidVM(model.dbmodel.ID, SessionProtectedSystem, SessionPlant,FireType);
            win.DataContext = vm;
            if (model.WettedArea == 0)
            {
                //MessageBox.Show("Area must be greater than zero.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            /*
            if (sizeModel == null)
            {
                DrumSizeBLL sizeBll = new DrumSizeBLL(SessionProtectedSystem, SessionPlant);
                sizeModel = sizeBll.GetSizeModel(model.dbmodel.ID);
            }
            double NLL = 0;
            if (sizeModel.Orientation == "Vertical")
            {
                NLL = sizeModel.Length;
            }
            else if (sizeModel.Orientation == "Horizontal")
            {
                NLL = sizeModel.Diameter;
            }
            else
            {
                NLL = sizeModel.Diameter;
            }

            if (model.dbmodel.ID == 0)
            {
                model.WettedArea = Algorithm.GetDrumArea(sizeModel.Orientation, sizeModel.HeadType, sizeModel.Elevation, sizeModel.Diameter, sizeModel.Length, NLL, sizeModel.BootHeight, sizeModel.BootDiameter);
                vm.model.Vessel = model.WettedArea;
            }
             */
            vm.model.NormalCpCv = fireFluidModel.NormalCpCv;
            vm.model.VaporMW = fireFluidModel.GasVaporMW;
            vm.model.TW =fireFluidModel.TW;
            vm.model.Vessel =model.WettedArea;

            vm.model.TW = UnitConvert.Convert(UOMEnum.Temperature, vm.model.TWUnit, vm.model.TW);
            vm.model.Vessel = UnitConvert.Convert(UOMEnum.Area, vm.model.VesselUnit, vm.model.Vessel);


            if (win.ShowDialog() == true)
            {
                fireFluidModel = vm.model.dbmodel;
                
                //model.AllGas = true;
            }
        }

        /// <summary>
        /// 夜相的操作
        /// </summary>
        /// <param name="obj"></param>
        private void OpenLiquidWin(object obj)
        {
            UnitConvert uc = new UnitConvert();
            DrumMutiPhaseView win = new DrumMutiPhaseView();
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            DrumMutiPhaseVM vm = new DrumMutiPhaseVM(model, SessionPlant, SessionProtectedSystem);
            win.DataContext = vm;
            if (model.WettedArea == 0)
            {
                MessageBox.Show("Area must be greater than zero.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            /*
            if (sizeModel == null)
            {                
                DrumSizeBLL sizeBll = new DrumSizeBLL(SessionProtectedSystem, SessionPlant);
                sizeModel = sizeBll.GetSizeModel(model.dbmodel.ID);
            }
            double NLL = 0;
            if (sizeModel.Orientation == "Vertical")
            {
                NLL = sizeModel.Length;
            }
            else if (sizeModel.Orientation == "Horizontal")
            {
                NLL = sizeModel.Diameter;
            }
            else
            {
                NLL = sizeModel.Diameter;
            }
            if (model.dbmodel.ID == 0)
            {
                model.WettedArea = Algorithm.GetDrumArea(sizeModel.Orientation, sizeModel.HeadType, sizeModel.Elevation, sizeModel.Diameter, sizeModel.Length, NLL, sizeModel.BootHeight, sizeModel.BootDiameter);
                
            }*/
            if (win.ShowDialog() == true)
            {
                model = vm.model;               
            }
        }

        private void CalcDrumArea()
        {
            if (sizeModel == null)
            {
                DrumSizeBLL sizeBll = new DrumSizeBLL(SessionProtectedSystem, SessionPlant);
                sizeModel = sizeBll.GetSizeModel(model.dbmodel.ID);
            }
            double NLL = 0;
            if (sizeModel.Orientation == "Vertical")
            {
                NLL = sizeModel.Length;
            }
            else if (sizeModel.Orientation == "Horizontal")
            {
                NLL = sizeModel.Diameter;
            }
            else
            {
                NLL = sizeModel.Diameter;
            }

            if (model.dbmodel.ID == 0)
            {
                model.WettedArea = Algorithm.GetDrumArea(sizeModel.Orientation, sizeModel.HeadType, sizeModel.Elevation, sizeModel.Diameter, sizeModel.Length, NLL, sizeModel.BootHeight, sizeModel.BootDiameter);
                model.WettedArea = UnitConvert.Convert(UOMEnum.Area, model.WettedAreaUnit, model.WettedArea);
            }
        }
      
        private void CalcDrum()
        {            
            model.ReliefPressure = reliefPressure;
            double Qfire = 0;
            double Area = model.WettedArea;
            //求出面积---你查看下把durmsize的 数据传进来。
            if (sizeModel == null)
            {
                DrumSizeBLL sizeBll = new DrumSizeBLL(SessionProtectedSystem, SessionPlant);
                sizeModel = sizeBll.GetSizeModel(model.dbmodel.ID);
            }
            if (sizeModel != null)
                Area = Algorithm.GetDrumArea(sizeModel.Orientation, sizeModel.HeadType, sizeModel.Elevation, sizeModel.Diameter, sizeModel.Length, sizeModel.NormalLiquidLevel, sizeModel.BootHeight, sizeModel.BootDiameter);
            //计算Qfire
            double C1 = 0;
            if (model.EquipmentExist)
            {
                C1 = 43200;
            }
            else
            {
                C1 = 70900;
            }
            Qfire = Algorithm.GetQ(C1, 1, Area);

            if (model.HeavyOilFluid)
            {
                double L = model.CrackingHeat;
                model.ReliefLoad = Qfire / L;
                model.ReliefMW = 114;
                model.ReliefTemperature = 400;
                model.ReliefCpCv = 1.12;
                model.ReliefZ = 0.723;
            }
            else
            {
                CalcDrumFire(model.FluidType,Qfire);
            }
            
            
            
        }
        
        private void CalcTank()
        {
            if (!model.CheckData()) return;
            string targetUnit = "barg";
            double designPressure = UnitConvert.Convert(model.DesignPressureUnit, targetUnit, model.DesignPressure);

            if (designPressure < 0)
            {
                MessageBox.Show("Design Pressure could not be negative");
                return;
            }
            if (designPressure > 1.034 && selectedHeatInputModel == "API 2000")
            {
                MessageBox.Show("Becasue Design Pressure is greater,Heat Input Model must be API 521");
                SelectedHeatInputModel = "API 521";
            }

            double area = model.WettedArea;
            double F = 1;
            double L = 1;
            double T = 1;
            double M = 1;


            double pressure = psv.Pressure;

            double reliefFirePressure = pressure * 1.21;
            if (model.HeavyOilFluid)
            {
                if (model.CrackingHeat <= 0)
                {
                    MessageBox.Show("cracking Heat not be empty or 小于0", "Message Box");
                    return;
                }
                double Q = 0;
                if (SelectedHeatInputModel == "API 521")
                {
                    //计算Qfire
                    double C1 = 0;
                    if (model.EquipmentExist)
                    {
                        C1 = 43200;
                    }
                    else
                    {
                        C1 = 70900;
                    }
                    Q = Algorithm.GetQ(C1, 1, area);
                }
                else
                {
                    Q = Algorithm.CalcStorageTankLoad(area, designPressure, F, L, T, M); //这是按照2000 来计算的
                }
                L = model.CrackingHeat;
                model.ReliefLoad = Q / L;
                model.ReliefMW = 114;
                model.ReliefTemperature = 400;

            }
            else
            {
                if (model.LatentHeat == 0)
                {
                    string tempdir = DirProtectedSystem + @"\temp\";
                    string dirLatent = tempdir + "StorageTankFire";
                    if (Directory.Exists(dirLatent))
                    {
                        Directory.Delete(dirLatent, true);
                    }
                    Directory.CreateDirectory(dirLatent);
                    CustomStreamDAL customStreamDAL = new CustomStreamDAL();
                    IList<CustomStream> list = customStreamDAL.GetAllList(SessionProtectedSystem);
                    CustomStream stream = list[0];
                    string gd = Guid.NewGuid().ToString();
                    string vapor = "S_" + gd.Substring(0, 5).ToUpper();
                    string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
                    string rate = "1";
                    int ImportResult = 0;
                    int RunResult = 0;

                    PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
                    string[] sourceFiles = Directory.GetFiles(tempdir, "*.inp");
                    string sourceFile = sourceFiles[0];
                    string[] lines = System.IO.File.ReadAllLines(sourceFile);
                    HeatMethod = ProIIMethod.GetHeatMethod(lines, EqName);
                    string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
                    IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                    string tray1_f = fcalc.Calculate(content, 1, reliefFirePressure.ToString(), 4, "",HeatMethod, stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
                    if (ImportResult == 1 || ImportResult == 2)
                    {
                        if (RunResult == 1 || RunResult == 2)
                        {
                            IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                            reader.InitProIIReader(tray1_f);
                            ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                            ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                            reader.ReleaseProIIReader();
                            CustomStream vaporFire = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                            CustomStream liquidFire = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                            double latent = vaporFire.SpEnthalpy - liquidFire.SpEnthalpy;
                            L = latent;
                            model.LatentHeat = L;

                            T = vaporFire.Temperature;
                            M = vaporFire.BulkMwOfPhase;
                            double Q = 0;
                            if (SelectedHeatInputModel == "API 521")
                            {
                                Q = Algorithm.GetQ(43200, 1, area);
                            }
                            else
                            {
                                Q = Algorithm.CalcStorageTankLoad(area, designPressure, F, L, T, M); //这是按照2000 来计算的
                            }

                            model.ReliefLoad = Q / latent;
                            model.ReliefMW = vaporFire.BulkMwOfPhase;
                            model.ReliefPressure = reliefFirePressure;
                            model.ReliefTemperature = vaporFire.Temperature;
                            if (vaporFire.BulkCPCVRatio != null)
                            {
                                model.ReliefCpCv = vaporFire.BulkCPCVRatio;
                            }
                            model.ReliefZ = vaporFire.VaporZFmKVal;


                            FlashCalcResultDAL flashCalcResultDAL = new FlashCalcResultDAL();
                            FlashCalcResult flashCalcResult = new FlashCalcResult();
                            flashCalcResult.ReliefCpCv = model.ReliefCpCv;
                            flashCalcResult.ReliefMW = model.ReliefMW;
                            flashCalcResult.ReliefZ = model.ReliefZ;
                            flashCalcResult.ReliefPressure = model.ReliefPressure;
                            flashCalcResult.ReliefTemperature = model.ReliefTemperature;
                            flashCalcResult.Latent = model.LatentHeat;
                            flashCalcResult.ScenarioID = ScenarioID;
                            flashCalcResultDAL.Add(flashCalcResult, SessionProtectedSystem);
                        }

                        else
                        {
                            MessageBox.Show("The simulation unsolved!", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("There is some errors in keyword file.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    double latent = model.LatentHeat;
                    double Q = 0;
                    if (SelectedHeatInputModel == "API 521")
                    {
                        //计算Qfire
                        double C1 = 0;
                        if (model.EquipmentExist)
                        {
                            C1 = 43200;
                        }
                        else
                        {
                            C1 = 70900;
                        }
                        Q = Algorithm.GetQ(C1, 1, area);
                    }
                    else
                    {
                        Q = Algorithm.CalcStorageTankLoad(area, designPressure, F, L, T, M); //这是按照2000 来计算的
                    }

                    model.ReliefLoad = Q / latent;
                    FlashCalcResultDAL flashCalcResultDAL = new FlashCalcResultDAL();
                    FlashCalcResult flashCalcResult = flashCalcResultDAL.GetModel(SessionProtectedSystem, ScenarioID);
                    if (flashCalcResult != null)
                    {
                        model.ReliefPressure = flashCalcResult.ReliefPressure;
                        model.ReliefTemperature = flashCalcResult.ReliefTemperature;
                        model.ReliefMW = flashCalcResult.ReliefMW;
                        model.ReliefCpCv = flashCalcResult.ReliefCpCv;
                        model.ReliefZ = flashCalcResult.ReliefZ;
                    }

                }

            }




        }

        private void CalcHX()
        {
            model.ReliefPressure = reliefPressure;
            double Qfire = 0;
            double Area = model.WettedArea;
            //求出面积---你查看下把durmsize的 数据传进来。

            //计算Qfire
            double C1 = 0;
            if (model.EquipmentExist)
            {
                C1 = 43200;
            }
            else
            {
                C1 = 70900;
            }
            Qfire = Algorithm.GetQ(C1, 1, Area);

            if (model.HeavyOilFluid)
            {
                double L = model.CrackingHeat;
                model.ReliefLoad = Qfire / L;
                model.ReliefMW = 114;
                model.ReliefTemperature = 400;
                model.ReliefCpCv = 1.12;
                model.ReliefZ = 0.723;
            }
            else
            {
                CalcDrumFire(model.FluidType, Qfire);
            }



        }
        
       
        private void Calc(object obj)
        {
            //if (!model.CheckData()) return;
            switch (FireType)
            {
                case 0: CalcDrum(); break;
                case 1: CalcTank(); break;
                case 2:
                case 3: CalcHX(); break;

                default: break;
            }
        }
        private void Save(object obj)
        {
            if (!model.CheckData()) return;
            WriteConvertModel();
            if (model.dbmodel.ReliefMW > 0)
            {
                fireBLL.SaveData(model.dbmodel, fireFluidModel, sizeModel, SessionProtectedSystem);
            }
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    wd.DialogResult = true;
                }
            }
        }
       
        public double ScenarioReliefPressure(ISession SessionPS)
        {
            PSVDAL psv = new PSVDAL();
            var psvModel = psv.GetAllList(SessionPS).FirstOrDefault();
            if (psvModel != null)
            {
                return psvModel.Pressure * 1.21;
            }
            return 0;
        }

        public double ScenarioFireReliefPressure(ISession SessionPS)
        {
            PSVDAL psv = new PSVDAL();
            var psvModel = psv.GetAllList(SessionPS).FirstOrDefault();
            if (psvModel != null)
            {
                return psvModel.Pressure * 1.21;
            }
            return 0;
        }
       

        public int GetDrumFluidType()
        {
            int result = 0;
            SplashScreenManager.Show(5);
            try
            {
                CustomStreamDAL dal = new CustomStreamDAL();
                IList<CustomStream> Feeds = dal.GetAllList(SessionProtectedSystem, false);
                IList<CustomStream> Products = dal.GetAllList(SessionProtectedSystem, true);
                double totalFeeds = 0;
                double totalVaporProducts = 0;
                List<CustomStream> liquidProducts = new List<CustomStream>();
                List<string> liquidProductNames = new List<string>();
                CustomStream waterCS = null;
                SplashScreenManager.SentMsgToScreen("Get feed info of drum ......  20%");
                foreach (CustomStream cs in Feeds)
                {
                    totalFeeds = totalFeeds + cs.WeightFlow;
                }
                foreach (CustomStream cs in Products)
                {
                    if (cs.VaporFraction == 1)
                    {
                        totalVaporProducts = totalVaporProducts + cs.WeightFlow;
                    }
                    else
                    {
                        if (cs.ProdType == "4")
                        {
                            waterCS = cs;
                        }
                        else
                        {
                            liquidProducts.Add(cs);
                            liquidProductNames.Add(cs.StreamName);
                        }
                    }
                }
                SplashScreenManager.SentMsgToScreen("Get product info of drum ......  40%");
                if (totalFeeds == totalVaporProducts)
                {
                    result = 1;//  Gas/Vapor Only
                }
                else
                {
                    if (liquidProducts.Count == 0)
                    {
                        liquidProducts.Add(waterCS);
                        liquidProductNames.Add(waterCS.StreamName);
                    }
                    SplashScreenManager.SentMsgToScreen("Get product info of drum ......  60%");
                    int resultFireCriticalPressure = CalcFireCriticalPressure(liquidProductNames);
                    SplashScreenManager.SentMsgToScreen("Get product info of drum ......  80%");
                    if (resultFireCriticalPressure == 1)
                    {
                        if (reliefPressure > fireCriticalPressure)
                        {
                            CalcSupercriticalCpCv(liquidProducts, liquidProductNames,null);
                            SplashScreenManager.SentMsgToScreen("Get product info of drum ......  100%");
                            result = 2;
                        }
                        else
                        {
                            int molflash = CalcPercent5Mol(liquidProducts, liquidProductNames);
                            SplashScreenManager.SentMsgToScreen("Get product info of drum ......  100%");
                            if (molflash == 1)
                            {
                                result = 3;
                            }
                            else
                            {
                                result = 5;
                            }
                        }
                    }
                    else
                    {
                        int molflash = CalcPercent5Mol(liquidProducts, liquidProductNames);
                        SplashScreenManager.SentMsgToScreen("Get product info of drum ......  100%");
                        if (molflash == 1)
                        {
                            result = 4;
                        }
                        else
                        {
                            result = 5;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
            }
            finally
            {
                SplashScreenManager.Close();                
            }
            return result;
        }

        public int GetHXFluidType()
        {
            int result = 0;
            SplashScreenManager.Show(5);
            try
            {              
                SplashScreenManager.SentMsgToScreen("Get product info  ......  40%");
                if (minVFStream.VaporFraction == 1)
                {
                    result = 1;//  Gas/Vapor Only
                }
                else
                {
                    List<CustomStream> liquidProducts = new List<CustomStream>();
                    List<string> liquidProductNames = new List<string>();
                    if (minVFStream.StreamName == this.HXProduct.StreamName)
                    {
                        liquidProducts.Add(HXProduct);
                        liquidProductNames.Add(HXProduct.StreamName);
                    }
                    else
                    {
                        liquidProductNames = mixFeedStreamName;
                        liquidProducts = lstFeeds;
                    }
                    SplashScreenManager.SentMsgToScreen("Get product info   ......  60%");
                    int resultFireCriticalPressure = CalcFireCriticalPressure(liquidProductNames);
                    SplashScreenManager.SentMsgToScreen("Get product info   ......  80%");
                    if (resultFireCriticalPressure == 1)
                    {
                        if (reliefPressure > fireCriticalPressure)
                        {
                            CalcSupercriticalCpCv(liquidProducts, liquidProductNames,minVFStream);
                            SplashScreenManager.SentMsgToScreen("Get product info   ......  100%");
                            result = 2;
                        }
                        else
                        {
                            int molflash = CalcPercent5Mol(liquidProducts, liquidProductNames);
                            SplashScreenManager.SentMsgToScreen("Get product info   ......  100%");
                            if (molflash == 1)
                            {
                                result = 3;
                            }
                            else
                            {
                                result = 5;
                            }
                        }
                    }
                    else
                    {
                        int molflash = CalcPercent5Mol(liquidProducts, liquidProductNames);
                        SplashScreenManager.SentMsgToScreen("Get product info   ......  100%");
                        if (molflash == 1)
                        {
                            result = 4;
                        }
                        else
                        {
                            result = 5;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
            }
            finally
            {
                SplashScreenManager.Close();
            }
            return result;
        }

        private CustomStream MixHxFeed(string[] feeds, List<CustomStream> lstFeeds)
        {           
            //需要做mixer
            int ImportResult = 0;
            int RunResult = 0;
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirMix = tempdir + "HX_Mix" + ScenarioID.ToString();
            if (Directory.Exists(dirMix))
                Directory.Delete(dirMix);
            Directory.CreateDirectory(dirMix);

            string sbcontent = string.Empty;
            string[] files = Directory.GetFiles(tempdir, "*.inp");
            string sourceFile = files[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);

            sbcontent = PROIIFileOperator.getUsableContent(feeds.ToList(), lines);

            IMixCalculate mixcalc = ProIIFactory.CreateMixCalculate(SourceFileInfo.FileVersion);
            string mixProduct = Guid.NewGuid().ToString().Substring(0, 6);
            string tray1_f = mixcalc.Calculate(sbcontent, lstFeeds, mixProduct, dirMix, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIProduct = reader.GetSteamInfo(mixProduct);
                    reader.ReleaseProIIReader();
                    CustomStream result = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIProduct);
                    return result;
                }
            }
            return null;
        }


        private int CalcPercent5Mol(List<CustomStream> liquidFeeds,List<string>liquidFeedNames)
        {
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirMol = tempdir + "Fire" + ScenarioID.ToString() + "_Mol5";
            if (Directory.Exists(dirMol))
                Directory.Delete(dirMol, true);
            Directory.CreateDirectory(dirMol);
            string molcontent = PROIIFileOperator.getUsableContent(liquidFeedNames, tempdir);
            int result= CalcPercent5Mol(molcontent, reliefPressure, liquidFeeds, dirMol);
            return result;
        }

        private int CalcPercent5Mol(string content, double reliefPressure, List<CustomStream> liquidFeeds, string dirPercent5Mol)
        {
            if (liquidFeeds.Count == 0)
                return 2;
            int result = 0;
            int ImportResult = 0;
            int RunResult = 0;
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string tray1_f = fcalc.Calculate(content, 1, reliefPressure.ToString(), 6, "0.05",HeatMethod, liquidFeeds, vapor, liquid, dirPercent5Mol, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                    ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    reader.ReleaseProIIReader();
                    molVapor = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                    CustomStream molLiquid = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                    molLatent = molVapor.SpEnthalpy - molLiquid.SpEnthalpy;
                    if (molVapor.WeightFlow > 0)
                        result = 1;
                    else
                        result = 2;
                    return result;
                }

                else
                {
                    //MessageBox.Show("The simulation unsolved!", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return 3;
                }
            }
            else
            {
                //MessageBox.Show("There is some errors in keyword file.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);
                return 3;
            }

        }

        private int CalcCriticalPressure(string content, double ReliefPressure, CustomStream stream, string dirPhase )
        {
            int ImportResult = 0;
            int RunResult = 0;
            int result = 0;
            IPHASECalculate PhaseCalc = ProIIFactory.CreatePHASECalculate(SourceFileInfo.FileVersion);
            string PH = "PH" + Guid.NewGuid().ToString().Substring(0, 4);
            string criticalPress = string.Empty;
            string criticalTemp = string.Empty;
            string phasef = PhaseCalc.Calculate(content, 1, ReliefPressure.ToString(), 4, "", stream, PH, dirPhase, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(phasef);
                    criticalPress = reader.GetCriticalPressure(PH);
                    criticalTemp = reader.GetCriticalTemperature(PH);
                    reader.ReleaseProIIReader();

                    if (string.IsNullOrEmpty(criticalPress) || double.Parse(criticalPress) <=0)
                    {
                        result = 2;
                    }
                    else
                    {
                        fireCriticalPressure = UnitConvert.Convert("KPa",UOMEnum.Pressure , double.Parse(criticalPress));
                        fireCriticalTemperature = UnitConvert.Convert("K", UOMEnum.Temperature, double.Parse(criticalTemp)); 
                        result = 1;
                    }
                    return result;
                }
                else
                {
                    //MessageBox.Show("The simulation unsolved!", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return 3;
                }
            }
            else
            {
                //MessageBox.Show("There is some errors in keyword file.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);
                return 4;
            }

        }

        private int CalcFireCriticalPressure(List<string> lstFeeds)
        {
            int result = 0;
            if (lstFeeds.Count == 0)
            {
                result= 2;
            }
            else
            {
                List<CustomStream> csFeeds = new List<CustomStream>();
                CustomStreamDAL csdal = new CustomStreamDAL();
                foreach (string s in lstFeeds)
                {
                    CustomStream cs = csdal.GetModel(SessionProtectedSystem, s);
                    csFeeds.Add(cs);
                }
                string tempdir = DirProtectedSystem + @"\temp\";
                string dirPhase = tempdir + "Fire" + ScenarioID.ToString() + "_Phase";
                if (Directory.Exists(dirPhase))
                    Directory.Delete(dirPhase,true);
                Directory.CreateDirectory(dirPhase);
                CustomStream stream = csFeeds[0];
                string[] streamComps = stream.TotalComposition.Split(',');
                int len = streamComps.Length;
                double[] streamCompValues = new double[len];
                double sumTotalMolarRate = 0;
                foreach (CustomStream cs in csFeeds)
                {
                    sumTotalMolarRate = sumTotalMolarRate + cs.TotalMolarRate;
                }
                foreach (CustomStream cs in csFeeds)
                {
                    string[] comps = cs.TotalComposition.Split(',');
                    for (int i = 0; i < len; i++)
                    {
                        streamCompValues[i] = streamCompValues[i] + double.Parse(comps[i]) * cs.TotalMolarRate / sumTotalMolarRate;
                    }
                }
                StringBuilder sumComposition = new StringBuilder();
                foreach (double comp in streamCompValues)
                {
                    sumComposition.Append(",").Append(comp.ToString());
                }
                stream.TotalComposition = sumComposition.ToString().Substring(1);
                double internPressure = UnitConvert.Convert("MPAG", "KPA", stream.Pressure);

                string phasecontent = PROIIFileOperator.getUsablePhaseContent(stream.StreamName, tempdir);
                double ReliefPressure = 1;
                result = CalcCriticalPressure(phasecontent, ReliefPressure, stream, dirPhase);
               
            }
            if (result == 2)
            {
                MessageBox.Show("Critical point NOT determined, please check result carefully.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning); 
            }
            return result;
        }


        private void CalcDrumFire(int FluidType,double QFire)
        {
            if (model.WettedArea == 0)
            {
                MessageBox.Show("Area must be greater than zero.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            switch (FluidType)
            {
                case 1:
                    //all gas only
                    GasMethod();
                    break;
                case 2: 
                    //suppercritical
                    GasMethod();
                    break;
                case 3:
                    CheckLatent();
                    MultiPhase3Method(QFire);
                    break;
                case 4:
                    CheckLatent();
                    MultiPhase4Method(QFire);
                    break;
                case 5:
                    CheckLatent();
                    NotDetermined(QFire);
                    break;

            }
        }


        private void CalcSupercriticalCpCv(List<CustomStream> liquidFeeds, List<string> liquidFeedNames, CustomStream mixVFStream)
        {
            double temperature =0;
            if (FireType == 0)
            {
                temperature = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", reliefPressure) / UnitConvert.Convert(UOMEnum.Pressure, "Kpa", drum.Pressure) * UnitConvert.Convert(UOMEnum.Temperature, "K", drum.Temperature);
            }
            else if (FireType == 2)
            {
                temperature = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", reliefPressure) / UnitConvert.Convert(UOMEnum.Pressure, "Kpa", mixVFStream.Pressure) * UnitConvert.Convert(UOMEnum.Temperature, "K", mixVFStream.Temperature);
            }
            double reliefTemperature =  UnitConvert.Convert("K", UOMEnum.Temperature, temperature);
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirSupercritical = tempdir + "Fire" + ScenarioID.ToString() + "_dirSupercritical";
            if (Directory.Exists(dirSupercritical))
                Directory.Delete(dirSupercritical,true);
            Directory.CreateDirectory(dirSupercritical);
            string content = PROIIFileOperator.getUsableContent(liquidFeedNames, tempdir);
            int result = 0;
            int ImportResult = 0;
            int RunResult = 0;
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            double maxTemp = reliefTemperature;
            if (maxTemp < fireCriticalTemperature)
                maxTemp = fireCriticalTemperature;

            string tray1_f = fcalc.Calculate(content, 1, reliefPressure.ToString(), 2, maxTemp.ToString(),HeatMethod, liquidFeeds, vapor, liquid, dirSupercritical, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    reader.ReleaseProIIReader();
                    CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                    if (cs.BulkCPCVRatio > 0)
                    {
                        fireFluidModel.NormalCpCv = cs.BulkCPCVRatio;
                    }
                    else
                    {
                        fireFluidModel.NormalCpCv = 1.4;
                    }
                    fireFluidModel.GasVaporMW = cs.BulkMwOfPhase;
                }

                else
                {
                    //MessageBox.Show("The simulation unsolved!", "Message Box",MessageBoxButton.OK,MessageBoxImage.Warning);
                    result = 1;
                }
            }
            else
            {
                //MessageBox.Show("There is some errors in keyword file.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);
                result=2;
            }

        }


        private void MultiPhase3Method(double QFire)
        {
            CalcDrumFireByLatent(QFire);         
        }

        private void MultiPhase4Method(double QFire)
        {           
            CalcDrumFireByLatent(QFire);
        }

        private void NotDetermined(double QFire)
        {
            CalcDrumFireByLatent(QFire);
            double temperature = 0;
            if (FireType == 0)
            {
                temperature = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", reliefPressure) / UnitConvert.Convert(UOMEnum.Pressure, "Kpa", drum.Pressure) * UnitConvert.Convert(UOMEnum.Temperature, "K", drum.Temperature);
            }
            else if (FireType == 2)
            {
                temperature = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", reliefPressure) / UnitConvert.Convert(UOMEnum.Pressure, "Kpa", mixFeedStream.Pressure) * UnitConvert.Convert(UOMEnum.Temperature, "K", mixFeedStream.Temperature);
            }
            model.ReliefTemperature = UnitConvert.Convert( "K",model.ReliefTemperatureUnit, temperature);
        }
        
        private void CalcDrumFireByLatent(double Qfire)
        {
            if (model.IsCalc)
            {               
                double latent = UnitConvert.Convert(model.LatentHeatUnit, UOMEnum.SpecificEnthalpy, model.LatentHeat);
                if (latent == 0)
                {
                    //MessageBox.Show("Latent could not be zero.","Message Box",MessageBoxButton.OK,MessageBoxImage.Error);
                    //return;
                }
                else
                {
                    model.ReliefLoad = Qfire / latent;
                }
                
            }
            else
            {
                double latent = UnitConvert.Convert(model.LatentHeat2Unit, UOMEnum.SpecificEnthalpy, model.LatentHeat2);
                if (latent == 0)
                {
                    //MessageBox.Show("Latent could not be zero.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);
                    //return;
                }
                else
                {
                    model.ReliefLoad = Qfire / latent;
                }
            }
            model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefLoadUnit, model.ReliefLoad);
            if (model.FluidType == 5)
            {
                if (normalVapor == null)
                {
                    MessageBox.Show("No vapor.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                model.ReliefMW = normalVapor.BulkMwOfPhase;
                model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, reliefPressure);
                model.ReliefCpCv = normalVapor.BulkCPCVRatio;
                model.ReliefZ = normalVapor.VaporZFmKVal;
                double temperature = 0;
                if (FireType == 0)
                {
                    temperature = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", reliefPressure) / UnitConvert.Convert(UOMEnum.Pressure, "Kpa", drum.Pressure) * UnitConvert.Convert(UOMEnum.Temperature, "K", drum.Temperature);
                }
                else if (FireType == 2)
                {
                    temperature = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", reliefPressure) / UnitConvert.Convert(UOMEnum.Pressure, "Kpa", mixFeedStream.Pressure) * UnitConvert.Convert(UOMEnum.Temperature, "K", mixFeedStream.Temperature);
                }
                model.ReliefTemperature = UnitConvert.Convert("K", UOMEnum.Temperature, temperature);
                model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit, model.ReliefTemperature);
            }
            else
            {
                if (model.dbmodel.ID == 0)
                {
                    model.ReliefMW = molVapor.BulkMwOfPhase;
                    model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, reliefPressure);
                    model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit, molVapor.Temperature);
                    model.ReliefCpCv = molVapor.BulkCPCVRatio;
                    model.ReliefZ = molVapor.VaporZFmKVal;
                }
            }

        }

        private void GasMethod()
        {
            double mw = fireFluidModel.GasVaporMW;
            double cpcv = fireFluidModel.NormalCpCv;
            double area = model.WettedArea;
            area = UnitConvert.Convert(model.WettedAreaUnit, UOMEnum.Area, area);
            double tw = UnitConvert.Convert("C", "K", fireFluidModel.TW);
            double p1 = UnitConvert.Convert("MPag", "Kpa", reliefPressure);
            double t1 = 0;
            if (FireType == 0)
            {
                t1 = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", reliefPressure) / UnitConvert.Convert(UOMEnum.Pressure, "Kpa", drum.Pressure) * UnitConvert.Convert(UOMEnum.Temperature, "K", drum.Temperature);
            }
            else if (FireType == 2)
            {
                t1 = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", reliefPressure) / UnitConvert.Convert(UOMEnum.Pressure, "Kpa", mixFeedStream.Pressure) * UnitConvert.Convert(UOMEnum.Temperature, "K", mixFeedStream.Temperature);
            }
            double w = Algorithm.GetFullVaporW(cpcv, mw, p1, area, tw, t1);
            if (t1 >= tw)
            {
                //MessageBox.Show(" T1 > Tw, relief load is set to zero.", "Message Box");
                return;
            }

            model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefLoadUnit, w);
            if (model.ReliefLoad < 0)
                model.ReliefLoad = 0;
            model.ReliefMW = mw;
            model.ReliefTemperature = UnitConvert.Convert("K", model.ReliefTemperatureUnit, t1);
            model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, reliefPressure);
            model.ReliefCpCv = 1.12;
            model.ReliefZ = 0.723;
        }


        private void GetMix()
        {
            string strproduct = string.Empty;
            string strfeed = string.Empty;

            if (psv.LocationDescription == "Shell")
            {
                strfeed = hx.ShellFeedStreams;
            }
            else
            {
                strfeed = hx.TubeFeedStreams;
            }
            if (strfeed == hx.ColdInlet)
            {
                strproduct = hx.ColdOutlet;
            }
            else
            {
                strproduct = hx.HotOutlet;
            }

            CustomStreamDAL dal = new CustomStreamDAL();
            string[] feeds = strfeed.Split(',');
            mixFeedStreamName = feeds.ToList();
            if (feeds.Length == 1)
            {
                minVFStream = dal.GetModel(SessionProtectedSystem, feeds[0]);
                lstFeeds.Add(minVFStream);
            }
            else
            {
                foreach (string s in feeds)
                {
                    CustomStream cs = dal.GetModel(SessionProtectedSystem, s);
                    lstFeeds.Add(cs);
                }
                minVFStream = MixHxFeed(feeds, lstFeeds);
            }
            mixFeedStream = minVFStream;
            SplashScreenManager.SentMsgToScreen("Get feed info  ......  20%");
            HXProduct = dal.GetModel(SessionProtectedSystem, strproduct);
            if (minVFStream.VaporFraction > HXProduct.VaporFraction)
            {
                minVFStream = HXProduct;
            }
            
        }
    }
}
