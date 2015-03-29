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
        public tbSource dbmodel;
        public SourceModel(tbSource m)
        {
            dbmodel = m;
            _MaxPossiblePressure = m.Maxpossiblepressure??0;
            _MaxPossiblePressure_Color = m.MaxpossiblepressureColor;
            _SourceType = m.Sourcetype;
            _SourceType_Color = m.SourcetypeColor;
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
                return dbmodel.Id;
            }
            set
            {
                if (dbmodel.Id != value)
                {
                    dbmodel.Id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        public string SourceName
        {
            get
            {
                return dbmodel.Sourcename;
            }
            set
            {
                if (dbmodel.Sourcename != value)
                {
                    dbmodel.Sourcename = value;
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
                if (dbmodel.Sourcetype == value && dbmodel.SourcetypeColor==ColorBorder.green.ToString())
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
                if (dbmodel.Maxpossiblepressure ==  UnitConvert.Convert(PressureUnit, UOMEnum.Pressure, value) && dbmodel.MaxpossiblepressureColor==ColorBorder.green.ToString())
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
                return dbmodel.Streamname;
            }
            set
            {
                if (dbmodel.Streamname != value)
                {
                    dbmodel.Streamname = value;
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
                return dbmodel.SourcenameColor;
            }
            set
            {
                dbmodel.SourcenameColor = value;
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
