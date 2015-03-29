using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;

namespace ReliefProMain.Models
{
    public class CustomStreamModel : ModelBase
    {
        public tbStream model;


        public int ID
        {
            get
            {
                return model.Id;
            }
            set
            {
                if (model.Id != value)
                {
                    model.Id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public string StreamName
        {
            get
            {
                return model.Streamname;
            }
            set
            {
                if (model.Streamname != value)
                {
                    model.Streamname = value;
                    NotifyPropertyChanged("StreamName");
                }
            }
        }


        public double Pressure
        {
            get
            {
                return model.Pressure??0;
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
                return model.Spenthalpy??0;
            }
            set
            {
                if (model.Spenthalpy != value)
                {
                    model.Spenthalpy = value;
                    NotifyPropertyChanged("SpEnthalpy");
                }
            }
        }

        public double Temperature
        {
            get
            {
                return model.Temperature??0;
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
                return model.Vaporfraction??0;
            }
            set
            {
                if (model.Vaporfraction != value)
                {
                    model.Vaporfraction = value;
                    NotifyPropertyChanged("VaporFraction");
                }
            }
        }

        public double WeightFlow
        {
            get
            {
                return model.Weightflow??0;
            }
            set
            {
                if (model.Weightflow != value)
                {
                    model.Weightflow = value;
                    NotifyPropertyChanged("WeightFlow");
                }
            }
        }


        public CustomStreamModel(tbStream m)
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
