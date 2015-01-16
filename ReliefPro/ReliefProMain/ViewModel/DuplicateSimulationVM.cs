using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using UOMLib;
using NHibernate;
using System.Windows;
using ReliefProCommon.Enum;
using System.ComponentModel.DataAnnotations;
using ReliefProBLL;

namespace ReliefProMain.ViewModel
{
    public class DuplicateSimulationVM : ViewModelBase
    {
        
        private bool _Rdo1;
        public bool Rdo1
        {
            get
            {
                return this._Rdo1;
            }
            set
            {
                this._Rdo1 = value;
                OnPropertyChanged("Rdo1");
            }
        }

        private bool _Rdo2;
        public bool Rdo2
        {
            get
            {
                return this._Rdo2;
            }
            set
            {
                this._Rdo2 = value;
                OnPropertyChanged("Rdo2");
            }
        }


        public DuplicateSimulationVM()
        {
            Rdo1 = true;
            
        }

        private ICommand _OKCommand;
        public ICommand OKCommand
        {
            get
            {
                if (_OKCommand == null)
                {
                    _OKCommand = new RelayCommand(OK);

                }
                return _OKCommand;
            }
        }

        private void OK(object window)
        {            
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        
    }
}
