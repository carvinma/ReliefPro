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
using ReliefProMain.Models;
using ProII;

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
        ScenarioDAL scDAL;
        Scenario sc;
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
                    //for(int i=0;i<Details.Count;i++)
                    //{
                    //    TowerScenarioHXModel detail =Details[i];
                    //    detail.DutyLost = true;
                    //}
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
            condenserCalc = condenserCalcDAL.GetModel( this.SessionProtectedSystem,ScenarioID);
            if (condenserCalc != null)
            {
                IsFlooding = condenserCalc.Flooding;
                SurgeTime = condenserCalc.SurgeTime;
                IsSurgeTime = condenserCalc.IsSurgeTime;    
            }
            scDAL = new ScenarioDAL();
            sc = scDAL.GetModel(scenarioID,SessionProtectedSystem);
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

            double refluxFlowStops = 1;
            double refluxFlow = 0;
            double ohProductFlowStops = 1;
            if (CurrentAccumulator != null && (sc.ScenarioName.Contains("Electric Power Failure") || sc.ScenarioName.Contains("Reflux Failure")))
            {
                refluxFlowStops = 0;
                LatentProductDAL lpdal = new LatentProductDAL();
                LatentProduct latentStream = lpdal.GetModel(SessionProtectedSystem, "-1");
                refluxFlow=latentStream.WeightFlow/latentStream.BulkDensityAct;
            }
            double ohProductFlow = GetOHProductFlow(SessionProtectedSystem);
            if (sc.ScenarioName.Contains("Electric Power Failure"))
            {
                ohProductFlowStops = 0;
                
            }
             
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
            if (CurrentAccumulator != null)
            {
                if (CurrentAccumulator.Diameter == 0)
                {
                    MessageBox.Show("Accumulator's diameter cann't be zero.", "Message Box");
                    return;
                }
                else
                {
                    diameter = CurrentAccumulator.Diameter;
                }

                if (CurrentAccumulator.NormalLiquidLevel == 0)
                {
                    MessageBox.Show("Accumulator's normal liquid level cann't be zero.", "Message Box");
                    return;
                }
                else
                {
                    liquidlevel = CurrentAccumulator.NormalLiquidLevel;
                }

                if (CurrentAccumulator.Length == 0)
                {
                    MessageBox.Show("Accumulator's length cann't be zero.", "Message Box");
                    return;
                }
                else
                {
                    length = CurrentAccumulator.Length;
                }

                accumulatorTotalVolume = 3.14159 * Math.Pow(diameter, 2) * length / 4 + 3.14159 * Math.Pow(diameter, 3) / 12;
                if (CurrentAccumulator.Orientation)
                {
                    accumulatorPartialVolume = liquidlevel * accumulatorTotalVolume / diameter;                    
                }
                else
                {
                    accumulatorPartialVolume = 3.14159 * Math.Pow(diameter, 2) * liquidlevel / 4 + 3.14159 * Math.Pow(diameter, 3) / 24;
                }
            }
            double surgeVolume = accumulatorTotalVolume - accumulatorPartialVolume;
            if (totalVolumeticFlowRate > 0)
            {
                double dSurgeTime = surgeVolume * 60 / totalVolumeticFlowRate;
                SurgeTime = dSurgeTime.ToString();
            }
            else
                SurgeTime = "0";
        }

        private double GetLatent(NHibernate.ISession Session)
        {
            double r=0;
            LatentDAL db = new LatentDAL();
            Latent model=db.GetModel(Session);
            if (model != null)
            {
                r = model.LatentEnthalpy;
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
                r = model.BulkDensityAct;
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
                r = model.Duty;
            }
            return r;
        }

        /// <summary>
        /// 获取顶部液相和水
        /// </summary>
        /// <param name="Session"></param>
        /// <returns></returns>
        private double GetOHProductFlow(NHibernate.ISession Session)
        {
            double r = 0;
            StreamDAL db = new StreamDAL();
            IList<CustomStream> list = db.GetAllList(Session);
            if (list != null)
            {
                foreach (CustomStream s in list)
                {
                    if (s.TotalMolarRate!=0)
                    {
                        double weightflow = s.WeightFlow;
                        double bulkdensityact = s.BulkDensityAct;
                        if (s.ProdType == "2" || s.ProdType == "4" || s.ProdType == "6")
                        {
                            r = r + weightflow / bulkdensityact;
                        }
                    }

                }
            }
            //return r * 3600;
            return r;
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
                SelectedHX.PinchFactor = vm.Factor;                
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
            //if (HeaterType == 1)
            //{
                if (condenserCalc == null)
                {
                    condenserCalc = new CondenserCalc();
                    condenserCalc.Flooding = IsFlooding;
                    condenserCalc.IsSurgeTime = IsSurgeTime;
                    condenserCalc.SurgeTime = SurgeTime;
                    condenserCalc.ScenarioID = ScenarioID;
                    condenserCalcDAL.Add(condenserCalc, SessionProtectedSystem);
                }
                else
                {
                    condenserCalc.Flooding = IsFlooding;
                    condenserCalc.IsSurgeTime = IsSurgeTime;
                    condenserCalc.SurgeTime = SurgeTime;
                    condenserCalc.ScenarioID = ScenarioID;
                    condenserCalcDAL.Update(condenserCalc, SessionProtectedSystem);
                }
                foreach (TowerScenarioHXModel m in Details)
                {
                    towerScenarioHXDAL.Update(m.model, SessionProtectedSystem);
                }

                //SessionProtectedSystem.Flush();
            //}
            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }



    }
}
