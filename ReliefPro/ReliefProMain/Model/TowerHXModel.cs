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

        private ObservableCollection<TowerHXDetailModel> _Details = null;
        public ObservableCollection<TowerHXDetailModel> Details
        {
            get { return _Details; }
            set
            {
                _Details = value;
                NotifyPropertyChanged("Details");
            }
        }
        internal ObservableCollection<TowerHXDetailModel> GetTowerHXDetails()
        {
            _Details = new ObservableCollection<TowerHXDetailModel>();
            using (var helper = new NHibernateHelper(_dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbTowerHXDetail db = new dbTowerHXDetail();
                int i = 0;
                foreach (var obj in db.GetAllList(Session, ID))
                {
                    TowerHXDetailModel d = new TowerHXDetailModel();
                    d.Parent = this;
                    d.SeqNumber = i;
                    d.DetailName = obj.DetailName;
                    d.ProcessSideFlowSource = obj.ProcessSideFlowSource;
                    d.Medium = obj.Medium;
                    d.MediumSideFlowSource = obj.MediumSideFlowSource;
                    d.ID = obj.ID;
                    d.HXID = obj.HXID;
                    d.Duty = obj.Duty;
                    d.DutyPercentage = obj.DutyPercentage;

                    _Details.Add(d);
                    i = i + 1;

                }
            }
            return _Details;
        }

        private TowerHXDetailModel _SelectedDetail;
        public TowerHXDetailModel SelectedDetail
        {
            get
            {
                return this._SelectedDetail;
            }
            set
            {
                this._SelectedDetail = value;
                NotifyPropertyChanged("SelectedDetail");
            }
        }
    }
}
