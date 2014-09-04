using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using NHibernate;
using ReliefProBLL.Common;
using ReliefProDAL;

namespace ReliefProMain.Models
{
    public class TowerFireModel: ModelBase
    {
        public TowerFire dbmodel { set; get; }
        public TowerFireModel(TowerFire model)
        {
            dbmodel = model;
            this._HeatInputModel = model.HeatInputModel;
            this._ID = model.ID;
            this._IsExist = model.IsExist;
            this._ReliefCpCv = model.ReliefCpCv;
            this._ReliefLoad = model.ReliefLoad;
            this._ReliefMW = model.ReliefMW;
            this._ReliefPressure = model.ReliefPressure;
            this._ReliefTemperature = model.ReliefTemperature;
            this._ReliefZ = model.ReliefZ;
        }

        private string _HeatInputModel;
        public string HeatInputModel
        {
            get
            {
                return _HeatInputModel;
            }
            set
            {
                if (_HeatInputModel != value)
                {
                    _HeatInputModel = value;
                    NotifyPropertyChanged("HeatInputModel");
                }
            }
        }
        private int _ID;
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID != value)
                {
                    _ID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        private bool _IsExist;
        public bool IsExist
        {
            get
            {
                return _IsExist;
            }
            set
            {
                if (_IsExist != value)
                {
                    _IsExist = value;
                    NotifyPropertyChanged("IsExist");
                }
            }
        }
        private double _ReliefLoad;
        public double ReliefLoad
        {
            get
            {
                return _ReliefLoad;
            }
            set
            {
                if (_ReliefLoad != value)
                {
                    _ReliefLoad = value;
                    NotifyPropertyChanged("ReliefLoad");
                }
            }
        }

        private double _ReliefTemperature;
        public double ReliefTemperature
        {
            get
            {
                return _ReliefTemperature;
            }
            set
            {
                if (_ReliefTemperature != value)
                {
                    _ReliefTemperature = value;
                    NotifyPropertyChanged("ReliefTemperature");
                }
            }
        }

        private double _ReliefPressure;
        public double ReliefPressure
        {
            get
            {
                return _ReliefPressure;
            }
            set
            {
                if (_ReliefPressure != value)
                {
                    _ReliefPressure = value;
                    NotifyPropertyChanged("ReliefPressure");
                }
            }
        }
        private double _ReliefMW;
        public double ReliefMW
        {
            get
            {
                return _ReliefMW;
            }
            set
            {
                if (_ReliefMW != value)
                {
                    _ReliefMW = value;
                    NotifyPropertyChanged("ReliefMW");
                }
            }
        }
        private double _ReliefCpCv;
        public double ReliefCpCv
        {
            get
            {
                return _ReliefCpCv;
            }
            set
            {
                if (_ReliefCpCv != value)
                {
                    _ReliefCpCv = value;
                    NotifyPropertyChanged("ReliefCpCv");
                }
            }
        }

        private double _ReliefZ;
        public double ReliefZ
        {
            get
            {
                return _ReliefZ;
            }
            set
            {
                if (_ReliefZ != value)
                {
                    _ReliefZ= value;
                    NotifyPropertyChanged("ReliefZ");
                }
            }
        }
    }
}
