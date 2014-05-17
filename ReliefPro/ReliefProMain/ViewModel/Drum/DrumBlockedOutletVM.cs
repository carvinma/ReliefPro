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
using ProII;

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumBlockedOutletVM : ViewModelBase
    {
        private DrumBll drum;
        public DrumBlockedOutlet Model;
        public List<string> lstDrumType;
        public ICommand CalcCMD;
        private string dbPSFile;

        public DrumBlockedOutletVM(int ScenarioID, string dbPSFile, string dbPlantFile)
        {
            this.dbPSFile = dbPSFile;
            Model = new DrumBlockedOutlet();
            drum = new DrumBll();
            Model = drum.GetBlockedOutletModel(dbPSFile);
            //Model.DrumID
            Model.ScenarioID = ScenarioID;
            drum.ReadConvertModel(Model, dbPlantFile);
            CalcCMD = new DelegateCommand<object>(CalcResult);
        }
        private void CalcResult(object obj)
        {
            double reliefLoad = 0, reliefMW = 0, reliefT = 0;
            if (drum.PfeedUpstream(dbPSFile) > drum.PSet(dbPSFile))
            {
                if (Model.DrumType == "Flashing Drum")
                {
                    //string version = "9.1";
                    // IFlashCalculateW flashCalc = ProIIFactory.CreateFlashCalculateW(version);
                    // string filePath = flashCalc.Calculate();
                }
                else
                { }
            }
            else
            {
                reliefLoad = 0;
            }

            drum.SaveDrumBlockedOutlet(Model, dbPSFile, reliefLoad, reliefMW, reliefT);
        }
    }
}
