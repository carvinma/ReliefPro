using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;

namespace ReliefProMain.Models
{
    public class CustomStreamModel : ModelBase
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


        public double Pressure
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
        public double SpEnthalpy
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

        public double Temperature
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

        public double VaporFraction
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

        public double WeightFlow
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

        private string temperatureUnit;
        public string TemperatureUnit
        {
            get
            {
                return temperatureUnit;
            }
            set
            {
                if (temperatureUnit != value)
                {
                    temperatureUnit = value;
                    NotifyPropertyChanged("TemperatureUnit");
                }
            }
        }
        private string pressureUnit;
        public string PressureUnit
        {
            get
            {
                return pressureUnit;
            }
            set
            {
                if (pressureUnit != value)
                {
                    pressureUnit = value;
                    NotifyPropertyChanged("PressureUnit");
                }
            }
        }
        private string weightFlowUnit;
        public string WeightFlowUnit
        {
            get
            {
                return weightFlowUnit;
            }
            set
            {
                if (weightFlowUnit != value)
                {
                    weightFlowUnit = value;
                    NotifyPropertyChanged("WeightFlowUnit");
                }
            }
        }

        private string spEnthalpyUnit;
        public string SpEnthalpyUnit
        {
            get
            {
                return spEnthalpyUnit;
            }
            set
            {
                if (spEnthalpyUnit != value)
                {
                    spEnthalpyUnit = value;
                    NotifyPropertyChanged("SpEnthalpyUnit");
                }
            }
        }

    }
}
