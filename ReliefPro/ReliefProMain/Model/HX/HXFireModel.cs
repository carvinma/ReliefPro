using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.HX;

namespace ReliefProMain.Model.HX
{
    public class HXFireModel : ModelBase
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



        public HXFire dbmodel { get; set; }
        public HXFireModel(HXFire model)
        {
            dbmodel = model;
            this.exposedToFire = model.ExposedToFire;
            this.type = model.Type;
            this.oD = model.OD;
            this.length = model.Length;
            this.elevation = model.Elevation;
            this.pipingContingency = model.PipingContingency;
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

        private double oD;
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
        public double PipingContingency
        {
            get { return pipingContingency; }
            set
            {
                pipingContingency = value;
                this.NotifyPropertyChanged("PipingContingency");
            }
        }
    }
}
