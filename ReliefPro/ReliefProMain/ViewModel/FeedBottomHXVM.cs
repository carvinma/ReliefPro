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



        FeedBottomHX model;
        dbFeedBottomHX db;
        dbHeatSource dbhs;
        dbSource dbsource;
        dbStream dbstream;
        dbProIIEqData dbProIIeq;
        dbProIIStreamData dbProIIStream;
        public FeedBottomHXVM(int HeatSourceID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            dbhs = new dbHeatSource();
            dbsource = new dbSource();
            dbstream = new dbStream();
            db = new dbFeedBottomHX();
            dbProIIeq = new dbProIIEqData();
            dbProIIStream = new dbProIIStreamData();
            model = db.GetModel(this.SessionProtectedSystem, HeatSourceID);
            if (model != null)
            {

            }
            else
            {
                HeatSource hs = dbhs.GetModel(HeatSourceID, SessionProtectedSystem);
                
            }
        }

        private void FBMethod(double feedTin, double feedTout, double feedMassRate, double feedSpEin, double feedSpEout, double bottomTin, double bottomTout, double bottomMassRate, double duty,double bottomReliefTin, double qaenGuess, double MaxErrorRate, ref double factor, ref bool isPinch, ref int iterateNumber)
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
            double QRQN = calculatedQR / duty;
            factor = QRQN;
            if (QRQN > 1.5)
                factor = 1.5;
               
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
            bool isPinch = false;
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
            FBMethod(feedTin, feedTout,feedMassRate,feedEin,feedEout,bottomTin,bottomTout,bottomMassRate,duty,bottomReliefTin,  qaenGuess, MaxErrorRate, ref factor, ref isPinch, ref iterateNumber);
        }


    }
}
