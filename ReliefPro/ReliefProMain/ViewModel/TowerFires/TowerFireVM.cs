﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using ReliefProMain.View.TowerFires;
using ReliefProMain.Models;
using UOMLib;
using ReliefProMain.ViewModel;
using NHibernate;
using System.IO;
using ProII;
using ReliefProCommon.CommonLib;
using ReliefProMain.View;

namespace ReliefProMain.ViewModel.TowerFires
{
    public class TowerFireVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        public SourceFile SourceFileInfo { get; set; }
        public string FileFullPath { get; set; }
        private string EqName;
        public ObservableCollection<string> HeatInputModels { get; set; }
        private Latent latent;
        private int ScenarioID;
        public TowerFireModel MainModel { get; set; }

        public TowerFireEqDAL fireEqDal = new TowerFireEqDAL();
        private ObservableCollection<TowerFireEqModel> _EqList;
        public ObservableCollection<TowerFireEqModel> EqList
        {
            get
            {
                return this._EqList;
            }
            set
            {
                this._EqList = value;
                OnPropertyChanged("EqList");
            }
        }
        public UOMLib.UOMEnum uomEnum { set; get; }
        Tower TowerInfo;
        public TowerFireVM(int ScenarioID, string EqName, SourceFile sourceFileInfo, ISession sessionPlant, ISession sessionProtectedSystem, string DirPlant, string DirProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            InitUnit();

            HeatInputModels = GetHeatInputModels();
            this.SourceFileInfo = sourceFileInfo;

            this.DirPlant = DirPlant;
            this.DirProtectedSystem = DirProtectedSystem;
            this.EqName = EqName;
            this.ScenarioID = ScenarioID;
            FileFullPath = DirPlant + @"\" + sourceFileInfo.FileNameNoExt + @"\" + sourceFileInfo.FileName;
            UnitConvert uc = new UnitConvert();
            TowerFireDAL db = new TowerFireDAL();
            ReliefProModel.TowerFire model = db.GetModel(SessionProtectedSystem, ScenarioID);
           
            model.HeatInputModel = HeatInputModels[0];
            MainModel = new TowerFireModel(model);
            ReadConvert();
            TowerFireEqDAL dbtfeq = new TowerFireEqDAL();
            IList<TowerFireEq> list = dbtfeq.GetAllList(SessionProtectedSystem, MainModel.ID);
            EqList = new ObservableCollection<TowerFireEqModel>();
            foreach (TowerFireEq eq in list)
            {
                TowerFireEqModel eqm = new TowerFireEqModel(eq);
                EqList.Add(eqm);
            }

            TowerDAL towerdal = new TowerDAL();
            TowerInfo = towerdal.GetModel(SessionProtectedSystem);

            SplashScreenManager.Show();
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            if (TowerInfo.TowerType == "Distillation")
            {
                LatentDAL dblatent = new LatentDAL();
                latent = dblatent.GetModel(SessionProtectedSystem);
            }
            else if (TowerInfo.TowerType == "Absorber")
            {
                latent = GetAbsorberFireLatent();
            }
            else
            {
                latent = GetRegeneratorLatent();
            }
            SplashScreenManager.SentMsgToScreen("Calculation finished");
            SplashScreenManager.Close();
        }

        private ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(Save);

                }
                return _SaveCommand;
            }
        }

        private void Save(object window)
        {

            TowerFireDAL towerFireDAL = new TowerFireDAL();
            WriteConvert();
            MainModel.dbmodel.HeatInputModel = MainModel.HeatInputModel;
            MainModel.dbmodel.IsExist = MainModel.IsExist;
            MainModel.dbmodel.ReliefCpCv = MainModel.ReliefCpCv;
            MainModel.dbmodel.ReliefZ = MainModel.ReliefZ;
            MainModel.dbmodel.ReliefMW = MainModel.ReliefMW;
            towerFireDAL.Update(MainModel.dbmodel, SessionProtectedSystem);
            SessionProtectedSystem.Flush();

            ScenarioDAL scenarioDAL = new ScenarioDAL();
            Scenario sc = scenarioDAL.GetModel(ScenarioID, SessionProtectedSystem);
            sc.ReliefLoad = MainModel.dbmodel.ReliefLoad;
            sc.ReliefMW = MainModel.ReliefMW;
            sc.ReliefPressure = MainModel.dbmodel.ReliefPressure;
            sc.ReliefTemperature = MainModel.dbmodel.ReliefTemperature;
            sc.ReliefCpCv = MainModel.ReliefCpCv;
            sc.ReliefZ = MainModel.ReliefZ;
            scenarioDAL.Update(sc, SessionProtectedSystem);
            SessionProtectedSystem.Flush();


            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        public ObservableCollection<string> GetHeatInputModels()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("API 521");
            return list;
        }

        private ICommand _EditClick;
        public ICommand EditClick
        {
            get
            {
                if (_EditClick == null)
                {
                    _EditClick = new RelayCommand(Edit);

                }
                return _EditClick;
            }
        }
        private void Edit(object obj)
        {
            CalReliefLoad(obj);
        }


        private ICommand _TotalClick;
        public ICommand TotalClick
        {
            get
            {
                if (_TotalClick == null)
                {
                    _TotalClick = new RelayCommand(Run);

                }
                return _TotalClick;
            }
        }

        private void Run(object obj)
        {
            try
            {
                SplashScreenManager.Show();
                double reliefload = 0;
                foreach (TowerFireEqModel eqm in EqList)
                {
                    if (eqm.FireZone)
                    {
                        reliefload = reliefload + eqm.ReliefLoad;
                    }
                }
                MainModel.ReliefLoad = reliefload;

                if (TowerInfo.TowerType == "Distillation")
                {
                    CalDisReliefOther();
                }
                else if (TowerInfo.TowerType == "Absorber")
                {
                    //求latent 时已经计算过了
                }
                else
                {
                    //求latent 时已经计算过了
                }
                SplashScreenManager.SentMsgToScreen("Calculation finished");
            }
            catch { }
            finally
            {
                SplashScreenManager.Close();
            }

        }

        private void ReadConvert()
        {
            if (MainModel.ReliefLoad != null)
                MainModel.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, reliefloadUnit, MainModel.dbmodel.ReliefLoad);
            if (MainModel.ReliefPressure != null)
                MainModel.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, reliefPressureUnit, MainModel.dbmodel.ReliefPressure);
            if (MainModel.ReliefTemperature != null)
                MainModel.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, reliefTemperatureUnit, MainModel.dbmodel.ReliefTemperature);
        }
        private void WriteConvert()
        {
            if (MainModel.ReliefLoad != null)
                MainModel.dbmodel.ReliefLoad = UnitConvert.Convert(reliefloadUnit, UOMEnum.MassRate, MainModel.ReliefLoad);
            if (MainModel.ReliefPressure != null)
                MainModel.dbmodel.ReliefPressure = UnitConvert.Convert(reliefPressureUnit, UOMEnum.Pressure, MainModel.ReliefPressure);
            if (MainModel.ReliefTemperature != null)
                MainModel.dbmodel.ReliefTemperature = UnitConvert.Convert(reliefTemperatureUnit, UOMEnum.Temperature, MainModel.ReliefTemperature);
        }
        private void InitUnit()
        {
            this.reliefloadUnit = uomEnum.UserMassRate;
            this.reliefPressureUnit = uomEnum.UserPressure;
            this.reliefTemperatureUnit = uomEnum.UserTemperature;
        }
        #region 单位-字段
        private string reliefloadUnit;
        public string ReliefLoadUnit
        {
            get { return reliefloadUnit; }
            set
            {
                reliefloadUnit = value;
                this.OnPropertyChanged("ReliefLoadUnit");
            }
        }

        private string reliefTemperatureUnit;
        public string ReliefTemperatureUnit
        {
            get { return reliefTemperatureUnit; }
            set
            {
                reliefTemperatureUnit = value;
                this.OnPropertyChanged("ReliefTemperatureUnit");
            }
        }

        private string reliefPressureUnit;
        public string ReliefPressureUnit
        {
            get { return reliefPressureUnit; }
            set
            {
                reliefPressureUnit = value;
                this.OnPropertyChanged("ReliefPressureUnit");
            }
        }
        #endregion


        private void CalReliefLoad(object obj)
        {
            double C1 = 0;
            if (MainModel.IsExist)
            {
                C1 = 43200;
            }
            else
            {
                C1 = 70900;
            }
            int id = int.Parse(obj.ToString());

            var query=from s in EqList
                                  where s.ID == id
                                  select s;

            TowerFireEqModel eqm = query.ToList()[0];

            double latentEnthalpy = latent.LatentEnthalpy;
            if (eqm.Type == "Column" || eqm.Type == "Side Column")
            {
                TowerFireColumnView v = new TowerFireColumnView();
                TowerFireColumnVM vm = new TowerFireColumnVM(eqm.ID, SessionPlant, SessionProtectedSystem);
                v.DataContext = vm;
                v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                if (v.ShowDialog() == true)
                {
                    eqm.WettedArea = vm.Area;
                    eqm.HeatInput = Algorithm.GetQ(C1, eqm.FFactor, eqm.WettedArea);
                    eqm.ReliefLoad = (eqm.HeatInput / latentEnthalpy);
                    eqm.Elevation = vm.model.Elevation;

                    eqm.dbmodel.WettedArea = eqm.WettedArea;
                    eqm.dbmodel.HeatInput = eqm.HeatInput;
                    eqm.dbmodel.ReliefLoad = eqm.HeatInput;
                    eqm.dbmodel.FFactor = eqm.FFactor;
                    eqm.dbmodel.FireZone = eqm.FireZone;
                    eqm.dbmodel.Elevation = eqm.Elevation;
                    fireEqDal.Update(eqm.dbmodel, SessionProtectedSystem);
                    SessionProtectedSystem.Flush();
                    
                   
                }
            }
            else if (eqm.Type == "Drum")
            {
                TowerFireDrumView v = new TowerFireDrumView();
                TowerFireDrumVM vm = new TowerFireDrumVM(eqm.ID, SessionPlant, SessionProtectedSystem);
                v.DataContext = vm;
                v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                if (v.ShowDialog() == true)
                {
                    eqm.WettedArea = vm.Area;
                    eqm.HeatInput = Algorithm.GetQ(C1, eqm.FFactor, eqm.WettedArea);
                    eqm.ReliefLoad = (eqm.HeatInput / latentEnthalpy);
                    eqm.Elevation = vm.model.Elevation;

                    eqm.dbmodel.WettedArea = eqm.WettedArea;
                    eqm.dbmodel.HeatInput = eqm.HeatInput;
                    eqm.dbmodel.ReliefLoad = eqm.ReliefLoad;
                    eqm.dbmodel.FFactor = eqm.FFactor;
                    eqm.dbmodel.FireZone = eqm.FireZone;
                    eqm.dbmodel.Elevation = eqm.Elevation;
                    fireEqDal.Update(eqm.dbmodel, SessionProtectedSystem);
                    SessionProtectedSystem.Flush();
                    
                }
            }
            else if (eqm.Type == "Shell-Tube HX")
            {
                TowerFireHXView v = new TowerFireHXView();
                TowerFireHXVM vm = new TowerFireHXVM(eqm.ID, SessionPlant, SessionProtectedSystem);
                v.DataContext = vm;
                v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                if (v.ShowDialog() == true)
                {
                    eqm.WettedArea = vm.Area;
                    eqm.HeatInput = Algorithm.GetQ(C1, eqm.FFactor, eqm.WettedArea);
                    eqm.ReliefLoad = (eqm.HeatInput / latentEnthalpy);
                    eqm.Elevation = vm.model.Elevation;

                    eqm.dbmodel.WettedArea = eqm.WettedArea;
                    eqm.dbmodel.HeatInput = eqm.HeatInput;
                    eqm.dbmodel.ReliefLoad = eqm.HeatInput;
                    eqm.dbmodel.FFactor = eqm.FFactor;
                    eqm.dbmodel.FireZone = eqm.FireZone;
                    eqm.dbmodel.Elevation = eqm.Elevation;
                    fireEqDal.Update(eqm.dbmodel, SessionProtectedSystem);
                    SessionProtectedSystem.Flush();
                }
            }
            else if (eqm.Type == "Air Cooler")
            {
                AreasView v = new AreasView();
                AreaVM vm = new AreaVM(eqm.ID, SessionPlant, SessionProtectedSystem);
                v.DataContext = vm;
                v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                if (v.ShowDialog() == true)
                {
                    eqm.WettedArea = vm.Area;
                    eqm.HeatInput = Algorithm.GetQ(C1, eqm.FFactor, eqm.WettedArea);
                    eqm.ReliefLoad = (eqm.HeatInput / latentEnthalpy);
                    
                    eqm.dbmodel.WettedArea = eqm.WettedArea;
                    eqm.dbmodel.HeatInput = eqm.HeatInput;
                    eqm.dbmodel.ReliefLoad = eqm.HeatInput;
                    eqm.dbmodel.FFactor = eqm.FFactor;
                    eqm.dbmodel.FireZone = eqm.FireZone;
                    fireEqDal.Update(eqm.dbmodel, SessionProtectedSystem);
                    SessionProtectedSystem.Flush();
                    
                }
            }
            //else if (eq.Type == "Other HX")
            //{
            //TowerFireOtherView v = new TowerFireOtherView();
            //TowerFireOtherVM vm = new TowerFireOtherVM(eq.ID, SessionPlant, SessionProtectedSystem);
            //v.DataContext = vm;
            //v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            //if (v.ShowDialog() == true)
            //{
            //    EqList.Clear();
            //    eq.WettedArea = vm.Area.ToString();
            //    eq.HeatInput = Algorithm.GetQ(C1, double.Parse(eq.FFactor), double.Parse(eq.WettedArea)).ToString();
            //    eq.ReliefLoad = (double.Parse(eq.HeatInput) / latentEnthalpy).ToString();
            //    db.Update(eq, SessionProtectedSystem);
            //    IList<TowerFireEq> list = db.GetAllList(SessionProtectedSystem, MainModel.ID);
            //    foreach (TowerFireEq q in list)
            //    {
            //        EqList.Add(q);
            //    }
            //    SessionProtectedSystem.Flush();
            //}
            // }
        }

        private void CalDisReliefOther()
        {
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            PSVDAL psvDAL = new PSVDAL();
            PSV psv = psvDAL.GetModel(SessionProtectedSystem);
            double pressure = psv.Pressure;

            double reliefFirePressure = pressure * 1.21;
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "TowerFire";
            if (!Directory.Exists(dirLatent))
                Directory.CreateDirectory(dirLatent);

            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
            reader.InitProIIReader(FileFullPath);
            ProIIStreamData proIITray1StreamData = reader.CopyStream(EqName, 1, 2, 1);
            reader.ReleaseProIIReader();
            CustomStream stream = ProIIToDefault.ConvertProIIStreamToCustomStream(proIITray1StreamData);


            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            string tray1_f = fcalc.Calculate(content, 1, reliefFirePressure.ToString(), 4, "", stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                    reader.ReleaseProIIReader();
                    CustomStream vaporFire = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                    //MainModel.ReliefLoad = vaporFire.WeightFlow;
                    MainModel.ReliefMW = vaporFire.BulkMwOfPhase;
                    MainModel.ReliefPressure = reliefFirePressure;
                    MainModel.ReliefTemperature = vaporFire.Temperature;
                    MainModel.ReliefCpCv = vaporFire.BulkCPCVRatio;
                    MainModel.ReliefZ = vaporFire.VaporZFmKVal;

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

        private Latent GetAbsorberFireLatent()
        {
            if (MainModel.ReliefMW == 0)
            {

                Latent lt = new Latent();
                PSVDAL psvDAL = new PSVDAL();
                PSV psv = psvDAL.GetModel(SessionProtectedSystem);
                double pressure = psv.Pressure;

                double reliefFirePressure = pressure * 1.21;
                string tempdir = DirProtectedSystem + @"\temp\";
                string dirLatent = tempdir + "TowerFire";
                if (!Directory.Exists(dirLatent))
                    Directory.CreateDirectory(dirLatent);

                SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
                IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                reader.InitProIIReader(FileFullPath);
                ProIIStreamData proIITray1StreamData = reader.CopyStream(EqName, TowerInfo.StageNumber, 2, 1);
                reader.ReleaseProIIReader();
                CustomStream stream = ProIIToDefault.ConvertProIIStreamToCustomStream(proIITray1StreamData);


                string gd = Guid.NewGuid().ToString();
                string vapor = "S_" + gd.Substring(0, 5).ToUpper();
                string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
                int ImportResult = 0;
                int RunResult = 0;
                PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
                string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
                IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                string tray1_f = fcalc.Calculate(content, 1, reliefFirePressure.ToString(), 6, "0.05", stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
                if (ImportResult == 1 || ImportResult == 2)
                {
                    if (RunResult == 1 || RunResult == 2)
                    {
                        reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                        reader.InitProIIReader(tray1_f);
                        ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                        ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                        reader.ReleaseProIIReader();
                        CustomStream vaporFire = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                        CustomStream liquidFire = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                        //MainModel.ReliefLoad = vaporFire.WeightFlow;
                        MainModel.ReliefMW = vaporFire.BulkMwOfPhase;
                        MainModel.ReliefPressure = reliefFirePressure;
                        MainModel.ReliefTemperature = vaporFire.Temperature;
                        MainModel.ReliefCpCv = vaporFire.BulkCPCVRatio;
                        MainModel.ReliefZ = vaporFire.VaporZFmKVal;
                        lt.LatentEnthalpy = vaporFire.SpEnthalpy - liquidFire.SpEnthalpy;
                        return lt;
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
            return null;
        }

        private Latent GetRegeneratorLatent()
        {
            if (MainModel.ReliefMW == 0)
            {
                Latent lt = new Latent();
                PSVDAL psvDAL = new PSVDAL();
                PSV psv = psvDAL.GetModel(SessionProtectedSystem);
                double pressure = psv.Pressure;

                double reliefFirePressure = pressure * 1.21;
                string tempdir = DirProtectedSystem + @"\temp\";
                string dirLatent = tempdir + "TowerFire";
                if (!Directory.Exists(dirLatent))
                    Directory.CreateDirectory(dirLatent);
                CustomStreamDAL csdal = new CustomStreamDAL();
                IList<CustomStream> list = csdal.GetAllList(SessionProtectedSystem, false);
                List<CustomStream> feedlist = list.ToList();
                string gd = Guid.NewGuid().ToString();
                string vapor = "S_" + gd.Substring(0, 5).ToUpper();
                string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
                int ImportResult = 0;
                int RunResult = 0;
                PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
                StringBuilder sbcontent = new StringBuilder();
                foreach (CustomStream cs in feedlist)
                {
                    string content = PROIIFileOperator.getUsableContent(cs.StreamName, tempdir);
                    sbcontent.Append(content).Append("\n");
                }
                IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                string tray1_f = fcalc.Calculate(sbcontent.ToString(), 1, reliefFirePressure.ToString(), 6, "0.05", feedlist, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
                if (ImportResult == 1 || ImportResult == 2)
                {
                    if (RunResult == 1 || RunResult == 2)
                    {
                        IProIIReader reader;
                        reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                        reader.InitProIIReader(tray1_f);
                        ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                        ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                        reader.ReleaseProIIReader();
                        CustomStream vaporFire = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                        CustomStream liquidFire = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                        //MainModel.ReliefLoad = vaporFire.WeightFlow;
                        MainModel.ReliefMW = vaporFire.BulkMwOfPhase;
                        MainModel.ReliefPressure = reliefFirePressure;
                        MainModel.ReliefTemperature = vaporFire.Temperature;
                        MainModel.ReliefCpCv = vaporFire.BulkCPCVRatio;
                        MainModel.ReliefZ = vaporFire.VaporZFmKVal;
                        lt.LatentEnthalpy = vaporFire.SpEnthalpy - liquidFire.SpEnthalpy;
                        return lt;
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
            return null; 
        }
    }
}
