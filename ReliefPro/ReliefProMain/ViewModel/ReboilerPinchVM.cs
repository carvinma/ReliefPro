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

        private double? _Coldtin;
        public double? Coldtin
        {
            get { return _Coldtin; }
            set
            {
                _Coldtin = value;
                OnPropertyChanged("Coldtin");
            }
        }
        private double? _Coldtout;
        public double? Coldtout
        {
            get { return _Coldtout; }
            set
            {
                _Coldtout = value;
                OnPropertyChanged("Coldtout");
            }
        }
        private double? _HeatTin;
        public double? HeatTin
        {
            get { return _HeatTin; }
            set
            {
                _HeatTin = value;
                ReliefHeatTin = _HeatTin;
                OnPropertyChanged("HeatTin");
            }
        }
        private double? _HeatTout;
        public double? HeatTout
        {
            get { return _HeatTout; }
            set
            {
                _HeatTout = value;
                OnPropertyChanged("HeatTout");
            }
        }

        private double? _ReliefHeatTin;
        public double? ReliefHeatTin
        {
            get { return _ReliefHeatTin; }
            set
            {
                _ReliefHeatTin = value;
                OnPropertyChanged("ReliefHeatTin");
            }
        }

        private double? _ReliefColdtout;
        public double? ReliefColdtout
        {
            get { return _ReliefColdtout; }
            set
            {
                _ReliefColdtout = value;
                OnPropertyChanged("ReliefColdtout");
            }
        }

        private double? _Area;
        public double? Area
        {
            get { return _Area; }
            set
            {
                _Area = value;
                OnPropertyChanged("Area");
            }
        }
        private double? _UDesign;
        public double? UDesign
        {
            get { return _UDesign; }
            set
            {
                _UDesign = value;
                OnPropertyChanged("UDesign");
            }
        }

        private double? _UDesignArea;
        public double? UDesignArea
        {
            get { return _UDesignArea; }
            set
            {
                _UDesignArea = value;
                OnPropertyChanged("UDesignArea");
            }
        }
        private double? _UClean;
        public double? UClean
        {
            get { return _UClean; }
            set
            {
                _UClean = value;
                OnPropertyChanged("UClean");
            }
        }

        private double? _UCD;
        public double? UCD
        {
            get { return _UCD; }
            set
            {
                _UCD = value;
                OnPropertyChanged("UCD");
            }
        }
        private double? _Duty;
        public double? Duty
        {
            get { return _Duty; }
            set
            {
                _Duty = value;
                OnPropertyChanged("Duty");
            }
        }

        private double? _ReliefDuty;
        public double? ReliefDuty
        {
            get { return _ReliefDuty; }
            set
            {
                _ReliefDuty = value;
                OnPropertyChanged("ReliefDuty");
            }
        }
        private double? _Factor;
        public double? Factor
        {
            get { return _Factor; }
            set
            {
                _Factor = value;
                OnPropertyChanged("Factor");
            }
        }







        private string _IsPinch_Color;
        public string IsPinch_Color
        {
            get { return _IsPinch_Color; }
            set
            {
                _IsPinch_Color = value;
                OnPropertyChanged("IsPinch_Color");
            }
        }


        private string _SourceType_Color;
        public string SourceType_Color
        {
            get { return _SourceType_Color; }
            set
            {
                _SourceType_Color = value;
                OnPropertyChanged("SourceType_Color");
            }
        }

        private string _Coldtin_Color;
        public string Coldtin_Color
        {
            get { return _Coldtin_Color; }
            set
            {
                _Coldtin_Color = value;
                OnPropertyChanged("Coldtin_Color");
            }
        }
        private string _Coldtout_Color;
        public string Coldtout_Color
        {
            get { return _Coldtout_Color; }
            set
            {
                _Coldtout_Color = value;
                OnPropertyChanged("Coldtout_Color");
            }
        }
        private string _HeatTin_Color;
        public string HeatTin_Color
        {
            get { return _HeatTin_Color; }
            set
            {
                _HeatTin_Color = value;
                OnPropertyChanged("HeatTin_Color");
            }
        }
        private string _HeatTout_Color;
        public string HeatTout_Color
        {
            get { return _HeatTout_Color; }
            set
            {
                _HeatTout_Color = value;
                OnPropertyChanged("HeatTout_Color");
            }
        }

        private string _ReliefHeatTin_Color;
        public string ReliefHeatTin_Color
        {
            get { return _ReliefHeatTin_Color; }
            set
            {
                _ReliefHeatTin_Color = value;
                OnPropertyChanged("ReliefHeatTin_Color");
            }
        }

        private string _ReliefColdtout_Color;
        public string ReliefColdtout_Color
        {
            get { return _ReliefColdtout_Color; }
            set
            {
                _ReliefColdtout_Color = value;
                OnPropertyChanged("ReliefColdtout_Color");
            }
        }

        private string _Area_Color;
        public string Area_Color
        {
            get { return _Area_Color; }
            set
            {
                _Area_Color = value;
                OnPropertyChanged("Area_Color");
            }
        }
        private string _UDesign_Color;
        public string UDesign_Color
        {
            get { return _UDesign_Color; }
            set
            {
                _UDesign_Color = value;
                OnPropertyChanged("UDesign_Color");
            }
        }

        private string _UDesignArea_Color;
        public string UDesignArea_Color
        {
            get { return _UDesignArea_Color; }
            set
            {
                _UDesignArea_Color = value;
                OnPropertyChanged("UDesignArea_Color");
            }
        }
        private string _UClean_Color;
        public string UClean_Color
        {
            get { return _UClean_Color; }
            set
            {
                _UClean_Color = value;
                OnPropertyChanged("UClean_Color");
            }
        }

        private string _UCD_Color;
        public string UCD_Color
        {
            get { return _UCD_Color; }
            set
            {
                _UCD_Color = value;
                OnPropertyChanged("UCD_Color");
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

        private string _ReliefDuty_Color;
        public string ReliefDuty_Color
        {
            get { return ReliefDuty_Color; }
            set
            {
                _ReliefDuty_Color = value;
                OnPropertyChanged("ReliefDuty_Color");
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
        UOMLib.UOMEnum uomEnum;
        public ReboilerPinchVM(int TowerScenarioHXID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            uomEnum = new UOMLib.UOMEnum(SessionPlant);
            InitUnit();
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
                double duty = hx.DutyCalcFactor.Value * detail.Duty.Value;
                Duty = duty;

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
                Factor = reboilerPinchModel.Factor;
                ReliefColdtout = reboilerPinchModel.ReliefColdtout;
                Area = reboilerPinchModel.Area;
                UDesignArea = reboilerPinchModel.Area.Value * reboilerPinchModel.UDesign.Value;
                UCD = UClean.Value / UDesign.Value;
                ReliefHeatTin = reboilerPinchModel.ReliefHeatTin;
                SourceType = reboilerPinchModel.SourceType;
            }
            ReadConvert();

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
            double QRQN = 1;
            double uQAQR = 0;
            while (curErrorRate > MaxErrorRate && iterateSum < iterateNumber)
            {
                reliefLtmd = GetLMTDRelief(productTin, productTout, reliefProductTout, reboilerTin, reboilerTout, nextQaenGuess);
                reliefCoeff = coeff;
                reliefArea = area;
                reliefDuty = reliefLtmd * reliefCoeff * reliefArea * 3.6;
                //reliefDuty = reliefLtmd * reliefCoeff * reliefArea ;
                assumedQA = duty * nextQaenGuess;
                calculatedQR = reliefDuty;
                QRQN = calculatedQR / duty;
                uQAQR = assumedQA / calculatedQR - 1;
                curErrorRate = Math.Abs(uQAQR);
                iterateSum = iterateSum + 1;
                if (curErrorRate > MaxErrorRate)
                {
                    nextQaenGuess = QRQN * (1 + 0.5 * uQAQR);
                }

            }

            if (QRQN < 1)
            {
                isPinch = true;
            }
            factor = QRQN;
            ReliefDuty = reliefDuty;
            Factor = factor;


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

            double productTin = Coldtin.Value;
            double productTout = Coldtout.Value ;
            double reliefProductTout = ReliefColdtout.Value;
            double reboilerTin = HeatTin.Value;
            double reboilerTout = HeatTout.Value;
            double coeff = UClean.Value;
            double area = Area.Value;
            double duty = Duty.Value;
            bool isUseSteam = false;
            double qaenGuess = 1.2;
            GetUDesign();
            //HeatMediumMethod(356, 382, 435.7, 580, 478, 109, 8655, 80880000, isUseSteam, qaenGuess, MaxErrorRate, ref factor, ref _IsPinch, ref iterateNumber);

            HeatMediumMethod(productTin, productTout, reliefProductTout, reboilerTin, reboilerTout, coeff, area, duty, isUseSteam, qaenGuess, MaxErrorRate, ref factor, ref _IsPinch, ref iterateNumber);

        }

        private void GetUDesign()
        {
            if (Coldtin!=null && Coldtout!=null && HeatTin!=null && HeatTout!=null)
            {
                double productTin = Coldtin.Value;
                double productTout = Coldtout.Value;
                double reboilerTin = HeatTin.Value;
                double reboilerTout = HeatTout.Value;
                double lmtd = GetLMTD(productTin, productTout, reboilerTin, reboilerTout);
                if (Area!=null && Duty!=null)
                {
                    double area = Area.Value;
                    double duty = Duty.Value;
                    double udesign = duty / lmtd / area / 3.6;
                    UDesign = udesign;
                    UDesignArea = udesign * area;
                    if (UClean!=null)
                    {
                        UCD = UClean.Value / udesign;
                    }

                }
            }


        }

        private double GetLMTD(double productTin, double productTout, double reboilerTin, double reboilerTout)
        {
            double t = reboilerTout + productTout - reboilerTin - productTin;
            double lnValue = (reboilerTout - productTin) / (reboilerTin - productTout);
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
            double reliefLtmd = relieft / Math.Log(lnValue);
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
            WriteConvert();
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
                reboilerPinchModel.Factor = Factor;
                reboilerPinchModel.Area = Area;
                reboilerPinchModel.ReliefColdtout = ReliefColdtout;
                reboilerPinchModel.SourceType = SourceType;
                reboilerPinchModel.ReliefHeatTin = ReliefHeatTin;
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
                reboilerPinchModel.Factor = Factor;
                reboilerPinchModel.Area = Area;
                reboilerPinchModel.ReliefColdtout = ReliefColdtout;
                reboilerPinchModel.SourceType = SourceType;
                reboilerPinchModel.ReliefHeatTin = ReliefHeatTin;
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
            WriteConvert();
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
                reboilerPinchModel.Factor = Factor;
                reboilerPinchModel.Area = Area;
                reboilerPinchModel.ReliefColdtout = ReliefColdtout;
                reboilerPinchModel.SourceType = SourceType;
                reboilerPinchModel.ReliefHeatTin = ReliefHeatTin;
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
                reboilerPinchModel.Factor = Factor;
                reboilerPinchModel.Area = Area;
                reboilerPinchModel.ReliefColdtout = ReliefColdtout;
                reboilerPinchModel.SourceType = SourceType;
                reboilerPinchModel.ReliefHeatTin = ReliefHeatTin;
                reboilerPinchDAL.Update(reboilerPinchModel, SessionProtectedSystem);
            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private void ReadConvert()
        {
            if (_Coldtin!=null)
                _Coldtin = UnitConvert.Convert(UOMEnum.Temperature, _ColdtinUnit, _Coldtin.Value);
            if (_Coldtout!=null)
                _Coldtout = UnitConvert.Convert(UOMEnum.Temperature, _ColdtoutUnit, _Coldtout.Value);
            if (_HeatTin!=null)
                _HeatTin = UnitConvert.Convert(UOMEnum.Temperature, _HeatTinUnit, _HeatTin.Value);

            if (_HeatTout!=null)
                _HeatTout = UnitConvert.Convert(UOMEnum.Temperature, _HeatToutUnit, _HeatTout.Value);
            if (_ReliefHeatTin!=null)
                _ReliefHeatTin = UnitConvert.Convert(UOMEnum.Temperature, _ReliefHeatTinUnit, _ReliefHeatTin.Value);
            if (_ReliefColdtout!=null)
                _ReliefColdtout = UnitConvert.Convert(UOMEnum.Temperature, _ReliefColdtoutUnit, _ReliefColdtout.Value);

            if (_Area!=null)
                _Area = UnitConvert.Convert(UOMEnum.Area, _AreaUnit, _Area.Value);
            if (_UDesign!=null)
                _UDesign = UnitConvert.Convert(UOMEnum.ThermalConductivity, _UDesignUnit, _UDesign.Value);
            if (_UDesignArea!=null)
                _UDesignArea = UnitConvert.Convert(UOMEnum.HeatTransCoeffcient, _UDesignAreaUnit, _UDesignArea.Value);

            if (_UClean!=null)
                _UClean = UnitConvert.Convert(UOMEnum.ThermalConductivity, _UCleanUnit, _UClean.Value);
            if (_UCD!=null)
                _UCD = UnitConvert.Convert(UOMEnum.Temperature, _UCDUnit, _UCD.Value);
            if (_Duty!=null)
                _Duty = UnitConvert.Convert(UOMEnum.EnthalpyDuty, _DutyUnit, _Duty.Value);

            if (_ReliefDuty != null)
                _ReliefDuty = UnitConvert.Convert(UOMEnum.EnthalpyDuty, _ReliefDutyUnit, _ReliefDuty.Value);
            //if (!string.IsNullOrEmpty(_HeaderPressure))
            //    _Coldtout = unitConvert.Convert(UOMEnum.Pressure, _HeaderPressureUnit, double.Parse(_Coldtout)).ToString();
        }
        private void WriteConvert()
        {
            if (_Coldtin!=null)
                _Coldtin = UnitConvert.Convert(_ColdtinUnit, UOMEnum.Temperature, _Coldtin.Value);
            if (_Coldtout!=null)
                _Coldtout = UnitConvert.Convert(_ColdtoutUnit, UOMEnum.Temperature, _Coldtout.Value);
            if (_HeatTin!=null)
                _HeatTin = UnitConvert.Convert(_HeatTinUnit, UOMEnum.Temperature, _HeatTin.Value);

            if (_HeatTout!=null)
                _HeatTout = UnitConvert.Convert(_HeatToutUnit, UOMEnum.Temperature, _HeatTout.Value);
            if (_ReliefHeatTin!=null)
                _ReliefHeatTin = UnitConvert.Convert(_ReliefHeatTinUnit, UOMEnum.Temperature, _ReliefHeatTin.Value);
            if (_ReliefColdtout!=null)
                _ReliefColdtout = UnitConvert.Convert(_ReliefColdtoutUnit, UOMEnum.Temperature, _ReliefColdtout.Value);

            if (_Area!=null)
                _Area = UnitConvert.Convert(_AreaUnit, UOMEnum.Area, _Area.Value);
            if (_UDesign!=null)
                _UDesign = UnitConvert.Convert(_UDesignUnit, UOMEnum.ThermalConductivity, _UDesign.Value);
            if (_UDesignArea!=null)
                _UDesignArea = UnitConvert.Convert(_UDesignAreaUnit, UOMEnum.HeatTransCoeffcient, _UDesignArea.Value);

            if (_UClean!=null)
                _UClean = UnitConvert.Convert(_UCleanUnit, UOMEnum.ThermalConductivity, _UClean.Value);
            if (_UCD!=null)
                _UCD = UnitConvert.Convert(_UCDUnit, UOMEnum.Temperature, _UCD.Value);
            if (_Duty!=null)
                _Duty = UnitConvert.Convert(_DutyUnit, UOMEnum.EnthalpyDuty, _Duty.Value);

            if (_ReliefDuty!=null)
                _ReliefDuty = UnitConvert.Convert(_ReliefDutyUnit, UOMEnum.EnthalpyDuty, _ReliefDuty.Value);
            //if (!string.IsNullOrEmpty(_HeaderPressure))
            //    _Coldtout = unitConvert.Convert(_HeaderPressureUnit,UOMEnum.Pressure,  double.Parse(_Coldtout)).ToString();
        }
        private void InitUnit()
        {
            this._ColdtinUnit = uomEnum.UserTemperature;
            this._ColdtoutUnit = uomEnum.UserTemperature;
            this._HeatTinUnit = uomEnum.UserTemperature;

            this._HeatToutUnit = uomEnum.UserTemperature;
            this._ReliefHeatTinUnit = uomEnum.UserTemperature;
            this._ReliefColdtoutUnit = uomEnum.UserTemperature;

            this._AreaUnit = uomEnum.UserArea;
            this._UDesignUnit = uomEnum.UserThermalConductivity;
            this._UDesignAreaUnit = uomEnum.UserHeatTransCoeffcient;

            this._UCleanUnit = uomEnum.UserThermalConductivity;
            this._UCDUnit = uomEnum.UserTemperature;
            this._DutyUnit = uomEnum.UserEnthalpyDuty;

            this._ReliefDutyUnit = uomEnum.UserEnthalpyDuty;
            this._HeaderPressureUnit = uomEnum.UserPressure;
        }
        #region 单位字段
        private string _ColdtinUnit;
        public string ColdtinUnit
        {
            get { return _ColdtinUnit; }
            set
            {
                _ColdtinUnit = value;
                OnPropertyChanged("ColdtinUnit");
            }
        }
        private string _ColdtoutUnit;
        public string ColdtoutUnit
        {
            get { return _ColdtoutUnit; }
            set
            {
                _ColdtoutUnit = value;
                OnPropertyChanged("ColdtoutUnit");
            }
        }
        private string _HeatTinUnit;
        public string HeatTinUnit
        {
            get { return _HeatTinUnit; }
            set
            {
                _HeatTinUnit = value;
                OnPropertyChanged("HeatTinUnit");
            }
        }
        private string _HeatToutUnit;
        public string HeatToutUnit
        {
            get { return _HeatToutUnit; }
            set
            {
                _HeatToutUnit = value;
                OnPropertyChanged("HeatToutUnit");
            }
        }

        private string _ReliefHeatTinUnit;
        public string ReliefHeatTinUnit
        {
            get { return _ReliefHeatTinUnit; }
            set
            {
                _ReliefHeatTinUnit = value;
                OnPropertyChanged("ReliefHeatTinUnit");
            }
        }

        private string _ReliefColdtoutUnit;
        public string ReliefColdtoutUnit
        {
            get { return _ReliefColdtoutUnit; }
            set
            {
                _ReliefColdtoutUnit = value;
                OnPropertyChanged("ReliefColdtoutUnit");
            }
        }

        private string _AreaUnit;
        public string AreaUnit
        {
            get { return _AreaUnit; }
            set
            {
                _AreaUnit = value;
                OnPropertyChanged("AreaUnit");
            }
        }
        private string _UDesignUnit;
        public string UDesignUnit
        {
            get { return _UDesignUnit; }
            set
            {
                _UDesignUnit = value;
                OnPropertyChanged("UDesignUnit");
            }
        }

        private string _UDesignAreaUnit;
        public string UDesignAreaUnit
        {
            get { return _UDesignAreaUnit; }
            set
            {
                _UDesignAreaUnit = value;
                OnPropertyChanged("UDesignAreaUnit");
            }
        }
        private string _UCleanUnit;
        public string UCleanUnit
        {
            get { return _UCleanUnit; }
            set
            {
                _UCleanUnit = value;
                OnPropertyChanged("UCleanUnit");
            }
        }

        private string _UCDUnit;
        public string UCDUnit
        {
            get { return _UCDUnit; }
            set
            {
                _UCDUnit = value;
                OnPropertyChanged("UCDUnit");
            }
        }
        private string _DutyUnit;
        public string DutyUnit
        {
            get { return _DutyUnit; }
            set
            {
                _DutyUnit = value;
                OnPropertyChanged("DutyUnit");
            }
        }

        private string _ReliefDutyUnit;
        public string ReliefDutyUnit
        {
            get { return _ReliefDutyUnit; }
            set
            {
                _ReliefDutyUnit = value;
                OnPropertyChanged("ReliefDutyUnit");
            }
        }

        private string _HeaderPressureUnit;
        public string HeaderPressureUnit
        {
            get { return _HeaderPressureUnit; }
            set
            {
                _HeaderPressureUnit = value;
                OnPropertyChanged("HeaderPressureUnit");
            }
        }
        #endregion

    }
}
