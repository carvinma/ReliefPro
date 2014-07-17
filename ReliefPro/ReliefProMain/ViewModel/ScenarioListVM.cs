using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using ReliefProMain.View;
using ReliefProMain.Model;
using NHibernate;
using ReliefProMain.ViewModel.Drums;
using System.Windows;
using ReliefProMain.CustomControl;
using ReliefProMain.ViewModel.Compressors;
using ReliefProMain.View.TowerFires;
using ReliefProMain.ViewModel.TowerFires;
using ReliefProMain.View.Drums;
using ReliefProMain.View.DrumFires;
using ReliefProMain.View.DrumDepressures;
using ReliefProMain.View.Compressors;
using ReliefProMain.View.StorageTanks;
using ReliefProMain.ViewModel.StorageTanks;
using ReliefProDAL.Compressors;
using ReliefProModel.HXs;
using ReliefProDAL.HXs;
using ReliefProMain.ViewModel.HXs;

namespace ReliefProMain.ViewModel
{
    public class ScenarioListVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        private string PrzFile;
        private string PrzVersion;
        private ObservableCollection<ScenarioModel> _Scenarios;
        public ObservableCollection<ScenarioModel> Scenarios
        {
            get { return _Scenarios; }
            set
            {
                _Scenarios = value;
                OnPropertyChanged("Scenarios");
            }
        }

        private ScenarioModel _SelectedScenario;
        public ScenarioModel SelectedScenario
        {
            get { return _SelectedScenario; }
            set
            {
                _SelectedScenario = value;
                OnPropertyChanged("SelectedScenario");
            }
        }

        public List<string> ScenarioNameList { set; get; }
        private string EqType;
        private string EqName;
        public ScenarioListVM(string eqName, string eqType, string przFile, string version, ISession sessionPlant, ISession sessionProtectedSystem, string dirPlant, string dirProtectedSystem)
        {
            EqName = eqName;
            EqType = eqType;
            PrzFile = przFile;
            PrzVersion = version;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;

            Scenarios = GetScenarios();
            ScenarioNameList = GetScenarioNames(eqType);

            cud += new ChangeUnitDelegate(ExcuteThumbMoved);

        }

        private ObservableCollection<ScenarioModel> GetScenarios()
        {
            ObservableCollection<ScenarioModel> scenarios = new ObservableCollection<ScenarioModel>();
            ScenarioDAL db = new ScenarioDAL();
            IList<Scenario> list = db.GetAllList(SessionProtectedSystem);
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                Scenario s = list[i];
                ScenarioModel m = new ScenarioModel();
                m.ID = s.ID;
                m.ScenarioName = s.ScenarioName;
                m.ReliefLoad = s.ReliefLoad;
                m.ReliefTemperature = s.ReliefTemperature;
                m.ReliefPressure = s.ReliefPressure;
                m.ReliefMW = s.ReliefMW;
                m.SeqNumber = i;
                scenarios.Add(m);
            }
            return scenarios;
        }

        private ICommand _AddCommand;
        public ICommand AddCommand
        {
            get
            {
                if (_AddCommand == null)
                {
                    _AddCommand = new DelegateCommand(Add);

                }
                return _AddCommand;
            }
        }

        public void Add()
        {

            ScenarioDAL db = new ScenarioDAL();
            Scenario s = new Scenario();
            db.Add(s, SessionProtectedSystem);

            ScenarioModel d = new ScenarioModel();
            d = ConvertToVModel(s, d);
            d.SeqNumber = Scenarios.Count;
            Scenarios.Add(d);

        }

        private ICommand _DeleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                {
                    _DeleteCommand = new RelayCommand(Delete);

                }
                return _DeleteCommand;
            }
        }

        public void Delete(object obj)
        {
            MessageBoxResult r = MessageBox.Show("Are you sure for deleting this S?", "Message Box", MessageBoxButton.YesNoCancel);
            if (r == MessageBoxResult.Yes)
            {
                int idx = int.Parse(obj.ToString());
                ScenarioModel s = Scenarios[idx];
                Scenarios.RemoveAt(idx);
                //从db里删除

                ScenarioDAL db = new ScenarioDAL();
                Scenario sc = db.GetModel(s.ID, SessionProtectedSystem);
                db.Delete(sc, SessionProtectedSystem);

                for (int i = 0; i < Scenarios.Count; i++)
                {
                    Scenarios[i].SeqNumber = i;
                }
            }
        }

        private ICommand _CalculateCommand;
        public ICommand CalculateCommand
        {
            get
            {
                if (_CalculateCommand == null)
                {
                    _CalculateCommand = new RelayCommand(Calculate);

                }
                return _CalculateCommand;
            }
        }

        public void Calculate(object obj)
        {
            int ScenarioID = int.Parse(obj.ToString());
            SelectedScenario = (from s in Scenarios
                                where s.ID == ScenarioID
                                select s).Single();
            if (!string.IsNullOrEmpty(SelectedScenario.ScenarioName))
            {

                ScenarioDAL db = new ScenarioDAL();
                Scenario sce = db.GetModel(ScenarioID, SessionProtectedSystem);
                sce.ScenarioName = SelectedScenario.ScenarioName;
                db.Update(sce, SessionProtectedSystem);
                string ScenarioName = SelectedScenario.ScenarioName.Replace(" ", string.Empty);
                if (EqType == "Tower")
                {
                    if (ScenarioName.Contains("Fire"))
                    {
                        CreateTowerFire(ScenarioID, SessionProtectedSystem);
                    }
                    else if (ScenarioName.Contains("Inlet"))
                    {
                        CreateInletValveOpen(ScenarioID);
                    }
                    else if (ScenarioName.Contains("Abnormal"))
                    {
                        CreateAbnormalHeatInput(ScenarioID, ScenarioName, SessionProtectedSystem);
                    }
                    else
                    {
                        CreateTowerCommon(ScenarioID, ScenarioName, SessionProtectedSystem);
                    }
                }
                else if (EqType == "Drum")
                {
                    if (ScenarioName.Contains("Outlet"))
                    {
                        DrumBlockedOutletView v = new DrumBlockedOutletView();
                        v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                        DrumBlockedOutletVM vm = new DrumBlockedOutletVM(ScenarioID, PrzFile, PrzVersion, SessionProtectedSystem, SessionPlant, DirPlant, DirProtectedSystem);
                        v.DataContext = vm;
                        if (v.ShowDialog() == true)
                        {
                            if (vm.CalcTuple != null)
                            {
                                SelectedScenario.ReliefLoad = vm.CalcTuple.Item1.ToString();
                                SelectedScenario.ReliefPressure = vm.CalcTuple.Item4.ToString();
                                SelectedScenario.ReliefTemperature = vm.CalcTuple.Item3.ToString();
                                SelectedScenario.ReliefMW = vm.CalcTuple.Item2.ToString();
                            }
                        }
                        //CreateDrumOutlet(ScenarioID, Session);
                    }
                    else if (ScenarioName.Contains("Fire"))
                    {
                        DrumFireView v = new DrumFireView();
                        v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                        DrumFireVM vm = new DrumFireVM(ScenarioID, PrzFile, PrzVersion, SessionProtectedSystem, SessionPlant, DirPlant, DirProtectedSystem);
                        v.DataContext = vm;
                        v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        if (v.ShowDialog() == true)
                        {
                            //需要把ReliefLoad等值传回给SelectedScenario.ReliefLoad。 参考CreateInletValveOpen
                            SelectedScenario.ReliefLoad = vm.model.dbmodel.ReliefLoad.ToString();
                            SelectedScenario.ReliefMW = vm.model.dbmodel.ReliefMW.ToString();
                            SelectedScenario.ReliefPressure = vm.model.dbmodel.ReliefPressure.ToString();
                            SelectedScenario.ReliefTemperature = vm.model.dbmodel.ReliefTemperature.ToString();
                        }

                    }
                    else if (ScenarioName.Contains("Inlet"))
                    {
                        CreateInletValveOpen(ScenarioID);
                    }
                    else if (ScenarioName.Contains("Depressuring"))
                    {
                        DrumDepressureView v = new DrumDepressureView();
                        v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                        DrumDepressuringVM vm = new DrumDepressuringVM(ScenarioID, SessionProtectedSystem, SessionPlant);
                        v.DataContext = vm;
                        v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        v.ShowDialog();
                        //CreateDrumDepressuring(ScenarioID, ScenarioName, Session);
                    }

                }
                else if (EqType == "Compressor")
                {
                    if (ScenarioName.Contains("Outlet"))
                    {
                        //我今天晚点会加一个tbCompresser表， 这样就可以从该表里获取设备的类型
                        Compressor comp = new Compressor();
                        CompressorDAL compDAL = new CompressorDAL();
                        comp = compDAL.GetModel(SessionProtectedSystem);
                        string CompresserType = comp.CompressorType;
                        if (CompresserType == "Centrifugal")
                        {
                            CentrifugalBlockedView v = new CentrifugalBlockedView();
                            v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                            CentrifugalVM vm = new CentrifugalVM(ScenarioID, SessionProtectedSystem, SessionPlant);
                            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            v.DataContext = vm;
                            if (v.ShowDialog() == true)
                            {
                                SelectedScenario.ReliefLoad = vm.model.dbmodel.Reliefload.ToString();
                                SelectedScenario.ReliefMW = vm.model.dbmodel.ReliefMW.ToString();
                                SelectedScenario.ReliefPressure = vm.model.dbmodel.ReliefPressure.ToString();
                                SelectedScenario.ReliefTemperature = vm.model.dbmodel.ReliefTemperature.ToString();
                            }
                        }
                        else if (CompresserType == "Piston")
                        {
                            PistonBlockedView v = new PistonBlockedView();
                            v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                            PistonVM vm = new PistonVM(ScenarioID, SessionProtectedSystem, SessionPlant);
                            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            v.DataContext = vm;
                            if (v.ShowDialog() == true)
                            {
                                SelectedScenario.ReliefLoad = vm.model.dbmodel.Reliefload.ToString();
                                SelectedScenario.ReliefMW = vm.model.dbmodel.ReliefMW.ToString();
                                SelectedScenario.ReliefPressure = vm.model.dbmodel.ReliefPressure.ToString();
                                SelectedScenario.ReliefTemperature = vm.model.dbmodel.ReliefTemperature.ToString();
                            }
                        }
                    }
                }
                if (EqType == "HX")
                {
                    HeatExchangerDAL hxDAL = new HeatExchangerDAL();
                    HeatExchanger hx = hxDAL.GetModel(SessionProtectedSystem);
                    if (hx.HXType == "Shell-Tube")
                    {
                        if (ScenarioName.Contains("Blocked Outlet"))
                        {
                            StorageTankFireView v = new StorageTankFireView();
                            HXBlockedOutletVM vm = new HXBlockedOutletVM(ScenarioID,PrzFile,PrzVersion, SessionProtectedSystem, SessionPlant,DirPlant,DirProtectedSystem);
                            v.DataContext = vm;
                            if (v.ShowDialog() == true)
                            {
                                SelectedScenario.ReliefLoad = vm.model.ReliefLoad.ToString();
                                SelectedScenario.ReliefMW = vm.model.dbmodel.ReliefMW.ToString();
                                SelectedScenario.ReliefPressure = vm.model.dbmodel.ReliefPressure.ToString();
                                SelectedScenario.ReliefTemperature = vm.model.dbmodel.ReliefTemperature.ToString();
                            }
                        }
                    }
                    else
                    {
                        if (ScenarioName.Contains("Fire"))
                        {
                            StorageTankFireView v = new StorageTankFireView();
                            AirCooledHXFireVM vm = new AirCooledHXFireVM(ScenarioID,PrzFile,PrzVersion, SessionProtectedSystem, SessionPlant,DirPlant,DirProtectedSystem);
                            v.DataContext = vm;
                            if (v.ShowDialog() == true)
                            {
                                SelectedScenario.ReliefLoad = vm.model.ToString();
                                SelectedScenario.ReliefMW = vm.model.dbmodel.ReliefMW.ToString();
                                SelectedScenario.ReliefPressure = vm.model.dbmodel.ReliefPressure.ToString();
                                SelectedScenario.ReliefTemperature = vm.model.dbmodel.ReliefTemperature.ToString();
                            }
                        }
                    }
                }
                else if (EqType == "StorageTank")
                {
                    if (ScenarioName.Contains("Fire"))
                    {
                        StorageTankFireView v = new StorageTankFireView();
                        DrumFireVM vm = new DrumFireVM(ScenarioID, PrzFile, PrzVersion, SessionProtectedSystem, SessionPlant, DirPlant, DirProtectedSystem, 1);
                        v.DataContext = vm;
                        if (v.ShowDialog() == true)
                        {
                            SelectedScenario.ReliefLoad = vm.model.dbmodel.ReliefLoad.ToString();
                            SelectedScenario.ReliefMW = vm.model.dbmodel.ReliefMW.ToString();
                            SelectedScenario.ReliefPressure = vm.model.dbmodel.ReliefPressure.ToString();
                            SelectedScenario.ReliefTemperature = vm.model.dbmodel.ReliefTemperature.ToString();
                        }
                    }
                }


            }
        }



        private void CreateTowerFire(int ScenarioID, NHibernate.ISession Session)
        {
            TowerFireDAL dbtf = new TowerFireDAL();
            ReliefProModel.TowerFire t = dbtf.GetModel(Session, ScenarioID);
            if (t == null)
            {
                ReliefProModel.TowerFire tf = new ReliefProModel.TowerFire();
                tf.ScenarioID = ScenarioID;
                tf.IsExist = false;
                tf.HeatInputModel = "API 521";
                dbtf.Add(tf, Session);

                TowerFireEqDAL dbtfeq = new TowerFireEqDAL();
                TowerDAL dbtower = new TowerDAL();
                Tower tower = dbtower.GetModel(Session);
                TowerFireEq eq = new TowerFireEq();
                eq.EqName = tower.TowerName;
                eq.Type = "Column";
                eq.FFactor = "1";
                eq.FireZone = true;
                eq.FireID = tf.ID;
                dbtfeq.Add(eq, Session);

                AccumulatorDAL dbaccumulator = new AccumulatorDAL();
                Accumulator accumulator = dbaccumulator.GetModel(Session);
                eq = new TowerFireEq();
                eq.EqName = accumulator.AccumulatorName;
                eq.Type = "Drum";
                eq.FFactor = "1";
                eq.FireZone = true;
                eq.FireID = tf.ID;
                dbtfeq.Add(eq, Session);

                eq = new TowerFireEq();
                eq.EqName = "Other";
                eq.Type = "Other HX";
                eq.FFactor = "1";
                eq.FireZone = true;
                eq.FireID = tf.ID;
                dbtfeq.Add(eq, Session);

                SideColumnDAL dbsidecolumn = new SideColumnDAL();
                IList<SideColumn> listSideColumn = dbsidecolumn.GetAllList(Session);
                foreach (SideColumn s in listSideColumn)
                {
                    eq = new TowerFireEq();
                    eq.EqName = s.EqName;
                    eq.Type = "Side Column";
                    eq.FFactor = "1";
                    eq.FireZone = true;
                    eq.FireID = tf.ID;
                    dbtfeq.Add(eq, Session);
                }


                TowerHXDetailDAL dbhx = new TowerHXDetailDAL();
                IList<TowerHXDetail> listHX = dbhx.GetAllList(Session);
                foreach (TowerHXDetail s in listHX)
                {
                    eq = new TowerFireEq();
                    eq.EqName = s.DetailName;
                    eq.FireZone = true;
                    eq.FFactor = "1";
                    eq.FireID = tf.ID;
                    if (s.Medium.Contains("Air"))
                    {
                        eq.Type = "Air Cooler";
                    }
                    else
                    {
                        eq.Type = "Shell-Tube HX";
                    }
                    dbtfeq.Add(eq, Session);
                }
            }

            TowerFireView v = new TowerFireView();
            TowerFireVM vm = new TowerFireVM(ScenarioID,EqName,PrzFile,PrzVersion, SessionPlant, SessionProtectedSystem,DirPlant,DirProtectedSystem);
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                SelectedScenario.ReliefLoad = vm.MainModel.ReliefLoad;
                SelectedScenario.ReliefMW = vm.MainModel.ReliefMW;
                SelectedScenario.ReliefPressure = vm.MainModel.ReliefPressure;
                SelectedScenario.ReliefTemperature = vm.MainModel.ReliefTemperature;
            }
        }

        private void CreateTowerCommon(int ScenarioID, string ScenarioName, NHibernate.ISession Session)
        {
            CreateTowerScenarioCalcData(ScenarioID, ScenarioName, Session);
            TowerScenarioCalcView v = new TowerScenarioCalcView();
            TowerScenarioCalcVM vm = new TowerScenarioCalcVM(ScenarioID,PrzFile, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (v.ShowDialog() == true)
            {
                SelectedScenario.ReliefLoad = vm.ReliefLoad;
                SelectedScenario.ReliefPressure = vm.ReliefPressure;
                SelectedScenario.ReliefTemperature = vm.ReliefTemperature;
                SelectedScenario.ReliefMW = vm.ReliefMW;
            }
        }

        private void CreateInletValveOpen(int ScenarioID)
        {
            InletValveOpenView v = new InletValveOpenView();
            InletValveOpenVM vm = new InletValveOpenVM(ScenarioID, EqName, EqType, PrzFile, PrzVersion, SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (v.ShowDialog() == true)
            {
                SelectedScenario.ReliefLoad = vm.ReliefLoad;
                SelectedScenario.ReliefMW = vm.ReliefMW;
                SelectedScenario.ReliefPressure = vm.ReliefPressure;
                SelectedScenario.ReliefTemperature = vm.ReliefTemperature;
            }
        }

        private void CreateAbnormalHeatInput(int ScenarioID, string ScenarioName, NHibernate.ISession Session)
        {
            CreateTowerAbnormalHeatInputData(ScenarioID, ScenarioName, Session);
            AbnormalHeatInputView v = new AbnormalHeatInputView();
            AbnormalHeatInputVM vm = new AbnormalHeatInputVM(ScenarioID, SessionPlant, SessionProtectedSystem);
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                SelectedScenario.ReliefLoad = vm.ReliefLoad;
                SelectedScenario.ReliefMW = vm.ReliefMW;
                SelectedScenario.ReliefPressure = vm.ReliefPressure;
                SelectedScenario.ReliefTemperature = vm.ReliefTemperature;
            }
        }


        private void CreateTowerScenarioCalcData(int ScenarioID, string ScenarioName, NHibernate.ISession Session)
        {
            SourceDAL dbSource = new SourceDAL();
            HeatSourceDAL dbhs = new HeatSourceDAL();
            ScenarioHeatSourceDAL scenarioHeatSourceDAL = new ScenarioHeatSourceDAL();
            TowerScenarioStreamDAL towerScenarioStreamDAL = new TowerScenarioStreamDAL();            
            List<Source> listSource = dbSource.GetAllList(Session).ToList();
            foreach (Source s in listSource)
            {
                TowerScenarioStream tss = towerScenarioStreamDAL.GetModel(Session, s.StreamName, ScenarioID);
                if (tss == null)
                {
                    tss = new TowerScenarioStream();
                    tss.ScenarioID = ScenarioID;
                    tss.StreamName = s.StreamName;
                    tss.FlowCalcFactor = GetSystemScenarioFactor("4", s.SourceType, ScenarioName);
                    tss.FlowStop = false;
                    tss.SourceType = s.SourceType;
                    tss.IsProduct = false;
                    towerScenarioStreamDAL.Add(tss, Session);
                }
                else if (tss.SourceType != s.SourceType)
                {
                    tss.FlowCalcFactor = GetSystemScenarioFactor("4", s.SourceType, ScenarioName);
                    tss.SourceType = s.SourceType;
                    towerScenarioStreamDAL.Update(tss, Session);
                }

                IList<HeatSource> listHeatSource = dbhs.GetAllList(Session, s.ID);
                foreach (HeatSource hs in listHeatSource)
                {
                    ScenarioHeatSource shs = scenarioHeatSourceDAL.GetModel(Session, hs.ID, ScenarioID);
                    if (shs == null)
                    {
                        if (hs.HeatSourceType == "Feed/Bottom HX")
                        {
                            shs = new ScenarioHeatSource();
                            shs.HeatSourceID = hs.ID;
                            shs.DutyFactor = "1";
                            shs.ScenarioStreamID = tss.ID;
                            shs.ScenarioID = ScenarioID;
                            scenarioHeatSourceDAL.Add(shs, SessionProtectedSystem);
                        }
                    }
                    
                }
            }

            SinkDAL sinkDAL = new SinkDAL();
            List<Sink> listSink = sinkDAL.GetAllList(Session).ToList();
            foreach (Sink s in listSink)
            {
                TowerScenarioStream tss = towerScenarioStreamDAL.GetModel(Session, s.StreamName, ScenarioID);
                if (tss == null)
                {
                    tss = new TowerScenarioStream();
                    tss.ScenarioID = ScenarioID;
                    tss.StreamName = s.StreamName;
                    tss.FlowCalcFactor = "1";// GetSystemScenarioFactor("5", s.SinkType, ScenarioName);
                    tss.FlowStop = false;
                    tss.IsProduct = true;
                    tss.SourceType = s.SinkType;
                    towerScenarioStreamDAL.Add(tss, Session);
                }
                else if (tss.SourceType != s.SinkType)
                {
                    tss.FlowCalcFactor = GetSystemScenarioFactor("5", s.SinkType, ScenarioName);
                    tss.SourceType = s.SinkType;
                    towerScenarioStreamDAL.Update(tss, Session);
                }
            }
            TowerHXDetailDAL towerHXDetailDAL = new TowerHXDetailDAL();
            TowerScenarioHXDAL towerScenarioHXDAL = new TowerScenarioHXDAL();
            TowerHXDAL towerHXDAL = new TowerHXDAL();
            IList<TowerHX> tHXs = towerHXDAL.GetAllList(Session);
            foreach (TowerHX hx in tHXs)
            {
                
                List<TowerHXDetail> listTowerHXDetail = towerHXDetailDAL.GetAllList(Session, hx.ID).ToList();
                foreach (TowerHXDetail detail in listTowerHXDetail)
                {
                    string ProcessSideFlowSourceFactor = GetSystemScenarioFactor("1", detail.ProcessSideFlowSource, ScenarioName);
                    string MediumFactor = GetSystemScenarioFactor("2", detail.Medium, ScenarioName);
                    string MediumSideFlowSourceFactor = GetSystemScenarioFactor("3", detail.MediumSideFlowSource, ScenarioName);
                    double factor = double.Parse(ProcessSideFlowSourceFactor) * double.Parse(MediumFactor) * double.Parse(MediumSideFlowSourceFactor);

                    TowerScenarioHX tsHX = towerScenarioHXDAL.GetModel(Session, detail.ID, ScenarioID);
                    if (tsHX == null)
                    {
                        tsHX = new TowerScenarioHX();
                        tsHX.DetailID = detail.ID;
                        tsHX.ScenarioID = ScenarioID;
                        tsHX.DutyLost = false;
                        tsHX.DutyCalcFactor = factor;
                        tsHX.DetailName = detail.DetailName;
                        tsHX.Medium = detail.Medium;
                        tsHX.HeaterType = hx.HeaterType;

                        if (ScenarioName == "BlockedOutlet" && double.Parse(detail.Duty) < 0)
                        {
                            tsHX.DutyLost = true;
                        }
                        towerScenarioHXDAL.Add(tsHX, Session);
                    }
                    else if(factor!=tsHX.DutyCalcFactor || tsHX.Medium != detail.Medium)
                    {
                        tsHX.DutyCalcFactor = factor;
                        tsHX.DetailName = detail.DetailName;
                        tsHX.Medium = detail.Medium;                        
                        if (ScenarioName == "BlockedOutlet" && double.Parse(detail.Duty) < 0)
                        {
                            tsHX.DutyLost = true;
                        }
                        towerScenarioHXDAL.Update(tsHX, Session);
                    }     
                }
            }
            IList<TowerScenarioHX> tsHXs = towerScenarioHXDAL.GetAllList(Session, ScenarioID);
            int count = tsHXs.Count;
            for (int i = 0; i < count; i++)
            {
                TowerHXDetail detail = towerHXDetailDAL.GetModel(tsHXs[i].DetailID, Session);
                if (detail == null)
                {
                    towerScenarioHXDAL.Delete(tsHXs[i], Session);
                }
            }

        }

       
        private void CreateTowerAbnormalHeatInputData(int ScenarioID, string ScenarioName, NHibernate.ISession Session)
        {
            AbnormalHeaterDetailDAL abnormalHeaterDetailDAL=new AbnormalHeaterDetailDAL();
            SourceDAL dbSource = new SourceDAL();
            HeatSourceDAL dbhs = new HeatSourceDAL();
            TowerScenarioStreamDAL towerScenarioStreamDAL = new TowerScenarioStreamDAL();
            List<Source> listSource = dbSource.GetAllList(Session).ToList();
            foreach (Source s in listSource)
            {
                TowerScenarioStream tss = towerScenarioStreamDAL.GetModel(Session, s.StreamName, ScenarioID);
                if (tss == null)
                {
                    tss = new TowerScenarioStream();
                    tss.ScenarioID = ScenarioID;
                    tss.StreamName = s.StreamName;
                    tss.FlowCalcFactor = GetSystemScenarioFactor("4", s.SourceType, ScenarioName);
                    tss.FlowStop = false;
                    tss.SourceType = s.SourceType;
                    tss.IsProduct = false;
                    towerScenarioStreamDAL.Add(tss, Session);
                }
                else if (tss.SourceType != s.SourceType)
                {
                    tss.FlowCalcFactor = GetSystemScenarioFactor("4", s.SourceType, ScenarioName);
                    tss.SourceType = s.SourceType;
                    towerScenarioStreamDAL.Update(tss, Session);
                }

                IList<HeatSource> listHeatSource = dbhs.GetAllList(Session, s.ID);
                foreach (HeatSource hs in listHeatSource)
                {
                    if (hs.HeatSourceType != "Feed/Bottom HX")
                    {
                        AbnormalHeaterDetail d;
                        d = abnormalHeaterDetailDAL.GetModel(Session, ScenarioID, hs.ID, 1);
                        if (d == null)
                        {
                            d = new AbnormalHeaterDetail();
                            d.AbnormalType = 1;
                            d.Duty = double.Parse(hs.Duty);
                            d.DutyFactor = 1;
                            d.HeaterID = hs.ID;
                            d.HeaterName = hs.HeatSourceName;
                            d.HeaterType = hs.HeatSourceType;
                            d.ScenarioID = ScenarioID;
                            abnormalHeaterDetailDAL.Add(d, Session);
                        }
                        else
                        {
                            d.Duty = double.Parse( hs.Duty);
                            d.HeaterName = hs.HeatSourceName;
                            abnormalHeaterDetailDAL.Update(d, Session);
                            Session.Flush();
                        }
                    }

                }

            }

            SinkDAL sinkDAL = new SinkDAL();
            List<Sink> listSink = sinkDAL.GetAllList(Session).ToList();
            foreach (Sink s in listSink)
            {
                TowerScenarioStream tss = towerScenarioStreamDAL.GetModel(Session, s.StreamName, ScenarioID);
                if (tss == null)
                {
                    tss = new TowerScenarioStream();
                    tss.ScenarioID = ScenarioID;
                    tss.StreamName = s.StreamName;
                    tss.FlowCalcFactor = "1";// GetSystemScenarioFactor("5", s.SinkType, ScenarioName);
                    tss.FlowStop = false;
                    tss.IsProduct = true;
                    tss.SourceType = s.SinkType;
                    towerScenarioStreamDAL.Add(tss, Session);
                }
                else if (tss.SourceType != s.SinkType)
                {
                    tss.FlowCalcFactor = GetSystemScenarioFactor("5", s.SinkType, ScenarioName);
                    tss.SourceType = s.SinkType;
                    towerScenarioStreamDAL.Update(tss, Session);
                }
            }
            TowerHXDetailDAL towerHXDetailDAL = new TowerHXDetailDAL();
            TowerScenarioHXDAL towerScenarioHXDAL = new TowerScenarioHXDAL();
            TowerHXDAL towerHXDAL = new TowerHXDAL();
            IList<TowerHX> tHXs = towerHXDAL.GetAllList(Session);
            foreach (TowerHX hx in tHXs)
            {

                List<TowerHXDetail> listTowerHXDetail = towerHXDetailDAL.GetAllList(Session, hx.ID).ToList();
                foreach (TowerHXDetail detail in listTowerHXDetail)
                {
                    string ProcessSideFlowSourceFactor = GetSystemScenarioFactor("1", detail.ProcessSideFlowSource, ScenarioName);
                    string MediumFactor = GetSystemScenarioFactor("2", detail.Medium, ScenarioName);
                    string MediumSideFlowSourceFactor = GetSystemScenarioFactor("3", detail.MediumSideFlowSource, ScenarioName);
                    double factor = double.Parse(ProcessSideFlowSourceFactor) * double.Parse(MediumFactor) * double.Parse(MediumSideFlowSourceFactor);

                    TowerScenarioHX tsHX = towerScenarioHXDAL.GetModel(Session, detail.ID, ScenarioID);
                    if (tsHX == null)
                    {
                        tsHX = new TowerScenarioHX();
                        tsHX.DetailID = detail.ID;
                        tsHX.ScenarioID = ScenarioID;
                        tsHX.DutyLost = false;
                        tsHX.DutyCalcFactor = factor;
                        tsHX.DetailName = detail.DetailName;
                        tsHX.Medium = detail.Medium;
                        tsHX.HeaterType = hx.HeaterType;

                        if (ScenarioName == "BlockedOutlet" && double.Parse(detail.Duty) < 0)
                        {
                            tsHX.DutyLost = true;
                        }
                        towerScenarioHXDAL.Add(tsHX, Session);

                        
                        if (tsHX.HeaterType == 3 || tsHX.HeaterType==4)
                        {
                            AbnormalHeaterDetail d = new AbnormalHeaterDetail();
                            d = new AbnormalHeaterDetail();
                            d.AbnormalType = 2;
                            d.Duty = double.Parse(hx.HeaterDuty) * double.Parse(detail.DutyPercentage)/100;
                            d.DutyFactor = 1;
                            d.HeaterID = detail.ID;
                            d.HeaterName = detail.DetailName;
                            d.HeaterType = detail.Medium;
                            d.ScenarioID = ScenarioID;
                            abnormalHeaterDetailDAL.Add(d, Session);
                        }

                    }
                    else if (factor != tsHX.DutyCalcFactor || tsHX.Medium != detail.Medium)
                    {
                        tsHX.DutyCalcFactor = factor;
                        tsHX.DetailName = detail.DetailName;
                        tsHX.Medium = detail.Medium;
                        if (ScenarioName == "BlockedOutlet" && double.Parse(detail.Duty) < 0)
                        {
                            tsHX.DutyLost = true;
                        }
                        towerScenarioHXDAL.Update(tsHX, Session);

                        if (tsHX.HeaterType == 3 || tsHX.HeaterType == 4)
                        {
                            AbnormalHeaterDetail d = abnormalHeaterDetailDAL.GetModel(Session,ScenarioID,tsHX.ID,2);
                            d.Duty = double.Parse(hx.HeaterDuty) * tsHX.DutyCalcFactor;
                            d.HeaterName = detail.DetailName;
                            d.HeaterType = detail.Medium;
                            abnormalHeaterDetailDAL.Update(d, Session);
                            Session.Flush();
                        }


                    }
                }
            }

            
            IList<TowerScenarioHX> tsHXs = towerScenarioHXDAL.GetAllList(Session, ScenarioID);
            int count = tsHXs.Count;
            for (int i = 0; i < count; i++)
            {
                TowerHXDetail detail = towerHXDetailDAL.GetModel(tsHXs[i].DetailID, Session);
                if (detail == null)
                {
                    towerScenarioHXDAL.Delete(tsHXs[i], Session);
                }
            }
            IList<AbnormalHeaterDetail> abnormalDetails = abnormalHeaterDetailDAL.GetAllList(Session, ScenarioID,2);
            count = abnormalDetails.Count;
            for (int i = 0; i < count; i++)
            {
                TowerHXDetail detail = towerHXDetailDAL.GetModel(abnormalDetails[i].HeaterID, Session);
                if (detail == null)
                {
                    abnormalHeaterDetailDAL.Delete(abnormalDetails[i], Session);
                }
            }



        }
        public Scenario ConvertToDBModel(ScenarioModel m, Scenario d)
        {
            d.ID = m.ID;
            d.ReliefLoad = m.ReliefLoad;
            d.ReliefMW = m.ReliefMW;
            d.ReliefPressure = m.ReliefPressure;
            d.ReliefTemperature = m.ReliefTemperature;
            d.ScenarioName = m.ScenarioName;
            return d;
        }

        public ScenarioModel ConvertToVModel(Scenario m, ScenarioModel d)
        {
            d.ID = m.ID;
            d.ReliefLoad = m.ReliefLoad;
            d.ReliefMW = m.ReliefMW;
            d.ReliefPressure = m.ReliefPressure;
            d.ReliefTemperature = m.ReliefTemperature;
            d.ScenarioName = m.ScenarioName;
            return d;
        }


        private List<string> GetScenarioNames(string eqType)
        {
            List<string> list = new List<string>();
            if (eqType == "Tower")
            {
                list.Add("Blocked Outlet");
                list.Add("Reflux Failure");
                list.Add("General Electric Power Failure");
                list.Add("Partial Electric Power Failure");
                list.Add("Cooling Water Failure");
                list.Add("Refrigerant Failure");
                list.Add("PumpAround Failure");
                list.Add("Abnormal Heat Input");
                list.Add("Cold Feed Stops");
                list.Add("Inlet Valve Fails Open");
                list.Add("Fire");
                list.Add("Steam Failure");
                list.Add("Automatic Controls Failure");
            }
            else if (eqType == "Drum")
            {
                list.Add("Blocked Outlet");
                list.Add("Inlet Valve Fails Open");
                list.Add("Fire");
                list.Add("Depressuring");
            }
            else if (eqType == "Compressor")
            {
                list.Add("Blocked Outlet");

            }
            else if (eqType == "StorageTank")
            {
                list.Add("Fire");

            }
            else if (eqType == "HX")
            {
                HeatExchangerDAL hxDAL = new HeatExchangerDAL();
                HeatExchanger hx = hxDAL.GetModel(SessionProtectedSystem);
                if (hx.HXType == "Shell-Tube")
                {
                    list.Add("Blocked Outlet");
                    list.Add("Tube Rupture");
                }
                
                list.Add("Fire");


            }
            return list;
        }

        private List<SystemScenarioFactor> GetSystemScenarioFactors()
        {

            SystemScenarioFactorDAL db = new SystemScenarioFactorDAL();
            IList<SystemScenarioFactor> list = db.GetAllList(SessionProtectedSystem);
            return list.ToList();

        }

        private string GetSystemScenarioFactor(string category, string categoryvalue, string ScenarioName)
        {
            string factor = "0";

            SystemScenarioFactorDAL db = new SystemScenarioFactorDAL();
            SystemScenarioFactor model = db.GetSystemScenarioFactor(SessionPlant, category, categoryvalue);
            switch (ScenarioName)
            {
                case "BlockedOutlet":
                    factor = model.BlockedOutlet;
                    break;
                case "RefluxFailure":
                    factor = model.RefluxFailure;
                    break;
                case "GeneralElectricPowerFailure":
                    factor = model.GeneralElectricPowerFailure;
                    break;
                case "PartialElectricPowerFailure":
                    factor = model.GeneralElectricPowerFailure;
                    break;
                case "CoolingWaterFailure":
                    factor = model.CoolingWaterFailure;
                    break;
                case "RefrigerantFailure":
                    factor = model.RefrigerantFailure;
                    break;
                case "PumpAroundFailure":
                    factor = model.PumpAroundFailure;
                    break;
                case "AbnormalHeatInput":
                    factor = model.AbnormalHeatInput;
                    break;
                case "ColdFeedStops":
                    factor = model.ColdFeedStops;
                    break;
                case "InletValveFailsOpen":
                    factor = model.InletValveFailsOpen;
                    break;
                case "Fire":
                    factor = model.Fire;
                    break;
                case "SteamFailure":
                    factor = model.SteamFailure;
                    break;
                case "AutomaticControlsFailure":
                    factor = model.AutomaticControlsFailure;
                    break;
            }
            return factor;
        }

        public ChangeUnitDelegate cud { get; set; }
        public void ExcuteThumbMoved(object ColInfo,object OrigionUnit,object TargetUnit)
        {
            string str = ColInfo.ToString();
        }
    }
}
