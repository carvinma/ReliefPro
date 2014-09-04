using ReliefProModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProMain.Models
{
    public class TowerFireEqModel:ModelBase
    {
        public TowerFireEq dbmodel;
        public TowerFireEqModel(TowerFireEq model)
        {
            dbmodel = model;
            this._ID = model.ID;
            this._Elevation = model.Elevation;
            this._EqName = model.EqName;
            this._FFactor = model.FFactor;
            this._FireID = model.FireID;
            this._FireZone = model.FireZone;
            this._HeatInput = model.HeatInput;
            this._Type = model.Type;
            this._WettedArea = model.WettedArea;
            this._ReliefLoad = model.ReliefLoad;

            this._FFactor_Color = model.FFactor_Color;
        }

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                this.NotifyPropertyChanged("ID");
            }
        }
        private string _EqName;
        public string EqName
        {
            get { return _EqName; }
            set
            {
                _EqName = value;
                this.NotifyPropertyChanged("EqName");
            }
        }



        private string _Type;
        public string Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
                this.NotifyPropertyChanged("Type");
            }
        }
        private double _Elevation;
        public double Elevation
        {
            get { return _Elevation; }
            set
            {
                _Elevation = value;
                this.NotifyPropertyChanged("Elevation");
            }
        }
        private int _FireID;
        public int FireID
        {
            get { return _FireID; }
            set
            {
                _FireID = value;
                this.NotifyPropertyChanged("FireID");
            }
        }
        

        private double _FFactor;
        public double FFactor
        {
            get { return _FFactor; }
            set
            {
                _FFactor = value;
                this.NotifyPropertyChanged("FFactor");
            }
        }
        private double _WettedArea;
        public double WettedArea
        {
            get { return _WettedArea; }
            set
            {
                _WettedArea = value;
                this.NotifyPropertyChanged("WettedArea");
            }
        }
        private double _HeatInput;
        public double HeatInput
        {
            get { return _HeatInput; }
            set
            {
                _HeatInput = value;
                this.NotifyPropertyChanged("HeatInput");
            }
        }
        private double _ReliefLoad;
        public double ReliefLoad
        {
            get { return _ReliefLoad; }
            set
            {
                _ReliefLoad = value;
                this.NotifyPropertyChanged("ReliefLoad");
            }
        }
        private bool _FireZone;
        public bool FireZone
        {
            get { return _FireZone; }
            set
            {
                _FireZone = value;
                this.NotifyPropertyChanged("FireZone");
            }
        }

        private string _FFactor_Color;
        public string FFactor_Color
        {
            get { return _FFactor_Color; }
            set
            {
                _FFactor_Color = value;
                this.NotifyPropertyChanged("FFactor_Color");
            }
        }
    }
}
