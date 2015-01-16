using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;
using ReliefProCommon.Enum;
using UOMLib;

namespace ReliefProMain.Models
{
    public class SourceModel : ModelBase
    {
        public Source dbmodel;
        public SourceModel(Source m)
        {
            dbmodel = m;
            _MaxPossiblePressure = m.MaxPossiblePressure;
            _MaxPossiblePressure_Color = m.MaxPossiblePressure_Color;
            _SourceType = m.SourceType;
            _SourceType_Color = m.SourceType_Color;
            _SourceTypes = GetSourceTypes();
            _IsEnabledSourceType = !m.IsSteam;
            _IsHeatSource = m.IsHeatSource;
            _IsSteam = m.IsSteam;
            _Description = m.Description;
        }
        private bool _IsEnabledSourceType;
        public bool IsEnabledSourceType
        {
            get
            {
                return _IsEnabledSourceType;
            }
            set
            {
                _IsEnabledSourceType = value;
                OnPropertyChanged("IsEnabledSourceType");
            }
        }
        public int ID
        {
            get
            {
                return dbmodel.ID;
            }
            set
            {
                if (dbmodel.ID != value)
                {
                    dbmodel.ID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        public string SourceName
        {
            get
            {
                return dbmodel.SourceName;
            }
            set
            {
                if (dbmodel.SourceName != value)
                {
                    dbmodel.SourceName = value;
                    NotifyPropertyChanged("SourceName");
                }
            }
        }

        private string _SourceType;
        public string SourceType
        {
            get
            {
                return _SourceType;
            }
            set
            {
                if (dbmodel.SourceType == value && dbmodel.SourceType_Color==ColorBorder.green.ToString())
                {
                    SourceType_Color = ColorBorder.green.ToString();
                }
                else
                {
                    SourceType_Color = ColorBorder.blue.ToString();
                }
                _SourceType = value;                
                NotifyPropertyChanged("SourceType");
            }
        }

        private string _Description;
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (_Description != value)
                {
                    dbmodel.Description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        private double _MaxPossiblePressure;
        public double MaxPossiblePressure
        {
            get
            {
                return _MaxPossiblePressure;
            }
            set
            {
                if (dbmodel.MaxPossiblePressure ==  UnitConvert.Convert(PressureUnit, UOMEnum.Pressure, value) && dbmodel.MaxPossiblePressure_Color==ColorBorder.green.ToString())
                {
                    MaxPossiblePressure_Color = ColorBorder.green.ToString();
                }
                else
                {
                    MaxPossiblePressure_Color = ColorBorder.blue.ToString();
                }
                _MaxPossiblePressure = value;
                NotifyPropertyChanged("MaxPossiblePressure");
            }
        }
        public string StreamName
        {
            get
            {
                return dbmodel.StreamName;
            }
            set
            {
                if (dbmodel.StreamName != value)
                {
                    dbmodel.StreamName = value;
                    NotifyPropertyChanged("StreamName");
                }
            }
        }

        private string pressureUnit;
        public string PressureUnit
        {
            get { return pressureUnit; }
            set
            {
                pressureUnit = value;
                this.NotifyPropertyChanged("PressureUnit");
            }
        }

        public List<string> _SourceTypes;
        public List<string> SourceTypes
        {
            get { return _SourceTypes; }
            set
            {
                _SourceTypes = value;
                this.NotifyPropertyChanged("SourceTypes");
            }
        }

        private bool _IsSteam;
        public bool IsSteam
        {
            get
            {
                return _IsSteam;
            }
            set
            {
                if (_IsSteam != value)
                {
                    _IsSteam = value;
                }
                if (_IsSteam)
                {
                    SourceType = "Supply Header";
                }
                IsEnabledSourceType = !value;
                OnPropertyChanged("IsSteam");

            }
        }

        private bool _IsHeatSource;
        public bool IsHeatSource
        {
            get
            {
                return _IsHeatSource;
            }
            set
            {
                _IsHeatSource = value;
                OnPropertyChanged("IsHeatSource");
            }
        }

        public string SourceName_Color
        {
            get
            {
                return dbmodel.SourceName_Color;
            }
            set
            {
                dbmodel.SourceName_Color = value;
                OnPropertyChanged("SourceName_Color");
            }
        }
        public string Description_Color { get; set; }

        private string _SourceType_Color;
        public string SourceType_Color
        {
            get
            {
                return _SourceType_Color;
            }
            set
            {
                _SourceType_Color = value;
                OnPropertyChanged("SourceType_Color");
            }
        }

        private string _MaxPossiblePressure_Color;
        public string MaxPossiblePressure_Color
        {
            get
            {
                return _MaxPossiblePressure_Color;
            }
            set
            {
                _MaxPossiblePressure_Color = value;
                OnPropertyChanged("MaxPossiblePressure_Color");
            }
        }


        public List<string> GetSourceTypes()
        {
            List<string> list = new List<string>();
            list.Add("Compressor(Motor)");
            list.Add("Compressor(Steam Turbine Driven)");
            list.Add("Pump(Steam Turbine Driven)");
            list.Add("Pump(Motor)");
            list.Add("Pressurized Vessel");
            list.Add("Supply Header");
            return list;
        }
    }
}
