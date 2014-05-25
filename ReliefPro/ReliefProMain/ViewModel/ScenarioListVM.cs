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
using ReliefProMain.ViewModel.Drum;
using System.Windows;

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
            EqName = eqType;
            EqType = eqType;
            PrzFile = przFile;
            PrzVersion = version;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;

            Scenarios = GetScenarios() ;
            ScenarioNameList = GetScenarioNames(eqType);

           

        }

        private ObservableCollection<ScenarioModel> GetScenarios()
        {
             ObservableCollection<ScenarioModel> scenarios = new ObservableCollection<ScenarioModel>();
             dbScenario db = new dbScenario();
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

            dbScenario db = new dbScenario();
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

                dbScenario db = new dbScenario();
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

                dbScenario db = new dbScenario();
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
                        CreateInletValveOpen(EqType, ScenarioID, SessionProtectedSystem);
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
                        Drum_BlockedOutlet v = new Drum_BlockedOutlet();
                        v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                        DrumBlockedOutletVM vm = new DrumBlockedOutletVM(ScenarioID, PrzFile,  PrzVersion, SessionProtectedSystem, SessionPlant,DirPlant,DirProtectedSystem);
                        v.DataContext = vm;
                        if (v.ShowDialog() == true)
                        {
                            //SelectedScenario.ReliefLoad = vm.model.CurrentTowerFire.ReliefLoad;
                            //SelectedScenario.ReliefPressure = vm.model.CurrentTowerFire.ReliefPressure;
                            //SelectedScenario.ReliefTemperature = vm.model.CurrentTowerFire.ReliefTemperature;
                            //SelectedScenario.ReliefMW = vm.model.CurrentTowerFire.ReliefMW;
                        }
                        //CreateDrumOutlet(ScenarioID, Session);
                    }
                    else if (ScenarioName.Contains("Fire"))
                    {
                        Drum_fire v = new Drum_fire();
                        v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                        DrumFireVM vm = new DrumFireVM(ScenarioID, PrzFile, PrzVersion, SessionProtectedSystem, SessionPlant, DirPlant, DirProtectedSystem);
                        v.DataContext = vm;
                        if (v.ShowDialog() == true)
                        {
                            //需要把ReliefLoad等值传回给SelectedScenario.ReliefLoad。 参考CreateInletValveOpen
                        }
                        
                    }
                    else if (ScenarioName.Contains("Inlet"))
                    {
                        //CreateInletValveOpen(eqType, ScenarioID, Session);
                    }
                    else if (ScenarioName.Contains("Depressuring"))
                    {
                        DrumDepressuring v = new DrumDepressuring();
                        v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                        DrumDepressuringVM vm = new DrumDepressuringVM(ScenarioID, SessionProtectedSystem, SessionPlant);
                        v.DataContext = vm;
                        v.ShowDialog();
                        //CreateDrumDepressuring(ScenarioID, ScenarioName, Session);
                    }

                }



            }
        }



        private void CreateTowerFire(int ScenarioID, NHibernate.ISession Session)
        {
            dbTowerFire dbtf = new dbTowerFire();
            ReliefProModel.TowerFire t = dbtf.GetModel(Session, ScenarioID);
            if (t == null)
            {
                ReliefProModel.TowerFire tf = new ReliefProModel.TowerFire();
                tf.ScenarioID = ScenarioID;
                tf.IsExist = false;
                tf.HeatInputModel = "API 521";
                dbtf.Add(tf, Session);

                dbTowerFireEq dbtfeq = new dbTowerFireEq();
                dbTower dbtower = new dbTower();
                Tower tower = dbtower.GetModel(Session);
                TowerFireEq eq = new TowerFireEq();
                eq.EqName = tower.TowerName;
                eq.Type = "Column";
                eq.FFactor = "1";
                eq.FireZone = true;
                eq.FireID = tf.ID;
                dbtfeq.Add(eq, Session);

                dbAccumulator dbaccumulator = new dbAccumulator();
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

                dbSideColumn dbsidecolumn = new dbSideColumn();
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


                dbTowerHXDetail dbhx = new dbTowerHXDetail();
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

            ReliefProMain.View.TowerFire.TowerFireView v = new View.TowerFire.TowerFireView();
            TowerFire.TowerFireVM vm = new TowerFire.TowerFireVM(ScenarioID, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                SelectedScenario.ReliefLoad = vm.CurrentModel.ReliefLoad;
                SelectedScenario.ReliefMW = vm.CurrentModel.ReliefMW;
                SelectedScenario.ReliefPressure = vm.CurrentModel.ReliefPressure;
                SelectedScenario.ReliefTemperature = vm.CurrentModel.ReliefTemperature;
            }
        }
        
        private void CreateTowerCommon(int ScenarioID, string ScenarioName, NHibernate.ISession Session)
        {
            dbTowerScenarioStream dbTowerSS = new dbTowerScenarioStream();
            IList<TowerScenarioStream> list = dbTowerSS.GetAllList(Session, ScenarioID);
            if (list.Count == 0)
            {
                dbSource dbSource = new dbSource();
                List<Source> listSource = dbSource.GetAllList(Session).ToList();

                foreach (Source s in listSource)
                {
                    TowerScenarioStream tss = new TowerScenarioStream();
                    tss.ScenarioID = ScenarioID;
                    tss.StreamName = s.StreamName;
                    tss.FlowCalcFactor = GetSystemScenarioFactor("4", s.SourceType, ScenarioName);
                    tss.FlowStop = false;
                    tss.SourceType = s.SourceType;
                    tss.IsProduct = false;
                    dbTowerSS.Add(tss, Session);
                }
                dbTowerFlashProduct dbTFP = new dbTowerFlashProduct();
                List<TowerFlashProduct> listProduct = dbTFP.GetAllList(Session).ToList();
                foreach (TowerFlashProduct p in listProduct)
                {
                    TowerScenarioStream tss = new TowerScenarioStream();
                    tss.ScenarioID = ScenarioID;
                    tss.StreamName = p.StreamName;
                    tss.FlowCalcFactor = "1";
                    tss.FlowStop = false;
                    tss.IsProduct = true;
                    tss.SourceType = string.Empty;
                    dbTowerSS.Add(tss, Session);
                }

                dbTowerScenarioHX dbTSHX = new dbTowerScenarioHX();
                dbTowerHX dbHX = new dbTowerHX();
                List<TowerHX> tHXs = dbHX.GetAllList(Session).ToList();
                foreach (TowerHX hx in tHXs)
                {
                    dbTowerHXDetail dbTHXDetail = new dbTowerHXDetail();
                    List<TowerHXDetail> listTowerHXDetail = dbTHXDetail.GetAllList(Session, hx.ID).ToList();
                    foreach (TowerHXDetail detail in listTowerHXDetail)
                    {
                        string ProcessSideFlowSourceFactor = GetSystemScenarioFactor("1", detail.ProcessSideFlowSource, ScenarioName);
                        string MediumFactor = GetSystemScenarioFactor("2", detail.Medium, ScenarioName);
                        string MediumSideFlowSource = GetSystemScenarioFactor("3", detail.MediumSideFlowSource, ScenarioName);
                        double factor = double.Parse(ProcessSideFlowSourceFactor) * double.Parse(MediumFactor) * double.Parse(MediumSideFlowSource);
                        TowerScenarioHX tsHX = new TowerScenarioHX();
                        tsHX.DetailID = detail.ID;
                        tsHX.ScenarioID = ScenarioID;
                        tsHX.DutyLost = false;
                        tsHX.DutyCalcFactor = factor.ToString();
                        tsHX.DetailName = detail.DetailName;
                        tsHX.Medium = detail.Medium;
                        tsHX.HeaterType = hx.HeaterType;

                        if (ScenarioName == "BlockedOutlet" && double.Parse(detail.Duty) < 0)
                        {
                            tsHX.DutyLost = true;
                        }


                        dbTSHX.Add(tsHX, Session);
                    }
                }
            }

            TowerScenarioCalcView v = new TowerScenarioCalcView();
            TowerScenarioCalcVM vm = new TowerScenarioCalcVM(ScenarioID, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                SelectedScenario.ReliefLoad = vm.ReliefLoad;
                SelectedScenario.ReliefPressure = vm.ReliefPressure;
                SelectedScenario.ReliefTemperature = vm.ReliefTemperature;
                SelectedScenario.ReliefMW = vm.ReliefMW;
                //Scenarios = GetScenarios();
            }
        }

        private void CreateInletValveOpen(string EqType, int ScenarioID, NHibernate.ISession Session)
        {           
            InletValveOpenView v = new InletValveOpenView();
            InletValveOpenVM vm = new InletValveOpenVM(ScenarioID, EqName, EqType, PrzFile, PrzVersion, SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                SelectedScenario.ReliefLoad = vm.ReliefLoad;
                SelectedScenario.ReliefMW = vm.ReliefMW;
                SelectedScenario.ReliefPressure = vm.ReliefPressure;
                SelectedScenario.ReliefTemperature = vm.ReliefTemperature;
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
                list.Add("Electric Power Failure");
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

            return list;
        }

        private List<SystemScenarioFactor> GetSystemScenarioFactors()
        {

            dbSystemScenarioFactor db = new dbSystemScenarioFactor();
            IList<SystemScenarioFactor> list = db.GetAllList(SessionProtectedSystem);
            return list.ToList();

        }

        private string GetSystemScenarioFactor(string category, string categoryvalue, string ScenarioName)
        {
            string factor = "0";

            dbSystemScenarioFactor db = new dbSystemScenarioFactor();
            SystemScenarioFactor model = db.GetSystemScenarioFactor(SessionPlant, category, categoryvalue);
            switch (ScenarioName)
            {
                case "BlockedOutlet":
                    factor = model.BlockedOutlet;
                    break;
                case "RefluxFailure":
                    factor = model.RefluxFailure;
                    break;
                case "ElectricPowerFailure":
                    factor = model.ElectricPowerFailure;
                    break;
                case "PartialElectricPowerFailure":
                    factor = model.ElectricPowerFailure;
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


    }
}
