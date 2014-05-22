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

namespace ReliefProMain.ViewModel
{
    public class ReboilerPinchVM : ViewModelBase
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }

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
        public ReboilerPinch CurrentReboilerPinch { get; set; }
        public ObservableCollection<string> GetSourceTypes()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Hot Oil/water");
            list.Add("Steam");
            return list;
        }
        string przFile;
        dbTowerScenarioHX dbtshx;
        dbTowerHXDetail dbDetail;
        public ReboilerPinchVM(int ID, string dbPSFile, string dbPFile)
        {
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            BasicUnit BU;
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbBasicUnit dbBU = new dbBasicUnit();
                IList<BasicUnit> list = dbBU.GetAllList(Session);
                BU = list.Where(s => s.IsDefault == 1).Single();
            }

            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                UnitConvert uc = new UnitConvert();
                var Session = helper.GetCurrentSession();
                dbDetail = new dbTowerHXDetail();
                dbtshx = new dbTowerScenarioHX();
                TowerScenarioHX hx = dbtshx.GetModel(ID, Session);
                SourceType = hx.Medium;
                TowerHXDetail detail = dbDetail.GetModel(hx.DetailID, Session);
                double duty = double.Parse(hx.DutyCalcFactor) * double.Parse(detail.Duty);
                Duty = duty.ToString();

                dbTower dbtower = new dbTower();
                dbCustomStream dbcs=new dbCustomStream();
                dbTowerFlashProduct dbtfp = new dbTowerFlashProduct();
                Tower tower = dbtower.GetModel(Session);
                if (tower != null) 
                {
                    przFile = System.IO.Path.GetDirectoryName(dbPlantFile) + @"\" + tower.PrzFile;
                    IList<CustomStream> list = dbcs.GetAllList(Session);
                    foreach (CustomStream cs in list)
                    {
                        if (cs.ProdType == "5"||(cs.ProdType=="2" && cs.Tray==tower.StageNumber))
                        {
                            Coldtout = cs.Temperature;
                            TowerFlashProduct tfp = dbtfp.GetModel(Session, cs.StreamName);
                            ReliefColdtout = tfp.Temperature;
                        }
                    }
                }
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
            bool isPinch = false;
            int iterateNumber = 50;
            double MaxErrorRate = 0.05;

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
            HeatMediumMethod(productTin, productTout, reliefProductTout, reboilerTin, reboilerTout, coeff, area, duty,isUseSteam, qaenGuess, MaxErrorRate, ref factor, ref isPinch, ref iterateNumber);
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
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

    }
}
