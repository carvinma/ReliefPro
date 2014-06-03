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
    public class FeedBottomHXVM:ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string PrzFile;
        private string PrzFileName;
        private string _Factor;
        public string Factor
        {
            get { return _Factor; }
            set
            {
                _Factor = value;
                OnPropertyChanged("Factor");
            }
        }
        private string _FeedTin;
        public string FeedTin
        {
            get { return _FeedTin; }
            set
            {
                _FeedTin = value;
                OnPropertyChanged("FeedTin");
            }
        }
        private string _FeedTout;
        public string FeedTout
        {
            get { return _FeedTout; }
            set
            {
                _FeedTout = value;
                OnPropertyChanged("FeedTout");
            }
        }
        private string _FeedMassRate;
        public string FeedMassRate
        {
            get { return _FeedMassRate; }
            set
            {
                _FeedMassRate = value;
                OnPropertyChanged("FeedMassRate");
            }
        }
        private string _FeedSpEin;
        public string FeedSpEin
        {
            get { return _FeedSpEin; }
            set
            {
                _FeedSpEin = value;
                OnPropertyChanged("FeedSpEin");
            }
        }
        private string _FeedSpEout;
        public string FeedSpEout
        {
            get { return _FeedSpEout; }
            set
            {
                _FeedSpEout = value;
                OnPropertyChanged("FeedSpEout");
            }
        }

        private string _BottomTin;
        public string BottomTin
        {
            get { return _BottomTin; }
            set
            {
                _BottomTin = value;
                OnPropertyChanged("BottomTin");
            }
        }
        private string _BottomTout;
        public string BottomTout
        {
            get { return _BottomTout; }
            set
            {
                _BottomTout = value;
                OnPropertyChanged("BottomTout");
            }
        }
        private string _BottomMassRate;
        public string BottomMassRate
        {
            get { return _BottomMassRate; }
            set
            {
                _BottomMassRate = value;
                OnPropertyChanged("BottomMassRate");
            }
        }

        private string _BottomReliefTin;
        public string BottomReliefTin
        {
            get { return _BottomReliefTin; }
            set
            {
                _BottomReliefTin = value;
                OnPropertyChanged("BottomReliefTin");
            }
        }
        private string _Duty;
        public string Duty
        {
            get { return _Duty; }
            set
            {
                _Duty = value;
                OnPropertyChanged("Duty");
            }
        }

        private string _FeedReliefTout;
        public string FeedReliefTout
        {
            get { return _FeedReliefTout; }
            set
            {
                _FeedReliefTout = value;
                OnPropertyChanged("FeedReliefTout");
            }
        }

        private string _FeedReliefSpEout;
        public string FeedReliefSpEout
        {
            get { return _FeedReliefSpEout; }
            set
            {
                _FeedReliefSpEout = value;
                OnPropertyChanged("FeedReliefSpEout");
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
        int HeatSourceID;
        public FeedBottomHXVM(int HeatSourceID,string PrzFile, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            this.PrzFile=PrzFile;
            PrzFileName = System.IO.Path.GetFileName(PrzFile);
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            heatSourceDAL = new HeatSourceDAL();
            sourceDAL = new SourceDAL();
            streamDAL = new StreamDAL();
            feedBottomHXDAL = new FeedBottomHXDAL();
            proIIEqDataDAL = new ProIIEqDataDAL();
            proIIStreamDataDAL = new ProIIStreamDataDAL();
            this.HeatSourceID = HeatSourceID;
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
                ProIIEqData hx = this.proIIEqDataDAL.GetModel(SessionPlant, PrzFileName, hs.HeatSourceName, "Hx");
                string[] productdata = hx.ProductData.Split(',');
                string[] feeddata = hx.FeedData.Split(',');
                ProIIStreamData f1 = this.proIIStreamDataDAL.GetModel(SessionPlant, feeddata[0], PrzFileName);
                ProIIStreamData f2 = proIIStreamDataDAL.GetModel(SessionPlant, feeddata[1], PrzFileName);
                ProIIStreamData p1 = proIIStreamDataDAL.GetModel(SessionPlant, productdata[0], PrzFileName);
                ProIIStreamData p2 = proIIStreamDataDAL.GetModel(SessionPlant, productdata[1], PrzFileName);
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
                Duty = (double.Parse(hx.DutyCalc) * 3600).ToString();

                TowerFlashProductDAL dbtfp=new TowerFlashProductDAL();
                IList< TowerFlashProduct> tfpList = dbtfp.GetAllList(SessionProtectedSystem);
                foreach (TowerFlashProduct p in tfpList)
                {
                    BottomReliefTin = p.Temperature;
                }

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
                reliefLtmd = GetLMTDRelief(feedTin,feedTout,bottomTin,bottomTout,bottomReliefTin, nextQaenGuess);

                reliefDuty =duty* reliefLtmd /Lmtd;
                assumedQA = duty * nextQaenGuess;
                calculatedQR = reliefDuty;
                QAQR =  calculatedQR/assumedQA ;
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
            double lnValue =  (bottomTin - feedTout)/(bottomTout - feedTin) ;
            double ltmd = t / Math.Log(lnValue);
            return ltmd;
        }
        private double GetLMTDRelief(double feedTin, double feedTout, double bottomTin, double bottomTout,double bottomReliefTin, double nextQaenGuess)
        {
            double feedReliefTin = feedTin;
            double feedReliefTout = feedReliefTin + nextQaenGuess * (feedTout - feedTin);
            double bottomReliefTout = bottomReliefTin - nextQaenGuess * (bottomTin - bottomTout);

            double relieft = (bottomReliefTin - feedReliefTout) - (bottomReliefTout - feedReliefTin);
            double lnValue =  (bottomReliefTin - feedReliefTout)/ (bottomReliefTout - feedReliefTin);
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

            double feedTin = double.Parse(FeedTin);
            double feedTout = double.Parse(FeedTout);
            double feedMassRate = double.Parse(FeedMassRate);
            double feedEin = double.Parse(FeedSpEin);
            double feedEout = double.Parse(FeedSpEout);
            double bottomTin = double.Parse(BottomTin);
            double bottomTout = double.Parse(BottomTout);
            double bottomMassRate = double.Parse(BottomMassRate);
            double bottomReliefTin = double.Parse(BottomReliefTin);
            double duty = double.Parse(Duty);
            double qaenGuess = 1.25;
            FBMethod(feedTin, feedTout, feedMassRate, feedEin, feedEout, bottomTin, bottomTout, bottomMassRate, duty, bottomReliefTin, qaenGuess, MaxErrorRate, ref factor, ref isConverged, ref iterateNumber);

            if (!isConverged)
            {
                FeedReliefTout = FeedTout;
                FeedReliefSpEout = FeedSpEout;
                Factor = "1";
            }
            else
            {
                double feedReliefTout=feedTin + factor * (feedTout - feedTin);
                double feedReliefSpEout = (feedEin + factor*duty)/feedMassRate;
                FeedReliefTout = feedReliefTout.ToString();
                FeedReliefSpEout = feedReliefSpEout.ToString();
                Factor = factor.ToString();
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
                feedBottomHXDAL.Add(model,SessionProtectedSystem);
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
                wd.DialogResult=true;
            }
        }

    }
}
