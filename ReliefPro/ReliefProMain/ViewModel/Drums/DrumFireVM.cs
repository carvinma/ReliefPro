﻿using System;
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
using ReliefProModel.Drums;
using UOMLib;
using ProII;
using ReliefProDAL;
using ReliefProModel;
using ReliefProCommon.CommonLib;
using ReliefProMain.View.DrumFires;

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
            DrumSizeView win = new DrumSizeView();
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            DrumSizeVM vm = new DrumSizeVM(model.dbmodel.ID, SessionPS, SessionPF);
            win.DataContext = vm;
            if (win.ShowDialog() == true)
            {
                sizeModel = vm.model.dbmodel;
                double Area = 0;
                if (sizeModel == null)
                {
                    DrumSizeBLL sizeBll = new DrumSizeBLL(SessionPS, SessionPF);
                    sizeModel = sizeBll.GetSizeModel(model.dbmodel.ID);
                }
                if (sizeModel != null)
                    Area = Algorithm.GetDrumArea(sizeModel.Orientation, sizeModel.HeadType, sizeModel.Elevation, sizeModel.Diameter, sizeModel.Length, sizeModel.NormalLiquidLevel, sizeModel.BootHeight, sizeModel.BootDiameter);
                model.WettedArea = Area;
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
            DrumFireFluidVM vm = new DrumFireFluidVM(model.dbmodel.ID, SessionPS, SessionPF);
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
                double dt1 = UnitConvert.Convert("T", "R", "C", t1);
                double dreliefLoad = UnitConvert.Convert("MR", "lb/hr", "kg/hr", load);

                //泄放结果。 老李需要保存到数据库里。
                double reliefPressure = dp1;
                double reliefMW = dmw;
                double reliefLoad = dreliefLoad;
                double reliefTemperature = dt1;

            }
        }
        private void Calc(object obj)
        {
            double Qfire = 0;
            double Area = model.WettedArea;
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
            CustomStreamDAL dbcs = new CustomStreamDAL();
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
            PSVDAL psv = new PSVDAL();
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
