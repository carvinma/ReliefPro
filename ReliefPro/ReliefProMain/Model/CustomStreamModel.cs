using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;

namespace ReliefProMain.Model
{
    public class CustomStreamModel: ModelBase
    {
        public CustomStream model;

       
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
        

        public string Pressure
        {
            get
            {
                return model.Pressure;
            }
            set
            {
                if (model.Pressure != value)
                {
                    model.Pressure = value;
                    NotifyPropertyChanged("Pressure");
                }
            }
        }
        public string SpEnthalpy
        {
            get
            {
                return model.SpEnthalpy;
            }
            set
            {
                if (model.SpEnthalpy != value)
                {
                    model.SpEnthalpy = value;
                    NotifyPropertyChanged("SpEnthalpy");
                }
            }
        }

        public string Temperature
        {
            get
            {
                return model.Temperature;
            }
            set
            {
                if (model.Temperature != value)
                {
                    model.Temperature = value;
                    NotifyPropertyChanged("Temperature");
                }
            }
        }

        public string VaporFraction
        {
            get
            {
                return model.VaporFraction;
            }
            set
            {
                if (model.VaporFraction != value)
                {
                    model.VaporFraction = value;
                    NotifyPropertyChanged("VaporFraction");
                }
            }
        }

        public string WeightFlow
        {
            get
            {
                return model.WeightFlow;
            }
            set
            {
                if (model.WeightFlow != value)
                {
                    model.WeightFlow = value;
                    NotifyPropertyChanged("WeightFlow");
                }
            }
        }
        

        public CustomStreamModel(CustomStream m)
        {
            model = m;
        }
       
    }
}
