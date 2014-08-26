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

namespace ReliefProMain.ViewModel
{
    public class AccumulatorVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        UOMLib.UOMEnum uomEnum;
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

        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ViewModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double Diameter
        {
            get
            {
                return this._Diameter;
            }
            set
            {
                this._Diameter = value;
                if (Horiz && _NormalLiquidLevel != null && _Diameter != null)
                {
                    NormalLiquidLevel = _Diameter / 2;
                }
                OnPropertyChanged("Diameter");
            }
        }

        private double _Length;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ViewModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
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
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ViewModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
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

        private string accumulatorName_Color = ColorBorder.blue.ToString();
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
        private string _Diameter_Color = ColorBorder.blue.ToString();
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

        private string _Length_Color = ColorBorder.blue.ToString();
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
        private string _NormalLiquidLevel_Color = ColorBorder.blue.ToString();
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
            uomEnum = new UOMLib.UOMEnum(sessionPlant);
            InitUnit();
            AccumulatorTypes = GetAccumulatorTypes();
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;

            AccumulatorDAL db = new AccumulatorDAL();
            CurrentAccumulator = db.GetModel(SessionProtectedSystem);
            Diameter = CurrentAccumulator.Diameter;
            Length = CurrentAccumulator.Length;
            NormalLiquidLevel = CurrentAccumulator.NormalLiquidLevel;
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
            WriteConvert();
            CurrentAccumulator.AccumulatorName = CurrentAccumulator.AccumulatorName.Trim();
            if (CurrentAccumulator.AccumulatorName == "")
            {
                throw new ArgumentException("Please type in a name for the Accumulator.");
            }

            AccumulatorDAL db = new AccumulatorDAL();
            Accumulator m = db.GetModel(SessionProtectedSystem);

            m.AccumulatorName = CurrentAccumulator.AccumulatorName;
            m.Diameter = Diameter;
            m.Length = Length;
            m.NormalLiquidLevel = NormalLiquidLevel;
            if (Horiz)
                m.Orientation = true;
            else
                m.Orientation = false;
            db.Update(m, SessionProtectedSystem);
            SessionProtectedSystem.Flush();


            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

        private void ReadConvert()
        {
            if (_Diameter != null)
                _Diameter = UnitConvert.Convert(UOMEnum.Length, _DiameterUnit, _Diameter);
            if (_Length != null)
                _Length = UnitConvert.Convert(UOMEnum.Length, _LengthUnit, _Length);
            if (_NormalLiquidLevel != null)
                _NormalLiquidLevel = UnitConvert.Convert(UOMEnum.Length, _NormalLiquidLevelUnit, _NormalLiquidLevel);
        }
        private void WriteConvert()
        {
            if (_Diameter != null)
                _Diameter = UnitConvert.Convert(_DiameterUnit, UOMEnum.Length, _Diameter);
            if (_Length != null)
                _Length = UnitConvert.Convert(_LengthUnit, UOMEnum.Length, _Length);
            if (_NormalLiquidLevel != null)
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
