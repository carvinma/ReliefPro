using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProCommon.Enum;
using ReliefProModel;
using NHibernate;
using ReliefProDAL;
using Remotion.Linq.Collections;

namespace ReliefProMain.Models
{
    public class PSVModel : ModelBase
    {
        public PSV dbmodel;
        public ISession SessionPlant;
        public string sourceFile;
        public PSVModel(PSV m, ISession SessionPlant, string sourceFile)
        {
            this.SessionPlant = SessionPlant;
            dbmodel= m;
            this.sourceFile = sourceFile;
            this.ID = m.ID;
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
            this.CriticalPressure = m.CriticalPressure;
            this.PSVName_Color = m.PSVName_Color;
            this.ValveNumber_Color = m.ValveNumber_Color;
            this.DrumPSVName_Color = m.DrumPSVName_Color;
            this.DrumPressure_Color = m.DrumPressure_Color;

            this._ReliefPressureFactor_Color = m.ReliefPressureFactor_Color;
            this.Pressure_Color = m.Pressure_Color;

            this.DischargeTo_Color = m.DischargeTo_Color;
            this.ValveType_Color = m.ValveType_Color;
            this.Location_Color = m.Location_Color;

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
                if(string.IsNullOrEmpty(value))
                {
                    PSVName_Color = ColorBorder.red.ToString();
                }
                else if (this._PSVName != value && !string.IsNullOrEmpty(value))
                {
                    PSVName_Color = ColorBorder.blue.ToString();
                }
                
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
                if(this._ValveNumber != value)
                {
                    ValveNumber_Color=ColorBorder.blue.ToString();
                }
                this._ValveNumber = value;
                int number = _ValveNumber;
                if (number == 1)
                    ReliefPressureFactor = 1.1;
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
                if (this._ValveType != value && value != null)
                {
                    this.ValveType_Color = ColorBorder.blue.ToString();
                }
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
                if (this._DischargeTo != value && value!=null)
                {
                    this.DischargeTo_Color = ColorBorder.blue.ToString();
                }

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
                if (value==0)
                {
                    Pressure_Color = ColorBorder.red.ToString();
                }
                this._Pressure = value;
                NotifyPropertyChanged("Pressure");
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
                if (this._ReliefPressureFactor != value)
                {
                    ReliefPressureFactor_Color = ColorBorder.blue.ToString();
                }
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
                if (this._DrumPSVName != value)
                {
                    DrumPSVName_Color = ColorBorder.blue.ToString();
                }
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
                if (this._DrumPressure != value)
                {
                    DrumPressure_Color = ColorBorder.blue.ToString();
                }
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
            get { return locationDescription; }
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
                if (location != value && value != null)
                {
                    Location_Color = ColorBorder.blue.ToString();
                }
                location = value;

                if (!string.IsNullOrEmpty(location))
                {
                    ProIIEqDataDAL proiieqdal = new ProIIEqDataDAL();
                    ProIIEqData data = proiieqdal.GetModel(SessionPlant, sourceFile, location);
                    if (data.EqType == "Hx")
                    {
                        LocationDescriptions = GetLocationDescriptions("HX");
                        
                    }
                    else
                    {
                        LocationDescriptions = GetLocationDescriptions("");
                    }
                    LocationDescription = LocationDescriptions[0];
                }



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

        private double _CriticalTemperature;
        public double CriticalTemperature
        {
            get { return _CriticalTemperature; }
            set
            {
                _CriticalTemperature = value;
                NotifyPropertyChanged("CriticalTemperature");
            }
        }
        private double _CriticalTemperature_Color;
        public double CriticalTemperature_Color
        {
            get { return _CriticalTemperature_Color; }
            set
            {
                _CriticalTemperature_Color = value;
                NotifyPropertyChanged("CriticalTemperature_Color");
            }
        }

        private double _CricondenbarPress;
        public double CricondenbarPress
        {
            get { return _CricondenbarPress; }
            set
            {
                _CricondenbarPress = value;
                NotifyPropertyChanged("CricondenbarPress");
            }
        }
        private double _CricondenbarPress_Color;
        public double CricondenbarPress_Color
        {
            get { return _CricondenbarPress_Color; }
            set
            {
                _CricondenbarPress_Color = value;
                NotifyPropertyChanged("CricondenbarPress_Color");
            }
        }

        private double _CricondenbarTemp;
        public double CricondenbarTemp
        {
            get { return _CricondenbarTemp; }
            set
            {
                _CricondenbarTemp = value;
                NotifyPropertyChanged("CricondenbarTemp");
            }
        }
        private double _CricondenbarTemp_Color;
        public double CricondenbarTemp_Color
        {
            get { return _CricondenbarTemp_Color; }
            set
            {
                _CricondenbarTemp_Color = value;
                NotifyPropertyChanged("CricondenbarTemp_Color");
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


        private string _CriticalTemperatureUnit;
        public string CriticalTemperatureUnit
        {
            get { return _CriticalTemperatureUnit; }
            set
            {
                _CriticalTemperatureUnit = value;
                OnPropertyChanged("CriticalTemperatureUnit");
            }
        }

        private string _CricondenbarPressUnit;
        public string CricondenbarPressUnit
        {
            get { return _CricondenbarPressUnit; }
            set
            {
                _CricondenbarPressUnit = value;
                OnPropertyChanged("CricondenbarPressUnit");
            }
        }


        private string _CricondenbarTempUnit;
        public string CricondenbarTempUnit
        {
            get { return _CricondenbarTempUnit; }
            set
            {
                _CricondenbarTempUnit = value;
                OnPropertyChanged("CricondenbarTempUnit");
            }
        }

        private string _ValveNumber_Color;
        public string ValveNumber_Color
        {
            get
            {
                return this._ValveNumber_Color;
            }
            set
            {
                this._ValveNumber_Color = value;
                NotifyPropertyChanged("ValveNumber_Color");
            }
        }

        private string _DrumPSVName_Color;
        public string DrumPSVName_Color
        {
            get
            {
                return this._DrumPSVName_Color;
            }
            set
            {
                this._DrumPSVName_Color = value;
                NotifyPropertyChanged("DrumPSVName_Color");
            }
        }


        private string _DischargeTo_Color;
        public string DischargeTo_Color
        {
            get
            {
                return this._DischargeTo_Color;
            }
            set
            {
                this._DischargeTo_Color = value;
                NotifyPropertyChanged("DischargeTo_Color");
            }
        }

        private string _ValveType_Color;
        public string ValveType_Color
        {
            get
            {
                return this._ValveType_Color;
            }
            set
            {
                this._ValveType_Color = value;
                NotifyPropertyChanged("ValveType_Color");
            }
        }



        private string _Location_Color;
        public string Location_Color
        {
            get
            {
                return this._Location_Color;
            }
            set
            {
                this._Location_Color = value;
                NotifyPropertyChanged("Location_Color");
            }
        }
        private ObservableCollection<string> locationDescriptions;
        public ObservableCollection<string> LocationDescriptions 
        { 
            get
            {
                return this.locationDescriptions;
            }
            set
            {
                this.locationDescriptions = value;
                NotifyPropertyChanged("LocationDescriptions");
            }
        }
        public ObservableCollection<string> GetLocationDescriptions(string eqType)
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            if (eqType == "HX")
            {
                list.Add("Shell");
                list.Add("Tube");
            }
            else
            {
                list.Add("Top");
                list.Add("Bottom");
            }
            return list;
        }

    }
}
