using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProMain.ViewModel
{
    public class DrumVM:ViewModelBase
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }

        private string _SelectedFile;
        public string SelectedFile
        {
            get
            {
                return this._SelectedFile;
            }
            set
            {
                this._SelectedFile = value;
                OnPropertyChanged("SelectedFile");
            }
        }
        private string _SelectedEq;
        public string SelectedEq
        {
            get
            {
                return this._SelectedEq;
            }
            set
            {
                this._SelectedEq = value;
                OnPropertyChanged("SelectedEq");
            }
        }

        
    }
}
