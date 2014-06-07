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
using UOMLib;
using NHibernate;

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

        private string _Diameter;
        public string Diameter
        {
            get
            {
                return this._Diameter;
            }
            set
            {
                this._Diameter = value;
                if (Horiz && string.IsNullOrEmpty(NormalLiquidLevel) && !string.IsNullOrEmpty(_Diameter))
                {
                    NormalLiquidLevel = (double.Parse(Diameter) / 2).ToString();
                }
                OnPropertyChanged("Diameter");
            }
        }

        private string _Length;
        public string Length
        {
            get
            {
                return this._Length;
            }
            set
            {
                this._Length = value;
                if (Vertical && string.IsNullOrEmpty(NormalLiquidLevel) && !string.IsNullOrEmpty(_Length))
                {
                    NormalLiquidLevel = (double.Parse(Length) / 2).ToString();
                }
                OnPropertyChanged("Length");
            }
        }
        private string _NormalLiquidLevel;
        public string NormalLiquidLevel
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
            if (!string.IsNullOrEmpty(_Diameter))
                _Diameter = UnitConvert.Convert(UOMEnum.Length, _DiameterUnit, double.Parse(_Diameter)).ToString();
            if (!string.IsNullOrEmpty(_Length))
                _Length = UnitConvert.Convert(UOMEnum.Length, _LengthUnit, double.Parse(_Length)).ToString();
            if (!string.IsNullOrEmpty(_NormalLiquidLevel))
                _NormalLiquidLevel = UnitConvert.Convert(UOMEnum.Length, _NormalLiquidLevelUnit, double.Parse(_NormalLiquidLevel)).ToString();
        }
        private void WriteConvert()
        {
            if (!string.IsNullOrEmpty(_Diameter))
                _Diameter = UnitConvert.Convert(_DiameterUnit, UOMEnum.Length, double.Parse(_Diameter)).ToString();
            if (!string.IsNullOrEmpty(_Length))
                _Length = UnitConvert.Convert(_LengthUnit, UOMEnum.Length, double.Parse(_Length)).ToString();
            if (!string.IsNullOrEmpty(_NormalLiquidLevel))
                _NormalLiquidLevel = UnitConvert.Convert(_NormalLiquidLevelUnit, UOMEnum.Length, double.Parse(_NormalLiquidLevel)).ToString();
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
