using System;
using System.Collections.Generic;
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
using ReliefProMain.View;
using UOMLib;
using NHibernate;
using System.Windows;
using System.IO;
using ReliefProCommon.CommonLib;
using ProII;

namespace ReliefProMain.ViewModel
{
    /// <summary>
    /// IsSteamFreezed 的判断
    /// condeser =0,则不被冷凝。 
    /// condeser factor =1 && stop=false 被冷凝
    /// 2者之间的。  比较大小，那个大，取那个。 返回结果后，判断是否冷凝。

    /// 如果是steam，则扣除它，然后，再按之前算法扣除。

    /// </summary>
    public class TowerScenarioCalcVM : ViewModelBase
    {
        private double _ReliefLoad;
        public double ReliefLoad
        {
            get
            {
                return this._ReliefLoad;
            }
            set
            {
                this._ReliefLoad = value;

                OnPropertyChanged("ReliefLoad");
            }
        }

        private double _ReliefMW;
        public double ReliefMW
        {
            get
            {
                return this._ReliefMW;
            }
            set
            {
                this._ReliefMW = value;

                OnPropertyChanged("ReliefMW");
            }
        }
        private double _ReliefTemperature;
        public double ReliefTemperature
        {
            get
            {
                return this._ReliefTemperature;
            }
            set
            {
                this._ReliefTemperature = value;

                OnPropertyChanged("ReliefTemperature");
            }
        }
        private double _ReliefPressure;
        public double ReliefPressure
        {
            get
            {
                return this._ReliefPressure;
            }
            set
            {
                this._ReliefPressure = value;

                OnPropertyChanged("ReliefPressure");
            }
        }
        private double _ReliefCpCv;
        public double ReliefCpCv
        {
            get
            {
                return this._ReliefCpCv;
            }
            set
            {
                this._ReliefCpCv = value;

                OnPropertyChanged("ReliefCpCv");
            }
        }
        private double _ReliefZ;
        public double ReliefZ
        {
            get
            {
                return this._ReliefZ;
            }
            set
            {
                this._ReliefZ = value;

                OnPropertyChanged("ReliefZ");
            }
        }
        public int ScenarioID { set; get; }
        public ISession SessionPlant { set; get; }
        public ISession SessionProtectedSystem { set; get; }
        public string DirPlant { set; get; }
        public string DirProtectedSystem { set; get; }
        public SourceFile SourceFileInfo { get; set; }
        private int SteamFreezed = 0;
        private TowerScenarioHXDAL towerScenarioHXDAL;
        private Scenario CurrentScenario = null;
        private bool IsSteamFreezed = false;
        UOMLib.UOMEnum uomEnum;
        private string ScenarioName;
        private string EqName;
        Tower tower;
        public TowerScenarioCalcVM(string EqName, string ScenarioName, int scenarioID, SourceFile sourceFileInfo, ISession sessionPlant, ISession sessionProtectedSystem, string DirPlant, string DirProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            InitUnit();
            ScenarioID = scenarioID;

            SourceFileInfo = sourceFileInfo;
            this.ScenarioName = ScenarioName;
            this.EqName = EqName;
            this.DirPlant = DirPlant;
            this.DirProtectedSystem = DirProtectedSystem;

            towerScenarioHXDAL = new TowerScenarioHXDAL();
            ScenarioDAL dbsc = new ScenarioDAL();
            CurrentScenario = dbsc.GetModel(ScenarioID, SessionProtectedSystem);
            ReliefLoad = CurrentScenario.ReliefLoad;
            ReliefMW = CurrentScenario.ReliefMW;
            ReliefPressure = CurrentScenario.ReliefPressure;
            ReliefTemperature = CurrentScenario.ReliefTemperature;
            ReliefCpCv = CurrentScenario.ReliefCpCv;
            ReliefZ = CurrentScenario.ReliefZ;
            SteamFreezed = CheckSteamFreezed();
            ReadConvert();

        }
        private int CheckSteamFreezed()
        {
            int SteamFreezed = 0;
            TowerHXDetailDAL detaildal = new TowerHXDetailDAL();

            IList<TowerScenarioHX> list = towerScenarioHXDAL.GetAllList(SessionProtectedSystem, ScenarioID, 1);
            if (list.Count == 0)
                SteamFreezed = 0;
            double sumDutyFactor = 0;
            foreach (TowerScenarioHX shx in list)
            {
                TowerHXDetail detail = detaildal.GetModel(SessionProtectedSystem, shx.DetailID);
                if (!shx.DutyLost)
                {
                    sumDutyFactor = sumDutyFactor + shx.DutyCalcFactor*detail.DutyPercentage;
                }
            }
            if (sumDutyFactor == 0)
                SteamFreezed = 0;
            else if (sumDutyFactor == 1)
                SteamFreezed = 1;
            else
                SteamFreezed = 2;


            return SteamFreezed;
        }

        private ICommand _ProductCommand;
        public ICommand ProductCommand
        {
            get
            {
                if (_ProductCommand == null)
                {
                    _ProductCommand = new RelayCommand(Product);

                }
                return _ProductCommand;
            }
        }

        private ICommand _FeedCommand;
        public ICommand FeedCommand
        {
            get
            {
                if (_FeedCommand == null)
                {
                    _FeedCommand = new RelayCommand(Feed);

                }
                return _FeedCommand;
            }
        }

        private void Feed(object window)
        {
            TowerScenarioFeedView v = new TowerScenarioFeedView();
            TowerScenarioFeedVM vm = new TowerScenarioFeedVM(ScenarioID, SourceFileInfo, SessionPlant, SessionProtectedSystem,false);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.ShowDialog();
        }
        private void Product(object window)
        {
            TowerScenarioProductView v = new TowerScenarioProductView();
            TowerScenarioFeedVM vm = new TowerScenarioFeedVM(ScenarioID, SourceFileInfo, SessionPlant, SessionProtectedSystem,true);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.ShowDialog();
        }
        private ICommand _CondenserCommand;
        public ICommand CondenserCommand
        {
            get
            {
                if (_CondenserCommand == null)
                {
                    _CondenserCommand = new RelayCommand(Condenser);

                }
                return _CondenserCommand;
            }
        }

        private void Condenser(object window)
        {
            TowerScenarioHXView v = new TowerScenarioHXView();
            v.Title = "Condenser";
            TowerScenarioHXVM vm = new TowerScenarioHXVM(1, ScenarioID, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.ShowDialog();
        }

        private ICommand _ReboilerCommand;
        public ICommand ReboilerCommand
        {
            get
            {
                if (_ReboilerCommand == null)
                {
                    _ReboilerCommand = new RelayCommand(Reboiler);

                }
                return _ReboilerCommand;
            }
        }

        private void Reboiler(object window)
        {
            TowerScenarioHXView v = new TowerScenarioHXView();
            v.Title = "Reboiler";
            TowerScenarioHXVM vm = new TowerScenarioHXVM(3, ScenarioID, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.ShowDialog();
        }

        private ICommand _PumparoundHeatingCommand;
        public ICommand PumparoundHeatingCommand
        {
            get
            {
                if (_PumparoundHeatingCommand == null)
                {
                    _PumparoundHeatingCommand = new RelayCommand(PumparoundHeating);

                }
                return _PumparoundHeatingCommand;
            }
        }

        private void PumparoundHeating(object window)
        {
            TowerScenarioHXView v = new TowerScenarioHXView();
            v.Title = "Pumparound Heating";
            TowerScenarioHXVM vm = new TowerScenarioHXVM(4, ScenarioID, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.ShowDialog();
        }

        private ICommand _PumparoundCoolingCommand;
        public ICommand PumparoundCoolingCommand
        {
            get
            {
                if (_PumparoundCoolingCommand == null)
                {
                    _PumparoundCoolingCommand = new RelayCommand(PumparoundCooling);

                }
                return _PumparoundCoolingCommand;
            }
        }

        private void PumparoundCooling(object window)
        {
            TowerScenarioHXView v = new TowerScenarioHXView();
            v.Title = "Pumparound Cooling";
            TowerScenarioHXVM vm = new TowerScenarioHXVM(2, ScenarioID, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.ShowDialog();
        }
        private ICommand _CalcCommand;
        public ICommand CalcCommand
        {
            get
            {
                if (_CalcCommand == null)
                {
                    _CalcCommand = new RelayCommand(Calc);

                }
                return _CalcCommand;
            }
        }

        private void Calc(object window)
        {
            TowerDAL towerdal = new TowerDAL();
            tower = towerdal.GetModel(SessionProtectedSystem);
            if (tower.TowerType == "Distillation")
            {
                CalcDistillation();
            }
            else if (tower.TowerType == "Absorbent Regenerator")
            {
                CalcRegenerator();
            }

        }

        private void Balance()
        {
            SourceDAL sourceDAL = new SourceDAL();
            TowerScenarioStreamDAL db = new TowerScenarioStreamDAL();
            StreamDAL dbstream = new StreamDAL();
            TowerFlashProductDAL dbtfp = new ReliefProDAL.TowerFlashProductDAL();
            IList<TowerScenarioStream> feeds = db.GetAllList(SessionProtectedSystem, ScenarioID, false);
            IList<TowerScenarioStream> products = db.GetAllList(SessionProtectedSystem, ScenarioID, true);
            double Total = 0;
            double steamTotal = 0;
            double currentTotal = 0;
            double diffTotal = 0;
            foreach (TowerScenarioStream s in feeds)
            {
                CustomStream cs = dbstream.GetModel(SessionProtectedSystem, s.StreamName);
                double wf = cs.WeightFlow;
                Total = Total + wf;
                if (!s.FlowStop)
                {
                    currentTotal = currentTotal + wf * s.FlowCalcFactor;
                    Source source = sourceDAL.GetModel(s.StreamName, SessionProtectedSystem);
                    if (source.IsSteam)
                        steamTotal = steamTotal + wf * s.FlowCalcFactor;
                }
            }
            diffTotal = Total - currentTotal;

            int count = 0;
            IList<TowerFlashProduct> listP = dbtfp.GetAllList(SessionProtectedSystem);

            if (steamTotal > 0)
            {
                IList<TowerFlashProduct> listP0 = (from p in listP
                                                   where (p.ProdType == "6")
                                                   orderby p.SpEnthalpy descending
                                                   select p).ToList();

                count = listP0.Count;
                for (int i = 0; i < count; i++)
                {
                    TowerFlashProduct p = listP0[i];
                    TowerScenarioStream s = (from m in products
                                             where m.StreamName == p.StreamName
                                             select m).SingleOrDefault();
                    double factor = 1;
                    double flowrate = p.WeightFlow;
                    double tempH = factor * flowrate;
                    if (tempH >= diffTotal)
                    {
                        double tempfactor = (tempH - diffTotal) / tempH;
                        s.FlowCalcFactor = tempfactor;
                        diffTotal = 0;
                        db.Update(s, SessionProtectedSystem);
                        break;
                    }
                    else
                    {
                        s.FlowCalcFactor = 0;
                        db.Update(s, SessionProtectedSystem);
                        diffTotal = diffTotal - tempH;
                    }
                }
            }
            if (diffTotal > 0)
            {
                IList<TowerFlashProduct> listP1 = (from p in listP
                                                   where (p.ProdType != "3" || p.ProdType != "4" || p.ProdType != "6")
                                                   orderby p.SpEnthalpy descending
                                                   select p).ToList();

                count = listP1.Count;
                for (int i = 0; i < count; i++)
                {
                    TowerFlashProduct p = listP1[i];
                    TowerScenarioStream s = (from m in products
                                             where m.StreamName == p.StreamName
                                             select m).SingleOrDefault();
                    double factor = 1;
                    double flowrate = p.WeightFlow;
                    double tempH = factor * flowrate;
                    if (tempH >= diffTotal)
                    {
                        double tempfactor = (tempH - diffTotal) / tempH;
                        s.FlowCalcFactor = tempfactor;
                        diffTotal = 0;
                        db.Update(s, SessionProtectedSystem);
                        break;
                    }
                    else
                    {
                        s.FlowCalcFactor = 0;
                        db.Update(s, SessionProtectedSystem);
                        diffTotal = diffTotal - tempH;
                    }
                }
            }
            if (diffTotal > 0)
            {
                IList<TowerFlashProduct> listP2 = (from p in listP
                                                   where (p.ProdType == "3" || p.ProdType == "4")
                                                   orderby p.SpEnthalpy descending
                                                   select p).ToList();

                count = listP2.Count;
                for (int i = 0; i < count; i++)
                {
                    TowerFlashProduct p = listP2[i];
                    TowerScenarioStream s = (from m in products
                                             where m.StreamName == p.StreamName
                                             select m).SingleOrDefault();
                    double factor = 1;
                    double flowrate = p.WeightFlow;
                    double tempH = factor * flowrate;
                    if (tempH >= diffTotal)
                    {
                        double tempfactor = (tempH - diffTotal) / tempH;
                        s.FlowCalcFactor = tempfactor;
                        db.Update(s, SessionProtectedSystem);
                        diffTotal = 0;
                        break;
                    }
                    else
                    {
                        s.FlowCalcFactor = 0;
                        db.Update(s, SessionProtectedSystem);
                        diffTotal = diffTotal - tempH;
                    }
                }
            }
            SessionProtectedSystem.Flush();


        }

        private void CalcDistillation()
        {
            double reliefLoad = 0;
            double reliefMW = 0;
            double reliefTemperature = 0;
            double reliefPressure = 0;
            if (SteamFreezed == 0)
            {
                SteamNotFreezedMethod(ref  reliefLoad, ref  reliefMW, ref  reliefTemperature, ref  reliefPressure);
            }
            else if (SteamFreezed == 1)
            {
                SteamFreezedMethod(ref  reliefLoad, ref  reliefMW, ref  reliefTemperature, ref  reliefPressure);
            }
            else
            {
                double reliefLoad1 = 0;
                double reliefMW1 = 0;
                double reliefTemperature1 = 0;
                double reliefPressure1 = 0;
                SteamNotFreezedMethod(ref  reliefLoad, ref  reliefMW, ref  reliefTemperature, ref  reliefPressure);
                SteamFreezedMethod(ref  reliefLoad1, ref  reliefMW1, ref  reliefTemperature1, ref  reliefPressure1);
                if (reliefLoad < reliefLoad1)
                {
                    reliefLoad = reliefLoad1;
                    reliefMW = reliefMW1;
                    reliefTemperature = reliefTemperature1;
                    reliefPressure = reliefPressure1;
                }

            }
            ReliefTemperature = reliefTemperature;
            ReliefPressure = reliefPressure;
            ReliefLoad = reliefLoad;
            ReliefMW = reliefMW;
        }

        private void CalcRegenerator()
        {
            LatentDAL ltdal = new LatentDAL();
            TowerScenarioHXDAL dbTSHX = new TowerScenarioHXDAL();
            TowerHXDetailDAL dbDetail = new TowerHXDetailDAL();
            ReboilerPinchDAL reboilerPinchDAL = new ReboilerPinchDAL();
            IList<TowerScenarioHX> list = dbTSHX.GetAllList(SessionProtectedSystem, ScenarioID);
            double rduty = 0;
            bool CondenserLost = true;
            foreach (TowerScenarioHX shx in list)
            {
                if (!shx.DutyLost)
                {
                    if (shx.IsPinch == true)
                    {
                        ReboilerPinch detail = reboilerPinchDAL.GetModel(SessionProtectedSystem, shx.ID);
                        rduty = rduty + shx.PinchFactor * detail.ReliefDuty;
                    }
                    else
                    {
                        TowerHXDetail detail = dbDetail.GetModel(SessionProtectedSystem, shx.DetailID);
                        rduty = rduty + shx.DutyCalcFactor * detail.Duty;
                    }
                }
                else
                {
                    TowerHXDetail detail = dbDetail.GetModel(SessionProtectedSystem, shx.DetailID);
                    if (detail.Duty < 0)
                        CondenserLost = false;
                }
            }
            Latent lt=ltdal.GetModel(SessionProtectedSystem);
            if (rduty < 0)
            {
                ReliefMW = lt.ReliefOHWeightFlow;
                ReliefPressure = lt.ReliefPressure;
                ReliefTemperature = lt.ReliefTemperature;
                ReliefCpCv = lt.ReliefCpCv;
                ReliefZ = lt.ReliefZ;
                return;
            }
            if (ScenarioName == "Blocked outlet" || ScenarioName == "Reflux failure")
            {
                if (!CondenserLost)
                {
                    ReliefMW = lt.ReliefOHWeightFlow;
                    ReliefPressure = lt.ReliefPressure;
                    ReliefTemperature = lt.ReliefTemperature;
                    ReliefCpCv = lt.ReliefCpCv;
                    ReliefZ = lt.ReliefZ;
                    return;
                }
            }
            CalRegSC(rduty);


        }

        private void CalRegSC(double duty)
        {            
            PSVDAL psvDAL = new PSVDAL();
            PSV psv = psvDAL.GetModel(SessionProtectedSystem);
            double pressure = psv.Pressure;
            string FileFullPath = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;
            double reliefFirePressure = pressure * psv.ReliefPressureFactor;
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + ScenarioName;
            if (!Directory.Exists(dirLatent))
                Directory.CreateDirectory(dirLatent);

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
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);

            //除以10的6次方
            string tray1_f = fcalc.Calculate(content, 1, reliefFirePressure.ToString(), 5, (duty / Math.Pow(10, 6)).ToString(), stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
                    ReliefLoad = vaporFire.WeightFlow;
                    ReliefMW = vaporFire.BulkMwOfPhase;
                    ReliefPressure = reliefFirePressure;
                    ReliefTemperature = vaporFire.Temperature;
                    ReliefCpCv = vaporFire.BulkCPCVRatio;
                    ReliefZ = vaporFire.VaporZFmKVal;
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




        private void SteamNotFreezedMethod(ref double reliefLoad, ref double reliefMW, ref double reliefTemperature, ref double reliefPressure)
        {
            Balance();
            double overHeadWeightFlow = 0;
            double waterWeightFlow = 0;
            double FeedTotal = 0;
            double ProductTotal = 0;
            double HeatTotal = 0;
            bool IsFB = false;
            PSVDAL dbpsv = new PSVDAL();
            PSV psv = dbpsv.GetModel(SessionProtectedSystem);

            LatentDAL dblatent = new LatentDAL();
            Latent latent = dblatent.GetModel(SessionProtectedSystem);

            FeedBottomHXDAL feedBottomHXDAL = new ReliefProDAL.FeedBottomHXDAL();
            FeedBottomHX fbhx = feedBottomHXDAL.GetModel(SessionProtectedSystem);
            if (fbhx != null)
            {
                ScenarioHeatSourceDAL scenarioHeatSourceDAL = new ScenarioHeatSourceDAL();
                ScenarioHeatSource scenarioHeatSource = scenarioHeatSourceDAL.GetModel(SessionProtectedSystem, fbhx.HeatSourceID, ScenarioID);
                if (scenarioHeatSource.IsFB)
                    IsFB = true;
            }

            CustomStreamDAL dbCS = new CustomStreamDAL();
            TowerScenarioStreamDAL db = new TowerScenarioStreamDAL();
            TowerFlashProductDAL dbFlashP = new TowerFlashProductDAL();
            IList<TowerScenarioStream> listStream = db.GetAllList(SessionProtectedSystem, ScenarioID);

            overHeadWeightFlow = 0;
            foreach (TowerScenarioStream s in listStream)
            {
                CustomStream cstream = dbCS.GetModel(SessionProtectedSystem, s.StreamName);
                if (cstream.IsProduct)
                {
                    TowerFlashProduct product = dbFlashP.GetModel(SessionProtectedSystem, cstream.StreamName);
                    if (!s.FlowStop)
                    {

                        if (IsSteamFreezed && product.ProdType == "4")
                        {                      
                            ProductTotal = ProductTotal + (s.FlowCalcFactor) * cstream.SpEnthalpy * product.WeightFlow;                           
                        }
                        else
                        {
                            //ProductTotal = ProductTotal + (s.FlowCalcFactor) * product.SpEnthalpy * product.WeightFlow;
                            if (s.IsNormal)
                            {
                                ProductTotal = ProductTotal + (s.FlowCalcFactor) * cstream.SpEnthalpy  * product.WeightFlow;
                            }
                            else
                            {
                                ProductTotal = ProductTotal + (s.FlowCalcFactor) * cstream.SpEnthalpy * s.ReliefNormalFactor * product.WeightFlow;
                            }
                        }
                        if (cstream.ProdType == "6")
                        {
                            waterWeightFlow = cstream.WeightFlow;
                        }
                        if (cstream.ProdType == "4")
                        {
                            overHeadWeightFlow = cstream.WeightFlow;
                        }
                    }
                }
                else
                {
                    if (!s.FlowStop)
                    {
                        if (fbhx != null)
                        {
                            if (fbhx.StreamName == s.StreamName && IsFB)
                            {
                                FeedTotal = FeedTotal + (s.FlowCalcFactor * fbhx.FeedReliefSpEout * cstream.WeightFlow);
                            }
                        }
                        else
                        {
                            FeedTotal = FeedTotal + (s.FlowCalcFactor * cstream.SpEnthalpy * cstream.WeightFlow);
                        }
                    }
                }
            }


            TowerScenarioHXDAL dbTSHX = new TowerScenarioHXDAL();
            TowerHXDetailDAL dbDetail = new TowerHXDetailDAL();
            ReboilerPinchDAL reboilerPinchDAL = new ReboilerPinchDAL();
            IList<TowerScenarioHX> list = dbTSHX.GetAllList(SessionProtectedSystem, ScenarioID);
            foreach (TowerScenarioHX shx in list)
            {
                if (!shx.DutyLost)
                {
                    if (shx.IsPinch == true)
                    {
                        ReboilerPinch detail = reboilerPinchDAL.GetModel(SessionProtectedSystem, shx.ID);
                        HeatTotal = HeatTotal + shx.PinchFactor * detail.ReliefDuty;
                    }
                    else
                    {
                        TowerHXDetail detail = dbDetail.GetModel(SessionProtectedSystem, shx.DetailID);
                        HeatTotal = HeatTotal + shx.DutyCalcFactor * detail.Duty;
                    }
                }
            }

            double latestH = latent.LatentEnthalpy;
            double totalH = FeedTotal - ProductTotal + HeatTotal;
            double wAccumulation = totalH / latestH + overHeadWeightFlow;
            double wRelief = wAccumulation;
            if (wRelief < 0)
            {
                wRelief = 0;
            }
            reliefLoad = wRelief + waterWeightFlow;
            double r= wRelief / latent.ReliefOHWeightFlow + waterWeightFlow / 18;
            if (r == 0)
            {
                reliefMW = latent.ReliefOHWeightFlow;
            }
            else
            {
                reliefMW = (wRelief + waterWeightFlow) / r;
            }
            reliefTemperature = latent.ReliefTemperature;
            reliefPressure = latent.ReliefPressure;
            ReliefCpCv = latent.ReliefCpCv;
            ReliefZ = latent.ReliefZ;

            if (reliefLoad < 0)
                reliefLoad = 0;
        }
        private void SteamFreezedMethod(ref double reliefLoad, ref double reliefMW, ref double reliefTemperature, ref double reliefPressure)
        {
            Balance();
            double overHeadWeightFlow = 0;
            double waterWeightFlow = 0;
            double FeedTotal = 0;
            double ProductTotal = 0;
            double HeatTotal = 0;

            PSVDAL dbpsv = new PSVDAL();
            PSV psv = dbpsv.GetModel(SessionProtectedSystem);

            LatentDAL dblatent = new LatentDAL();
            Latent latent = dblatent.GetModel(SessionProtectedSystem);

            CustomStreamDAL dbCS = new CustomStreamDAL();
            TowerScenarioStreamDAL db = new TowerScenarioStreamDAL();
            TowerFlashProductDAL dbFlashP = new TowerFlashProductDAL();
            IList<TowerScenarioStream> listStream = db.GetAllList(SessionProtectedSystem, ScenarioID);

            overHeadWeightFlow = 0;
            foreach (TowerScenarioStream s in listStream)
            {
                CustomStream cstream = dbCS.GetModel(SessionProtectedSystem, s.StreamName);
                if (cstream.IsProduct)
                {
                    TowerFlashProduct product = dbFlashP.GetModel(SessionProtectedSystem, cstream.StreamName);
                    if (!s.FlowStop)
                    {
                        if (cstream.ProdType == "6")
                        {
                            ProductTotal = ProductTotal + (s.FlowCalcFactor * cstream.SpEnthalpy * product.WeightFlow);
                        }
                        else
                        {
                            if (s.IsNormal)
                            {
                                ProductTotal = ProductTotal + (s.FlowCalcFactor) * cstream.SpEnthalpy * product.WeightFlow;
                            }
                            else
                            {
                                ProductTotal = ProductTotal + (s.FlowCalcFactor) * cstream.SpEnthalpy * s.ReliefNormalFactor * product.WeightFlow;
                            }
                        }
                        if (cstream.ProdType == "4")
                        {
                            overHeadWeightFlow = cstream.WeightFlow;
                        }
                    }
                }
                else
                {
                    if (!s.FlowStop)
                    {
                        FeedTotal = FeedTotal + (s.FlowCalcFactor * cstream.SpEnthalpy * cstream.WeightFlow);
                    }
                }
            }


            TowerScenarioHXDAL dbTSHX = new TowerScenarioHXDAL();
            TowerHXDetailDAL dbDetail = new TowerHXDetailDAL();
            ReboilerPinchDAL reboilerPinchDAL = new ReboilerPinchDAL();
            IList<TowerScenarioHX> list = dbTSHX.GetAllList(SessionProtectedSystem, ScenarioID);
            foreach (TowerScenarioHX shx in list)
            {
                if (!shx.DutyLost)
                {
                    if (shx.IsPinch == true)
                    {
                        ReboilerPinch detail = reboilerPinchDAL.GetModel(SessionProtectedSystem, shx.ID);
                        HeatTotal = HeatTotal + shx.PinchFactor * detail.ReliefDuty;
                    }
                    else
                    {
                        TowerHXDetail detail = dbDetail.GetModel(SessionProtectedSystem, shx.DetailID);
                        HeatTotal = HeatTotal + shx.DutyCalcFactor * detail.Duty;
                    }
                }
            }

            double latestH = latent.LatentEnthalpy;
            double totalH = FeedTotal - ProductTotal + HeatTotal;
            double wAccumulation = totalH / latestH + overHeadWeightFlow;
            double wRelief = wAccumulation;
            if (wRelief < 0)
            {
                wRelief = 0;
            }
            reliefLoad = wRelief + waterWeightFlow;
            double r = wRelief / latent.ReliefOHWeightFlow + waterWeightFlow / 18;
            if (r == 0)
            {
                reliefMW = latent.ReliefOHWeightFlow;
            }
            else
            {
                reliefMW = (wRelief + waterWeightFlow) / r;
            }
            reliefTemperature = latent.ReliefTemperature;
            reliefPressure = latent.ReliefPressure;
            ReliefCpCv = latent.ReliefCpCv;
            ReliefZ = latent.ReliefZ;
            if (reliefLoad < 0)
                reliefLoad = 0;           
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
            WriteConvert();

            LatentProductDAL lpdal=new LatentProductDAL();
            LatentProduct vaporProduct = lpdal.GetModel(SessionProtectedSystem, "2");

            ScenarioDAL dbTS = new ScenarioDAL();
            Scenario scenario = dbTS.GetModel(ScenarioID, SessionProtectedSystem);
            scenario.ReliefLoad = ReliefLoad;
            scenario.ReliefMW = ReliefMW;
            scenario.ReliefTemperature = ReliefTemperature;
            scenario.ReliefPressure = ReliefPressure;
            scenario.ReliefCpCv = ReliefCpCv;
            scenario.ReliefZ = ReliefZ;
            if (tower.TowerType == "Distillation")
            {
                scenario.ReliefCpCv = vaporProduct.BulkCPCVRatio;
                scenario.ReliefZ = vaporProduct.VaporZFmKVal;
            }
            dbTS.Update(scenario, SessionProtectedSystem);
            SessionProtectedSystem.Flush();

            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private void ReadConvert()
        {
            if (_ReliefLoad != null)
                _ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, _ReliefLoadUnit, _ReliefLoad);
            if (_ReliefTemperature != null)
                _ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, _ReliefTemperatureUnit, _ReliefTemperature);
            if (_ReliefPressure != null)
                _ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, _ReliefPressureUnit, _ReliefPressure);
        }
        private void WriteConvert()
        {
            if (_ReliefLoad != null)
                _ReliefLoad = UnitConvert.Convert(_ReliefLoadUnit, UOMEnum.MassRate, _ReliefLoad);
            if (_ReliefTemperature != null)
                _ReliefTemperature = UnitConvert.Convert(_ReliefTemperatureUnit, UOMEnum.Temperature, _ReliefTemperature);
            if (_ReliefPressure != null)
                _ReliefPressure = UnitConvert.Convert(_ReliefPressureUnit, UOMEnum.Pressure, _ReliefPressure);
        }
        private void InitUnit()
        {
            this._ReliefLoadUnit = uomEnum.UserMassRate;
            this._ReliefTemperatureUnit = uomEnum.UserTemperature;
            this._ReliefPressureUnit = uomEnum.UserPressure;
        }
        #region 单位字段
        private string _ReliefLoadUnit;
        public string ReliefLoadUnit
        {
            get
            {
                return this._ReliefLoadUnit;
            }
            set
            {
                this._ReliefLoadUnit = value;

                OnPropertyChanged("ReliefLoadUnit");
            }
        }

        private string _ReliefTemperatureUnit;
        public string ReliefTemperatureUnit
        {
            get
            {
                return this._ReliefTemperatureUnit;
            }
            set
            {
                this._ReliefTemperatureUnit = value;

                OnPropertyChanged("ReliefTemperatureUnit");
            }
        }

        private string _ReliefPressureUnit;
        public string ReliefPressureUnit
        {
            get
            {
                return this._ReliefPressureUnit;
            }
            set
            {
                this._ReliefPressureUnit = value;

                OnPropertyChanged("ReliefPressureUnit");
            }
        }
        #endregion
    }
}
