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

namespace ReliefProMain.ViewModel
{
    public class TowerScenarioCalcVM:ViewModelBase
    {
        private string _ReliefLoad;
        public string ReliefLoad
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

        private string _ReliefMW;
        public string ReliefMW
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
        private string _ReliefTemperature;
        public string ReliefTemperature
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
        private string _ReliefPressure;
        public string ReliefPressure
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

        public int ScenarioID { set; get; }
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public TowerScenarioCalcVM(int scenarioID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            ScenarioID = scenarioID;
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            dbScenario dbsc = new dbScenario();
            Scenario s = dbsc.GetModel(ScenarioID, SessionProtectedSystem);
            ReliefLoad = s.ReliefLoad;
            ReliefMW = s.ReliefMW;
            ReliefPressure = s.ReliefPressure;
            ReliefTemperature = s.ReliefTemperature;
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
            TowerScenarioFeedVM vm = new TowerScenarioFeedVM(ScenarioID, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
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
            Balance();
            double overHeadWeightFlow = 0;
            double waterWeightFlow = 0;
            double FeedTotal = 0;
            double ProductTotal = 0;
            double HeatTotal = 0;

            dbPSV dbpsv = new dbPSV();
            PSV psv = dbpsv.GetModel(SessionProtectedSystem);

            dbLatent dblatent = new dbLatent();
            Latent latent = dblatent.GetModel(SessionProtectedSystem);

            dbCustomStream dbCS = new dbCustomStream();
            dbTowerScenarioStream db = new dbTowerScenarioStream();
            dbTowerFlashProduct dbFlashP = new dbTowerFlashProduct();
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
                        ProductTotal = ProductTotal + (double.Parse(s.FlowCalcFactor) * double.Parse(product.SpEnthalpy) * double.Parse(product.WeightFlow));
                        if (cstream.ProdType == "6")
                        {
                            waterWeightFlow = double.Parse(cstream.WeightFlow);
                        }
                        if (cstream.ProdType == "4")
                        {
                            overHeadWeightFlow = double.Parse(cstream.WeightFlow);
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
            IList<TowerScenarioHX> list = dbTSHX.GetAllList(SessionProtectedSystem, ScenarioID);
            foreach (TowerScenarioHX shx in list)
            {
                if (!shx.DutyLost)
                {
                    TowerHXDetail detail = dbDetail.GetModel(SessionProtectedSystem, shx.DetailID);
                    HeatTotal = HeatTotal + double.Parse(shx.DutyCalcFactor) * double.Parse(detail.Duty);
                }
            }

            double latestH = double.Parse(latent.LatentEnthalpy);
            double totalH = FeedTotal - ProductTotal + HeatTotal;
            double wAccumulation = totalH / latestH + overHeadWeightFlow;
            double wRelief = wAccumulation;
            if (wRelief < 0)
            {
                wRelief = 0;
            }
            double reliefLoad = wAccumulation + waterWeightFlow;
            double reliefMW = (wAccumulation + waterWeightFlow) / (wAccumulation / double.Parse(latent.ReliefOHWeightFlow) + waterWeightFlow / 18);
            ReliefTemperature = latent.ReliefTemperature;            
            ReliefPressure = latent.ReliefPressure;
            ReliefLoad = reliefLoad.ToString();
            ReliefMW = reliefMW.ToString();
            dbScenario dbTS = new dbScenario();
            Scenario scenario = dbTS.GetModel(ScenarioID, SessionProtectedSystem);
            scenario.ReliefLoad = ReliefLoad.ToString();
            scenario.ReliefPressure = latent.ReliefPressure;
            scenario.ReliefMW = ReliefMW.ToString();
            scenario.ReliefTemperature = latent.ReliefTemperature;
            dbTS.Update(scenario, SessionProtectedSystem);
            SessionProtectedSystem.Flush();
        }

        private void Balance()
        {

            dbTowerScenarioStream db = new dbTowerScenarioStream();
            dbStream dbstream = new dbStream();
            dbTowerFlashProduct dbtfp = new ReliefProDAL.dbTowerFlashProduct();
            IList<TowerScenarioStream> feeds = db.GetAllList(SessionProtectedSystem, ScenarioID, false);
            IList<TowerScenarioStream> products = db.GetAllList(SessionProtectedSystem, ScenarioID, true);
            double Total = 0;
            double currentTotal = 0;
            double diffTotal = 0;
            foreach (TowerScenarioStream s in feeds)
            {
                CustomStream cs = dbstream.GetModel(SessionProtectedSystem, s.StreamName);
                double wf = double.Parse(cs.WeightFlow);
                Total = Total + wf;
                if (!s.FlowStop)
                {
                    currentTotal = wf * double.Parse(s.FlowCalcFactor);
                }
            }
            diffTotal = Total - currentTotal;

            IList<TowerFlashProduct> listP = dbtfp.GetAllList(SessionProtectedSystem);

            IList<TowerFlashProduct> listP1 = (from p in listP
                                               where (p.ProdType != "3" || p.ProdType != "4" || p.ProdType != "6")
                                               orderby p.SpEnthalpy descending
                                               select p).ToList();

            int count = listP1.Count;
            for (int i = 0; i < count; i++)
            {
                TowerFlashProduct p = listP1[i];
                TowerScenarioStream s = (from m in products
                                         where m.StreamName == p.StreamName
                                         select m).SingleOrDefault();
                double factor = 1;
                double flowrate = double.Parse(p.WeightFlow);
                double tempH = factor * flowrate;
                if (tempH >= diffTotal)
                {
                    double tempfactor = (tempH - diffTotal) / tempH;
                    s.FlowCalcFactor = tempfactor.ToString();
                    diffTotal = 0;
                    db.Update(s, SessionProtectedSystem);
                    break;
                }
                else
                {
                    s.FlowCalcFactor = "0";
                    db.Update(s, SessionProtectedSystem);
                    diffTotal = diffTotal - tempH;
                }
            }
            if (diffTotal > 0)
            {
                IList<TowerFlashProduct> listP2 = (from p in listP
                                                   where (p.ProdType == "3" || p.ProdType == "4" || p.ProdType == "6")
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
                    double flowrate = double.Parse(p.WeightFlow);
                    double tempH = factor * flowrate;
                    if (tempH >= diffTotal)
                    {
                        double tempfactor = (tempH - diffTotal) / tempH;
                        s.FlowCalcFactor = tempfactor.ToString();
                        db.Update(s, SessionProtectedSystem);
                        diffTotal = 0;
                        break;
                    }
                    else
                    {
                        s.FlowCalcFactor = "0";
                        db.Update(s, SessionProtectedSystem);
                        diffTotal = diffTotal - tempH;
                    }
                }
            }
            SessionProtectedSystem.Flush();


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
            dbScenario dbTS = new dbScenario();
            Scenario scenario = dbTS.GetModel(ScenarioID, SessionProtectedSystem);
            scenario.ReliefLoad = ReliefLoad;
            scenario.ReliefMW = ReliefMW;
            scenario.ReliefTemperature = ReliefTemperature;
            scenario.ReliefPressure = ReliefPressure;
            dbTS.Update(scenario, SessionProtectedSystem);
            SessionProtectedSystem.Flush();

            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult=true;
            }
        }
    }
}
