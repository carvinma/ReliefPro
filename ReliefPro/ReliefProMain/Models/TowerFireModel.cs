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
        public TowerFire model { set; get; }
        public TowerFireModel(TowerFire m)
        {
            model = m;
        }

        public string HeatInputModel
        {
            get
            {
                return model.HeatInputModel;
            }
            set
            {
                if (model.HeatInputModel != value)
                {
                    model.HeatInputModel = value;
                    NotifyPropertyChanged("HeatInputModel");
                }
            }
        }
        public int ID
        {
            get
            {
                return model.ID;
            }
            set
            {
                if (model.ID != value)
                {
                    model.ID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        public bool IsExist
        {
            get
            {
                return model.IsExist;
            }
            set
            {
                if (model.IsExist != value)
                {
                    model.IsExist = value;
                    NotifyPropertyChanged("IsExist");
                }
            }
        }

        public double? ReliefLoad
        {
            get
            {
                return model.ReliefLoad;
            }
            set
            {
                if (model.ReliefLoad != value)
                {
                    model.ReliefLoad = value;
                    NotifyPropertyChanged("ReliefLoad");
                }
            }
        }

        public double? ReliefTemperature
        {
            get
            {
                return model.ReliefTemperature;
            }
            set
            {
                if (model.ReliefTemperature != value)
                {
                    model.ReliefTemperature = value;
                    NotifyPropertyChanged("ReliefTemperature");
                }
            }
        }

        public double? ReliefPressure
        {
            get
            {
                return model.ReliefPressure;
            }
            set
            {
                if (model.ReliefPressure != value)
                {
                    model.ReliefPressure = value;
                    NotifyPropertyChanged("ReliefPressure");
                }
            }
        }

        public double? ReliefMW
        {
            get
            {
                return model.ReliefMW;
            }
            set
            {
                if (model.ReliefMW != value)
                {
                    model.ReliefMW = value;
                    NotifyPropertyChanged("ReliefMW");
                }
            }
        }

        public double? ReliefCpCv
        {
            get
            {
                return model.ReliefCpCv;
            }
            set
            {
                if (model.ReliefCpCv != value)
                {
                    model.ReliefCpCv = value;
                    NotifyPropertyChanged("ReliefCpCv");
                }
            }
        }

        public double? ReliefZ
        {
            get
            {
                return model.ReliefZ;
            }
            set
            {
                if (model.ReliefZ != value)
                {
                    model.ReliefZ= value;
                    NotifyPropertyChanged("ReliefZ");
                }
            }
        }
    }
}
