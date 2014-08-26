using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;

namespace ReliefProMain.Models
{
    public class SinkModel : ModelBase
    {
        public Sink dbmodel;
        public SinkModel(Sink m)
        {
            dbmodel = m;
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

        public string SinkType
        {
            get
            {
                return dbmodel.SinkType;
            }
            set
            {
                if (dbmodel.SinkType != value)
                {
                    dbmodel.SinkType = value;
                    NotifyPropertyChanged("SinkType");
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
    }
}
