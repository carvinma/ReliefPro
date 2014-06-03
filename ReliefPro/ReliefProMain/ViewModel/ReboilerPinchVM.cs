using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UOMLib;
using NHibernate;

namespace ReliefProMain.ViewModel
{
    public class ReboilerPinchVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }

        private bool _IsPinch;
        public bool IsPinch
        {
            get { return _IsPinch; }
            set
            {
                _IsPinch = value;
                OnPropertyChanged("IsPinch");
            }
        }
        private ObservableCollection<string> _SourceTypes;
        public ObservableCollection<string> SourceTypes
        {
            get { return _SourceTypes; }
            set
            {
                _SourceTypes = value;
                OnPropertyChanged("SourceTypes");
            }
        }

        private string _SourceType;
        public string SourceType
        {
            get { return _SourceType; }
            set
            {
                _SourceType = value;
                OnPropertyChanged("SourceType");
            }
        }

        private string _Coldtin;
        public string Coldtin
        {
            get { return _Coldtin; }
            set
            {
                _Coldtin = value;
                OnPropertyChanged("Coldtin");
            }
        }
        private string _Coldtout;
        public string Coldtout
        {
            get { return _Coldtout; }
            set
            {
                _Coldtout = value;
                OnPropertyChanged("Coldtout");
            }
        }
        private string _HeatTin;
        public string HeatTin
        {
            get { return _HeatTin; }
            set
            {
                _HeatTin = value;                
                OnPropertyChanged("HeatTin");
            }
        }
        private string _HeatTout;
        public string HeatTout
        {
            get { return _HeatTout; }
            set
            {
                _HeatTout = value;
                OnPropertyChanged("HeatTout");
            }
        }

        private string _ReliefHeatIn;
        public string ReliefHeatIn
        {
            get { return _ReliefHeatIn; }
            set
            {
                _ReliefHeatIn = value;
                OnPropertyChanged("ReliefHeatIn");
            }
        }

        private string _ReliefColdtout;
        public string ReliefColdtout
        {
            get { return _ReliefColdtout; }
            set
            {
                _ReliefColdtout = value;
                OnPropertyChanged("ReliefColdtout");
            }
        }

        private string _Area;
        public string Area
        {
            get { return _Area; }
            set
            {
                _Area = value;
                OnPropertyChanged("Area");
            }
        }
        private string _UDesign;
        public string UDesign
        {
            get { return _UDesign; }
            set
            {
                _UDesign = value;
                OnPropertyChanged("UDesign");
            }
        }

        private string _UDesignArea;
        public string UDesignArea
        {
            get { return _UDesignArea; }
            set
            {
                _UDesignArea = value;
                OnPropertyChanged("UDesignArea");
            }
        }
        private string _UClean;
        public string UClean
        {
            get { return _UClean; }
            set
            {
                _UClean = value;
                OnPropertyChanged("UClean");
            }
        }

        private string _UCD;
        public string UCD
        {
            get { return _UCD; }
            set
            {
                _UCD = value;
                OnPropertyChanged("UCD");
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

        private string _ReliefDuty;
        public string ReliefDuty
        {
            get { return _ReliefDuty; }
            set
            {
                _ReliefDuty = value;
                OnPropertyChanged("ReliefDuty");
            }
        }
        public ReboilerPinch reboilerPinchModel { get; set; }
        public ObservableCollection<string> GetSourceTypes()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Hot Oil/water");
            list.Add("Steam");
            return list;
        }
        TowerScenarioHXDAL towerScenarioHXDAL;
        TowerHXDetailDAL towerHXDetailDAL;
        ReboilerPinchDAL reboilerPinchDAL;
        int TowerScenarioHXID;
        public ReboilerPinchVM(int TowerScenarioHXID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            this.TowerScenarioHXID = TowerScenarioHXID;
            towerHXDetailDAL = new TowerHXDetailDAL();
            towerScenarioHXDAL = new TowerScenarioHXDAL();
            reboilerPinchDAL = new ReboilerPinchDAL();
            reboilerPinchModel = reboilerPinchDAL.GetModel(SessionProtectedSystem, TowerScenarioHXID);
            if (reboilerPinchModel == null)
            {
                TowerScenarioHX hx = towerScenarioHXDAL.GetModel(TowerScenarioHXID, SessionProtectedSystem);
                SourceType = hx.Medium;
                TowerHXDetail detail = towerHXDetailDAL.GetModel(hx.DetailID, SessionProtectedSystem);
                double duty = double.Parse(hx.DutyCalcFactor) * double.Parse(detail.Duty);
                Duty = duty.ToString();

                TowerDAL dbtower = new TowerDAL();
                CustomStreamDAL dbcs = new CustomStreamDAL();
                TowerFlashProductDAL dbtfp = new TowerFlashProductDAL();
                Tower tower = dbtower.GetModel(SessionProtectedSystem);
                if (tower != null)
                {
                    IList<CustomStream> list = dbcs.GetAllList(SessionProtectedSystem);
                    foreach (CustomStream cs in list)
                    {
                        if (cs.ProdType == "5" || (cs.ProdType == "2" && cs.Tray == tower.StageNumber))
                        {
                            Coldtout = cs.Temperature;
                            TowerFlashProduct tfp = dbtfp.GetModel(SessionProtectedSystem, cs.StreamName);
                            ReliefColdtout = tfp.Temperature;
                        }
                    }
                }
            }
            else
            {
                IsPinch = reboilerPinchModel.IsPinch;
                Coldtin = reboilerPinchModel.Coldtin;
                Coldtout = reboilerPinchModel.Coldtout;
                Duty = reboilerPinchModel.Duty;
                HeatTin = reboilerPinchModel.HeatTin;                
                HeatTout = reboilerPinchModel.HeatTout;
                ReliefDuty = reboilerPinchModel.ReliefDuty;
                SourceType = reboilerPinchModel.SourceType;
                UClean = reboilerPinchModel.UClean;
                UDesign = reboilerPinchModel.UDesign;
            }


        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="productTin"></param>
        /// <param name="productTout"></param>
        /// <param name="reliefProductTout"></param>
        /// <param name="reboilerTin"></param>
        /// <param name="reboilerTout"></param>
        /// <param name="coeff"></param>
        /// <param name="area"></param>
        /// <param name="duty"></param>
        /// <param name="isUseSteam"></param>
        /// <param name="qaenGuess"></param>
        /// <param name="MaxErrorRate"></param>
        /// <param name="factor"></param>
        /// <param name="isPinch"></param>
        /// <param name="iterateNumber"></param>
        private void HeatMediumMethod(double productTin, double productTout, double reliefProductTout, double reboilerTin, double reboilerTout, double coeff, double area, double duty, bool isUseSteam, double qaenGuess, double MaxErrorRate, ref double factor, ref bool isPinch, ref int iterateNumber)
        {
            int iterateSum = 0;
            double nextQaenGuess = qaenGuess;
            double curErrorRate = 1;
            double reliefReboilerTin = reboilerTin;
            double reliefLtmd = 0;
            double reliefCoeff = coeff;
            double reliefArea = area;
            double reliefDuty = 0;
            double assumedQA = 0;
            double calculatedQR = 0;
            double QRQA = 1;
            double uQAQR = 0;
            while (curErrorRate > MaxErrorRate && iterateSum < iterateNumber)
            {
                reliefLtmd = GetLMTDRelief(productTin, productTout, reliefProductTout, reboilerTin, reboilerTout, nextQaenGuess);
                reliefCoeff = coeff;
                reliefArea = area;
                reliefDuty = reliefLtmd * reliefCoeff * reliefArea*3.6;
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
                ReliefDuty = reliefDuty.ToString();
            }
            else
            {
                ReliefDuty = duty.ToString();
            }
            

        }


        private ICommand _PinchCalcCommand;
        public ICommand PinchCalcCommand
        {
            get
            {
                if (_PinchCalcCommand == null)
                {
                    _PinchCalcCommand = new RelayCommand(PinchCalc);

                }
                return _PinchCalcCommand;
            }
        }

        private void PinchCalc(object obj)
        {
            double factor = 1;
            int iterateNumber = 50;
            double MaxErrorRate = 0.005;

            double productTin=double.Parse(Coldtin); 
            double productTout=double.Parse(Coldtout);
            double reliefProductTout=double.Parse(ReliefColdtout);
            double reboilerTin=double.Parse(HeatTin);
            double reboilerTout=double.Parse(HeatTout);
            double coeff = double.Parse(UClean) ; 
            double area=double.Parse(Area); 
            double duty=double.Parse(Duty);
            bool isUseSteam=false;
            double qaenGuess=1.2;
            GetUDesign();
            HeatMediumMethod(productTin, productTout, reliefProductTout, reboilerTin, reboilerTout, coeff, area, duty,isUseSteam, qaenGuess, MaxErrorRate, ref factor, ref _IsPinch, ref iterateNumber);
        }

        private void GetUDesign()
        {
            if (!string.IsNullOrEmpty(Coldtin) && !string.IsNullOrEmpty(Coldtout) && !string.IsNullOrEmpty(HeatTin) && !string.IsNullOrEmpty(HeatTout))
            {
                double productTin = double.Parse(Coldtin);
                double productTout = double.Parse(Coldtout);
                double reboilerTin = double.Parse(HeatTin);
                double reboilerTout = double.Parse(HeatTout);               
                double lmtd = GetLMTD(productTin, productTout, reboilerTin, reboilerTout);
                if (!string.IsNullOrEmpty(Area) && !string.IsNullOrEmpty(Duty))
                {
                    double area = double.Parse(Area);
                    double duty = double.Parse(Duty);
                    double udesign = duty / lmtd /area/3.6;
                    UDesign= udesign.ToString();
                    UDesignArea = (udesign * area).ToString();
                    if(!string.IsNullOrEmpty(UClean))
                    {
                        UCD = (double.Parse(UClean) / udesign).ToString();
                    }

                }
            }
            
         
        }

       private double GetLMTD(double productTin, double productTout,  double reboilerTin, double reboilerTout)
        {
           double t = reboilerTout + productTout - reboilerTin - productTin;
           double  lnValue = (reboilerTout - productTin) / (reboilerTin - productTout);
           double ltmd = t / Math.Log(lnValue);
           return ltmd;
        }
       private double GetLMTDRelief(double productTin, double productTout, double reliefProductTout, double reboilerTin, double reboilerTout, double nextQaenGuess)
        {
           double reliefProductTin = reliefProductTout - nextQaenGuess * (productTout - productTin);
           double reliefReboilerTin = reboilerTin;
           double reliefReboilerTout = reliefReboilerTin - nextQaenGuess * (reboilerTin - reboilerTout);
           
           double relieft = reliefReboilerTout + reliefProductTout - reliefProductTin - reliefReboilerTin;
           double lnValue = (reliefReboilerTout - reliefProductTin) / (reliefReboilerTin - reliefProductTout);
           double  reliefLtmd = relieft / Math.Log(lnValue);
           return reliefLtmd;
        }

        private ICommand _SavePinchCommand;
        public ICommand SavePinchCommand
        {
            get
            {
                if (_SavePinchCommand == null)
                {
                    _SavePinchCommand = new RelayCommand(SavePinch);

                }
                return _SavePinchCommand;
            }
        }

        private void SavePinch(object window)
        {
            if (reboilerPinchModel == null)
            {
                reboilerPinchModel = new ReboilerPinch();
                reboilerPinchModel.IsPinch = true;
                reboilerPinchModel.Coldtin = Coldtin;
                reboilerPinchModel.Coldtout = Coldtout;
                reboilerPinchModel.Duty = Duty;
                reboilerPinchModel.HeatTin = HeatTin;
                reboilerPinchModel.HeatTout = HeatTout;
                reboilerPinchModel.ReliefDuty = ReliefDuty;
                reboilerPinchModel.SourceType = SourceType;
                reboilerPinchModel.UClean = UClean;
                reboilerPinchModel.UDesign = UDesign;
                reboilerPinchModel.TowerScenarioHXID = TowerScenarioHXID;                
                reboilerPinchDAL.Add(reboilerPinchModel, SessionProtectedSystem);
                
            }
            else
            {
                reboilerPinchModel.IsPinch = true;
                reboilerPinchModel.Coldtin = Coldtin;
                reboilerPinchModel.Coldtout = Coldtout;
                reboilerPinchModel.Duty = Duty;
                reboilerPinchModel.HeatTin = HeatTin;
                reboilerPinchModel.HeatTout = HeatTout;
                reboilerPinchModel.ReliefDuty = ReliefDuty;
                reboilerPinchModel.SourceType = SourceType;
                reboilerPinchModel.UClean = UClean;
                reboilerPinchModel.UDesign = UDesign;
                reboilerPinchDAL.Update(reboilerPinchModel, SessionProtectedSystem);
            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private ICommand _SaveNormalCommand;
        public ICommand SaveNormalCommand
        {
            get
            {
                if (_SaveNormalCommand == null)
                {
                    _SaveNormalCommand = new RelayCommand(SaveNormalPinch);

                }
                return _SaveNormalCommand;
            }
        }

        private void SaveNormalPinch(object window)
        {
            if (reboilerPinchModel == null)
            {
                reboilerPinchModel.IsPinch = false;
                reboilerPinchModel = new ReboilerPinch();
                reboilerPinchModel.Coldtin = Coldtin;
                reboilerPinchModel.Coldtout = Coldtout;
                reboilerPinchModel.Duty = Duty;
                reboilerPinchModel.HeatTin = HeatTin;
                reboilerPinchModel.HeatTout = HeatTout;
                reboilerPinchModel.ReliefDuty = ReliefDuty;
                reboilerPinchModel.SourceType = SourceType;
                reboilerPinchModel.UClean = UClean;
                reboilerPinchModel.UDesign = UDesign;
                reboilerPinchModel.TowerScenarioHXID = TowerScenarioHXID;
                reboilerPinchDAL.Add(reboilerPinchModel, SessionProtectedSystem);

            }
            else
            {
                reboilerPinchModel.IsPinch = false;
                reboilerPinchModel.Coldtin = Coldtin;
                reboilerPinchModel.Coldtout = Coldtout;
                reboilerPinchModel.Duty = Duty;
                reboilerPinchModel.HeatTin = HeatTin;
                reboilerPinchModel.HeatTout = HeatTout;
                reboilerPinchModel.ReliefDuty = ReliefDuty;
                reboilerPinchModel.SourceType = SourceType;
                reboilerPinchModel.UClean = UClean;
                reboilerPinchModel.UDesign = UDesign;
                reboilerPinchDAL.Update(reboilerPinchModel, SessionProtectedSystem);
            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }
    }
}
