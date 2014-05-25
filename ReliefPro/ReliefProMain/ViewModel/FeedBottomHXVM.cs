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
        private string _FeedMasseRate;
        public string FeedMasseRate
        {
            get { return _FeedMasseRate; }
            set
            {
                _FeedMasseRate = value;
                OnPropertyChanged("FeedMasseRate");
            }
        }
        private string _FeedEin;
        public string FeedEin
        {
            get { return _FeedEin; }
            set
            {
                _FeedEin = value;
                OnPropertyChanged("FeedEin");
            }
        }
        private string _FeedEout;
        public string FeedEout
        {
            get { return _FeedEout; }
            set
            {
                _FeedEout = value;
                OnPropertyChanged("FeedEout");
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
        private string _BottomMasseRate;
        public string BottomMasseRate
        {
            get { return _BottomMasseRate; }
            set
            {
                _BottomMasseRate = value;
                OnPropertyChanged("BottomMasseRate");
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
        public FeedBottomHXVM(int HeatSourceID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;

            db = new dbFeedBottomHX();
            model = db.GetModel(this.SessionProtectedSystem, HeatSourceID);
            if (model != null)
            {
            }
        }

        private void FBMethod(double feedTin, double feedTout, double feedMassRate, double feedEin, double feedEout, double bottomTin, double bottomTout, double bottomMassRate, double duty,double bottomReliefTin, double qaenGuess, double MaxErrorRate, ref double factor, ref bool isPinch, ref int iterateNumber)
        {
            int iterateSum = 0;
            double nextQaenGuess = qaenGuess;
            double curErrorRate = 1;
            double feedReliefTin = feedTin;
            double reliefLtmd = 0;
            double reliefDuty = 0;
            double assumedQA = 0;
            double calculatedQR = 0;
            double QRQA = 1;
            double uQAQR = 0;
            double Lmtd = getLMTD(feedTin, feedTout, bottomTin, bottomTout);

            while (curErrorRate > MaxErrorRate && iterateSum < iterateNumber)
            {
                reliefLtmd = GetLMTDRelief(feedTin,feedTout,bottomTin,bottomTout,bottomReliefTin, nextQaenGuess);

                reliefDuty =duty* reliefLtmd /Lmtd;
                assumedQA = duty * nextQaenGuess;
                calculatedQR = reliefDuty;
                QRQA = calculatedQR / assumedQA;
                uQAQR = assumedQA / calculatedQR - 1;
                curErrorRate = Math.Abs(uQAQR);
                iterateSum = iterateSum + 1;
                if (curErrorRate > MaxErrorRate)
                {
                    nextQaenGuess = QRQA * (1 + 0.5 * uQAQR);
                }

            }
            double QRQN = calculatedQR / duty;
            if (QRQN < 1)
            {
                isPinch = true;
                factor = QRQN;
                //ReliefDuty = reliefDuty.ToString();
            }
            else
            {
                //ReliefDuty = duty.ToString();
            }


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
            double MaxErrorRate = 0.05;

            double feedTin = double.Parse(FeedTin);
            double feedTout = double.Parse(FeedTout);
            double feedMassRate = double.Parse(FeedMasseRate);
            double feedEin = double.Parse(FeedEin);
            double feedEout = double.Parse(FeedEout);
            double bottomTin = double.Parse(BottomTin);
            double bottomTout = double.Parse(BottomTout);
            double bottomMassRate = double.Parse(BottomMasseRate);
            double bottomReliefTin = double.Parse(BottomReliefTin);
            double duty = double.Parse(Duty);
            double qaenGuess = 1.25;
            FBMethod(feedTin, feedTout,feedMassRate,feedEin,feedEout,bottomTin,bottomTout,bottomMassRate,duty,bottomReliefTin,  qaenGuess, MaxErrorRate, ref factor, ref isPinch, ref iterateNumber);
        }


    }
}
