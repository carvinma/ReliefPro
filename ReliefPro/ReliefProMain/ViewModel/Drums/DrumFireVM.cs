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
        private Drum drum;

        private CustomStream molVapor;
        private double molLatent;

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
            
            //如果是drum的话，获得当前drum的信息
            if (FireType == 0)
            {
                DrumDAL drumdal = new DrumDAL();
                drum = drumdal.GetModel(SessionProtectedSystem);
            }
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

            DrumFireFluidBLL fluidBll = new DrumFireFluidBLL(SessionPS, SessionPF);
            fireFluidModel = fluidBll.GetFireFluidModel(model.dbmodel.ID);
            reliefPressure = ScenarioFireReliefPressure(SessionPS);

            CustomStreamDAL csdal=new CustomStreamDAL();
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
                model.FluidType = GetFluidType();
            }
            else if (model.FluidType == 3 || model.FluidType == 4)
            {
                molLatent = model.LatentHeat;
            }

            if (model.dbmodel.ID == 0 && model.FluidType == 1)
            {
                fireFluidModel.NormalCpCv = normalVapor.BulkCPCVRatio;
                fireFluidModel.GasVaporMW = normalVapor.BulkMwOfPhase;
                fireFluidModel.TW=UnitConvert.Convert(UOMEnum.Temperature,uomEnum.UserTemperature,593);
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
                        MessageBox.Show("Relief Condition is near critical , Latent heat is set to default or user defined value.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        model.LatentHeat2 = UnitConvert.Convert(UOMEnum.SpecificEnthalpy, model.LatentHeatUnit, 115);
                        model.IsCalc = false;
                    }
                    else
                    {
                        if (molLatent < 115)
                        {
                            MessageBox.Show("Calculated latent heat is less than bound value and default  or user defined value is used.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                            model.LatentHeat2 = UnitConvert.Convert(UOMEnum.SpecificEnthalpy, model.LatentHeatUnit, 115);
                            model.IsCalc = false;
                        }

                    }
                    model.LatentHeat = molLatent;
                    if (molLatent >= 115)
                    {
                        model.IsCalc = true;
                    }
                }
                else if (model.FluidType == 4)
                {
                    if (molLatent < 115)
                    {
                        MessageBox.Show("Calculated latent heat is less than bound value and default  or user defined value is used.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    MessageBox.Show("Flash for latent heat failed and default 46 KJ/Kg is used as latent heat.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            DrumFireFluidVM vm = new DrumFireFluidVM(model.dbmodel.ID, SessionProtectedSystem, SessionPlant);
            win.DataContext = vm;
            if (model.WettedArea == 0)
            {
                MessageBox.Show("Area must be greater than zero.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
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
            vm.model.NormalCpCv = fireFluidModel.NormalCpCv;
            vm.model.VaporMW = fireFluidModel.GasVaporMW;
            vm.model.TW =fireFluidModel.TW;
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
                
            }
            if (win.ShowDialog() == true)
            {
                model = vm.model;               
            }
        }

       
        private void CalcDrum()
        {
            PSVDAL psvdal = new PSVDAL();
            PSV psv = psvdal.GetModel(SessionProtectedSystem);
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


            PSVDAL psvDAL = new PSVDAL();
            PSV psv = psvDAL.GetModel(SessionProtectedSystem);
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
                    if (!Directory.Exists(dirLatent))
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

                    string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
                    IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                    string tray1_f = fcalc.Calculate(content, 1, reliefFirePressure.ToString(), 4, "", stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
            if (!model.CheckData()) return;
            CustomStreamDAL customStreamDAL = new CustomStreamDAL();
            CustomStream feed = new CustomStream();
            IList<CustomStream> feeds = customStreamDAL.GetAllList(SessionProtectedSystem, false);
            if (feeds.Count == 0)
                return;
            CustomStream maxTStream = feeds[0];
            foreach (CustomStream cs in feeds)
            {
                if (cs.VaporFraction != null && maxTStream.VaporFraction != null)
                {
                    if (cs.VaporFraction > maxTStream.VaporFraction)
                        maxTStream = cs;
                }
            }
            CustomStream product = new CustomStream();
            IList<CustomStream> products = customStreamDAL.GetAllList(SessionProtectedSystem, true);
            if (products.Count > 0)
                product = products[0];

            if (feed.VaporFraction == 1 && product.VaporFraction == 1)
            {
                MessageBox.Show("No calc", "Message Box");
                return;
            }

            double Q = 0;
            double area = model.WettedArea;
            double F = 1;
            double L = 1;
            double T = 1;
            double M = 1;
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
                return;
            }
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

            PSVDAL psvDAL = new PSVDAL();
            PSV psv = psvDAL.GetModel(SessionProtectedSystem);
            double pressure = psv.Pressure;

            double reliefFirePressure = pressure * 1.21;
            if (model.HeavyOilFluid)
            {
                if (model.CrackingHeat <= 0)
                {
                    MessageBox.Show("cracking Heat not be empty or 小于0", "Message Box");
                    return;
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
                    CustomStream stream = new CustomStream();
                    if (maxTStream.VaporFraction == 1)
                        stream = getFlashCalcLiquidStreamVF1(maxTStream);
                    else
                        stream = getFlashCalcLiquidStreamVF0(maxTStream);

                    if (stream != null)
                    {
                        string tempdir = DirProtectedSystem + @"\temp\";
                        string dirLatent = tempdir + "Fire2";
                        if (!Directory.Exists(dirLatent))
                            Directory.CreateDirectory(dirLatent);

                        string gd = Guid.NewGuid().ToString();
                        string vapor = "S_" + gd.Substring(0, 5).ToUpper();
                        string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
                        string second = "0.05";
                        int ImportResult = 0;
                        int RunResult = 0;
                        PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
                        string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
                        IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                        string tray1_f = fcalc.Calculate(content, 1, reliefFirePressure.ToString(), 6, second, stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
                                model.LatentHeat = latent;
                                model.ReliefLoad = Q / latent;
                                if (model.ReliefLoad < 0)
                                    model.ReliefLoad = 0;
                                model.ReliefMW = vaporFire.BulkMwOfPhase;
                                model.ReliefPressure = reliefFirePressure;
                                model.ReliefTemperature = vaporFire.Temperature;
                                model.ReliefCpCv = vaporFire.BulkCPCVRatio;
                                model.ReliefZ = vaporFire.VaporZFmKVal;

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
                }
                else
                {
                    double latent = model.LatentHeat;
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

        }

        private void Calc(object obj)
        {
            if (!model.CheckData()) return;
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
        private CustomStream getFlashCalcLiquidStreamVF0(CustomStream stream)
        {
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "Fire" + ScenarioID.ToString() + "_0";
            if (!Directory.Exists(dirLatent))
                Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string tray1_f = fcalc.Calculate(content, 1, "0", 5, "0", stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                    reader.ReleaseProIIReader();
                    CustomStream liquidcs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                    return liquidcs;

                }

                else
                {
                    MessageBox.Show("The simulation unsolved!", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
            }
            else
            {
                MessageBox.Show("There is some errors in keyword file.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private CustomStream getFlashCalcLiquidStreamVF1(CustomStream stream)
        {
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "Fire"+ScenarioID.ToString()+"_1";
            if (!Directory.Exists(dirLatent))
                Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string tray1_f = fcalc.Calculate(content, 1, "0", 4, "", stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                    reader.ReleaseProIIReader();
                    CustomStream liquidcs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                    return liquidcs;

                }

                else
                {
                    MessageBox.Show("The simulation unsolved!", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
            }
            else
            {
                MessageBox.Show("There is some errors in keyword file.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }


        public int GetFluidType()
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
                    int resultFireCriticalPressure = CalcFireCriticalPressure(liquidProducts);
                    SplashScreenManager.SentMsgToScreen("Get product info of drum ......  80%");
                    if (resultFireCriticalPressure == 1)
                    {
                        if (reliefPressure > fireCriticalPressure)
                        {
                            CalcSupercriticalCpCv(liquidProducts, liquidProductNames);
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
            string tray1_f = fcalc.Calculate(content, 1, reliefPressure.ToString(), 6, "0.05", liquidFeeds, vapor, liquid, dirPercent5Mol, ref ImportResult, ref RunResult);
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
                    MessageBox.Show("The simulation unsolved!", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return 3;
                }
            }
            else
            {
                MessageBox.Show("There is some errors in keyword file.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        fireCriticalTemperature = double.Parse(criticalTemp);
                        result = 1;
                    }
                    return result;
                }
                else
                {
                    MessageBox.Show("The simulation unsolved!", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return 3;
                }
            }
            else
            {
                MessageBox.Show("There is some errors in keyword file.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);
                return 4;
            }

        }

        private int CalcFireCriticalPressure(List<CustomStream> arrFeeds)
        {
            int result = 0;
            if (arrFeeds.Count == 0)
            {
                result= 2;
            }
            else
            {
                string tempdir = DirProtectedSystem + @"\temp\";
                string dirPhase = tempdir + "Fire" + ScenarioID.ToString() + "_Phase";
                if (Directory.Exists(dirPhase))
                    Directory.Delete(dirPhase,true);
                Directory.CreateDirectory(dirPhase);
                CustomStream stream = arrFeeds[0];
                string[] streamComps = stream.TotalComposition.Split(',');
                int len = streamComps.Length;
                double[] streamCompValues = new double[len];
                double sumTotalMolarRate = 0;
                foreach (CustomStream cs in arrFeeds)
                {
                    sumTotalMolarRate = sumTotalMolarRate + cs.TotalMolarRate;
                }
                foreach (CustomStream cs in arrFeeds)
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


        private void CalcSupercriticalCpCv(List<CustomStream> liquidFeeds, List<string> liquidFeedNames)
        {
            double temperature = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", reliefPressure) / UnitConvert.Convert(UOMEnum.Pressure, "Kpa", drum.Pressure) * UnitConvert.Convert(UOMEnum.Temperature, "K", drum.Temperature);
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

            string tray1_f = fcalc.Calculate(content, 1, reliefPressure.ToString(), 2, maxTemp.ToString(), liquidFeeds, vapor, liquid, dirSupercritical, ref ImportResult, ref RunResult);
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
                    MessageBox.Show("The simulation unsolved!", "Message Box",MessageBoxButton.OK,MessageBoxImage.Warning);

                }
            }
            else
            {
                MessageBox.Show("There is some errors in keyword file.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);

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
            double temperature = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", reliefPressure) / UnitConvert.Convert(UOMEnum.Pressure, "Kpa", drum.Pressure) * UnitConvert.Convert(UOMEnum.Temperature, "K", drum.Temperature);
            model.ReliefTemperature = UnitConvert.Convert( "K",UOMEnum.Temperature, temperature);
        }
        private void CalcDrumFireByLatent(double Qfire)
        {
            if (model.IsCalc)
            {               
                double latent = UnitConvert.Convert(model.LatentHeatUnit, UOMEnum.SpecificEnthalpy, model.LatentHeat);
                if (latent == 0)
                {
                    MessageBox.Show("Latent could not be zero.","Message Box",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
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
                    MessageBox.Show("Latent could not be zero.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    model.ReliefLoad = Qfire / latent;
                }
            }

            if (model.FluidType == 5)
            {
                model.ReliefMW = normalVapor.BulkMwOfPhase;
                model.ReliefPressure = reliefPressure;              
                model.ReliefCpCv = normalVapor.BulkCPCVRatio;
                model.ReliefZ = normalVapor.VaporZFmKVal;
                double temperature = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", reliefPressure) / UnitConvert.Convert(UOMEnum.Pressure, "Kpa", drum.Pressure) * UnitConvert.Convert(UOMEnum.Temperature, "K", drum.Temperature);
                model.ReliefTemperature = UnitConvert.Convert("K", UOMEnum.Temperature, temperature);
            }
            else
            {
                if (model.dbmodel.ID == 0)
                {
                    model.ReliefMW = molVapor.BulkMwOfPhase;
                    model.ReliefPressure = reliefPressure;
                    model.ReliefTemperature = molVapor.Temperature;
                    model.ReliefCpCv = molVapor.BulkCPCVRatio;
                    model.ReliefZ = molVapor.VaporZFmKVal;
                }
            }

        }

        private void GasMethod()
        {
            double mw = fireFluidModel.GasVaporMW;
            double cpcv = fireFluidModel.NormalCpCv;
            double area = fireFluidModel.ExposedVesse;
            double tw = UnitConvert.Convert("C", "K", fireFluidModel.TW);
            //double tn = UnitConvert.Convert("C", "K", fireFluidModel.NormaTemperature);
            //double pn = UnitConvert.Convert("MPag", "Kpa", fireFluidModel.NormalPressure);
            double p1 = UnitConvert.Convert("MPag", "Kpa", reliefPressure);
            double t1 = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", reliefPressure) / UnitConvert.Convert(UOMEnum.Pressure, "Kpa", drum.Pressure) * UnitConvert.Convert(UOMEnum.Temperature, "K", drum.Temperature);
            double w = Algorithm.GetFullVaporW(cpcv,mw, p1, area, tw, t1);
            if (t1 >= tw)
            {
                MessageBox.Show("T1 could not be greater than TW", "Message Box");
                return;
            }

            model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, uomEnum.UserMassRate, w); 
            model.ReliefMW = mw;
            model.ReliefTemperature = UnitConvert.Convert("K", uomEnum.UserTemperature, t1); 
            model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, reliefPressure);
            model.ReliefCpCv = 1.12;
            model.ReliefZ = 0.723;
        }
    }
}
