using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.IO;

namespace ReliefProMain.ViewModel
{
    public class AbnormalHeatInputVM:ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private int ScenarioID;

        private ScenarioHeatSourceDAL db;
        private HeatSourceDAL dbHS;
        
        private ObservableCollection<ScenarioHeatSourceModel> _HeatSources;
        public ObservableCollection<ScenarioHeatSourceModel> HeatSources
        {
            get { return _HeatSources; }
            set
            {
                _HeatSources = value;
                OnPropertyChanged("_HeatSources");
            }
        }
        private HeatSourceModel _SelectedHeatSource;
        public HeatSourceModel SelectedHeatSource
        {
            get { return _SelectedHeatSource; }
            set
            {
                _SelectedHeatSource = value;
                OnPropertyChanged("SelectedHeatSource");
            }
        }

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


        public AbnormalHeatInputVM(int ScenarioID, ISession SessionPlant, ISession SessionProtectedSystem)
        {
            this.ScenarioID = ScenarioID;
            this.SessionPlant = SessionPlant;
            this.SessionProtectedSystem = SessionProtectedSystem;
            HeatSources = new ObservableCollection<ScenarioHeatSourceModel>();
            db = new ScenarioHeatSourceDAL();
            IList<ScenarioHeatSource> list = db.GetScenarioHeatSourceList(this.SessionProtectedSystem, ScenarioID);
            foreach (ScenarioHeatSource s in list)
            {
                HeatSource hs = dbHS.GetModel(s.HeatSourceID, SessionProtectedSystem);
                ScenarioHeatSourceModel shs = new ScenarioHeatSourceModel(s);
                shs.Duty = hs.Duty;
                shs.DutyFactor = s.DutyFactor;
                shs.HeatSourceID = s.HeatSourceID;
                shs.HeatSourceName = hs.HeatSourceName;
                shs.HeatSourceType = hs.HeatSourceType;

            }
            if (list.Count == 0)
            {
                SourceDAL dbSource = new SourceDAL();
                HeatSourceDAL dbhs = new HeatSourceDAL();
               
                IList<HeatSource> listHeatSource = dbhs.GetAllList(SessionProtectedSystem);
                foreach (HeatSource hs in listHeatSource)
                {                  
                    if (hs.HeatSourceType != "Feed/Bottom HX")
                    {
                        ScenarioHeatSource shs = new ScenarioHeatSource();
                        ScenarioHeatSourceModel shsm = new ScenarioHeatSourceModel(shs);
                        shsm.HeatSourceID = hs.ID;
                        shsm.DutyFactor = "1";
                        shsm.Duty = hs.Duty;
                        shsm.ScenarioStreamID = 0;
                        shsm.ScenarioID = ScenarioID;
                        shsm.HeatSourceName = hs.HeatSourceName;
                        shsm.HeatSourceType = hs.HeatSourceType;
                        HeatSources.Add(shsm);
                    }
                }
            }
            ScenarioDAL dbsc = new ScenarioDAL();
            Scenario sc = dbsc.GetModel(ScenarioID, SessionProtectedSystem);
            ReliefLoad = sc.ReliefLoad;
            ReliefMW = sc.ReliefMW;
            ReliefPressure = sc.ReliefPressure;
            ReliefTemperature = sc.ReliefTemperature;
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
            TowerHXDetailDAL dbDetail = new TowerHXDetailDAL();
            IList<TowerScenarioHX> list = dbTSHX.GetAllList(SessionProtectedSystem, ScenarioID);
            foreach (TowerScenarioHX shx in list)
            {
                if (!shx.DutyLost)
                {
                    TowerHXDetail detail = dbDetail.GetModel(SessionProtectedSystem, shx.DetailID);
                    HeatTotal = HeatTotal + double.Parse(shx.DutyCalcFactor) * double.Parse(detail.Duty);
                }
            }
            double totalAbnomalDuty = GetAbnormalTotalDuty();
            double latestH = double.Parse(latent.LatentEnthalpy);
            double totalH = FeedTotal - ProductTotal + HeatTotal + totalAbnomalDuty;
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
            ScenarioDAL dbTS = new ScenarioDAL();
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

            TowerScenarioStreamDAL db = new TowerScenarioStreamDAL();
            StreamDAL dbstream = new StreamDAL();
            TowerFlashProductDAL dbtfp = new ReliefProDAL.TowerFlashProductDAL();
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
        private double GetAbnormalTotalDuty()
        {
            double total = 0;
            foreach (ScenarioHeatSourceModel m in HeatSources)
            { 
                total=total+(double.Parse(m.DutyFactor)-1)*double.Parse(m.Duty);
            }
            return total;
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

        public void Save(object obj)
        {
            
            foreach (ScenarioHeatSourceModel m in HeatSources)
            {
                db.Update(m.model, SessionProtectedSystem);
            }
            ScenarioDAL dbTS = new ScenarioDAL();
            Scenario scenario = dbTS.GetModel(ScenarioID, SessionProtectedSystem);
            scenario.ReliefLoad = ReliefLoad;
            scenario.ReliefMW = ReliefMW;
            scenario.ReliefTemperature = ReliefTemperature;
            scenario.ReliefPressure = ReliefPressure;
            dbTS.Update(scenario, SessionProtectedSystem);

            SessionProtectedSystem.Flush();
            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private ObservableCollection<ScenarioHeatSourceModel> GetHeatSources(int ScenarioStreamID)
        {
            ObservableCollection<ScenarioHeatSourceModel> list = new ObservableCollection<ScenarioHeatSourceModel>();
            IList<ScenarioHeatSource> eqs = db.GetScenarioStreamList(SessionProtectedSystem, ScenarioStreamID);
            foreach (ScenarioHeatSource eq in eqs)
            {
                HeatSource hs = dbHS.GetModel(eq.HeatSourceID, SessionProtectedSystem);
                ScenarioHeatSourceModel model = new ScenarioHeatSourceModel(eq);
                model.HeatSourceName = hs.HeatSourceName;
                model.HeatSourceType = hs.HeatSourceType;
                model.Duty = hs.Duty;
                list.Add(model);
            }
            return list;
        }
       
    }
}
