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
        public tbSink dbmodel;
        public SinkModel(tbSink m)
        {
            dbmodel = m;
            _MaxPossiblePressure = m.Maxpossiblepressure??0;
            _MaxPossiblePressure_Color = m.MaxpossiblepressureColor;
            _SinkType = m.Sinktype;
            _SinkType_Color = m.SinktypeColor;
            _SinkTypes = GetSinkTypes();
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
        public string SinkName
        {
            get
            {
                return dbmodel.Sinkname;
            }
            set
            {
                if (dbmodel.Sinkname != value)
                {
                    dbmodel.Sinkname = value;
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
                if (dbmodel.Sinktype == value && dbmodel.SinktypeColor == ColorBorder.green.ToString())
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
