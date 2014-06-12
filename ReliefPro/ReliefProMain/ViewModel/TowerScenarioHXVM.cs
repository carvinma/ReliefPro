using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using System.Collections.ObjectModel;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using ReliefProMain.View;
using NHibernate;
using ReliefProMain.Model;

namespace ReliefProMain.ViewModel
{
    public class TowerScenarioHXVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public int HeaterType { get; set; }
        public int ScenarioID { get; set; }
        private CondenserCalc condenserCalc;
        public CondenserCalcDAL condenserCalcDAL;
        private Dictionary<int, TowerScenarioHXModel> dicHXs;
        TowerScenarioHXDAL towerScenarioHXDAL ;
        public double ScenarioCondenserDuty = 0;
        public Accumulator CurrentAccumulator { get; set; }
        
        private string _IsDisplay;
        public string IsDisplay
        {
            get
            {
                return this._IsDisplay;
            }
            set
            {
                this._IsDisplay = value;
                OnPropertyChanged("IsDisplay");
            }
        }


        private string _SurgeTime;
        public string SurgeTime
        {
            get
            {
                return this._SurgeTime;
            }
            set
            {
                this._SurgeTime = value;
                OnPropertyChanged("SurgeTime");
            }
        }
        private bool _IsFlooding;
        public bool IsFlooding
        {
            get
            {
                return this._IsFlooding;
            }
            set
            {
                this._IsFlooding = value;
                if (_IsFlooding)
                {
                    IsSurgeTime = false;
                    for(int i=0;i<Details.Count;i++)
                    {
                        TowerScenarioHXModel detail =Details[i];
                        detail.DutyLost = true;
                    }
                }

                OnPropertyChanged("IsFlooding");
            }
        }

        private bool _IsSurgeTime;
        public bool IsSurgeTime
        {
            get
            {
                return this._IsSurgeTime;
            }
            set
            {
                this._IsSurgeTime = value;
                if (_IsSurgeTime)
                {
                    IsFlooding = false;

                }
                else
                {

                }
                OnPropertyChanged("IsSurgeTime");
            }
        }
        //list of orders from the customer
        public ObservableCollection<TowerScenarioHXModel> _Details;
        public ObservableCollection<TowerScenarioHXModel> Details
        {
            get { return _Details; }
            set
            {
                _Details = value;
                OnPropertyChanged("Details");
            }
        }
        public TowerScenarioHXModel _SelectedHX;
        public TowerScenarioHXModel SelectedHX
        {
            get { return _SelectedHX; }
            set
            {
                _SelectedHX = value;
                OnPropertyChanged("SelectedHX");
            }
        }
        
        public TowerScenarioHXVM(int type, int scenarioID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            this.SessionPlant = sessionPlant;
            this.SessionProtectedSystem = sessionProtectedSystem;
            ScenarioID=scenarioID;
            HeaterType = type;
            
            if (HeaterType ==1)
            {
                IsDisplay = "Visible";
            }
            else
            {
                IsDisplay = "Hidden";
            }
            towerScenarioHXDAL = new TowerScenarioHXDAL();
            dicHXs = new Dictionary<int, TowerScenarioHXModel>();
            Details = GetTowerHXScenarioDetails();
            condenserCalcDAL = new CondenserCalcDAL();
            condenserCalc = condenserCalcDAL.GetModel(1, this.SessionProtectedSystem);
            if (condenserCalc != null)
            {
                IsFlooding = condenserCalc.Flooding;
                SurgeTime = condenserCalc.SurgeTime;
                IsSurgeTime = condenserCalc.IsSurgeTime;    
            }
           
        }
        internal ObservableCollection<TowerScenarioHXModel> GetTowerHXScenarioDetails()
        {
            Details = new ObservableCollection<TowerScenarioHXModel>();
            IList<TowerScenarioHX> list = towerScenarioHXDAL.GetAllList(SessionProtectedSystem, ScenarioID, HeaterType).ToList();
            foreach (TowerScenarioHX hx in list)
            {
                double duty = GetCondenserDetailDuty(SessionProtectedSystem, hx.DetailID);
                ScenarioCondenserDuty = ScenarioCondenserDuty + hx.DutyCalcFactor * duty;
                TowerScenarioHXModel shx = new TowerScenarioHXModel(hx);
                Details.Add(shx);
                dicHXs.Add(shx.ID, shx);
            }

            return Details;
        }
       
        private ICommand _CalculateSurgeTimeClick;
        public ICommand CalculateSurgeTimeClick
        {
            get { return _CalculateSurgeTimeClick ?? (_CalculateSurgeTimeClick = new RelayCommand(CalculateSurgeTime)); }
        }


        private void CalculateSurgeTime(object win)
        {
            AccumulatorDAL dbaccumulator = new AccumulatorDAL();
            CurrentAccumulator = dbaccumulator.GetModel(SessionProtectedSystem);
            
            ScenarioDAL dbTS = new ScenarioDAL();
            Scenario ts = dbTS.GetModel(ScenarioID, SessionProtectedSystem);

            double refluxFlowStops = 0;
            double refluxFlow = 0;
            double ohProductFlowStops = 0;
            double ohProductFlow = GetOHProductFlow(SessionProtectedSystem);

            double density = GetLatentLiquidDensity(SessionProtectedSystem);
            double totalCondenserDuty = Math.Abs(ScenarioCondenserDuty);
            double latent = GetLatent(SessionProtectedSystem);
            double volumeflowrate = totalCondenserDuty / latent / density;
            double totalVolumeticFlowRate = volumeflowrate - refluxFlow * refluxFlowStops - ohProductFlow * ohProductFlowStops;
            double accumulatorTotalVolume = 0;
            double accumulatorPartialVolume = 0;
            double diameter = 0;
            double liquidlevel = 0;
            double length = 0;
            if (string.IsNullOrEmpty(CurrentAccumulator.Diameter))
            {
                MessageBox.Show("Accumulator's diameter cann't be empty.","Message Box");
                return;
            }
            else
            {
                diameter = double.Parse(CurrentAccumulator.Diameter);
            }

            if (string.IsNullOrEmpty(CurrentAccumulator.NormalLiquidLevel))
            {
                MessageBox.Show("Accumulator's normal liquid level cann't be empty.", "Message Box");
                return;
            }
            else
            {
                liquidlevel = double.Parse(CurrentAccumulator.NormalLiquidLevel);
            }

            if (string.IsNullOrEmpty(CurrentAccumulator.Length))
            {
                MessageBox.Show("Accumulator's length cann't be empty.", "Message Box");
                return;
            }
            else
            {
                length = double.Parse(CurrentAccumulator.Length);
            }

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
            double dSurgeTime = surgeVolume * 60 / totalVolumeticFlowRate;
            SurgeTime = dSurgeTime.ToString();
        }

        private double GetLatent(NHibernate.ISession Session)
        {
            double r=0;
            LatentDAL db = new LatentDAL();
            Latent model=db.GetModel(Session);
            if (model != null)
            {
                r = double.Parse(model.LatentEnthalpy);
            }
            return r;
        }
        private double GetLatentLiquidDensity(NHibernate.ISession Session)
        {
            double r = 0;
            LatentProductDAL db = new LatentProductDAL();
            LatentProduct model = db.GetModel(Session, "2");
            if (model != null)
            {
                r = double.Parse(model.BulkDensityAct);
            }
            return r;
        }
        private double GetCondenserDetailDuty(NHibernate.ISession Session,int id)
        {
            double r = 0;
            TowerHXDetailDAL db = new TowerHXDetailDAL();
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
            StreamDAL db = new StreamDAL();
            IList<CustomStream> list = db.GetAllList(Session);
            if (list != null)
            {
                foreach (CustomStream s in list)
                {
                    if (s.TotalMolarRate != "0")
                    {
                        double weightflow = double.Parse(s.WeightFlow);
                        double bulkdensityact = double.Parse(s.BulkDensityAct);
                        if (s.ProdType == "2" || s.ProdType == "6")
                        {
                            r = r + weightflow / bulkdensityact;
                        }
                    }

                }
            }
            return r*3600;
        }

        private ICommand _PinchCalcCommand;
        public ICommand PinchCalcCommand
        {
            get { return _PinchCalcCommand ?? (_PinchCalcCommand = new RelayCommand(PinchCalc)); }
        }


        private void PinchCalc(object obj)
        {
            int ID = int.Parse(obj.ToString());
            SelectedHX = dicHXs[ID];
            ReboilerPinchView v = new ReboilerPinchView();
            ReboilerPinchVM vm = new ReboilerPinchVM(ID, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (v.ShowDialog() == true)
            {
                SelectedHX.IsPinch = vm.IsPinch;
                if (vm.Factor != "")
                {
                    SelectedHX.PinchFactor = double.Parse(vm.Factor);
                }
            }
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
            if (HeaterType == 1)
            {
                if (condenserCalc == null)
                {
                    condenserCalc = new CondenserCalc();
                    condenserCalc.Flooding = IsFlooding;
                    condenserCalc.IsSurgeTime = IsSurgeTime;
                    condenserCalc.SurgeTime = SurgeTime;
                    condenserCalcDAL.Add(condenserCalc, SessionProtectedSystem);
                }
                else
                {
                    condenserCalc.Flooding = IsFlooding;
                    condenserCalc.IsSurgeTime = IsSurgeTime;
                    condenserCalc.SurgeTime = SurgeTime;
                    condenserCalcDAL.Update(condenserCalc, SessionProtectedSystem);
                }
                foreach (TowerScenarioHXModel m in Details)
                {
                    towerScenarioHXDAL.Update(m.model, SessionProtectedSystem);
                }

                SessionProtectedSystem.Flush();
            }
            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }



    }
}
