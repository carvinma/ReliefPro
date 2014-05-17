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

namespace ReliefProMain.ViewModel
{
    public class ScenarioListVM:ViewModelBase
    {
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

        public string dbProtectedSystemFile { set; get; }
        public string dbPlantFile { set; get; }
        public List<string> ScenarioNameList { set; get; }
        public ScenarioListVM( string dbPSFile, string dbPFile)
        {
            ScenarioNameList = GetScenarioNames();
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            Scenarios= new ObservableCollection<ScenarioModel>();
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbScenario db = new dbScenario();
                IList<Scenario> list = db.GetAllList(Session);
                foreach (Scenario s in list)
                {
                    ScenarioModel m = new ScenarioModel();
                    m.ID = s.ID;
                    m.ScenarioName = s.ScenarioName;
                    m.ReliefTemperature = s.ReliefTemperature;
                    m.ReliefPressure = s.ReliefPressure;
                    m.ReliefMW = s.ReliefMW;
                    Scenarios.Add(m);
                }
            }
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
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbScenario db = new dbScenario();
                Scenario s = new Scenario();
                db.Add(s, Session);

                ScenarioModel d = new ScenarioModel();
                d = ConvertToVModel(s, d);
                d.SeqNumber = Scenarios.Count - 1;
                Scenarios.Add(d);
            }
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
            int idx=int.Parse(obj.ToString());
            Scenarios.RemoveAt(idx);
            for (int i = 0; i < Scenarios.Count; i++)
            {
                Scenarios[i].SeqNumber = i;
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
                using (var helper = new NHibernateHelper(dbProtectedSystemFile))
                {
                    var Session = helper.GetCurrentSession();
                    dbScenario db = new dbScenario();
                    Scenario sce = db.GetModel(ScenarioID, Session);
                    sce.ScenarioName = SelectedScenario.ScenarioName;
                    db.Update(sce, Session);
                    string ScenarioName = SelectedScenario.ScenarioName.Replace(" ", string.Empty);
                    if (ScenarioName.Contains("Fire"))
                    {
                        CreateTowerFire(ScenarioID, Session);
                    }
                    else if (ScenarioName.Contains("Inlet"))
                    {
                        CreateInletValveOpen(ScenarioID, Session);
                    }
                    else
                    {
                        CreateTowerCommon(ScenarioID, ScenarioName, Session);
                    }

                }
            }
        }



        private void CreateTowerFire(int ScenarioID,NHibernate.ISession Session)
        {
            dbTowerFire dbtf = new dbTowerFire();
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

            ReliefProMain.View.TowerFire.TowerFireView v = new View.TowerFire.TowerFireView();
            TowerFire.TowerFireVM vm = new TowerFire.TowerFireVM(ScenarioID, dbProtectedSystemFile, dbPlantFile);
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                //SelectedScenario.ReliefLoad = vm.model.CurrentTowerFire.ReliefLoad;
                //SelectedScenario.ReliefPressure = vm.model.CurrentTowerFire.ReliefPressure;
                //SelectedScenario.ReliefTemperature = vm.model.CurrentTowerFire.ReliefTemperature;
                //SelectedScenario.ReliefMW = vm.model.CurrentTowerFire.ReliefMW;
            }
        }

        private void CreateTowerCommon(int ScenarioID,string ScenarioName, NHibernate.ISession Session)
        {
            
            dbSource dbSource = new dbSource();
            List<Source> listSource = dbSource.GetAllList(Session).ToList();
            dbTowerScenarioStream dbTowerSS = new dbTowerScenarioStream();
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

                    if (ScenarioName=="BlockedOutlet" && double.Parse(detail.Duty)<0)
                    {
                        tsHX.DutyLost = true;
                    }


                    dbTSHX.Add(tsHX, Session);
                }
            }
            TowerScenarioCalcView v = new TowerScenarioCalcView();
            v.ScenarioID = ScenarioID;
            v.dbPlantFile = dbPlantFile;
            v.dbProtectedSystemFile = dbProtectedSystemFile;
            TowerScenarioCalcVM vm = new TowerScenarioCalcVM();
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                //SelectedScenario.ReliefLoad = vm.CurrentScenario.ReliefLoad;
                //SelectedScenario.ReliefPressure = vm.CurrentScenario.ReliefPressure;
                //SelectedScenario.ReliefTemperature = vm.CurrentScenario.ReliefTemperature;
                //SelectedScenario.ReliefMW = vm.CurrentScenario.ReliefMW;
            }
        }

        private void CreateInletValveOpen(int ScenarioID,  NHibernate.ISession Session)
        {
            InletValveOpenView v = new InletValveOpenView();           
            InletValveOpenVM vm = new InletValveOpenVM(ScenarioID,dbProtectedSystemFile,dbPlantFile);
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                
            }
        }


        public Scenario ConvertToDBModel( ScenarioModel m,Scenario d)
        {
            d.ID = m.ID;
            d.ReliefLoad = m.ReliefLoad;
            d.ReliefMW = m.ReliefMW;
            d.ReliefPressure = m.ReliefPressure;
            d.ReliefTemperature = m.ReliefTemperature;
            d.ScenarioName = m.ScenarioName;
            return d;
        }

        public ScenarioModel ConvertToVModel(Scenario m,ScenarioModel d)
        {
            d.ID = m.ID;
            d.ReliefLoad = m.ReliefLoad;
            d.ReliefMW = m.ReliefMW;
            d.ReliefPressure = m.ReliefPressure;
            d.ReliefTemperature = m.ReliefTemperature;
            d.ScenarioName = m.ScenarioName;
            return d;
        }


        private List<string> GetScenarioNames()
        {
            List<string> list = new List<string>();
            list.Add("Blocked Outlet");
            list.Add("Reflux Failure");
            list.Add("Electric Power Failure");
            list.Add("Cooling Water Failure");
            list.Add("Refrigerant Failure");
            list.Add("PumpAround Failure");
            list.Add("Abnormal Heat Input");
            list.Add("Cold Feed Stops");
            list.Add("Inlet Valve Fails Open");
            list.Add("Fire");
            list.Add("Steam Failure");
            list.Add("Automatic Controls Failure");

            return list;
        }

        private List<SystemScenarioFactor> GetSystemScenarioFactors()
        {
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                dbSystemScenarioFactor db = new dbSystemScenarioFactor();
                var Session = helper.GetCurrentSession();
                IList<SystemScenarioFactor> list = db.GetAllList(Session);
                return list.ToList();
            }

        }

        private string GetSystemScenarioFactor(string category, string categoryvalue, string ScenarioName)
        {
            string factor = "0";
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                dbSystemScenarioFactor db = new dbSystemScenarioFactor();
                var Session = helper.GetCurrentSession();
                SystemScenarioFactor model = db.GetSystemScenarioFactor(Session, category, categoryvalue);
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


            }
            return factor;
        }


    }
}
 