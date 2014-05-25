using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Commands;
using ReliefProMain.Model;
using ReliefProMain.View;
using ReliefProModel.Drum;
using UOMLib;
using ProII;
using ReliefProDAL;
using ReliefProModel;
using ReliefProCommon.CommonLib;
namespace ReliefProMain.ViewModel.Drum
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
                OnPropertyChanged("SelectedHeatInputModel");
            }
        }
        public List<string> lstHeatInputModel { get; set; }
        public DrumFireModel model { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        private string PrzFile;
        private string PrzVersion;

        private double reliefPressure;
        private int ScenarioID;
        private DrumFireBLL fireBLL;
        private DrumFireFluid fireFluidModel;
        private DrumSize sizeModel;
        public DrumFireVM(int ScenarioID, string przFile, string version, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem)
        {
            this.ScenarioID = ScenarioID;
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
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
            lstHeatInputModel.Add("API 2000");

            fireBLL = new DrumFireBLL(SessionPS, SessionPF);
            var fireModel = fireBLL.GetDrumFireModel(ScenarioID);
            fireModel = fireBLL.ReadConvertModel(fireModel);
            if (!string.IsNullOrEmpty(fireModel.HeatInputModel))
                selectedHeatInputModel = fireModel.HeatInputModel;

            UOMLib.UOMEnum uomEnum = new UOMLib.UOMEnum(SessionPF);
            model = new DrumFireModel(fireModel);
            model.WettedAreaUnit = uomEnum.UserArea;
            model.LatentHeatUnit = uomEnum.UserSpecificEnthalpy;
            model.CrackingHeatUnit = uomEnum.UserSpecificEnthalpy;
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefPressureUnit = uomEnum.UserPressure;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;

            reliefPressure = ScenarioReliefPressure(SessionPS);
        }
        private void WriteConvertModel()
        {
            UnitConvert uc = new UnitConvert();
            model.dbmodel.WettedArea = uc.Convert(model.WettedAreaUnit, UOMLib.UOMEnum.Area.ToString(), model.WettedArea);
            model.dbmodel.LatentHeat = uc.Convert(model.LatentHeatUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.LatentHeat);
            model.dbmodel.CrackingHeat = uc.Convert(model.CrackingHeatUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.CrackingHeat);
            model.dbmodel.ReliefLoad = uc.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad);
            model.dbmodel.ReliefPressure = uc.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
            model.dbmodel.ReliefTemperature = uc.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
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
            Drum_Size win = new Drum_Size();
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            DrumSizeVM vm = new DrumSizeVM(model.dbmodel.ID, SessionPS, SessionPF);
            win.DataContext = vm;
            if (win.ShowDialog() == true)
            {
                sizeModel = vm.model.dbmodel;
            }
        }

        private void OpenFluidWin(object obj)
        {
            Drum_fireFluid win = new Drum_fireFluid();
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            DrumFireFluidVM vm = new DrumFireFluidVM(model.dbmodel.ID, SessionPS, SessionPF);
            win.DataContext = vm;
            if (win.ShowDialog() == true)
            {
                fireFluidModel = vm.model.dbmodel;
            }
        }
        private void Calc(object obj)
        {
            double Qfire = 0;
            double Area = 0;
            //求出面积---你查看下把durmsize的 数据传进来。
            if (sizeModel == null)
            {
                DrumSizeBLL sizeBll = new DrumSizeBLL(SessionPS, SessionPF);
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


            //取出liquid stream
            dbCustomStream dbcs = new dbCustomStream();
            IList<CustomStream> listStream = dbcs.GetAllList(SessionPS, true);
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
            string content = PROIIFileOperator.getUsableContent(liquidStream.StreamName, DirPlant);
            IFlashCalculate flashcalc = ProIIFactory.CreateFlashCalculate(PrzVersion);
            string f = flashcalc.Calculate(content, 1, reliefPressure.ToString(), 6, "0.05", liquidStream, vapor, liquid,  tempdir);
            IProIIReader reader = ProIIFactory.CreateReader(PrzVersion);
            reader.InitProIIReader(f);
            ProIIStreamData proIIvapor = reader.GetSteamInfo(vapor);
            ProIIStreamData proIIliquid = reader.GetSteamInfo(liquid);
            reader.ReleaseProIIReader();
            CustomStream csVapor = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIvapor);
            CustomStream csLiquid = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIliquid);
            double latent = double.Parse(csVapor.SpEnthalpy) - double.Parse(csLiquid.SpEnthalpy);

            double reliefLoad = Qfire / latent;
            double reliefMW = double.Parse(csVapor.BulkMwOfPhase);
            double reliefT = double.Parse(csVapor.Temperature);

        }
        private void Save(object obj)
        {
            WriteConvertModel();
            fireBLL.SaveData(model.dbmodel, fireFluidModel, sizeModel, SessionPS);
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
            dbPSV psv = new dbPSV();
            var psvModel = psv.GetAllList(SessionPS).FirstOrDefault();
            if (psvModel != null)
            {
                if (!string.IsNullOrEmpty(psvModel.Pressure))
                    return double.Parse(psvModel.Pressure) * double.Parse(psvModel.ReliefPressureFactor);
            }
            return 0;
        }
    }
}
