using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProMain.Commands;
using System.IO;
using System.Collections.ObjectModel;
using ReliefProDAL;
using ReliefProModel;
using NHibernate;
namespace ReliefProMain.ViewModel.Trees
{
    public class UnitVM : ViewModelBase
    {       
       
        private string _UnitName;
        public string UnitName
        {
            get
            {
                return this._UnitName;
            }
            set
            {
                this._UnitName = value;

                OnPropertyChanged("UnitName");
            }
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

                OnPropertyChanged("ID");
            }
        }
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


        private ObservableCollection<PSVM> _PSCollection;
        public ObservableCollection<PSVM> PSCollection
        {
            get { return _PSCollection; }
            set
            {
                if (_PSCollection != value)
                {
                    _PSCollection = value;
                    OnPropertyChanged("PSCollection");
                }
            }
        }
        public UnitVM(TreeUnit unit, ISession SessionPlant)
        {
            PSCollection = new ObservableCollection<PSVM>();
            ID = unit.ID;
            UnitName = unit.UnitName;
            TreePSDAL treePSDAL = new TreePSDAL();
            IList<TreePS> list = treePSDAL.GetAllList(ID, SessionPlant);
            foreach (TreePS p in list)
            {
                PSVM ps = new PSVM(p);
                PSCollection.Add(ps);
            }
        }
        private void OnCheckChanged()
        {
            foreach (PSVM p in PSCollection)
            {
                p.IsChecked = IsChecked;
            }
        }
        

    }
}
