using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProModel;
using ReliefProModel.Drum;
using ReliefProLL;
using System.Windows.Input;
using ReliefProMain.Commands;

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumBlockedOutletVM : ViewModelBase
    {
        private DrumBll drum;
        public DrumBlockedOutlet Model;
        public List<string> lstDrumType;
        public ICommand CalcCMD;
        private string selectedDrumType;
        public string SelectedDrumType
        {
            get { return selectedDrumType; }
            set
            {
                selectedDrumType = value;
                OnPropertyChanged("SelectedDrumType");
            }
        }
        public DrumBlockedOutletVM()
        {
            Model = new DrumBlockedOutlet();
            drum = new DrumBll();
            int drumID = 0;
            Model = drum.GetBlockedOutletModel(drumID);
            lstDrumType = new List<string>();
            lstDrumType.Add("General Seperator");
            lstDrumType.Add("Flashing Drum");

            CalcCMD = new DelegateCommand<object>(CalcResult);
        }
        private void CalcResult(object obj)
        {
            drum.SaveDrumBlockedOutlet(Model);
        }
    }
}
