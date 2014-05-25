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


namespace ReliefProMain.Model
{     
    public class TowerHXModel : ModelBase
    {
        private string _dbProtectedSystemFile;
        public TowerHXModel(string dbProtectedSystemFile)
        {
            _dbProtectedSystemFile = dbProtectedSystemFile;
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

        private string _HeaterName;
        public string HeaterName
        {
            get
            {
                return this._HeaterName;
            }
            set
            {
                this._HeaterName = value;
                NotifyPropertyChanged("HeaterName");
            }
        }
        private string _Description;
        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
                NotifyPropertyChanged("Description");
            }
        }
        private string _HeaterDuty;
        public string HeaterDuty
        {
            get
            {
                return this._HeaterDuty;
            }
            set
            {
                this._HeaterDuty = value;
                NotifyPropertyChanged("HeaterDuty");
            }
        }

        private int _HeaterType;
        public int HeaterType
        {
            get
            {
                return this._HeaterType;
            }
            set
            {
                this._HeaterType = value;
                NotifyPropertyChanged("HeaterType");
            }
        }

        
    }
}
