using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.IO;
using NHibernate;
using ReliefProLL;
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

namespace ReliefProMain.ViewModel.Drums
{
    public class DrumFireVM : ViewModelBase
    {
        public ICommand FluidCMD { get; set; }
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
            FluidCMD = new DelegateCommand<object>(OpenFluidWin);
            CalcCMD = new DelegateCommand<object>(Calc);
            OKCMD = new DelegateCommand<object>(Save);
            DrumSizeCMD = new DelegateCommand<object>(OpenDrumSize);
            lstHeatInputModel = new List<string>();
            lstHeatInputModel.Add("API 521");
            if (FireType == 1)
            {
                lstHeatInputModel.Add("API 2000");
                SelectedHeatInputModel = "API 2000";
            }

            fireBLL = new DrumFireBLL(SessionPS, SessionPF);
            var fireModel = fireBLL.GetDrumFireModel(ScenarioID);
            fireModel = fireBLL.ReadConvertModel(fireModel);
            if (!string.IsNullOrEmpty(fireModel.HeatInputModel))
                SelectedHeatInputModel = fireModel.HeatInputModel;

            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionDBPath == SessionPF.Connection.ConnectionString);
            model = new DrumFireModel(fireModel);
            model.WettedAreaUnit = uomEnum.UserArea;
            model.LatentHeatUnit = uomEnum.UserSpecificEnthalpy;
            model.CrackingHeatUnit = uomEnum.UserSpecificEnthalpy;
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefPressureUnit = uomEnum.UserPressure;
            model.DesignPressureUnit = uomEnum.UserPressure;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
            model.NoneAllGas = !model.AllGas;

            reliefPressure = ScenarioReliefPressure(SessionPS);
        }
        private void WriteConvertModel()
        {
            model.dbmodel.WettedArea = UnitConvert.Convert(model.WettedAreaUnit, UOMLib.UOMEnum.Area.ToString(), model.WettedArea);
            model.dbmodel.LatentHeat = UnitConvert.Convert(model.LatentHeatUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.LatentHeat);
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
                    model.WettedArea = Area;
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
                        Area = Algorithm.GetHXArea(hxFireSize.ExposedToFire, hxFireSize.Type, hxFireSize.Length, hxFireSize.OD);
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
            UnitConvert uc = new UnitConvert();
            DrumFireFluidView win = new DrumFireFluidView();
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            DrumFireFluidVM vm = new DrumFireFluidVM(model.dbmodel.ID, SessionProtectedSystem, SessionPlant);
            win.DataContext = vm;
            if (win.ShowDialog() == true)
            {
                fireFluidModel = vm.model.dbmodel;
                //需要转换成算法GetFullVaporW 要求的单位。
                double dmw = fireFluidModel.GasVaporMW;
                double dp1 = fireFluidModel.PSVPressure * 1.21;
                double darea = fireFluidModel.ExposedVesse;
                double dtw = fireFluidModel.TW;
                double dtn = fireFluidModel.NormaTemperature;
                double dpn = fireFluidModel.NormalPressure;



                fireFluidModel = vm.model.dbmodel;
                double mw = dmw;
                double p1 = UnitConvert.Convert("P", "Mpag", "psia", dp1);
                double area = UnitConvert.Convert("A", "m2", "ft2", darea);
                double tw = UnitConvert.Convert("T", "C", "R", dtw);
                double tn = UnitConvert.Convert("T", "C", "R", dtn);
                double pn = UnitConvert.Convert("P", "Mpag", "psia", dpn);

                double t1 = 0;
                double load = Algorithm.GetFullVaporW(mw, p1, area, tw, pn, tn, ref t1);
                double relieftT = UnitConvert.Convert("T", "R", "C", t1);
                double reliefLoad = UnitConvert.Convert("MR", "lb/hr", "kg/hr", load);

                //泄放结果。 老李需要保存到数据库里。
                double reliefPressure = dp1;
                double reliefMW = dmw;
                double reliefTemperature = relieftT;

                model.ReliefLoad = reliefLoad;
                model.ReliefMW = reliefMW;
                model.ReliefTemperature = relieftT;
                model.ReliefPressure = reliefPressure;
            }
        }
        private void CalcDrum()
        {

            if (!model.CheckData()) return;
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
            }
            else if (model.AllGas)
            {
                double mw = fireFluidModel.GasVaporMW;
                double area = UnitConvert.Convert("m2", "ft2", fireFluidModel.ExposedVesse);
                double tw = UnitConvert.Convert("C", "R", fireFluidModel.TW);
                double tn = UnitConvert.Convert("C", "R", fireFluidModel.NormaTemperature);
                double pn = UnitConvert.Convert("MPag", "psia", fireFluidModel.NormalPressure);
                double p1 = UnitConvert.Convert("MPag", "psia", fireFluidModel.PSVPressure * 1.21);
                double t1 = 0;
                double w = Algorithm.GetFullVaporW(mw, p1, area, tw, pn, tn, ref t1);
                if (t1 >= tw)
                {
                    MessageBox.Show("T1 could not be greater than TW", "Message Box");
                    return;
                }
                double reliefLoad = UnitConvert.Convert("Kg/hr", "lb/hr", w);
                model.ReliefLoad = reliefLoad;
                model.ReliefMW = mw;
                model.ReliefTemperature = UnitConvert.Convert("R", "C", t1); ;

            }
            else
            {

                if (model.LatentHeat == 0)
                {
                    //取出liquid stream
                    CustomStreamDAL dbcs = new CustomStreamDAL();
                    IList<CustomStream> listStream = dbcs.GetAllList(SessionProtectedSystem, true);
                    CustomStream liquidStream = new CustomStream();
                    foreach (CustomStream s in listStream)
                    {
                        if (s.ProdType == "2")
                        {
                            liquidStream = s;
                        }
                    }
                    //闪蒸计算
                    string vapor = "V_" + Guid.NewGuid().ToString().Substring(0, 6);
                    string liquid = "L_" + Guid.NewGuid().ToString().Substring(0, 6);
                    string tempdir = DirProtectedSystem + @"\DrumFire";
                    if (!Directory.Exists(tempdir))
                    {
                        Directory.CreateDirectory(tempdir);
                    }
                    int ImportResult = 0;
                    int RunResult = 0;
                    string dir = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\";
                    string content = PROIIFileOperator.getUsableContent(liquidStream.StreamName, dir);
                    IFlashCalculate flashcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                    string f = flashcalc.Calculate(content, 1, reliefPressure.ToString(), 6, "0.05", liquidStream, vapor, liquid, tempdir, ref ImportResult, ref RunResult);
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(f);
                    ProIIStreamData proIIvapor = reader.GetSteamInfo(vapor);
                    ProIIStreamData proIIliquid = reader.GetSteamInfo(liquid);
                    reader.ReleaseProIIReader();
                    CustomStream csVapor = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIvapor);
                    CustomStream csLiquid = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIliquid);
                    double latent = csVapor.SpEnthalpy - csLiquid.SpEnthalpy;

                    double reliefLoad = Qfire / latent;
                    double reliefMW = csVapor.BulkMwOfPhase;
                    double reliefT = csVapor.Temperature;
                    model.ReliefLoad = reliefLoad;
                    model.ReliefMW = reliefMW;
                    model.ReliefPressure = reliefPressure;
                    model.ReliefTemperature = reliefT;
                    model.LatentHeat = latent;
                    if (csVapor.BulkCPCVRatio != null)
                    {
                        model.ReliefCpCv = csVapor.BulkCPCVRatio;
                    }
                    model.ReliefZ = csVapor.VaporZFmKVal;


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
                    double latent = model.LatentHeat;
                    model.ReliefLoad = Qfire / latent;
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
                            MessageBox.Show("Prz file is error", "Message Box");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("inp file is error", "Message Box");
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
                                model.ReliefMW = vaporFire.BulkMwOfPhase;
                                model.ReliefPressure = reliefFirePressure;
                                model.ReliefTemperature = vaporFire.Temperature;
                                //model.ReliefCpCv = double.Parse(vaporFire.BulkCPCVRatio);
                                //model.ReliefZ = double.Parse(vaporFire.VaporZFmKVal);

                            }

                            else
                            {
                                MessageBox.Show("Prz file is error", "Message Box");
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("inp file is error", "Message Box");
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
            fireBLL.SaveData(model.dbmodel, fireFluidModel, sizeModel, SessionProtectedSystem);
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
                return psvModel.Pressure * psvModel.ReliefPressureFactor;
            }
            return 0;
        }


        private CustomStream getFlashCalcLiquidStreamVF0(CustomStream stream)
        {
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "Fire1";
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
                    MessageBox.Show("Prz file is error", "Message Box");
                    return null;
                }
            }
            else
            {
                MessageBox.Show("inp file is error", "Message Box");
                return null;
            }
        }

        private CustomStream getFlashCalcLiquidStreamVF1(CustomStream stream)
        {
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "Fire1";
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
                    MessageBox.Show("Prz file is error", "Message Box");
                    return null;
                }
            }
            else
            {
                MessageBox.Show("inp file is error", "Message Box");
                return null;
            }
        }
    }
}
