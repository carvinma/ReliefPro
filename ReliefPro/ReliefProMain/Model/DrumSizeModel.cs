using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.Drums;

namespace ReliefProMain.Model
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

        public DrumSize dbmodel { get; set; }

        public DrumSizeModel(DrumSize sizemodel)
        {
            dbmodel = sizemodel;
            this.orientation = sizemodel.Orientation;
            this.headnumber = sizemodel.HeadNumber;
            this.headType = sizemodel.HeadType;
            this.elevation = sizemodel.Elevation;
            this.diameter = sizemodel.Diameter;
            this.length = sizemodel.Length;
            this.normalLiquidLevel = sizemodel.NormalLiquidLevel;
            this.bootDiameter = sizemodel.BootDiameter;
            this.bootHeight = sizemodel.BootHeight;
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

        public double diameter;
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

        public double normalLiquidLevel;
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

        public double bootHeight;
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
                this.NotifyPropertyChanged("Orientation");
            }
        }

        public double headnumber;
        public double Headnumber
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

    }
}
