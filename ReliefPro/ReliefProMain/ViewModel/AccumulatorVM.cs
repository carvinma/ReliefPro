using System;
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
using UOMLib;
using NHibernate;
using System.Windows;
using ReliefProCommon.Enum;
using System.ComponentModel.DataAnnotations;
using ReliefProBLL;

namespace ReliefProMain.ViewModel
{
    public class AccumulatorVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        UOMLib.UOMEnum uomEnum;


        private string _AccumulatorName;
        public string AccumulatorName
        {
            get
            {
                return this._AccumulatorName;
            }
            set
            {
                if (CurrentAccumulator.AccumulatorName == value && CurrentAccumulator.AccumulatorName_Color == ColorBorder.green.ToString())
                {
                    this.AccumulatorName_Color = ColorBorder.green.ToString();
                }
                else
                {
                    this.AccumulatorName_Color = ColorBorder.blue.ToString();
                }

                this._AccumulatorName = value;
                OnPropertyChanged("AccumulatorName");
            }
        }

        private bool _Horiz;
        public bool Horiz
        {
            get
            {
                return this._Horiz;
            }
            set
            {
                this._Horiz = value;
                OnPropertyChanged("Horiz");
            }
        }

        private bool _Vertical;
        public bool Vertical
        {
            get
            {
                return this._Vertical;
            }
            set
            {
                this._Vertical = value;
                OnPropertyChanged("Vertical");
            }
        }

        private double _Diameter;
       
        [ReliefProMain.Util.RegularExpression(ViewModelBase.IsNum, ErrorMessage = "GreaterThanZero")]
        public double Diameter
        {
            get
            {
                return this._Diameter;
            }
            set
            {                
                this._Diameter = value;
                if (Horiz)
                {
                    NormalLiquidLevel = _Diameter / 2;
                }
                OnPropertyChanged("Diameter");
            }
        }

        private double _Length;
        
        [ReliefProMain.Util.RegularExpression(ViewModelBase.IsNum, ErrorMessage = "GreaterThanZero")]
        public double Length
        {
            get
            {
                return this._Length;
            }
            set
            {
                
                this._Length = value;
                if (Vertical)
                {
                    NormalLiquidLevel = _Length / 2;
                }
                OnPropertyChanged("Length");
            }
        }

        private double _NormalLiquidLevel;
        
        [ReliefProMain.Util.RegularExpression(ViewModelBase.IsNum, ErrorMessage = "GreaterThanZero")]
        public double NormalLiquidLevel
        {
            get
            {
                return this._NormalLiquidLevel;
            }
            set
            {
                
                this._NormalLiquidLevel = value;
                OnPropertyChanged("NormalLiquidLevel");
            }
        }

        private string accumulatorName_Color;
        public string AccumulatorName_Color
        {
            get
            {
                return this.accumulatorName_Color;
            }
            set
            {
                this.accumulatorName_Color = value;

                OnPropertyChanged("AccumulatorName_Color");
            }
        }
        private string _Diameter_Color;
        public string Diameter_Color
        {
            get
            {
                return this._Diameter_Color;
            }
            set
            {
                this._Diameter_Color = value;

                OnPropertyChanged("Diameter_Color");
            }
        }

        private string _Length_Color;
        public string Length_Color
        {
            get
            {
                return this._Length_Color;
            }
            set
            {
                this._Length_Color = value;

                OnPropertyChanged("Length_Color");
            }
        }
        private string _NormalLiquidLevel_Color;
        public string NormalLiquidLevel_Color
        {
            get
            {
                return this._NormalLiquidLevel_Color;
            }
            set
            {
                this._NormalLiquidLevel_Color = value;
                OnPropertyChanged("NormalLiquidLevel_Color");
            }
        }

        public List<string> AccumulatorTypes { get; set; }
        public Accumulator CurrentAccumulator { get; set; }
        public List<string> GetAccumulatorTypes()
        {
            List<string> list = new List<string>();
            list.Add("Pressure Driven");
            list.Add("Pump(Motor)");
            list.Add("Pump(Turbine)");
            return list;
        }
        public AccumulatorVM(string name, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            InitUnit();
            //AccumulatorTypes = GetAccumulatorTypes(); //没用到

            SessionProtectedSystem = sessionProtectedSystem;

            AccumulatorDAL db = new AccumulatorDAL();
            CurrentAccumulator = db.GetModel(SessionProtectedSystem);
            AccumulatorName = CurrentAccumulator.AccumulatorName;
            Diameter = CurrentAccumulator.Diameter;
            Length = CurrentAccumulator.Length;
            NormalLiquidLevel = CurrentAccumulator.NormalLiquidLevel;
            Diameter_Color = CurrentAccumulator.Diameter_Color;
            Length_Color = CurrentAccumulator.Length_Color;
            NormalLiquidLevel_Color = CurrentAccumulator.NormalLiquidLevel_Color;

            if (CurrentAccumulator.Orientation)
            {
                Horiz = true;
                Vertical = false;
            }
            else
            {
                Horiz = false;
                Vertical = true;
            }
            ReadConvert();
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
            if (!this.CheckData()) return;
                      
            if (string.IsNullOrEmpty(AccumulatorName))
            {
                //throw new ArgumentException("Please type in a name for the Accumulator.");
                MessageBox.Show("Please type in a name for the Accumulator.", "Message Box");
                return;
            }
            bool bEdit = false;
            if (CurrentAccumulator.Orientation != Horiz || CurrentAccumulator.Diameter != Diameter || CurrentAccumulator.Length != Length || CurrentAccumulator.NormalLiquidLevel != NormalLiquidLevel)
            {
                bEdit = true;
            }
            if (bEdit)
            {
                ScenarioDAL scdal = new ScenarioDAL();
                IList<Scenario> scList = scdal.GetAllList(SessionProtectedSystem);
                if (scList.Count > 0)
                {
                    MessageBoxResult r = MessageBox.Show("Are you sure to edit data? it need to rerun all Scenario", "Message Box", MessageBoxButton.YesNo);
                    if (r == MessageBoxResult.Yes)
                    {
                        ScenarioBLL scBLL = new ScenarioBLL(SessionProtectedSystem);
                        scBLL.DeleteSCOther();
                        scBLL.ClearScenario();

                        //SessionProtectedSystem.Flush();
                    }
                    WriteConvert();
                    AccumulatorDAL db = new AccumulatorDAL();

                    CurrentAccumulator.AccumulatorName = AccumulatorName;
                    CurrentAccumulator.Diameter = Diameter;
                    CurrentAccumulator.Length = Length;
                    CurrentAccumulator.NormalLiquidLevel = NormalLiquidLevel;
                    CurrentAccumulator.AccumulatorName_Color = accumulatorName_Color;
                    CurrentAccumulator.Diameter_Color = Diameter_Color;
                    CurrentAccumulator.Length_Color = Length_Color;
                    CurrentAccumulator.NormalLiquidLevel_Color = NormalLiquidLevel_Color;

                    if (Horiz)
                        CurrentAccumulator.Orientation = true;
                    else
                        CurrentAccumulator.Orientation = false;
                    db.Update(CurrentAccumulator, SessionProtectedSystem);
                }
                
            }

            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

        private void ReadConvert()
        {
            //if (_Diameter != null)
                _Diameter = UnitConvert.Convert(UOMEnum.Length, _DiameterUnit, _Diameter);
            //if (_Length != null)
                _Length = UnitConvert.Convert(UOMEnum.Length, _LengthUnit, _Length);
            //if (_NormalLiquidLevel != null)
                _NormalLiquidLevel = UnitConvert.Convert(UOMEnum.Length, _NormalLiquidLevelUnit, _NormalLiquidLevel);
        }
        private void WriteConvert()
        {
            //if (_Diameter != null)
                _Diameter = UnitConvert.Convert(_DiameterUnit, UOMEnum.Length, _Diameter);
            //if (_Length != null)
                _Length = UnitConvert.Convert(_LengthUnit, UOMEnum.Length, _Length);
            //if (_NormalLiquidLevel != null)
                _NormalLiquidLevel = UnitConvert.Convert(_NormalLiquidLevelUnit, UOMEnum.Length, _NormalLiquidLevel);
        }
        private void InitUnit()
        {
            this._DiameterUnit = uomEnum.UserLength;
            this._LengthUnit = uomEnum.UserLength;
            this._NormalLiquidLevelUnit = uomEnum.UserLength;
        }
        #region 单位-字段
        private string _DiameterUnit;
        public string DiameterUnit
        {
            get
            {
                return this._DiameterUnit;
            }
            set
            {
                this._DiameterUnit = value;
                OnPropertyChanged("DiameterUnit");
            }
        }

        private string _LengthUnit;
        public string LengthUnit
        {
            get
            {
                return this._LengthUnit;
            }
            set
            {
                this._LengthUnit = value;
                OnPropertyChanged("LengthUnit");
            }
        }

        private string _NormalLiquidLevelUnit;
        public string NormalLiquidLevelUnit
        {
            get
            {
                return this._NormalLiquidLevelUnit;
            }
            set
            {
                this._NormalLiquidLevelUnit = value;
                OnPropertyChanged("NormalLiquidLevelUnit");
            }
        }
        #endregion
    }
}
