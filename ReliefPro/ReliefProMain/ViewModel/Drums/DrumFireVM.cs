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
using ReliefProMain.Model;
using ReliefProMain.View;
using ReliefProModel.Drums;
using UOMLib;
using ProII;
using ReliefProDAL;
using ReliefProModel;
using ReliefProCommon.CommonLib;
using ReliefProMain.View.DrumFires;
using ReliefProMain.View.StorageTanks;

namespace ReliefProMain.ViewModel.Drums
{
    public class DrumFireVM : ViewModelBase
    {
        public ICommand FluidCMD { get; set; }
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        public ICommand DrumSizeCMD { get; set; }
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
        private string PrzFile;
        private string PrzVersion;

        private double reliefPressure;
        private int ScenarioID;
        private DrumFireBLL fireBLL;
        private DrumFireFluid fireFluidModel;
        private DrumSize sizeModel;
        private DrumSizeVM tmpVM;
        private int FireType;
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
        /// <param name="FireType">0-DrumSize,1-TankSize</param>
        public DrumFireVM(int ScenarioID, string przFile, string version, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem, int FireType = 0)
        {
            this.FireType = FireType;
            this.ScenarioID = ScenarioID;
            this.SessionProtectedSystem = SessionPS;
            this.SessionPlant = SessionPF;
            this.PrzFile = przFile;
            this.PrzVersion = version;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
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

            UOMLib.UOMEnum uomEnum = new UOMLib.UOMEnum(SessionPF);
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
            model.dbmodel.WettedArea = UnitConvert.Convert(model.WettedAreaUnit, UOMLib.UOMEnum.Area.ToString(), model.WettedArea.Value);
            model.dbmodel.LatentHeat = UnitConvert.Convert(model.LatentHeatUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.LatentHeat.Value);
            model.dbmodel.CrackingHeat = UnitConvert.Convert(model.CrackingHeatUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.CrackingHeat.Value);
            model.dbmodel.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad.Value);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure.Value);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature.Value);
            model.dbmodel.DesignPressure = UnitConvert.Convert(model.DesignPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.DesignPressure.Value);
            
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
                DrumSizeVM vm = new DrumSizeVM(model.dbmodel.ID,SessionProtectedSystem, SessionPlant);
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
                        Area = Algorithm.GetDrumArea(sizeModel.Orientation, sizeModel.HeadType, sizeModel.Elevation.Value, sizeModel.Diameter.Value, sizeModel.Length.Value, sizeModel.NormalLiquidLevel.Value, sizeModel.BootHeight.Value, sizeModel.BootDiameter.Value);
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
                        Area = Algorithm.GetDrumArea(sizeModel.Orientation, sizeModel.HeadType, sizeModel.Elevation.Value, sizeModel.Diameter.Value, sizeModel.Length.Value, sizeModel.NormalLiquidLevel.Value, sizeModel.BootHeight.Value, sizeModel.BootDiameter.Value);
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
                double dmw = fireFluidModel.GasVaporMW.Value;
                double dp1 = fireFluidModel.PSVPressure.Value * 1.21;
                double darea = fireFluidModel.ExposedVesse.Value;
                double dtw = fireFluidModel.TW.Value;
                double dtn = fireFluidModel.NormaTemperature.Value;
                double dpn = fireFluidModel.NormalPressure.Value;



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
            double Qfire = 0;
            double Area = model.WettedArea.Value;
            //求出面积---你查看下把durmsize的 数据传进来。
            if (sizeModel == null)
            {
                DrumSizeBLL sizeBll = new DrumSizeBLL(SessionProtectedSystem, SessionPlant);
                sizeModel = sizeBll.GetSizeModel(model.dbmodel.ID);
            }
            if (sizeModel != null)
                Area = Algorithm.GetDrumArea(sizeModel.Orientation, sizeModel.HeadType, sizeModel.Elevation.Value, sizeModel.Diameter.Value, sizeModel.Length.Value, sizeModel.NormalLiquidLevel.Value, sizeModel.BootHeight.Value, sizeModel.BootDiameter.Value);
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
                double L = model.CrackingHeat.Value;
                model.ReliefLoad = Qfire / L;
                model.ReliefMW = 114;
                model.ReliefTemperature = 400;
            }
            else if (model.AllGas)
            {
                double mw = fireFluidModel.GasVaporMW.Value;
                double area = UnitConvert.Convert("m2", "ft2", fireFluidModel.ExposedVesse.Value);
                double tw = UnitConvert.Convert("C", "R", fireFluidModel.TW.Value);
                double tn = UnitConvert.Convert("C", "R", fireFluidModel.NormaTemperature.Value);
                double pn = UnitConvert.Convert("MPag", "psia", fireFluidModel.NormalPressure.Value);
                double p1 = UnitConvert.Convert("MPag", "psia", fireFluidModel.PSVPressure.Value * 1.21);
                double t1 = 0;
                double w= Algorithm.GetFullVaporW(mw, p1, area, tw, pn, tn, ref t1);
                if (t1 >= tw)
                {
                    MessageBox.Show("T1 could not be greater than TW","Message Box");
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
                    string content = PROIIFileOperator.getUsableContent(liquidStream.StreamName, DirPlant);
                    IFlashCalculate flashcalc = ProIIFactory.CreateFlashCalculate(PrzVersion);
                    string f = flashcalc.Calculate(content, 1, reliefPressure.ToString(), 6, "0.05", liquidStream, vapor, liquid, tempdir, ref ImportResult, ref RunResult);
                    IProIIReader reader = ProIIFactory.CreateReader(PrzVersion);
                    reader.InitProIIReader(f);
                    ProIIStreamData proIIvapor = reader.GetSteamInfo(vapor);
                    ProIIStreamData proIIliquid = reader.GetSteamInfo(liquid);
                    reader.ReleaseProIIReader();
                    CustomStream csVapor = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIvapor);
                    CustomStream csLiquid = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIliquid);
                    double latent = csVapor.SpEnthalpy.Value - csLiquid.SpEnthalpy.Value;

                    double reliefLoad = Qfire / latent;
                    double reliefMW = csVapor.BulkMwOfPhase.Value;
                    double reliefT = csVapor.Temperature.Value;
                    model.ReliefLoad = reliefLoad;
                    model.ReliefMW = reliefMW;
                    model.ReliefPressure = reliefPressure;
                    model.ReliefTemperature = reliefT;
                    model.LatentHeat = latent;
                    if (csVapor.BulkCPCVRatio != null)
                    {
                        model.ReliefCpCv = csVapor.BulkCPCVRatio.Value;
                    }
                    model.ReliefZ = csVapor.VaporZFmKVal.Value;


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
                    double latent = model.LatentHeat.Value;
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
            string targetUnit = "barg";
            double designPressure = UnitConvert.Convert(model.DesignPressureUnit, targetUnit, model.DesignPressure.Value);

            if (designPressure < 0)
            {
                MessageBox.Show("Design Pressure could not be negative");
                return;
            }
            if (designPressure>1.034 && selectedHeatInputModel=="API 2000")
            {
                MessageBox.Show("Becasue Design Pressure is greater,Heat Input Model must be API 521");
                SelectedHeatInputModel = "API 521";                
            }
            
            double area=model.WettedArea.Value;
            double F=1;
            double L=1;
            double T=1;
            double M=1;
            
            
            PSVDAL psvDAL = new PSVDAL();
            PSV psv = psvDAL.GetModel(SessionProtectedSystem);
            double pressure = psv.Pressure.Value;

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
                L = model.CrackingHeat.Value;
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
                    PROIIFileOperator.DecompressProIIFile(PrzFile, tempdir);
                    string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
                    IFlashCalculateW fcalc = ProIIFactory.CreateFlashCalculateW(PrzVersion);
                    string tray1_f = fcalc.Calculate(content, 1, reliefFirePressure.ToString(), 4, "", stream, vapor, liquid, rate, dirLatent, ref ImportResult, ref RunResult);
                    if (ImportResult == 1 || ImportResult == 2)
                    {
                        if (RunResult == 1 || RunResult == 2)
                        {
                            IProIIReader reader = ProIIFactory.CreateReader(PrzVersion);
                            reader.InitProIIReader(tray1_f);
                            ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                            ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                            reader.ReleaseProIIReader();
                            CustomStream vaporFire = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                            CustomStream liquidFire = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                            double latent = vaporFire.SpEnthalpy.Value - liquidFire.SpEnthalpy.Value;
                            L = latent;
                            model.LatentHeat = L;

                            T = vaporFire.Temperature.Value;
                            M = vaporFire.BulkMwOfPhase.Value;
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
                            model.ReliefMW =vaporFire.BulkMwOfPhase.Value;
                            model.ReliefPressure = reliefFirePressure;
                            model.ReliefTemperature = vaporFire.Temperature.Value;
                            if (vaporFire.BulkCPCVRatio != null)
                            {
                                model.ReliefCpCv = vaporFire.BulkCPCVRatio.Value;
                            }
                            model.ReliefZ = vaporFire.VaporZFmKVal.Value;


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
                    double latent = model.LatentHeat.Value;
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
                    FlashCalcResult flashCalcResult = flashCalcResultDAL.GetModel(SessionProtectedSystem,ScenarioID);
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
        private void Calc(object obj)
        {
            switch (FireType)
            {
                case 0: CalcDrum(); break;
                case 1: CalcTank(); break;
                default: break;
            }
        }
        private void Save(object obj)
        {
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
                if (psvModel.Pressure!=null)
                    return psvModel.Pressure.Value * psvModel.ReliefPressureFactor.Value;
            }
            return 0;
        }
    }
}
