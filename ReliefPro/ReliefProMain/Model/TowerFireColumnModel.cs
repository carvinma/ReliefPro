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
    public class TowerFireColumnModel : ModelBase
    {
        private ObservableCollection<TowerFireColumnDetail> _Details;
        public ObservableCollection<TowerFireColumnDetail> Details
        {
            get
            {
                return this._Details;
            }
            set
            {
                this._Details = value;
                NotifyPropertyChanged("Details");
            }
        }

        public string NumberOfSegment
        {
            get
            {
                return this.Instance.NumberOfSegment;
            }
            set
            {
                this.Instance.NumberOfSegment = value;
                _Details.Clear();
                int count = int.Parse(this.Instance.NumberOfSegment);
                for (int i = 0; i < count; i++)
                {
                    TowerFireColumnDetail detail=new TowerFireColumnDetail();
                    detail.Segment = (i + 1).ToString();
                    detail.ColumnID = Instance.ID;
                    _Details.Add(detail);
                }
                Instance.NumberOfSegment = this.Instance.NumberOfSegment;
                NotifyPropertyChanged("NumberOfSegment");
                NotifyPropertyChanged("Details");

            }
        }
        private TowerFireColumn _Instance;
        public TowerFireColumn Instance
        {
            get
            {
                return this._Instance;
            }
            set
            {
                this._Instance = value;
                NotifyPropertyChanged("Instance");
            }
        }
    }
}
