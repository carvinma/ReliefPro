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
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProModel;
using ReliefProMain.ViewModel;

namespace ReliefProMain.View
{
    /// <summary>
    /// TowerScenarioCalc.xaml 的交互逻辑
    /// </summary>
    public partial class TowerScenarioCalcView : Window
    {
        public int ScenarioID;
        public string dbProtectedSystemFile;
        public string dbPlantFile;
        
        public TowerScenarioCalcView()
        {
            InitializeComponent();
        }

        private void btnFeed_Click(object sender, RoutedEventArgs e)
        {
            TowerScenarioFeedView frm = new TowerScenarioFeedView();
            frm.dbProtectedSystemFile = dbProtectedSystemFile;
            frm.dbPlantFile = dbPlantFile;
            frm.ScenarioID = ScenarioID;
            frm.ShowDialog();
        }

        private void btnReboiler_Click(object sender, RoutedEventArgs e)
        {
            TowerScenarioHXView v = new TowerScenarioHXView();
            v.Title = "Reboiler";
            TowerScenarioHXVM vm = new TowerScenarioHXVM(3, ScenarioID, dbProtectedSystemFile, dbPlantFile);
            v.DataContext = vm;
            v.ShowDialog();
        }

        private void btnPumparoundHeating_Click(object sender, RoutedEventArgs e)
        {
            TowerScenarioHXView v = new TowerScenarioHXView();
            v.Title = "Pumparound Heating";
            TowerScenarioHXVM vm = new TowerScenarioHXVM(4, ScenarioID, dbProtectedSystemFile, dbPlantFile);
            v.DataContext = vm;
            v.ShowDialog();
        }

        private void btnCondenser_Click(object sender, RoutedEventArgs e)
        {
            TowerScenarioHXView v = new TowerScenarioHXView();
            v.Title = "Condenser";
            TowerScenarioHXVM vm = new TowerScenarioHXVM(1, ScenarioID, dbProtectedSystemFile, dbPlantFile);
            v.DataContext = vm;
            v.ShowDialog();
        }

        private void btnPumparoundCooling_Click(object sender, RoutedEventArgs e)
        {
            TowerScenarioHXView v = new TowerScenarioHXView();
            v.Title = "Pumparound Cooling";
            TowerScenarioHXVM vm = new TowerScenarioHXVM(2, ScenarioID, dbProtectedSystemFile, dbPlantFile);
            v.DataContext = vm;
            v.ShowDialog();
        }

        private void btnCalculation_Click(object sender, RoutedEventArgs e)
        {
            double overHeadWeightFlow = 0;
            double waterWeightFlow = 0;
            double FeedTotal = 0;
            double ProductTotal = 0;
            double HeatTotal = 0;
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbPSV dbpsv = new dbPSV();
                PSV psv = dbpsv.GetModel(Session);

                dbLatent dblatent = new dbLatent();
                Latent latent = dblatent.GetModel(Session);

                dbCustomStream dbCS = new dbCustomStream();
                dbTowerScenarioStream db = new dbTowerScenarioStream();
                dbTowerFlashProduct dbFlashP = new dbTowerFlashProduct();
                IList<TowerScenarioStream> listStream = db.GetAllList(Session, ScenarioID);
                foreach (TowerScenarioStream s in listStream)
                {
                    CustomStream cstream = dbCS.GetModel(Session, s.StreamName);
                    if (cstream.IsProduct)
                    {
                        TowerFlashProduct product = dbFlashP.GetModel(Session, cstream.StreamName);
                        if (!s.FlowStop)
                        {
                            ProductTotal = ProductTotal + (double.Parse(s.FlowCalcFactor) * double.Parse(product.SpEnthalpy) * double.Parse(product.WeightFlow));
                            if (cstream.ProdType == "6")
                            {
                                waterWeightFlow = double.Parse(cstream.WeightFlow);
                            }
                        }
                    }
                    else
                    {
                        if (!s.FlowStop)
                        {
                            FeedTotal = FeedTotal + (double.Parse(s.FlowCalcFactor) * double.Parse(cstream.SpEnthalpy) * double.Parse(cstream.WeightFlow));
                        }
                    }
                }





                dbTowerScenarioHX dbTSHX = new dbTowerScenarioHX();
                dbTowerHXDetail dbDetail = new dbTowerHXDetail();
                IList<TowerScenarioHX> list = dbTSHX.GetAllList(Session, ScenarioID);
                foreach (TowerScenarioHX shx in list)
                {
                    if (!shx.DutyLost)
                    {
                        TowerHXDetail detail = dbDetail.GetModel(Session, shx.DetailID);
                        HeatTotal = HeatTotal + double.Parse(shx.DutyCalcFactor) * double.Parse(detail.Duty);
                    }
                }
                overHeadWeightFlow = double.Parse(latent.ReliefOHWeightFlow);
                double latestH = double.Parse(latent.LatestEnthalpy);
                double totalH = FeedTotal - ProductTotal + HeatTotal;
                double wAccumulation = totalH / latestH + overHeadWeightFlow;
                double wRelief = wAccumulation;
                if (wRelief < 0)
                {
                    wRelief = 0;
                }
                double ReliefLoad = wAccumulation + double.Parse(latent.ReliefOHWeightFlow);
                double ReliefMW = (wAccumulation + waterWeightFlow) / (wAccumulation / double.Parse(latent.ReliefOHWeightFlow) + waterWeightFlow / 18);
                txtTemperature.Text = latent.ReliefTemperature;
                txtReliefLoad.Text = ReliefLoad.ToString();
                txtReliefMW.Text = ReliefMW.ToString();

                dbTowerScenario dbTS = new dbTowerScenario();
                TowerScenario scenario = dbTS.GetModel(ScenarioID, Session);
                scenario.ReliefLoad = ReliefLoad.ToString();
                scenario.ReliefMW = ReliefMW.ToString();
                scenario.ReliefTemperature = latent.ReliefTemperature;
                dbTS.Update(scenario, Session);
                Session.Flush();
            }
            this.DialogResult = true;


           
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
           
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbTowerScenario dbTS = new dbTowerScenario();
                TowerScenario scenario = dbTS.GetModel(ScenarioID, Session);
                scenario.ReliefLoad = txtReliefLoad.Text;
                scenario.ReliefMW = txtReliefMW.Text;
                scenario.ReliefTemperature = txtTemperature.Text;
                dbTS.Update(scenario, Session);
                Session.Flush();
            }
            this.DialogResult = true;



        }
    }
}
