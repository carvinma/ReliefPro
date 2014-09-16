using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;
using UOMLib;
using ReliefProCommon.Enum;

namespace ReliefProMain.Models
{
    public class SinkModel : ModelBase
    {
        public Sink dbmodel;
        public SinkModel(Sink m)
        {
            dbmodel = m;
            _MaxPossiblePressure = m.MaxPossiblePressure;
            _MaxPossiblePressure_Color = m.MaxPossiblePressure_Color;
            _SinkType = m.SinkType;
            _SinkType_Color = m.SinkType_Color;
            _SinkTypes = GetSinkTypes();
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
        public string SinkName
        {
            get
            {
                return dbmodel.SinkName;
            }
            set
            {
                if (dbmodel.SinkName != value)
                {
                    dbmodel.SinkName = value;
                    NotifyPropertyChanged("SinkName");
                }
            }
        }
        private string _SinkType;
        public string SinkType
        {
            get
            {
                return _SinkType;
            }
            set
            {
                if (dbmodel.SinkType == value && dbmodel.SinkType_Color == ColorBorder.green.ToString())
                {
                    SinkType_Color = ColorBorder.green.ToString();
                }
                else
                {
                    SinkType_Color = ColorBorder.blue.ToString();
                }
                _SinkType = value;
                NotifyPropertyChanged("SinkType");
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

        private double _MaxPossiblePressure;
        public double MaxPossiblePressure
        {
            get
            {
                return _MaxPossiblePressure;
            }
            set
            {                
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

        public List<string> _SinkTypes;
        public List<string> SinkTypes
        {
            get { return _SinkTypes; }
            set
            {
                _SinkTypes = value;
                this.NotifyPropertyChanged("SinkTypes");
            }
        }

        public List<string> GetSinkTypes()
        {
            List<string> list = new List<string>();
            list.Add("Compressor(Motor)");
            list.Add("Compressor(Steam Turbine Driven)");
            list.Add("Pump(Steam Turbine Driven)");
            list.Add("Pump(Motor)");
            list.Add("Pressurized Vessel");
            return list;
        }

        private string _SinkType_Color;
        public string SinkType_Color
        {
            get
            {
                return _SinkType_Color;
            }
            set
            {
                _SinkType_Color = value;
                OnPropertyChanged("SinkType_Color");
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
    }
}
