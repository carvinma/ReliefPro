using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ReliefProModel;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.View.TowerFire;
using ReliefProMain.ViewModel.TowerFire;

namespace ReliefProMain.View
{
    /// <summary>
    /// TowerScenario.xaml 的交互逻辑
    /// </summary>
    public partial class ScenarioListView : Window
    {
        
        public ScenarioListView()
        {
            InitializeComponent();                      
        }
        /*
        public List<Scenario> TowerScenarios { get; set; }
        public List<string> Scenarios { get; set; }
        public string dbProtectedSystemFile;
        public string dbPlantFile;
        public Scenario objTowerScenarioAdd=new Scenario();
        public void calc(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int ScenarioID = int.Parse(btn.Tag.ToString());
            if (btn.Tag != null)
            {
                Scenario ts = GetTowerScenario(ScenarioID);
                if (ts.ScenarioName.Contains("Fire"))
                {
                    TowerFireView v = new TowerFireView();
                    TowerFireVM vm = new TowerFireVM(ScenarioID, dbProtectedSystemFile, dbPlantFile);
                    v.DataContext = vm;
                    v.ShowDialog();
                }
                else
                {
                    TowerScenarioCalcView calc = new TowerScenarioCalcView();
                    calc.ScenarioID = ScenarioID;
                    calc.dbPlantFile = dbPlantFile;
                    calc.dbProtectedSystemFile = dbProtectedSystemFile;
                    calc.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    calc.ShowDialog();
                }
                TowerScenarios = GetTowerScenarios();
                myGrid.ItemsSource = TowerScenarios;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Scenarios = GetScenarios();
            ScenarioName.ItemsSource = Scenarios;

            TowerScenarios = GetTowerScenarios();
            myGrid.ItemsSource = TowerScenarios;

        }

        private List<string> GetScenarios()
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

        private void myGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            objTowerScenarioAdd = myGrid.SelectedItem as Scenario;
            //DataGridRow dgr =(DataGridRow) myGrid.SelectedItem;
            //FrameworkElement element_Calc = myGrid.Columns[3].GetCellContent(dgr);
            //if (element_Calc.GetType() == typeof(Button))
            //{
            //    if (objTowerScenarioAdd.ID == 0)
            //    {
            //        ((Button)element_Calc).IsEnabled = false;
            //    }
            //}
        }

        private void myGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            FrameworkElement element_Scenario = myGrid.Columns[2].GetCellContent(e.Row);
            if (element_Scenario.GetType() == typeof(ComboBox))
            {
                string ScenarioName = ((ComboBox)element_Scenario).SelectedValue.ToString();
                objTowerScenarioAdd.ScenarioName = ScenarioName;
            }
        }

        private void myGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (objTowerScenarioAdd != null)
            {
                if (string.IsNullOrEmpty(objTowerScenarioAdd.ScenarioName))
                {
                    MessageBox.Show("Please Select Scenario");
                    return;
                }
                else if(objTowerScenarioAdd.ID==0)
                {
                    using (var helper = new NHibernateHelper(dbProtectedSystemFile))
                    {
                        dbScenario db = new dbScenario();
                        var Session = helper.GetCurrentSession();
                        //Session.Save(objTowerScenarioAdd);
                        var strScenarioID= db.Add(objTowerScenarioAdd, Session);
                        int ScenarioID = int.Parse(strScenarioID.ToString()) ;
                       string ScenarioName=objTowerScenarioAdd.ScenarioName.Replace(" ",string.Empty);

                       if (ScenarioName.Contains("Fire"))
                       {
                           dbTowerFire dbtf = new dbTowerFire();
                           ReliefProModel.TowerFire tf = new ReliefProModel.TowerFire();
                           tf.ScenarioID = ScenarioID;
                           tf.IsExist = false;
                           tf.HeatInputModel = "API 521";
                           dbtf.Add(tf,Session);

                           dbTowerFireEq dbtfeq = new dbTowerFireEq();
                           dbTower dbtower = new dbTower();
                           Tower tower=dbtower.GetModel(Session);
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

                           dbSideColumn dbsidecolumn=new dbSideColumn();
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




                        dbTowerScenarioHX dbTSHX=new dbTowerScenarioHX();
                        dbTowerHX dbHX = new dbTowerHX();
                        List<TowerHX> tHXs = dbHX.GetAllList(Session).ToList();
                        foreach (TowerHX hx in tHXs)
                        {
                            dbTowerHXDetail dbTHXDetail = new dbTowerHXDetail();
                            List<TowerHXDetail> listTowerHXDetail = dbTHXDetail.GetAllList(Session,hx.ID).ToList();
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
                                dbTSHX.Add(tsHX, Session);
                            }
                        }
                        TowerScenarios = db.GetAllList(Session).ToList();
                        myGrid.ItemsSource = TowerScenarios;
   
                    }
                }
            }
        }

        private List<Scenario> GetTowerScenarios()
        {
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                dbScenario db = new dbScenario();
                var Session = helper.GetCurrentSession();
                IList<Scenario> list = db.GetAllList(Session);
                return list.ToList();
            }
            
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

        public virtual string BlockedOutlet { get; set; }
        public virtual string RefluxFailure { get; set; }
        public virtual string ElectricPowerFailure { get; set; }
        public virtual string CoolingWaterFailure { get; set; }
        public virtual string RefrigerantFailure { get; set; }
        public virtual string PumpAroundFailure { get; set; }
        public virtual string AbnormalHeatInput { get; set; }
        public virtual string ColdFeedStops { get; set; }
        public virtual string InletValveFailsOpen { get; set; }
        public virtual string Fire { get; set; }
        public virtual string SteamFailure { get; set; }
        public virtual string AutomaticControlsFailure { get; set; }

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
                        factor= model.BlockedOutlet;
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

        private Scenario GetTowerScenario(int ScenarioID)
        {
            Scenario model = null;
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                dbScenario db = new dbScenario();
                var Session = helper.GetCurrentSession();
                model = db.GetModel(ScenarioID,Session);
            }
            return model;
        }
         */
        
    }
   
    
}