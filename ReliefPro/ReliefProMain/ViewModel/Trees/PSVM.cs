using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProMain.Commands;
using System.IO;
using ReliefProModel;

namespace ReliefProMain.ViewModel.Trees
{
    public class PSVM : ViewModelBase
    {              
        private string _PSName;
        public string PSName
        {
            get
            {
                return this._PSName;
            }
            set
            {
                this._PSName = value;

                OnPropertyChanged("PSName");
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
                }
            }
        }

        public PSVM(TreePS treePS)
        {           
            PSName = treePS.PSName;
            ID = treePS.ID;
            IsChecked = false;
        }

    }
}
