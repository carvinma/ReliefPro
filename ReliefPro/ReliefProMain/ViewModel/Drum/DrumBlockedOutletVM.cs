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
using ReliefProMain.Model;
using UOMLib;

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumBlockedOutletVM : ViewModelBase
    {
        private DrumBll drum;
        public DrumBlockedOutlet model { get; set; }
        public DrumBlockedOutletModel unitModel { get; set; }
        public ICommand CalcCMD { get; set; }
        private string dbPSFile;
        private string dbPlantFile;

        public DrumBlockedOutletVM(int ScenarioID, string dbPSFile, string dbPlantFile)
        {
            this.dbPSFile = dbPSFile;
            this.dbPlantFile = dbPlantFile;
            model = new DrumBlockedOutlet();
            unitModel = new DrumBlockedOutletModel();
            drum = new DrumBll();
            model = drum.GetBlockedOutletModel(dbPSFile);
            model.DrumID = drum.GetDrumID(dbPSFile);
            model.ScenarioID = ScenarioID;
            model = drum.ReadConvertModel(model, dbPlantFile);
            CalcCMD = new DelegateCommand<object>(CalcResult);
            DrumBlockedOutletModel dd = new DrumBlockedOutletModel();
        }
        private void WriteConvertModel()
        {
            UnitConvert uc = new UnitConvert();
            UOMLib.UOMEnum uomEnum = new UOMEnum(dbPlantFile);
            model.MaxPressure = uc.Convert(unitModel.PressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.MaxPressure);
            //model.MaxStreamRate = uc.Convert(unitModel.StreamRateUnit, UOMLib.UOMEnum.Pressure.ToString(), model.MaxPressure);
            //model.NormalFlashDuty = uc.Convert(unitModel.FlashingDutyUnit, UOMLib.UOMEnum.Pressure.ToString(), model.MaxPressure);
            //model.FDReliefCondition = uc.Convert(unitModel.ReliefConditionUnit, UOMLib.UOMEnum.Pressure.ToString(), model.MaxPressure);
        }
        private void CalcResult(object obj)
        {
            double reliefLoad = 0, reliefMW = 0, reliefT = 0;
            if (drum.PfeedUpstream(dbPSFile) > drum.PSet(dbPSFile))
            {
                if (model.DrumType == "Flashing Drum")
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
            WriteConvertModel();
            drum.SaveDrumBlockedOutlet(model, dbPSFile, reliefLoad, reliefMW, reliefT);
        }
    }
}
