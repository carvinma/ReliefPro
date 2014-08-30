using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using UOMLib;
using NHibernate;

namespace ReliefProMain.ViewModel
{
    public class FeedBottomHXVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string FileName;
        private double _Factor;
        public double Factor
        {
            get { return _Factor; }
            set
            {
                _Factor = value;
                OnPropertyChanged("Factor");
            }
        }
        private double _FeedTin;
        public double FeedTin
        {
            get { return _FeedTin; }
            set
            {
                _FeedTin = value;
                OnPropertyChanged("FeedTin");
            }
        }
        private double _FeedTout;
        public double FeedTout
        {
            get { return _FeedTout; }
            set
            {
                _FeedTout = value;
                OnPropertyChanged("FeedTout");
            }
        }
        private double _FeedMassRate;
        public double FeedMassRate
        {
            get { return _FeedMassRate; }
            set
            {
                _FeedMassRate = value;
                OnPropertyChanged("FeedMassRate");
            }
        }
        private double _FeedSpEin;
        public double FeedSpEin
        {
            get { return _FeedSpEin; }
            set
            {
                _FeedSpEin = value;
                OnPropertyChanged("FeedSpEin");
            }
        }
        private double _FeedSpEout;
        public double FeedSpEout
        {
            get { return _FeedSpEout; }
            set
            {
                _FeedSpEout = value;
                OnPropertyChanged("FeedSpEout");
            }
        }

        private double _BottomTin;
        public double BottomTin
        {
            get { return _BottomTin; }
            set
            {
                _BottomTin = value;
                OnPropertyChanged("BottomTin");
            }
        }
        private double _BottomTout;
        public double BottomTout
        {
            get { return _BottomTout; }
            set
            {
                _BottomTout = value;
                OnPropertyChanged("BottomTout");
            }
        }
        private double _BottomMassRate;
        public double BottomMassRate
        {
            get { return _BottomMassRate; }
            set
            {
                _BottomMassRate = value;
                OnPropertyChanged("BottomMassRate");
            }
        }

        private double _BottomReliefTin;
        public double BottomReliefTin
        {
            get { return _BottomReliefTin; }
            set
            {
                _BottomReliefTin = value;
                OnPropertyChanged("BottomReliefTin");
            }
        }
        private double _Duty;
        public double Duty
        {
            get { return _Duty; }
            set
            {
                _Duty = value;
                OnPropertyChanged("Duty");
            }
        }

        private double _FeedReliefTout;
        public double FeedReliefTout
        {
            get { return _FeedReliefTout; }
            set
            {
                _FeedReliefTout = value;
                OnPropertyChanged("FeedReliefTout");
            }
        }

        private double _FeedReliefSpEout;
        public double FeedReliefSpEout
        {
            get { return _FeedReliefSpEout; }
            set
            {
                _FeedReliefSpEout = value;
                OnPropertyChanged("FeedReliefSpEout");
            }
        }











        private string _Factor_Color;
        public string Factor_Color
        {
            get { return _Factor_Color; }
            set
            {
                _Factor_Color = value;
                OnPropertyChanged("Factor_Color");
            }
        }
        private string _FeedTin_Color;
        public string FeedTin_Color
        {
            get { return _FeedTin_Color; }
            set
            {
                _FeedTin_Color = value;
                OnPropertyChanged("FeedTin_Color");
            }
        }
        private string _FeedTout_Color;
        public string FeedTout_Color
        {
            get { return _FeedTout_Color; }
            set
            {
                _FeedTout_Color = value;
                OnPropertyChanged("FeedTout_Color");
            }
        }
        private string _FeedMassRate_Color;
        public string FeedMassRate_Color
        {
            get { return _FeedMassRate_Color; }
            set
            {
                _FeedMassRate_Color = value;
                OnPropertyChanged("FeedMassRate_Color");
            }
        }
        private string _FeedSpEin_Color;
        public string FeedSpEin_Color
        {
            get { return _FeedSpEin_Color; }
            set
            {
                _FeedSpEin_Color = value;
                OnPropertyChanged("FeedSpEin_Color");
            }
        }
        private string _FeedSpEout_Color;
        public string FeedSpEout_Color
        {
            get { return _FeedSpEout_Color; }
            set
            {
                _FeedSpEout_Color = value;
                OnPropertyChanged("FeedSpEout_Color");
            }
        }

        private string _BottomTin_Color;
        public string BottomTin_Color
        {
            get { return _BottomTin_Color; }
            set
            {
                _BottomTin_Color = value;
                OnPropertyChanged("BottomTin_Color");
            }
        }
        private string _BottomTout_Color;
        public string BottomTout_Color
        {
            get { return _BottomTout_Color; }
            set
            {
                _BottomTout_Color = value;
                OnPropertyChanged("BottomTout_Color");
            }
        }
        private string _BottomMassRate_Color;
        public string BottomMassRate_Color
        {
            get { return _BottomMassRate_Color; }
            set
            {
                _BottomMassRate_Color = value;
                OnPropertyChanged("BottomMassRate_Color");
            }
        }

        private string _BottomReliefTin_Color;
        public string BottomReliefTin_Color
        {
            get { return _BottomReliefTin_Color; }
            set
            {
                _BottomReliefTin_Color = value;
                OnPropertyChanged("BottomReliefTin_Color");
            }
        }
        private string _Duty_Color;
        public string Duty_Color
        {
            get { return _Duty_Color; }
            set
            {
                _Duty_Color = value;
                OnPropertyChanged("Duty_Color");
            }
        }

        private string _FeedReliefTout_Color;
        public string FeedReliefTout_Color
        {
            get { return _FeedReliefTout_Color; }
            set
            {
                _FeedReliefTout_Color = value;
                OnPropertyChanged("FeedReliefTout_Color");
            }
        }

        private string _FeedReliefSpEout_Color;
        public string FeedReliefSpEout_Color
        {
            get { return _FeedReliefSpEout_Color; }
            set
            {
                _FeedReliefSpEout_Color = value;
                OnPropertyChanged("FeedReliefSpEout_Color");
            }
        }


        FeedBottomHX model;
        FeedBottomHXDAL feedBottomHXDAL;
        HeatSourceDAL heatSourceDAL;
        SourceDAL sourceDAL;
        StreamDAL streamDAL;
        ProIIEqDataDAL proIIEqDataDAL;
        ProIIStreamDataDAL proIIStreamDataDAL;
        CustomStream csFeedIn;
        CustomStream csFeedOut;
        CustomStream csBottomIn;
        CustomStream csBottomOut;
        UOMLib.UOMEnum uomEnum;
        int HeatSourceID;
        public FeedBottomHXVM(int HeatSourceID, SourceFile sourceFileInfo, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionDBPath == sessionPlant.Connection.ConnectionString);
            InitUnit();
            FileName = sourceFileInfo.FileName;
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            heatSourceDAL = new HeatSourceDAL();
            sourceDAL = new SourceDAL();
            streamDAL = new StreamDAL();
            feedBottomHXDAL = new FeedBottomHXDAL();
            proIIEqDataDAL = new ProIIEqDataDAL();
            proIIStreamDataDAL = new ProIIStreamDataDAL();
            this.HeatSourceID = HeatSourceID;

            #region
            model = this.feedBottomHXDAL.GetModel(this.SessionProtectedSystem, HeatSourceID);
            if (model != null)
            {
                FeedTin = model.FeedTin;
                FeedTout = model.FeedTout;
                BottomTin = model.BottomTin;
                BottomTout = model.BottomTout;
                FeedSpEin = model.FeedSpEin;
                FeedSpEout = model.FeedSpEout;
                BottomMassRate = model.BottomMassRate;
                FeedMassRate = model.FeedMassRate;
                Duty = model.Duty;
                BottomReliefTin = model.BottomReliefTin;
                FeedReliefTout = model.FeedReliefTout;
                FeedReliefSpEout = model.FeedReliefSpEout;
                Factor = model.Factor;
            }
            else
            {
                HeatSource hs = this.heatSourceDAL.GetModel(HeatSourceID, SessionProtectedSystem);
                ProIIEqData hx = this.proIIEqDataDAL.GetModel(SessionPlant, FileName, hs.HeatSourceName, "Hx");
                string[] productdata = hx.ProductData.Split(',');
                string[] feeddata = hx.FeedData.Split(',');
                ProIIStreamData f1 = this.proIIStreamDataDAL.GetModel(SessionPlant, feeddata[0], FileName);
                ProIIStreamData f2 = proIIStreamDataDAL.GetModel(SessionPlant, feeddata[1], FileName);
                ProIIStreamData p1 = proIIStreamDataDAL.GetModel(SessionPlant, productdata[0], FileName);
                ProIIStreamData p2 = proIIStreamDataDAL.GetModel(SessionPlant, productdata[1], FileName);
                if (double.Parse(f1.Temperature) > double.Parse(f2.Temperature))
                {
                    csBottomIn = ProIIToDefault.ConvertProIIStreamToCustomStream(f1);
                    csBottomOut = ProIIToDefault.ConvertProIIStreamToCustomStream(p1);
                    csFeedIn = ProIIToDefault.ConvertProIIStreamToCustomStream(f2);
                    csFeedOut = ProIIToDefault.ConvertProIIStreamToCustomStream(p2);
                }
                else
                {
                    csBottomIn = ProIIToDefault.ConvertProIIStreamToCustomStream(f2);
                    csBottomOut = ProIIToDefault.ConvertProIIStreamToCustomStream(p2);
                    csFeedIn = ProIIToDefault.ConvertProIIStreamToCustomStream(f1);
                    csFeedOut = ProIIToDefault.ConvertProIIStreamToCustomStream(p1);
                }
                FeedTin = csFeedIn.Temperature;
                FeedTout = csFeedOut.Temperature;
                BottomTin = csBottomIn.Temperature;
                BottomTout = csBottomOut.Temperature;
                FeedSpEin = csFeedIn.SpEnthalpy;
                FeedSpEout = csFeedOut.SpEnthalpy;
                BottomMassRate = csBottomIn.WeightFlow;
                FeedMassRate = csFeedIn.WeightFlow;
                Duty = (double.Parse(hx.DutyCalc) * 3600);



                TowerFlashProductDAL dbtfp = new TowerFlashProductDAL();
                IList<TowerFlashProduct> tfpList = dbtfp.GetAllList(SessionProtectedSystem);

                TowerFlashProduct tfp = dbtfp.GetModel(sessionProtectedSystem, csBottomIn.StreamName);
                if (tfp == null)
                {

                }
                BottomReliefTin = tfp.Temperature;

            #endregion

                ReadConvert();
            }
        }

        private void FBMethod(double feedTin, double feedTout, double feedMassRate, double feedSpEin, double feedSpEout, double bottomTin, double bottomTout, double bottomMassRate, double duty, double bottomReliefTin, double qaenGuess, double MaxErrorRate, ref double factor, ref bool isConverged, ref int iterateNumber)
        {
            int iterateSum = 0;
            double nextQaenGuess = qaenGuess;
            double curErrorRate = 1;
            double feedReliefTin = feedTin;
            double reliefLtmd = 0;
            double reliefDuty = 0;
            double assumedQA = 0;
            double calculatedQR = 0;
            double QAQR = 1;
            double uQAQR = 0;
            double Lmtd = getLMTD(feedTin, feedTout, bottomTin, bottomTout);

            while (curErrorRate > MaxErrorRate && iterateSum < iterateNumber)
            {
                reliefLtmd = GetLMTDRelief(feedTin, feedTout, bottomTin, bottomTout, bottomReliefTin, nextQaenGuess);

                reliefDuty = duty * reliefLtmd / Lmtd;
                assumedQA = duty * nextQaenGuess;
                calculatedQR = reliefDuty;
                QAQR = calculatedQR / assumedQA;
                uQAQR = QAQR - 1;
                curErrorRate = Math.Abs(uQAQR);
                iterateSum = iterateSum + 1;
                if (curErrorRate > MaxErrorRate)
                {
                    nextQaenGuess = nextQaenGuess * (1 + 0.5 * uQAQR);
                }
            }
            if (curErrorRate < MaxErrorRate)
            {
                isConverged = true;
                double QRQN = calculatedQR / duty;
                factor = QRQN;
                if (QRQN > 1.5)
                    factor = 1.5;
            }
            else
                factor = 1;

        }

        private double getLMTD(double feedTin, double feedTout, double bottomTin, double bottomTout)
        {
            double t = (bottomTin - feedTout) - (bottomTout - feedTin);
            double lnValue = (bottomTin - feedTout) / (bottomTout - feedTin);
            double ltmd = t / Math.Log(lnValue);
            return ltmd;
        }
        private double GetLMTDRelief(double feedTin, double feedTout, double bottomTin, double bottomTout, double bottomReliefTin, double nextQaenGuess)
        {
            double feedReliefTin = feedTin;
            double feedReliefTout = feedReliefTin + nextQaenGuess * (feedTout - feedTin);
            double bottomReliefTout = bottomReliefTin - nextQaenGuess * (bottomTin - bottomTout);

            double relieft = (bottomReliefTin - feedReliefTout) - (bottomReliefTout - feedReliefTin);
            double lnValue = (bottomReliefTin - feedReliefTout) / (bottomReliefTout - feedReliefTin);
            double reliefLtmd = relieft / Math.Log(lnValue);
            return reliefLtmd;
        }

        private ICommand _CalculateCommand;
        public ICommand CalculateCommand
        {
            get
            {
                if (_CalculateCommand == null)
                {
                    _CalculateCommand = new RelayCommand(Calc);

                }
                return _CalculateCommand;
            }
        }

        private void Calc(object obj)
        {
            double factor = 1;
            bool isConverged = false;
            int iterateNumber = 50;
            double MaxErrorRate = 0.005;

            double feedTin = FeedTin;
            double feedTout = FeedTout;
            double feedMassRate = FeedMassRate;
            double feedEin = FeedSpEin;
            double feedEout = FeedSpEout;
            double bottomTin = BottomTin;
            double bottomTout = BottomTout;
            double bottomMassRate = BottomMassRate;
            double bottomReliefTin = BottomReliefTin;
            double duty = Duty;
            double qaenGuess = 1.25;
            FBMethod(feedTin, feedTout, feedMassRate, feedEin, feedEout, bottomTin, bottomTout, bottomMassRate, duty, bottomReliefTin, qaenGuess, MaxErrorRate, ref factor, ref isConverged, ref iterateNumber);

            if (!isConverged)
            {
                FeedReliefTout = FeedTout;
                FeedReliefSpEout = FeedSpEout;
                Factor = 1;
            }
            else
            {
                double feedReliefTout = feedTin + factor * (feedTout - feedTin);
                double feedReliefSpEout = feedEin + factor * duty / feedMassRate;
                FeedReliefTout = feedReliefTout;
                FeedReliefSpEout = feedReliefSpEout;
                Factor = factor;
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

        private void Save(object window)
        {
            WriteConvert();
            if (model == null)
            {
                model = new FeedBottomHX();
                model.BottomMassRate = BottomMassRate;
                model.BottomReliefTin = BottomReliefTin;
                model.BottomTin = BottomTin;
                model.BottomTout = BottomTout;
                model.Duty = Duty;
                model.FeedMassRate = FeedMassRate;
                model.FeedReliefSpEout = FeedReliefSpEout;
                model.FeedReliefTout = FeedReliefTout;
                model.FeedSpEin = FeedSpEin;
                model.FeedSpEout = FeedSpEout;
                model.FeedTin = FeedTin;
                model.FeedTout = FeedTout;
                model.HeatSourceID = HeatSourceID;
                model.FeedReliefTout = FeedReliefTout;
                model.FeedReliefSpEout = FeedReliefSpEout;
                model.Factor = Factor;
                feedBottomHXDAL.Add(model, SessionProtectedSystem);
            }
            else
            {
                model.BottomMassRate = BottomMassRate;
                model.BottomReliefTin = BottomReliefTin;
                model.BottomTin = BottomTin;
                model.BottomTout = BottomTout;
                model.Duty = Duty;
                model.FeedMassRate = FeedMassRate;
                model.FeedReliefSpEout = FeedReliefSpEout;
                model.FeedReliefTout = FeedReliefTout;
                model.FeedSpEin = FeedSpEin;
                model.FeedSpEout = FeedSpEout;
                model.FeedTin = FeedTin;
                model.FeedTout = FeedTout;
                model.HeatSourceID = HeatSourceID;
                model.FeedReliefTout = FeedReliefTout;
                model.FeedReliefSpEout = FeedReliefSpEout;
                model.Factor = Factor;
                feedBottomHXDAL.Update(model, SessionProtectedSystem);
                SessionProtectedSystem.Flush();
            }

            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private void WriteConvert()
        {
            FeedTin = UnitConvert.Convert(FeedTinUnit, UOMEnum.Temperature, FeedTin);
            FeedTout = UnitConvert.Convert(FeedToutUnit, UOMEnum.Temperature, FeedTout);

            FeedMassRate = UnitConvert.Convert(FeedMassRateUnit, UOMEnum.MassRate, FeedMassRate);
            FeedSpEin = UnitConvert.Convert(FeedSpEinUnit, UOMEnum.SpecificEnthalpy, FeedSpEin);
            FeedSpEout = UnitConvert.Convert(FeedSpEoutUnit, UOMEnum.SpecificEnthalpy, FeedSpEout);

            BottomTin = UnitConvert.Convert(BottomTinUnit, UOMEnum.Temperature, BottomTin);
            BottomTout = UnitConvert.Convert(BottomToutUnit, UOMEnum.Temperature, BottomTout);

            BottomReliefTin = UnitConvert.Convert(BottomReliefTinUnit, UOMEnum.Temperature, BottomReliefTin);
            BottomMassRate = UnitConvert.Convert(BottomMassRateUnit, UOMEnum.MassRate, BottomMassRate);
            Duty = UnitConvert.Convert(DutyUnit, UOMEnum.EnthalpyDuty, Duty);
            FeedReliefTout = UnitConvert.Convert(FeedReliefToutUnit, UOMEnum.Temperature, FeedReliefTout);
            FeedReliefSpEout = UnitConvert.Convert(FeedReliefSpEoutUnit, UOMEnum.SpecificEnthalpy, FeedReliefSpEout);
        }
        private void ReadConvert()
        {
            FeedTin = UnitConvert.Convert(UOMEnum.Temperature, uomEnum.UserTemperature, FeedTin);
            FeedTout = UnitConvert.Convert(UOMEnum.Temperature, uomEnum.UserTemperature, FeedTout);

            FeedMassRate = UnitConvert.Convert(UOMEnum.MassRate, uomEnum.UserMassRate, FeedMassRate);
            FeedSpEin = UnitConvert.Convert(UOMEnum.SpecificEnthalpy, uomEnum.UserSpecificEnthalpy, FeedSpEin);
            FeedSpEout = UnitConvert.Convert(UOMEnum.SpecificEnthalpy, uomEnum.UserSpecificEnthalpy, FeedSpEout);

            BottomTin = UnitConvert.Convert(UOMEnum.Temperature, uomEnum.UserTemperature, BottomTin);
            BottomTout = UnitConvert.Convert(UOMEnum.Temperature, uomEnum.UserTemperature, BottomTout);

            BottomReliefTin = UnitConvert.Convert(UOMEnum.Temperature, uomEnum.UserTemperature, BottomReliefTin);
            BottomMassRate = UnitConvert.Convert(UOMEnum.MassRate, uomEnum.UserMassRate, BottomMassRate);
            Duty = UnitConvert.Convert(UOMEnum.EnthalpyDuty, uomEnum.UserEnthalpyDuty, Duty);
            FeedReliefTout = UnitConvert.Convert(UOMEnum.Temperature, uomEnum.UserTemperature, FeedReliefTout);
            FeedReliefSpEout = UnitConvert.Convert(UOMEnum.SpecificEnthalpy, uomEnum.UserSpecificEnthalpy, FeedReliefSpEout);
        }
        private void InitUnit()
        {
            this.feedTinUnit = uomEnum.UserTemperature;
            this.feedToutUnit = uomEnum.UserTemperature;
            this.feedMassRateUnit = uomEnum.UserMassRate;
            this.feedSpEinUnit = uomEnum.UserSpecificEnthalpy;
            this.feedSpEoutUnit = uomEnum.UserSpecificEnthalpy;
            this.bottomTinUnit = uomEnum.UserTemperature;
            this.bottomToutUnit = uomEnum.UserTemperature;
            this.bottomReliefTinUnit = uomEnum.UserTemperature;
            this.bottomMassRateUnit = uomEnum.UserMassRate;
            this.dutyUnit = uomEnum.UserEnthalpyDuty;
            this.feedReliefToutUnit = uomEnum.UserTemperature;
            this.feedReliefSpEoutUnit = uomEnum.UserSpecificEnthalpy;
        }
        #region 单位-字段

        private string feedTinUnit;
        public string FeedTinUnit
        {
            get { return feedTinUnit; }
            set
            {
                feedTinUnit = value;
                OnPropertyChanged("FeedTinUnit");
            }
        }
        private string feedToutUnit;
        public string FeedToutUnit
        {
            get { return feedToutUnit; }
            set
            {
                feedToutUnit = value;
                OnPropertyChanged("FeedToutUnit");
            }
        }
        private string feedMassRateUnit;
        public string FeedMassRateUnit
        {
            get { return feedMassRateUnit; }
            set
            {
                feedMassRateUnit = value;
                OnPropertyChanged("FeedMassRateUnit");
            }
        }
        private string feedSpEinUnit;
        public string FeedSpEinUnit
        {
            get { return feedSpEinUnit; }
            set
            {
                feedSpEinUnit = value;
                OnPropertyChanged("FeedSpEinUnit");
            }
        }
        private string feedSpEoutUnit;
        public string FeedSpEoutUnit
        {
            get { return feedSpEoutUnit; }
            set
            {
                feedSpEoutUnit = value;
                OnPropertyChanged("FeedSpEoutUnit");
            }
        }
        private string bottomTinUnit;
        public string BottomTinUnit
        {
            get { return bottomTinUnit; }
            set
            {
                bottomTinUnit = value;
                OnPropertyChanged("BottomTinUnit");
            }
        }
        private string bottomToutUnit;
        public string BottomToutUnit
        {
            get { return bottomToutUnit; }
            set
            {
                bottomToutUnit = value;
                OnPropertyChanged("BottomToutUnit");
            }
        }
        private string bottomReliefTinUnit;
        public string BottomReliefTinUnit
        {
            get { return bottomReliefTinUnit; }
            set
            {
                bottomReliefTinUnit = value;
                OnPropertyChanged("BottomReliefTinUnit");
            }
        }
        private string bottomMassRateUnit;
        public string BottomMassRateUnit
        {
            get { return bottomMassRateUnit; }
            set
            {
                bottomMassRateUnit = value;
                OnPropertyChanged("BottomMassRateUnit");
            }
        }
        private string dutyUnit;
        public string DutyUnit
        {
            get { return dutyUnit; }
            set
            {
                dutyUnit = value;
                OnPropertyChanged("DutyUnit");
            }
        }
        private string feedReliefToutUnit;
        public string FeedReliefToutUnit
        {
            get { return feedReliefToutUnit; }
            set
            {
                feedReliefToutUnit = value;
                OnPropertyChanged("FeedReliefToutUnit");
            }
        }
        private string feedReliefSpEoutUnit;
        public string FeedReliefSpEoutUnit
        {
            get { return feedReliefSpEoutUnit; }
            set
            {
                feedReliefSpEoutUnit = value;
                OnPropertyChanged("FeedReliefSpEoutUnit");
            }
        }
        #endregion
    }
}
