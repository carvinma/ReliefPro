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
using System.Collections.ObjectModel;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using ReliefProMain.View;

namespace ReliefProMain.ViewModel
{
    public class TowerScenarioHXVM : ViewModelBase
    {
        public string dbProtectedSystemFile;
        public string dbPlantFile;
        public ObservableCollection<TowerScenarioHX> details = null;
        public int HeaterType { get; set; }
        public int ScenarioID { get; set; }
        public bool IsFlooding { get; set; }
        public bool IsSurgeTime { get; set; }
        public double SurgeTime { get; set; }
        public bool IsDisplay { get; set; }
        public double ScenarioCondenserDuty = 0;
        public Accumulator CurrentAccumulator { get; set; }


        public TowerScenarioHXVM(int type,int scenarioID, string dbPSFile, string dbPFile)
        {
            ScenarioID=scenarioID;
            HeaterType = type;
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            if (HeaterType == 0)
            {
                IsDisplay = true;
            }
            else
            {
                IsDisplay = false;
            }
            
        }
        internal ObservableCollection<TowerScenarioHX> GetTowerHXScenarioDetails()
        {
            details = new ObservableCollection<TowerScenarioHX>();
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                

                dbTowerScenarioHX db = new dbTowerScenarioHX();
                IList<TowerScenarioHX> list = db.GetAllList(Session, ScenarioID, HeaterType).ToList();               
                foreach (TowerScenarioHX hx in list)
                {
                    double duty = GetCondenserDetailDuty(Session,hx.DetailID);
                    ScenarioCondenserDuty =ScenarioCondenserDuty+ double.Parse(hx.DutyCalcFactor) * duty;
                    details.Add(hx);
                }
            }
            return details;
        }
        //list of orders from the customer
        public ObservableCollection<TowerScenarioHX> Details
        {
            get { return GetTowerHXScenarioDetails(); }
            set
            {
                details = value;
                OnPropertyChanged("Details");
            }
        }

        private ICommand _CalculateSurgeTimeClick;
        public ICommand CalculateSurgeTimeClick
        {
            get { return _CalculateSurgeTimeClick ?? (_CalculateSurgeTimeClick = new RelayCommand(CalculateSurgeTime)); }
        }


        private void CalculateSurgeTime(object win)
        {
            
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbAccumulator dbaccumulator = new dbAccumulator();
                CurrentAccumulator=dbaccumulator.GetModel(Session);

                dbTowerScenario dbTS = new dbTowerScenario();
                TowerScenario ts = dbTS.GetModel(ScenarioID, Session);

                double refluxFlowStops = 0;
                double refluxFlow = 0;
                double ohProductFlowStops = 0;
                double ohProductFlow = GetOHProductFlow(Session);

                double density = GetLatentLiquidDensity(Session);
                double totalCondenserDuty = Math.Abs(ScenarioCondenserDuty);                
                double latent = GetLatent(Session);
                double volumeflowrate = totalCondenserDuty / latent / density;
                double totalVolumeticFlowRate = volumeflowrate - refluxFlow * refluxFlowStops - ohProductFlow * ohProductFlowStops;
                double accumulatorTotalVolume = 0;
                double accumulatorPartialVolume = 0;
                double diameter = float.Parse(CurrentAccumulator.Diameter);
                double liquidlevel = float.Parse(CurrentAccumulator.NormalLiquidLevel);
                double length = float.Parse(CurrentAccumulator.Length);
                accumulatorTotalVolume = 3.14159 * Math.Pow(diameter, 2) * length / 4 + 3.14159 * Math.Pow(diameter, 3) / 12;
                if (CurrentAccumulator.Orientation)
                {
                    accumulatorPartialVolume = 3.14159 * Math.Pow(diameter, 2) * liquidlevel / 4 + 3.14159 * Math.Pow(diameter, 3) / 24;
                }
                else
                {
                    accumulatorPartialVolume = liquidlevel * accumulatorTotalVolume / diameter;

                }
                double surgeVolume = accumulatorTotalVolume - accumulatorPartialVolume;
                SurgeTime = surgeVolume * 60 / totalVolumeticFlowRate;
            }
          

        }

        private double GetLatent(NHibernate.ISession Session)
        {
            double r=0;
            dbLatent db = new dbLatent();
            Latent model=db.GetModel(Session);
            if (model != null)
            {
                r = double.Parse(model.LatestEnthalpy);
            }
            return r;
        }
        private double GetLatentLiquidDensity(NHibernate.ISession Session)
        {
            double r = 0;
            dbLatentProduct db = new dbLatentProduct();
            LatentProduct model = db.GetModel(Session, "");
            if (model != null)
            {
                r = double.Parse(model.BulkDensityAct);
            }
            return r;
        }
        private double GetCondenserDetailDuty(NHibernate.ISession Session,int id)
        {
            double r = 0;
            dbTowerHXDetail db = new dbTowerHXDetail();
            TowerHXDetail model = db.GetModel(Session,id);
            if (model != null)
            {
                r = double.Parse(model.Duty);
            }
            return r;
        }

        private double GetOHProductFlow(NHibernate.ISession Session)
        {
            double r = 0;
            dbStream db = new dbStream();
            IList<CustomStream> list = db.GetAllList(Session);
            if (list != null)
            {
                foreach (CustomStream s in list)
                {
                    double weightflow = double.Parse(s.WeightFlow);
                    double bulkdensityact = double.Parse(s.BulkDensityAct);
                    if (s.ProdType == "2" || s.ProdType=="6")
                    {
                        r = r + weightflow / bulkdensityact;
                    }

                }
            }
            return r*3600;
        }
    }
}
