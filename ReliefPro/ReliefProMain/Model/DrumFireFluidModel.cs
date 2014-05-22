using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel.Drum;

namespace ReliefProMain.Model
{
    public class DrumFireFluidModel : ModelBase
    {
        private string vesselUnit;
        public string VesselUnit
        {
            get { return vesselUnit; }
            set
            {
                vesselUnit = value;
                this.NotifyPropertyChanged("VesselUnit");
            }
        }
        public string temperatureUnit;
        public string TemperatureUnit
        {
            get { return temperatureUnit; }
            set
            {
                temperatureUnit = value;
                this.NotifyPropertyChanged("TemperatureUnit");
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
        private string psvpressureUnit;
        public string PSVPressureUnit
        {
            get { return psvpressureUnit; }
            set
            {
                psvpressureUnit = value;
                this.NotifyPropertyChanged("PSVPressureUnit");
            }
        }

        public DrumFireFluid dbmodel { get; set; }

        public DrumFireFluidModel(DrumFireFluid firemodel)
        {
            dbmodel = firemodel;
            this.vaporMW = firemodel.GasVaporMW;
            this.vessel = firemodel.ExposedVesse;
            this.temperature = firemodel.NormaTemperature;
            this.pressure = firemodel.NormalPressure;
            this.pSVPressure = firemodel.PSVPressure;
            this.tw = firemodel.TW;
        }

        private double vaporMW;
        public double VaporMW
        {
            get { return vaporMW; }
            set
            {
                vaporMW = value;
                this.NotifyPropertyChanged("VaporMW");
            }
        }
        private double vessel;
        public double Vessel
        {
            get { return vessel; }
            set
            {
                vessel = value;
                this.NotifyPropertyChanged("Vessel");
            }
        }

        private double temperature;
        public double Temperature
        {
            get { return temperature; }
            set
            {
                temperature = value;
                this.NotifyPropertyChanged("Temperature");
            }
        }

        private double pressure;
        public double Pressure
        {
            get { return pressure; }
            set
            {
                pressure = value;
                this.NotifyPropertyChanged("Pressure");
            }
        }

        private double pSVPressure;
        public double PSVPressure
        {
            get { return pSVPressure; }
            set
            {
                pSVPressure = value;
                this.NotifyPropertyChanged("PSVPressure");
            }
        }

        private double tw;
        public double TW
        {
            get { return tw; }
            set
            {
                tw = value;
                this.NotifyPropertyChanged("TW");
            }
        }
    }
}
