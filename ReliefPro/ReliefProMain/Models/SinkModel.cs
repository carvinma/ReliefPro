using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;

namespace ReliefProMain.Models
{
    public class SinkModel : ModelBase
    {
        public Sink model;
        public SinkModel(Sink m)
        {
            model = m;
        }
        public int ID
        {
            get
            {
                return model.ID;
            }
            set
            {
                if (model.ID != value)
                {
                    model.ID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        public string SinkName
        {
            get
            {
                return model.SinkName;
            }
            set
            {
                if (model.SinkName != value)
                {
                    model.SinkName = value;
                    NotifyPropertyChanged("SinkName");
                }
            }
        }

        public string SinkType
        {
            get
            {
                return model.SinkType;
            }
            set
            {
                if (model.SinkType != value)
                {
                    model.SinkType = value;
                    NotifyPropertyChanged("SinkType");
                }
            }
        }

        public string Description
        {
            get
            {
                return model.Description;
            }
            set
            {
                if (model.Description != value)
                {
                    model.Description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        public double MaxPossiblePressure
        {
            get
            {
                return model.MaxPossiblePressure;
            }
            set
            {
                if (model.MaxPossiblePressure != value)
                {
                    model.MaxPossiblePressure = value;
                    NotifyPropertyChanged("MaxPossiblePressure");
                }
            }
        }
        public string StreamName
        {
            get
            {
                return model.StreamName;
            }
            set
            {
                if (model.StreamName != value)
                {
                    model.StreamName = value;
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
    }
}
