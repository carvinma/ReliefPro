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
    public class TowerHXModel : ModelBase
    {
        
        public TowerHXModel()
        {
            
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
        private double? _HeaterDuty;
        public double? HeaterDuty
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
