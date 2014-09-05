using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;
using ReliefProCommon.Enum;

namespace ReliefProMain.Models
{
    public class SourceModel : ModelBase
    {
        public Source dbmodel;
        public SourceModel(Source m)
        {
            dbmodel = m;
            _SourceTypes = GetSourceTypes();
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

        public string SourceType
        {
            get
            {
                return dbmodel.SourceType;
            }
            set
            {
                if (dbmodel.SourceType != value)
                {
                    dbmodel.SourceType = value;
                    NotifyPropertyChanged("SourceType");
                }
            }
        }

        public string Description
        {
            get
            {
                return dbmodel.Description;
            }
            set
            {
                if (dbmodel.Description != value)
                {
                    dbmodel.Description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        public double MaxPossiblePressure
        {
            get
            {
                return dbmodel.MaxPossiblePressure;
            }
            set
            {
                MaxPossiblePressure_Color = ColorBorder.blue.ToString();
                dbmodel.MaxPossiblePressure = value;
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
        public bool _IsSteam;
        public bool IsSteam
        {
            get
            {
                return this._IsSteam;
            }
            set
            {
                this._IsSteam = value;
                if (this._IsSteam)
                {
                    SourceType = "Pressurized Vessel";
                }
                OnPropertyChanged("IsSteam");

            }
        }
        public bool _IsHeatSource;
        public bool IsHeatSource
        {
            get
            {
                return this._IsHeatSource;
            }
            set
            {
                this._IsHeatSource = value;
                OnPropertyChanged("IsHeatSource");
            }
        }

        public string _SourceName_Color;
        public string SourceName_Color {
            get
            {
                return this._SourceName_Color;
            }
            set
            {
                this._SourceName_Color = value;
                OnPropertyChanged("SourceName_Color");
            }
        }
        public string Description_Color { get; set; }

        private string _SourceType_Color;
        public string SourceType_Color
        {
            get
            {
                return this._SourceType_Color;
            }
            set
            {
                this._SourceType_Color = value;
                OnPropertyChanged("SourceType_Color");
            }
        }
        private string _MaxPossiblePressure_Color;
        public string MaxPossiblePressure_Color
        {
            get
            {
                return this._MaxPossiblePressure_Color;
            }
            set
            {
                this._MaxPossiblePressure_Color = value;
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
            return list;
        }
    }
}
