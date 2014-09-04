using ReliefProModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProMain.Models
{
    public class TowerFireCoolerModel:ModelBase
    {      
        public TowerFireCooler dbmodel;
        public TowerFireCoolerModel(TowerFireCooler model)
        {
            dbmodel = model;
            this._ID = model.ID;
            this._PipingContingency = model.PipingContingency;
            this._WettedArea = model.WettedArea;
            this._EqID = model.EqID;
            this._WettedArea_Color = model.WettedArea_Color;
            this._PipingContingency_Color = model.PipingContingency_Color;
        }

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                NotifyPropertyChanged("ID");
            }
        }

        private int _EqID;
        public int EqID
        {
            get { return _EqID; }
            set
            {
                _EqID = value;
                NotifyPropertyChanged("EqID");
            }
        }

        private double _WettedArea;
        public double WettedArea
        {
            get { return _WettedArea; }
            set
            {
                _WettedArea = value;
                NotifyPropertyChanged("WettedArea");
            }
        }

        private double _PipingContingency;
        public double PipingContingency
        {
            get { return _PipingContingency; }
            set
            {
                _PipingContingency = value;
                NotifyPropertyChanged("_PipingContingency");
            }
        }

        private string _WettedArea_Color;
        public string WettedArea_Color
        {
            get { return _WettedArea_Color; }
            set
            {
                _WettedArea_Color = value;
                NotifyPropertyChanged("WettedArea_Color");
            }
        }

        private string _PipingContingency_Color;
        public string PipingContingency_Color
        {
            get { return _PipingContingency_Color; }
            set
            {
                _PipingContingency_Color = value;
                NotifyPropertyChanged("PipingContingency_Color");
            }
        }
    }
}
