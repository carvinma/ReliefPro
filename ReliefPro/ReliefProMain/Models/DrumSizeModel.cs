﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.Drums;
using System.Collections.ObjectModel;

namespace ReliefProMain.Models
{

    public class DrumSizeModel : ModelBase
    {
        private string elevationUnit;
        public string ElevationUnit
        {
            get { return elevationUnit; }
            set
            {
                elevationUnit = value;
                this.NotifyPropertyChanged("ElevationUnit");
            }
        }

        public string diameterUnit;
        public string DiameterUnit
        {
            get { return diameterUnit; }
            set
            {
                diameterUnit = value;
                this.NotifyPropertyChanged("DiameterUnit");
            }
        }
        private string lengthUnit;
        public string LengthUnit
        {
            get { return lengthUnit; }
            set
            {
                lengthUnit = value;
                this.NotifyPropertyChanged("LengthUnit");
            }
        }

        public string normalLiquidLevelUnit;
        public string NormalLiquidLevelUnit
        {
            get { return normalLiquidLevelUnit; }
            set
            {
                normalLiquidLevelUnit = value;
                this.NotifyPropertyChanged("NormalLiquidLevelUnit");
            }
        }
        private string bootDiameterUnit;
        public string BootDiameterUnit
        {
            get { return bootDiameterUnit; }
            set
            {
                bootDiameterUnit = value;
                this.NotifyPropertyChanged("BootDiameterUnit");
            }
        }

        public string bootHeightUnit;
        public string BootHeightUnit
        {
            get { return bootHeightUnit; }
            set
            {
                bootHeightUnit = value;
                this.NotifyPropertyChanged("BootHeightUnit");
            }
        }

        public bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                this.NotifyPropertyChanged("IsEnabled");
            }
        }

        public DrumSize dbmodel { get; set; }

        public DrumSizeModel(DrumSize sizemodel)
        {
            dbmodel = sizemodel;
            LstHeadType = new ObservableCollection<string>();
            LstOrientation = new ObservableCollection<string>();
            GetOrientations();
            this.Orientation = sizemodel.Orientation;
            this.headnumber = sizemodel.HeadNumber;
            this.HeadType = sizemodel.HeadType;
            this.elevation = sizemodel.Elevation;
            this.diameter = sizemodel.Diameter;
            this.length = sizemodel.Length;
            this.normalLiquidLevel = sizemodel.NormalLiquidLevel;
            this.bootDiameter = sizemodel.BootDiameter;
            this.bootHeight = sizemodel.BootHeight;

            this.orientation_Color = sizemodel.Orientation_Color;
            this.headnumber_Color = sizemodel.HeadNumber_Color;
            this.headType_Color = sizemodel.HeadType_Color;
            this.elevation_Color = sizemodel.Elevation_Color;
            this.diameter_Color = sizemodel.Diameter_Color;
            this.length_Color = sizemodel.Length_Color;
            this.normalLiquidLevel_Color = sizemodel.NormalLiquidLevel_Color;
            this.bootDiameter_Color = sizemodel.BootDiameter_Color;
            this.bootHeight_Color = sizemodel.BootHeight_Color;

        }
        private double elevation;

        public double Elevation
        {
            get { return elevation; }
            set
            {
                elevation = value;
                this.NotifyPropertyChanged("Elevation");
            }
        }

        private double diameter;

        public double Diameter
        {
            get { return diameter; }
            set
            {
                diameter = value;
                this.NotifyPropertyChanged("Diameter");
            }
        }
        private double length;

        public double Length
        {
            get { return length; }
            set
            {
                length = value;
                this.NotifyPropertyChanged("Length");
            }
        }

        private double normalLiquidLevel;
        public double NormalLiquidLevel
        {
            get { return normalLiquidLevel; }
            set
            {
                normalLiquidLevel = value;
                this.NotifyPropertyChanged("NormalLiquidLevel");
            }
        }
        private double bootDiameter;        
        public double BootDiameter
        {
            get { return bootDiameter; }
            set
            {
                bootDiameter = value;
                this.NotifyPropertyChanged("BootDiameter");
            }
        }

        private double bootHeight;       
        public double BootHeight
        {
            get { return bootHeight; }
            set
            {
                bootHeight = value;
                this.NotifyPropertyChanged("BootHeight");
            }
        }


        private string orientation;
        public string Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                LstHeadType.Clear();
                if(orientation=="Vertical")
                {
                    LstHeadType.Add("Eclipse");
                    LstHeadType.Add("Flat");
                    IsEnabled = true;
                    this.HeadType = LstHeadType[0] ;
                }
                else  if(orientation=="Horizontal")
                {
                    LstHeadType.Add("Eclipse");
                    LstHeadType.Add("Sphere");
                    IsEnabled = true;
                    this.HeadType = LstHeadType[0];
                }
                else
                {
                    LstHeadType.Add("");
                    IsEnabled = false;
                    this.HeadType = LstHeadType[0];
                }
                this.NotifyPropertyChanged("Orientation");
            }
        }

        private int headnumber;
        [ReliefProMain.Util.RegularExpression(ModelBase.IsNum, ErrorMessage = "GreaterThanZero")]
        public int Headnumber
        {
            get { return headnumber; }
            set
            {
                headnumber = value;
                this.NotifyPropertyChanged("Headnumber");
            }
        }

        private string headType;
        public string HeadType
        {
            get { return headType; }
            set
            {
                headType = value;
                this.NotifyPropertyChanged("HeadType");
            }
        }



        private string elevation_Color;
        public string Elevation_Color
        {
            get { return elevation_Color; }
            set
            {
                elevation_Color = value;
                this.NotifyPropertyChanged("Elevation_Color");
            }
        }

        private string diameter_Color;
        public string Diameter_Color
        {
            get { return diameter_Color; }
            set
            {
                diameter_Color = value;
                this.NotifyPropertyChanged("Diameter_Color");
            }
        }
        private string length_Color;
        public string Length_Color
        {
            get { return length_Color; }
            set
            {
                length_Color = value;
                this.NotifyPropertyChanged("Length_Color");
            }
        }

        private string normalLiquidLevel_Color;
        public string NormalLiquidLevel_Color
        {
            get { return normalLiquidLevel_Color; }
            set
            {
                normalLiquidLevel_Color = value;
                this.NotifyPropertyChanged("NormalLiquidLevel_Color");
            }
        }
        private string bootDiameter_Color;
        public string BootDiameter_Color
        {
            get { return bootDiameter_Color; }
            set
            {
                bootDiameter_Color = value;
                this.NotifyPropertyChanged("BootDiameter_Color");
            }
        }

        private string bootHeight_Color;
        public string BootHeight_Color
        {
            get { return bootHeight_Color; }
            set
            {
                bootHeight_Color = value;
                this.NotifyPropertyChanged("BootHeight_Color");
            }
        }


        private string orientation_Color;
        public string Orientation_Color
        {
            get { return orientation_Color; }
            set
            {
                orientation_Color = value;
                this.NotifyPropertyChanged("Orientation_Color");
            }
        }

        private string headnumber_Color;
        public string Headnumber_Color
        {
            get { return headnumber_Color; }
            set
            {
                headnumber_Color = value;
                this.NotifyPropertyChanged("Headnumber_Color");
            }
        }

        private string headType_Color;
        public string HeadType_Color
        {
            get { return headType_Color; }
            set
            {
                headType_Color = value;
                this.NotifyPropertyChanged("HeadType_Color");
            }
        }

        public ObservableCollection <string> lstOrientation;
        public ObservableCollection <string> LstOrientation
        {
            get { return lstOrientation; }
            set
            {
                lstOrientation = value;
                this.NotifyPropertyChanged("LstOrientation");
            }
        }

        public ObservableCollection <string> lstHeadType;
        public ObservableCollection <string> LstHeadType
        {
            get { return lstHeadType; }
            set
            {
                lstHeadType = value;
                this.NotifyPropertyChanged("LstHeadType");
            }
        }


        private void GetOrientations()
        {
            lstOrientation.Add("Vertical");
            lstOrientation.Add("Horizontal");
            lstOrientation.Add("Spherical");
        }

        

     
            
    }
}
