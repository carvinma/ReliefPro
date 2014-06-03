using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProMain.Commands;
using System.IO;
namespace ReliefProMain.ViewModel
{
    public class CreateProtectedSystemVM : ViewModelBase
    {
       
       
        public string dirUnit { get; set; }

        public string dirProtectedSystem{ get; set; }
        public string visioProtectedSystem { get; set; }
        public string dbProtectedSystemFile { get; set; }


        private string _ProtectedSystemName;
        public string ProtectedSystemName
        {
            get
            {
                return this._ProtectedSystemName;
            }
            set
            {
                this._ProtectedSystemName = value;

                OnPropertyChanged("ProtectedSystemName");
            }
        }

        public CreateProtectedSystemVM(string dir)
        {
            dirUnit = dir;
        }

        private ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(Save);
                    
                }
                return _SaveCommand;
            }
        }

        private void Save(object window)
        {
            dirProtectedSystem = dirUnit + @"\" + ProtectedSystemName;
            Directory.CreateDirectory(dirProtectedSystem);
            string protectedsystem1 = dirProtectedSystem;
            string dbProtectedSystem = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\protectedsystem.mdb";
            string dbProtectedSystem_target = protectedsystem1 + @"\protectedsystem.mdb";
            System.IO.File.Copy(dbProtectedSystem, dbProtectedSystem_target, true);
            visioProtectedSystem = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\protectedsystem.vsd";
            string visioProtectedSystem_target = protectedsystem1 + @"\design.vsd";
            System.IO.File.Copy(visioProtectedSystem, visioProtectedSystem_target, true);

            dbProtectedSystemFile = dbProtectedSystem_target;
            dirProtectedSystem = protectedsystem1;
            visioProtectedSystem = visioProtectedSystem_target;

            System.Windows.Window wd = window as System.Windows.Window;
            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

    }
}
