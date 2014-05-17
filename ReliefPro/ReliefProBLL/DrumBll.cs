using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProDAL.Drum;
using ReliefProModel;
using ReliefProModel.Drum;
using UOMLib;


namespace ReliefProBLL
{
    public class DrumBll
    {
        private dbDrumBlockedOutlet dbBlockedOutlet = new dbDrumBlockedOutlet();
        public void SaveDrumBlockedOutlet(DrumBlockedOutlet model, string dbProtectedSystemFile)
        {
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbBlockedOutlet.SaveDrumBlockedOutlet(Session, model);
            }
        }
        public DrumBlockedOutlet GetBlockedOutletModel(string dbProtectedSystemFile)
        {
            DrumBlockedOutlet Model = new DrumBlockedOutlet();
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbDrum dbdrum = new dbDrum();
                List<Drum> lstDrum = dbdrum.GetAllList(Session).ToList();
                if (lstDrum.Count() > 0)
                {
                    Model.DrumType = lstDrum[0].DrumType;
                    Model.NormalFlashDuty = double.Parse(lstDrum[0].Duty);
                    Model.DrumID = lstDrum[0].ID;
                }

                var tmpModel = dbBlockedOutlet.GetModelByDrumID(Session, Model.DrumID);
                if (tmpModel != null)
                {
                    Model = tmpModel;
                    return Model;
                }
                dbSource dbSource = new dbSource();
                List<Source> listSource = dbSource.GetAllList(Session).ToList();
                if (listSource.Count() > 0)
                {
                    double MaxPressure = 0;
                    if (double.TryParse(listSource.First().MaxPossiblePressure, out MaxPressure))
                    {
                        Model.MaxPressure = MaxPressure;
                    }
                }
                dbScenario towerScenario = new dbScenario();
                List<Scenario> listTowerScenario = towerScenario.GetAllList(Session).ToList();
                if (listTowerScenario.Count() > 0)
                {
                    double MaxStreamRate = 0;
                    if (double.TryParse(listTowerScenario.First().ReliefLoad, out MaxStreamRate))
                        Model.MaxStreamRate = MaxStreamRate;
                }
            }
            return Model;
        }

        public DrumBlockedOutlet WriteConvertModel(DrumBlockedOutlet model, string dbPlantFile)
        {
            DrumBlockedOutlet outletModel = new DrumBlockedOutlet();
            return outletModel;
        }
        public DrumBlockedOutlet ReadConvertModel(DrumBlockedOutlet model, string dbPlantFile)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(dbPlantFile);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            DrumBlockedOutlet outletModel = new DrumBlockedOutlet();
            UnitConvert uc = new UnitConvert();
            outletModel = model;
            outletModel.MaxPressure = uc.Convert(unitInfo.GetBasicUnitDefaultUserSet(UOMLib.UOMEnum.UnitTypeEnum.Pressure, dbPlantFile), UOMLib.UOMEnum.Pressure.ToString(), outletModel.MaxPressure);
            outletModel.MaxStreamRate = uc.Convert(unitInfo.GetBasicUnitDefaultUserSet(UOMLib.UOMEnum.UnitTypeEnum.WeightFlow, dbPlantFile), UOMLib.UOMEnum.WeightFlow.ToString(), outletModel.MaxPressure);
            outletModel.NormalFlashDuty = uc.Convert(unitInfo.GetBasicUnitDefaultUserSet(UOMLib.UOMEnum.UnitTypeEnum.EnthalpyDuty, dbPlantFile), UOMLib.UOMEnum.EnthalpyDuty.ToString(), outletModel.MaxPressure);
            return outletModel;
        }
    }
}
