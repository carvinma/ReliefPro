using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;

namespace ReliefProMain.Models
{
    public class PSVModel : ModelBase
    {
        public PSV dbmodel;
        public PSVModel(PSV m)
        {
            dbmodel= m;
            this.PSVName = m.PSVName;
            this.Pressure = m.Pressure;
            this.ReliefPressureFactor = m.ReliefPressureFactor;
            this.ValveNumber = m.ValveNumber;
            this.ValveType = m.ValveType;
            this.DrumPSVName = m.DrumPSVName;
            this.Location = m.Location;
            this.DrumPressure = m.DrumPressure;
            this.Description = m.Description;
            this.LocationDescription = m.LocationDescription;
            this.DischargeTo = m.DischargeTo;

            this._ReliefPressureFactor_Color = m.ReliefPressureFactor_Color;
            this.Pressure_Color = m.Pressure_Color;

        }
        private string _PSVName;
        public string PSVName
        {
            get
            {
                return this._PSVName;
            }
            set
            {
                this._PSVName = value;
                NotifyPropertyChanged("PSVName");
            }
        }
        private int _ID;
        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
                NotifyPropertyChanged("ID");
            }
        }

        private int _ValveNumber;
        public int ValveNumber
        {
            get
            {
                return this._ValveNumber;
            }
            set
            {
                this._ValveNumber = value;
                int number = _ValveNumber;
                if (number == 1)
                    ReliefPressureFactor = 1.0;
                else
                    ReliefPressureFactor = 1.16;
                NotifyPropertyChanged("ValveNumber");
            }
        }

        private string _ValveType;
        public string ValveType
        {
            get
            {
                return this._ValveType;
            }
            set
            {
                this._ValveType = value;
                NotifyPropertyChanged("ValveType");
            }
        }
        private string _DischargeTo;
        public string DischargeTo
        {
            get
            {
                return this._DischargeTo;
            }
            set
            {
                this._DischargeTo = value;
                NotifyPropertyChanged("DischargeTo");
            }
        }
        private double _Pressure;
        public double Pressure
        {
            get
            {
                return this._Pressure;
            }
            set
            {
                this._Pressure = value;
                NotifyPropertyChanged("Pressure");
            }
        }
        private string _PressureUnit;
        public string PressureUnit
        {
            get
            {
                return this._PressureUnit;
            }
            set
            {
                this._PressureUnit = value;
                NotifyPropertyChanged("PressureUnit");
            }
        }
        private double _ReliefPressureFactor;
        public double ReliefPressureFactor
        {
            get
            {
                return this._ReliefPressureFactor;
            }
            set
            {
                this._ReliefPressureFactor = value;
                NotifyPropertyChanged("ReliefPressureFactor");
            }
        }
        private string _DrumPSVName;
        public string DrumPSVName
        {
            get
            {
                return this._DrumPSVName;
            }
            set
            {
                this._DrumPSVName = value;
                NotifyPropertyChanged("DrumPSVName");
            }
        }
        private double _DrumPressure;
        public double DrumPressure
        {
            get
            {
                return this._DrumPressure;
            }
            set
            {
                this._DrumPressure = value;
                NotifyPropertyChanged("DrumPressure");
            }
        }
        private string _DrumPressureUnit;
        public string DrumPressureUnit
        {
            get
            {
                return this._DrumPressureUnit;
            }
            set
            {
                this._DrumPressureUnit = value;
                NotifyPropertyChanged("DrumPressureUnit");
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                NotifyPropertyChanged("Description");
            }
        }
        private string locationDescription;
        public string LocationDescription
        {
            get { return description; }
            set
            {
                locationDescription = value;
                NotifyPropertyChanged("LocationDescription");
            }
        }
        private string location;
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                NotifyPropertyChanged("Location");
            }
        }

        private double _CriticalPressure;
        public double CriticalPressure
        {
            get { return _CriticalPressure; }
            set
            {
                _CriticalPressure = value;
                NotifyPropertyChanged("CriticalPressure");
            }
        }
        private double _CriticalPressure_Color;
        public double CriticalPressure_Color
        {
            get { return _CriticalPressure_Color; }
            set
            {
                _CriticalPressure_Color = value;
                NotifyPropertyChanged("CriticalPressure_Color");
            }
        }

        private string _PSVName_Color;
        public string PSVName_Color
        {
            get
            {
                return this._PSVName_Color;
            }
            set
            {
                this._PSVName_Color = value;
                NotifyPropertyChanged("PSVName_Color");
            }
        }
        private string _DrumPressure_Color;
        public string DrumPressure_Color
        {
            get
            {
                return this._DrumPressure_Color;
            }
            set
            {
                this._DrumPressure_Color = value;
                NotifyPropertyChanged("DrumPressure_Color");
            }
        }

        private string _ReliefPressureFactor_Color;
        public string ReliefPressureFactor_Color
        {
            get
            {
                return this._ReliefPressureFactor_Color;
            }
            set
            {
                this._ReliefPressureFactor_Color = value;
                NotifyPropertyChanged("ReliefPressureFactor_Color");
            }
        }

        private string _Pressure_Color;
        public string Pressure_Color
        {
            get
            {
                return this._Pressure_Color;
            }
            set
            {
                this._Pressure_Color = value;
                NotifyPropertyChanged("Pressure_Color");
            }
        }

        private string psvPressureUnit;
        public string PSVPressureUnit
        {
            get { return psvPressureUnit; }
            set
            {
                psvPressureUnit = value;
                OnPropertyChanged("PSVPressureUnit");
            }
        }

       


        private string _CriticalPressureUnit;
        public string CriticalPressureUnit
        {
            get { return _CriticalPressureUnit; }
            set
            {
                _CriticalPressureUnit = value;
                OnPropertyChanged("CriticalPressureUnit");
            }
        }
    }
}
