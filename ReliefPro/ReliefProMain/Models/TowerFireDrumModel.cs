using ReliefProModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProMain.Models
{
    public class TowerFireDrumModel:ModelBase
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

        public TowerFireDrum dbmodel { get; set; }

        public TowerFireDrumModel(TowerFireDrum sizemodel)
        {
            dbmodel = sizemodel;
            this.orientation = sizemodel.Orientation;
            this.headNumber = sizemodel.HeadNumber;
            this.headType = sizemodel.HeadType;
            this.elevation = sizemodel.Elevation;
            this.diameter = sizemodel.Diameter;
            this.length = sizemodel.Length;
            this.normalLiquidLevel = sizemodel.NormalLiquidLevel;
            this.bootDiameter = sizemodel.BootDiameter;
            this.bootHeight = sizemodel.BootHeight;
            this.PipingContingency = sizemodel.PipingContingency;
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
        [ReliefProMain.Util.Required(ErrorMessage = "ElevationWarning")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
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
        [ReliefProMain.Util.Required(ErrorMessage = "DiameterWarning")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
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
        [ReliefProMain.Util.Required(ErrorMessage = "LengthWarning")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
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
        [ReliefProMain.Util.Required(ErrorMessage = "NormalLiquidLevelWarning")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
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

        public int headNumber;
        public int HeadNumber
        {
            get { return headNumber; }
            set
            {
                headNumber = value;
                this.NotifyPropertyChanged("HeadNumber");
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
        private double _PipingContingency;
        [ReliefProMain.Util.Required(ErrorMessage = "PipingContingencyWarning")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double PipingContingency
        {
            get
            {
                return this._PipingContingency;
            }
            set
            {
                this._PipingContingency = value;
                NotifyPropertyChanged("PipingContingency");
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

        public string diameter_Color;
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

        public string normalLiquidLevel_Color;
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

        public string bootHeight_Color;
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

        public string headnumber_Color;
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
        private string pipingContingency_Color;
        public string PipingContingency_Color
        {
            get
            {
                return this.pipingContingency_Color;
            }
            set
            {
                this.pipingContingency_Color = value;
                NotifyPropertyChanged("PipingContingency_Color");

            }
        }
    }
}
