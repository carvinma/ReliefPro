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
        private string dbPSFile;

        public DrumBlockedOutletVM(string dbPSFile, string dbPlantFile)
        {
            this.dbPSFile = dbPSFile;
            Model = new DrumBlockedOutlet();
            drum = new DrumBll();
            Model = drum.GetBlockedOutletModel(dbPSFile);
            drum.ReadConvertModel(Model, dbPlantFile);
            CalcCMD = new DelegateCommand<object>(CalcResult);
        }
        private void CalcResult(object obj)
        {
            drum.SaveDrumBlockedOutlet(Model, dbPSFile);
        }
    }
}
