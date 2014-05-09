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
    public class TowerFireModel: ModelBase
    {
        private ObservableCollection<TowerFireEq> _EqList;
        public ObservableCollection<TowerFireEq> EqList
        {
            get
            {
                return this._EqList;
            }
            set
            {
                this._EqList = value;                
                NotifyPropertyChanged("EqList");
            }
        }

        private TowerFire _CurrentTowerFire;
        public TowerFire CurrentTowerFire
        {
            get
            {
                return this._CurrentTowerFire;
            }
            set
            {
                this._CurrentTowerFire = value;
                NotifyPropertyChanged("CurrentTowerFire");
            }
        }
    }
}
