using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProMain.Commands;
using System.IO;
using System.Collections.ObjectModel;
using NHibernate;
using ReliefProDAL;
using ReliefProModel;
using ReliefProBLL.Common;

namespace ReliefProMain.ViewModel.Trees
{
    public class PlantVM : ViewModelBase
    {               
        private string _PlantName;
        public string PlantName
        {
            get
            {
                return this._PlantName;
            }
            set
            {
                this._PlantName = value;

                OnPropertyChanged("PlantName");
            }
        }
        public string PlantDir;
        private bool _IsChecked;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    OnPropertyChanged("IsChecked");
                    OnCheckChanged();
                }
            }
        }
        private void OnCheckChanged()
        {
            foreach (UnitVM uc in UnitCollection)
            {
                uc.IsChecked = IsChecked;
            }
        }
        private ObservableCollection<UnitVM> _UnitCollection;
        public ObservableCollection<UnitVM> UnitCollection
        {
            get { return _UnitCollection; }
            set
            {
                if (_UnitCollection != value)
                {
                    _UnitCollection = value;
                    OnPropertyChanged("UnitCollection");
                }
            }
        }
        public PlantVM(string plantName,string dirPlant)
        {
            UnitCollection = new ObservableCollection<UnitVM>();
            string dbPlant_target = dirPlant + @"\plant.mdb";
            PlantDir = dirPlant;
            NHibernateHelper helperProtectedSystem = new NHibernateHelper(dbPlant_target);
            ISession SessionPlant = helperProtectedSystem.GetCurrentSession();
            PlantName = plantName;
            IsChecked = false;

            TreeUnitDAL treeUnitDAL = new TreeUnitDAL();
            IList<TreeUnit> list = treeUnitDAL.GetAllList(SessionPlant);
            foreach (TreeUnit u in list)
            {
                UnitVM uvm = new UnitVM(u, SessionPlant);
                UnitCollection.Add(uvm);
            }
        }

    }
}
