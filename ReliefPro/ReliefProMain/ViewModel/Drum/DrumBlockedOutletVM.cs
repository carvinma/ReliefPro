using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProModel;
using ReliefProModel.Drum;
using ReliefProBLL;

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


        public DrumBlockedOutletVM(string dbPSFile, string dbProtectedSystemFile)
        {
            Model = new DrumBlockedOutlet();
            drum = new DrumBll();
            int drumID = 0;
            Model = drum.GetBlockedOutletModel(dbPSFile);
            CalcCMD = new DelegateCommand<object>(CalcResult);
        }
        private void CalcResult(object obj)
        {
            //drum.SaveDrumBlockedOutlet(Model);
        }
    }
}
