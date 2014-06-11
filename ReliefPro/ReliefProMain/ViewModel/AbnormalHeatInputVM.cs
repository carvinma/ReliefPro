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
using ReliefProMain.ViewModel.Drums;
using System.Windows;
using System.IO;
using UOMLib;

namespace ReliefProMain.ViewModel
{
    public class AbnormalHeatInputVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private int ScenarioID;

        private AbnormalHeaterDetailDAL abnormalHeaterDetailDAL;
        private ScenarioDAL scenarioDAL;
        private PSVDAL psvDAL;
        UOMLib.UOMEnum uomEnum;
        private ObservableCollection<AbnormalHeaterDetailModel> _Heaters;
        public ObservableCollection<AbnormalHeaterDetailModel> Heaters
        {
            get { return _Heaters; }
            set
            {
                _Heaters = value;
                OnPropertyChanged("Heaters");
            }
        }
        private HeatSourceModel _SelectedHeater;
        public HeatSourceModel SelectedHeater
        {
            get { return _SelectedHeater; }
            set
            {
                _SelectedHeater = value;
                OnPropertyChanged("SelectedHeater");
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
            uomEnum = new UOMLib.UOMEnum(SessionPlant);
            InitUnit();
            this.ScenarioID = ScenarioID;
            this.SessionPlant = SessionPlant;
            this.SessionProtectedSystem = SessionProtectedSystem;
            Heaters = new ObservableCollection<AbnormalHeaterDetailModel>();
            scenarioDAL = new ScenarioDAL();
            psvDAL = new PSVDAL();
            IList<AbnormalHeaterDetail> list = abnormalHeaterDetailDAL.GetAllList(this.SessionProtectedSystem, ScenarioID);
            int i = 0;
            foreach (AbnormalHeaterDetail s in list)
            {
                AbnormalHeaterDetailModel m = new AbnormalHeaterDetailModel(s);
                m.Duty = s.Duty;
                m.DutyFactor = s.DutyFactor;
                m.AbnormalType = s.AbnormalType;
                m.HeaterID = s.HeaterID;
                m.HeaterName = s.HeaterName;
                m.ID = s.ID;
                m.ScenarioID = s.ScenarioID;
                m.SeqNumber = i;
                i++;
            }
            if (list.Count == 0)
            {
                //SourceDAL sourceDAL = new SourceDAL();
                //IList<HeatSource> listHeatSource = heatSourceDAL.GetAllList(SessionProtectedSystem);
                //foreach (HeatSource hs in listHeatSource)
                //{
                //    ScenarioHeatSource shs = new ScenarioHeatSource();
                //    ScenarioHeatSourceModel shsm = new ScenarioHeatSourceModel(shs);
                //    shsm.HeatSourceID = hs.ID;
                //    shsm.DutyFactor = "1";
                //    shsm.Duty = hs.Duty;
                //    shsm.ScenarioStreamID = 0;
                //    shsm.ScenarioID = ScenarioID;
                //    shsm.HeatSourceName = hs.HeatSourceName;
                //    shsm.HeatSourceType = hs.HeatSourceType;
                //    HeatSources.Add(shsm);

                //}
            }
            Scenario sc = scenarioDAL.GetModel(ScenarioID, SessionProtectedSystem);
            ReliefLoad = sc.ReliefLoad;
            ReliefMW = sc.ReliefMW;
            ReliefPressure = sc.ReliefPressure;
            ReliefTemperature = sc.ReliefTemperature;
            ReadConvert();
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


            PSV psv = this.psvDAL.GetModel(SessionProtectedSystem);

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


            TowerScenarioHXDAL dbTSHX = new TowerScenarioHXDAL();
            TowerHXDetailDAL dbDetail = new TowerHXDetailDAL();
            IList<TowerScenarioHX> list = dbTSHX.GetAllList(SessionProtectedSystem, ScenarioID);
            foreach (TowerScenarioHX shx in list)
            {
                if (!shx.DutyLost)
                {
                    TowerHXDetail detail = dbDetail.GetModel(SessionProtectedSystem, shx.DetailID);
                    HeatTotal = HeatTotal + shx.DutyCalcFactor * double.Parse(detail.Duty);
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

            TowerScenarioStreamDAL towerScenarioStreamDAL = new TowerScenarioStreamDAL();
            StreamDAL dbstream = new StreamDAL();
            TowerFlashProductDAL dbtfp = new ReliefProDAL.TowerFlashProductDAL();
            IList<TowerScenarioStream> feeds = towerScenarioStreamDAL.GetAllList(SessionProtectedSystem, ScenarioID, false);
            IList<TowerScenarioStream> products = towerScenarioStreamDAL.GetAllList(SessionProtectedSystem, ScenarioID, true);
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
                    towerScenarioStreamDAL.Update(s, SessionProtectedSystem);
                    break;
                }
                else
                {
                    s.FlowCalcFactor = "0";
                    towerScenarioStreamDAL.Update(s, SessionProtectedSystem);
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
                        towerScenarioStreamDAL.Update(s, SessionProtectedSystem);
                        diffTotal = 0;
                        break;
                    }
                    else
                    {
                        s.FlowCalcFactor = "0";
                        towerScenarioStreamDAL.Update(s, SessionProtectedSystem);
                        diffTotal = diffTotal - tempH;
                    }
                }
            }
            SessionProtectedSystem.Flush();


        }
        private double GetAbnormalTotalDuty()
        {
            double total = 0;
            foreach (AbnormalHeaterDetailModel m in Heaters)
            {
                total = total + (m.DutyFactor - 1) * m.Duty;
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
            WriteConvert();
            foreach (AbnormalHeaterDetailModel m in Heaters)
            {
                abnormalHeaterDetailDAL.Update(m.model, SessionProtectedSystem);
            }

            Scenario scenario = this.scenarioDAL.GetModel(ScenarioID, SessionProtectedSystem);
            scenario.ReliefLoad = ReliefLoad;
            scenario.ReliefMW = ReliefMW;
            scenario.ReliefTemperature = ReliefTemperature;
            scenario.ReliefPressure = ReliefPressure;
            scenarioDAL.Update(scenario, SessionProtectedSystem);

            SessionProtectedSystem.Flush();
            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        
        private void ReadConvert()
        {
            if (!string.IsNullOrEmpty(_ReliefLoad))
                _ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, _ReliefLoadUnit, double.Parse(_ReliefLoad)).ToString();
            if (!string.IsNullOrEmpty(_ReliefTemperature))
                _ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, _ReliefTemperatureUnit, double.Parse(_ReliefTemperature)).ToString();
            if (!string.IsNullOrEmpty(_ReliefPressure))
                _ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, _ReliefPressureUnit, double.Parse(_ReliefPressure)).ToString();
        }
        private void WriteConvert()
        {
            if (!string.IsNullOrEmpty(_ReliefLoad))
                _ReliefLoad = UnitConvert.Convert(_ReliefLoadUnit, UOMEnum.MassRate, double.Parse(_ReliefLoad)).ToString();
            if (!string.IsNullOrEmpty(_ReliefTemperature))
                _ReliefTemperature = UnitConvert.Convert(_ReliefTemperatureUnit, UOMEnum.Temperature, double.Parse(_ReliefTemperature)).ToString();
            if (!string.IsNullOrEmpty(_ReliefPressure))
                _ReliefPressure = UnitConvert.Convert(_ReliefPressureUnit, UOMEnum.Pressure, double.Parse(_ReliefPressure)).ToString();
        }
        private void InitUnit()
        {
            this._ReliefLoadUnit = uomEnum.UserMassRate;
            this._ReliefTemperatureUnit = uomEnum.UserTemperature;
            this._ReliefPressureUnit = uomEnum.UserPressure;
        }
        #region 单位字段
        private string _ReliefLoadUnit;
        public string ReliefLoadUnit
        {
            get
            {
                return this._ReliefLoadUnit;
            }
            set
            {
                this._ReliefLoadUnit = value;

                OnPropertyChanged("ReliefLoadUnit");
            }
        }

        private string _ReliefTemperatureUnit;
        public string ReliefTemperatureUnit
        {
            get
            {
                return this._ReliefTemperatureUnit;
            }
            set
            {
                this._ReliefTemperatureUnit = value;

                OnPropertyChanged("ReliefTemperatureUnit");
            }
        }

        private string _ReliefPressureUnit;
        public string ReliefPressureUnit
        {
            get
            {
                return this._ReliefPressureUnit;
            }
            set
            {
                this._ReliefPressureUnit = value;

                OnPropertyChanged("ReliefPressureUnit");
            }
        }
        #endregion
    }
}
