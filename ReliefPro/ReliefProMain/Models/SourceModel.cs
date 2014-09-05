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
                    SourceType_Color = ColorBorder.blue.ToString();
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
                if (dbmodel.MaxPossiblePressure != value)
                {
                    MaxPossiblePressure_Color = ColorBorder.blue.ToString();
                    dbmodel.MaxPossiblePressure = value;
                    NotifyPropertyChanged("MaxPossiblePressure");
                }

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
        public bool IsSteam
        {
            get
            {
                return dbmodel.IsSteam;
            }
            set
            {
                if (dbmodel.IsSteam!=value)
                {
                    dbmodel.IsSteam != value;
                }
                OnPropertyChanged("IsSteam");

            }
        }
        
        public bool IsHeatSource
        {
            get
            {
                return dbmodel.IsHeatSource;
            }
            set
            {
                dbmodel.IsHeatSource = value;
                OnPropertyChanged("IsHeatSource");
            }
        }

        public string SourceName_Color {
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

        public string SourceType_Color
        {
            get
            {
                return dbmodel.SourceType_Color;
            }
            set
            {
                dbmodel.SourceType_Color = value;
                OnPropertyChanged("SourceType_Color");
            }
        }

        public string MaxPossiblePressure_Color
        {
            get
            {
                return dbmodel.MaxPossiblePressure_Color;
            }
            set
            {
                dbmodel.MaxPossiblePressure_Color = value;
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
