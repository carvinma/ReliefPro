using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;
using System.Collections.ObjectModel;

namespace ReliefProMain.Models
{
    public class TowerFireColumnDetailModel: ModelBase
    {
        public ObservableCollection<string> Internals { get; set; }
        public TowerFireColumnDetail dbmodel;
        public TowerFireColumnDetailModel(TowerFireColumnDetail model)
        {
            Internals = getInternals();
            dbmodel = model;
            this._ID = model.ID;
            this._ColumnID = model.ColumnID;
            this._Internal = model.Internal;
            this._Internal = model.Internal;
            this._Diameter = model.Diameter;
            this._Height = model.Height;
            this._Trays = model.Trays;
            this._Trays_Color = model.Trays_Color;
            this._Height_Color = model.Height_Color;
            this._Diameter_Color = model.Diameter_Color;
            this._Segment = model.Segment;
            this._Internal = model.Internal;
        }

        private int _Segment;
        public int Segment
        {
            get
            {
                return this._Segment;
            }
            set
            {
                this._Segment = value;
                NotifyPropertyChanged("Segment");
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

        private int _ColumnID;
        public int ColumnID
        {
            get
            {
                return this._ColumnID;
            }
            set
            {
                this._ColumnID = value;
                NotifyPropertyChanged("ColumnID");
            }
        }
        
        private string _Internal;
        public string Internal
        {
            get
            {
                return this._Internal;
            }
            set
            {
                this._Internal = value;
                NotifyPropertyChanged("Internal");
            }
        }

        private double _Diameter;
        [ReliefProMain.Util.Required(ErrorMessage = "DiameterWarning")]
        [ReliefProMain.Util.RegularExpression(ModelBase.IsNum, ErrorMessage = "GreaterThanZero")]
        public double Diameter
        {
            get
            {
                return this._Diameter;
            }
            set
            {
                this._Diameter = value;
                NotifyPropertyChanged("Diameter");
            }
        }
        private double _Height;
        [ReliefProMain.Util.Required(ErrorMessage = "HeightWarning")]
        [ReliefProMain.Util.RegularExpression(ModelBase.IsNum, ErrorMessage = "GreaterThanZero")]
        public double Height
        {
            get
            {
                return this._Height;
            }
            set
            {
                this._Height = value;
                NotifyPropertyChanged("Height");
            }
        }
        private int _Trays;
        [ReliefProMain.Util.Required(ErrorMessage = "TraysWarning")]
        [ReliefProMain.Util.RegularExpression(ModelBase.IsNum, ErrorMessage = "GreaterThanZero")]
        public int Trays
        {
            get
            {
                return this._Trays;
            }
            set
            {
                this._Trays = value;
                NotifyPropertyChanged("Trays");
            }
        }

        private string _Diameter_Color;
        public string Diameter_Color
        {
            get
            {
                return this._Diameter_Color;
            }
            set
            {
                this._Diameter_Color = value;
                NotifyPropertyChanged("Diameter_Color");

            }
        }

        private string _Height_Color;
        public string Height_Color
        {
            get
            {
                return this._Height_Color;
            }
            set
            {
                this._Height_Color = value;
                NotifyPropertyChanged("Height_Color");

            }
        }

        private string _Trays_Color;
        public string Trays_Color
        {
            get
            {
                return this._Trays_Color;
            }
            set
            {
                this._Trays_Color = value;
                NotifyPropertyChanged("Trays_Color");

            }
        }
        private ObservableCollection<string> getInternals()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Trayed");
            list.Add("Packed");
            return list;
        }
    }
}
