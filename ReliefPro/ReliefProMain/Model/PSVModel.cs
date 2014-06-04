using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;

namespace ReliefProMain.Model
{
    public class PSVModel : ModelBase
    {
        public PSVModel()
        {
            PSVName = "PSV1";
            ValveNumber = "2";
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

        private string _ValveNumber;
        public string ValveNumber
        {
            get
            {
                return this._ValveNumber;
            }
            set
            {
                this._ValveNumber = value;
                int number = int.Parse(_ValveNumber);
                if (number == 1)
                    ReliefPressureFactor = "1.0";
                else
                    ReliefPressureFactor = "1.16";
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
        private string _Pressure;
        public string Pressure
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
        private string _ReliefPressureFactor;
        public string ReliefPressureFactor
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
        private string _DrumPressure;
        public string DrumPressure
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
    }
}
