﻿using System;
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
    /// <summary>
    /// IsSteamFreezed 的判断
    /// condeser =0,则不被冷凝。 
    /// condeser factor =1 && stop=false 被冷凝
    /// 2者之间的。  比较大小，那个大，取那个。 返回结果后，判断是否冷凝。

    /// 如果是steam，则扣除它，然后，再按之前算法扣除。

    /// </summary>
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
        private string PrzFile;
        private int SteamFreezed=0;
        private TowerScenarioHXDAL towerScenarioHXDAL;
       
        private bool IsSteamFreezed = false;
        UnitConvert unitConvert;
        UOMLib.UOMEnum uomEnum;
        public TowerScenarioCalcVM(int scenarioID, string PrzFile, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            unitConvert = new UnitConvert();
            uomEnum = new UOMLib.UOMEnum(SessionPlant);
            InitUnit();
            this.PrzFile = PrzFile;
            ScenarioID = scenarioID;
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            towerScenarioHXDAL = new TowerScenarioHXDAL();
            ScenarioDAL dbsc = new ScenarioDAL();
            Scenario s = dbsc.GetModel(ScenarioID, SessionProtectedSystem);
            ReliefLoad = s.ReliefLoad;
            ReliefMW = s.ReliefMW;
            ReliefPressure = s.ReliefPressure;
            ReliefTemperature = s.ReliefTemperature;
            SteamFreezed = CheckSteamFreezed();
            ReadConvert();

        }
        private int CheckSteamFreezed()
        {
            int SteamFreezed = 0;

            IList<TowerScenarioHX> list = towerScenarioHXDAL.GetAllList(SessionProtectedSystem, ScenarioID, 1);
            if (list.Count == 0)
                SteamFreezed = 0;
            double sumDutyFactor = 0;
            foreach (TowerScenarioHX shx in list)
            {
                if (!shx.DutyLost)
                {
                    sumDutyFactor = sumDutyFactor + shx.DutyCalcFactor;
                }
            }
            if (sumDutyFactor == 0)
                SteamFreezed = 0;
            else if (sumDutyFactor == 1)
                SteamFreezed = 1;
            else
                SteamFreezed = 2;


            return SteamFreezed;
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
            TowerScenarioFeedVM vm = new TowerScenarioFeedVM(ScenarioID, PrzFile, SessionPlant, SessionProtectedSystem);
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
            double reliefLoad=0;
            double reliefMW=0;
            double reliefTemperature=0;
            double reliefPressure=0;
            if (SteamFreezed==0)
            {
                SteamNotFreezedMethod(ref  reliefLoad, ref  reliefMW, ref  reliefTemperature, ref  reliefPressure);
            }
            else if (SteamFreezed == 1)
            {
                SteamFreezedMethod(ref  reliefLoad, ref  reliefMW, ref  reliefTemperature, ref  reliefPressure);
            }
            else
            {
                double reliefLoad1 = 0;
                double reliefMW1 = 0;
                double reliefTemperature1 = 0;
                double reliefPressure1 = 0;
                SteamNotFreezedMethod(ref  reliefLoad, ref  reliefMW, ref  reliefTemperature, ref  reliefPressure);
                SteamFreezedMethod(ref  reliefLoad1, ref  reliefMW1, ref  reliefTemperature1, ref  reliefPressure1);
                if (reliefLoad < reliefLoad1)
                {
                    reliefLoad = reliefLoad1;
                    reliefMW = reliefMW1;
                    reliefTemperature = reliefTemperature1;
                    reliefPressure = reliefPressure1;
                }
               
            }
            ReliefTemperature = reliefTemperature.ToString();
            ReliefPressure = reliefPressure.ToString(); ;
            ReliefLoad = reliefLoad.ToString();
            ReliefMW = reliefMW.ToString();
           
        }

        private void Balance()
        {
            SourceDAL sourceDAL=new SourceDAL();
            TowerScenarioStreamDAL db = new TowerScenarioStreamDAL();
            StreamDAL dbstream = new StreamDAL();
            TowerFlashProductDAL dbtfp = new ReliefProDAL.TowerFlashProductDAL();
            IList<TowerScenarioStream> feeds = db.GetAllList(SessionProtectedSystem, ScenarioID, false);
            IList<TowerScenarioStream> products = db.GetAllList(SessionProtectedSystem, ScenarioID, true);
            double Total = 0;
            double steamTotal = 0;
            double currentTotal = 0;
            double diffTotal = 0;
            foreach (TowerScenarioStream s in feeds)
            {
                CustomStream cs = dbstream.GetModel(SessionProtectedSystem, s.StreamName);
                double wf = double.Parse(cs.WeightFlow);
                Total = Total + wf;
                if (!s.FlowStop)
                {
                    currentTotal =currentTotal+ wf * double.Parse(s.FlowCalcFactor);
                    Source source = sourceDAL.GetModel(s.StreamName, SessionProtectedSystem);
                    if(source.IsSteam)
                        steamTotal = steamTotal + wf * double.Parse(s.FlowCalcFactor);
                }
            }
            diffTotal = Total - currentTotal;

            int count = 0;
            IList<TowerFlashProduct> listP = dbtfp.GetAllList(SessionProtectedSystem);

            if (steamTotal > 0 )
            {
                IList<TowerFlashProduct> listP0 = (from p in listP
                                                   where (p.ProdType == "6")
                                                   orderby p.SpEnthalpy descending
                                                   select p).ToList();

                count = listP0.Count;
                for (int i = 0; i < count; i++)
                {
                    TowerFlashProduct p = listP0[i];
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
            }
            if (diffTotal > 0)
            {
                IList<TowerFlashProduct> listP1 = (from p in listP
                                                   where (p.ProdType != "3" || p.ProdType != "4" || p.ProdType != "6")
                                                   orderby p.SpEnthalpy descending
                                                   select p).ToList();

                count = listP1.Count;
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
            }
            if (diffTotal > 0)
            {
                IList<TowerFlashProduct> listP2 = (from p in listP
                                                   where (p.ProdType == "3" || p.ProdType == "4" )
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



        private void SteamNotFreezedMethod(ref double reliefLoad, ref double reliefMW, ref double reliefTemperature, ref double reliefPressure)
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

                        if (IsSteamFreezed && product.ProdType == "4")
                        {
                            ProductTotal = ProductTotal + (double.Parse(s.FlowCalcFactor) * double.Parse(cstream.SpEnthalpy) * double.Parse(product.WeightFlow));
                        }
                        else
                        {
                            ProductTotal = ProductTotal + (double.Parse(s.FlowCalcFactor) * double.Parse(product.SpEnthalpy) * double.Parse(product.WeightFlow));
                        }
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
            ReboilerPinchDAL reboilerPinchDAL = new ReboilerPinchDAL();
            IList<TowerScenarioHX> list = dbTSHX.GetAllList(SessionProtectedSystem, ScenarioID);
            foreach (TowerScenarioHX shx in list)
            {
                if (!shx.DutyLost)
                {
                    if (shx.IsPinch == true)
                    {
                        ReboilerPinch detail = reboilerPinchDAL.GetModel(SessionProtectedSystem, shx.ID);
                        HeatTotal = HeatTotal + shx.PinchFactor * double.Parse(detail.ReliefDuty);
                    }
                    else
                    {
                        TowerHXDetail detail = dbDetail.GetModel(SessionProtectedSystem, shx.DetailID);
                        HeatTotal = HeatTotal + shx.DutyCalcFactor * double.Parse(detail.Duty);
                    }
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
<<<<<<< .mine
            reliefLoad = wAccumulation + waterWeightFlow;
            reliefMW = (wAccumulation + waterWeightFlow) / (wAccumulation / double.Parse(latent.ReliefOHWeightFlow) + waterWeightFlow / 18);
            reliefTemperature = double.Parse(latent.ReliefTemperature);
            reliefPressure = double.Parse(latent.ReliefPressure);

=======
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
>>>>>>> .r470
        }
        private void SteamFreezedMethod(ref double reliefLoad, ref double reliefMW, ref double reliefTemperature,ref double reliefPressure)
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
                        if (cstream.ProdType == "6")
                        {
                            ProductTotal = ProductTotal + (double.Parse(s.FlowCalcFactor) * double.Parse(cstream.SpEnthalpy) * double.Parse(product.WeightFlow));
                        }
                        else
                        {
                            ProductTotal = ProductTotal + (double.Parse(s.FlowCalcFactor) * double.Parse(product.SpEnthalpy) * double.Parse(product.WeightFlow));
                        
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
            ReboilerPinchDAL reboilerPinchDAL = new ReboilerPinchDAL();
            IList<TowerScenarioHX> list = dbTSHX.GetAllList(SessionProtectedSystem, ScenarioID);
            foreach (TowerScenarioHX shx in list)
            {
                if (!shx.DutyLost)
                {
                    if (shx.IsPinch == true)
                    {
                        ReboilerPinch detail = reboilerPinchDAL.GetModel(SessionProtectedSystem, shx.ID);
                        HeatTotal = HeatTotal + shx.PinchFactor * double.Parse(detail.ReliefDuty);
                    }
                    else
                    {
                        TowerHXDetail detail = dbDetail.GetModel(SessionProtectedSystem, shx.DetailID);
                        HeatTotal = HeatTotal + shx.DutyCalcFactor * double.Parse(detail.Duty);
                    }
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
            reliefLoad = wAccumulation + waterWeightFlow;
            reliefMW = (wAccumulation + waterWeightFlow) / (wAccumulation / double.Parse(latent.ReliefOHWeightFlow) + waterWeightFlow / 18);
            reliefTemperature = double.Parse(latent.ReliefTemperature);
            reliefPressure = double.Parse(latent.ReliefPressure);
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
            WriteConvert();
            ScenarioDAL dbTS = new ScenarioDAL();
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
                wd.DialogResult = true;
            }
        }

        private void ReadConvert()
        {
            if (!string.IsNullOrEmpty(_ReliefLoad))
                _ReliefLoad = unitConvert.Convert(UOMEnum.MassRate, _ReliefLoadUnit, double.Parse(_ReliefLoad)).ToString();
            if (!string.IsNullOrEmpty(_ReliefTemperature))
                _ReliefTemperature = unitConvert.Convert(UOMEnum.Temperature, _ReliefTemperatureUnit, double.Parse(_ReliefTemperature)).ToString();
            if (!string.IsNullOrEmpty(_ReliefPressure))
                _ReliefPressure = unitConvert.Convert(UOMEnum.Pressure, _ReliefPressureUnit, double.Parse(_ReliefPressure)).ToString();
        }
        private void WriteConvert()
        {
            if (!string.IsNullOrEmpty(_ReliefLoad))
                _ReliefLoad = unitConvert.Convert(_ReliefLoadUnit, UOMEnum.MassRate, double.Parse(_ReliefLoad)).ToString();
            if (!string.IsNullOrEmpty(_ReliefTemperature))
                _ReliefTemperature = unitConvert.Convert(_ReliefTemperatureUnit, UOMEnum.Temperature, double.Parse(_ReliefTemperature)).ToString();
            if (!string.IsNullOrEmpty(_ReliefPressure))
                _ReliefPressure = unitConvert.Convert(_ReliefPressureUnit, UOMEnum.Pressure, double.Parse(_ReliefPressure)).ToString();
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
