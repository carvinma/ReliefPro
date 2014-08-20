using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel.Drums;

namespace ReliefProMain.Models
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

        private string twUnit;
        public string TWUnit
        {
            get { return twUnit; }
            set
            {
                twUnit = value;
                this.NotifyPropertyChanged("TWUnit");
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

        private double? vaporMW;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double? VaporMW
        {
            get { return vaporMW; }
            set
            {
                vaporMW = value;
                this.NotifyPropertyChanged("VaporMW");
            }
        }
        private double? vessel;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double? Vessel
        {
            get { return vessel; }
            set
            {
                vessel = value;
                this.NotifyPropertyChanged("Vessel");
            }
        }

        private double? temperature;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double? Temperature
        {
            get { return temperature; }
            set
            {
                temperature = value;
                this.NotifyPropertyChanged("Temperature");
            }
        }

        private double? pressure;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double? Pressure
        {
            get { return pressure; }
            set
            {
                pressure = value;
                this.NotifyPropertyChanged("Pressure");
            }
        }

        private double? pSVPressure;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double? PSVPressure
        {
            get { return pSVPressure; }
            set
            {
                pSVPressure = value;
                this.NotifyPropertyChanged("PSVPressure");
            }
        }

        private double? tw;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double? TW
        {
            get { return tw; }
            set
            {
                tw = value;
                this.NotifyPropertyChanged("TW");
            }
        }



        private string vaporMW_Color;
        public string VaporMW_Color
        {
            get { return vaporMW_Color; }
            set
            {
                vaporMW_Color = value;
                this.NotifyPropertyChanged("VaporMW_Color");
            }
        }
        private string vessel_Color;
        public string Vessel_Color
        {
            get { return vessel_Color; }
            set
            {
                vessel_Color = value;
                this.NotifyPropertyChanged("Vessel_Color");
            }
        }

        private string temperature_Color;
        public string Temperature_Color
        {
            get { return temperature_Color; }
            set
            {
                temperature_Color = value;
                this.NotifyPropertyChanged("Temperature_Color");
            }
        }

        private string pressure_Color;
        public string Pressure_Color
        {
            get { return pressure_Color; }
            set
            {
                pressure_Color = value;
                this.NotifyPropertyChanged("Pressure_Color");
            }
        }

        private string pSVPressure_Color;
        public string PSVPressure_Color
        {
            get { return pSVPressure_Color; }
            set
            {
                pSVPressure_Color = value;
                this.NotifyPropertyChanged("PSVPressure_Color");
            }
        }

        private string tw_Color;
        public string TW_Color
        {
            get { return tw_Color; }
            set
            {
                tw_Color = value;
                this.NotifyPropertyChanged("TW_Color");
            }
        }
    }
}
