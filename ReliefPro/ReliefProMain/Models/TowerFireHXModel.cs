using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;

namespace ReliefProMain.Models
{
    public class TowerFireHXModel : ModelBase
    {
        private string oDUnit;
        public string ODUnit
        {
            get { return oDUnit; }
            set
            {
                oDUnit = value;
                NotifyPropertyChanged("ODUnit");
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



        public TowerFireHX dbmodel { get; set; }
        public TowerFireHXModel(TowerFireHX model)
        {
            dbmodel = model;
            this.exposedToFire = model.ExposedToFire;
            this.type = model.Type;
            this.oD = model.OD;
            this.length = model.Length;
            this.elevation = model.Elevation;
            this.pipingContingency = model.PipingContingency;

            this.exposedToFire_Color = model.ExposedToFire_Color;
            this.type_Color = model.Type_Color;
            this.oD_Color = model.OD_Color;
            this.length_Color = model.Length_Color;
            this.elevation_Color = model.Elevation_Color;
            this.pipingContingency_Color = model.PipingContingency_Color;
        }

       
        private string type;
        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                this.NotifyPropertyChanged("Type");
            }
        }

        private string exposedToFire;
        public string ExposedToFire
        {
            get { return exposedToFire; }
            set
            {
                exposedToFire = value;
                NotifyPropertyChanged("ExposedToFire");
            }
        }
        private double oD;
        [ReliefProMain.Util.Required(ErrorMessage = "ODWarning")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double OD
        {
            get { return oD; }
            set
            {
                oD = value;
                NotifyPropertyChanged("OD");
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

        private double pipingContingency;
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double PipingContingency
        {
            get { return pipingContingency; }
            set
            {
                pipingContingency = value;
                this.NotifyPropertyChanged("PipingContingency");
            }
        }


        private string type_Color;
        public string Type_Color
        {
            get { return type_Color; }
            set
            {
                type_Color = value;
                this.NotifyPropertyChanged("Type_Color");
            }
        }
        private string exposedToFire_Color;
        public string ExposedToFire_Color
        {
            get { return exposedToFire_Color; }
            set
            {
                exposedToFire_Color = value;
                NotifyPropertyChanged("ExposedToFire_Color");
            }
        }
        private string oD_Color;
        public string _Color
        {
            get { return oD_Color; }
            set
            {
                oD_Color = value;
                NotifyPropertyChanged("OD_Color");
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

        private string pipingContingency_Color;
        public string PipingContingency_Color
        {
            get { return pipingContingency_Color; }
            set
            {
                pipingContingency_Color = value;
                this.NotifyPropertyChanged("PipingContingency_Color");
            }
        }


        
    }
}
