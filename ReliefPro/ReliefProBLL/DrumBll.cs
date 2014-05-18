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
        public int GetDrumID(string dbProtectedSystemFile)
        {
            dbDrum db = new dbDrum();
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                Drum model = db.GetModel(Session);
                if (model != null)
                    return model.ID;
            }
            return 0;
        }
        public void SaveDrumBlockedOutlet(DrumBlockedOutlet model, string dbProtectedSystemFile, double reliefLoad, double reliefMW, double reliefT)
        {
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbBlockedOutlet.SaveDrumBlockedOutlet(Session, model);
                dbScenario db = new dbScenario();
                var sModel = db.GetModel(model.ScenarioID, Session);

                sModel.ReliefLoad = reliefLoad.ToString();
                sModel.ReliefMW = reliefMW.ToString();
                sModel.ReliefTemperature = reliefT.ToString();
                sModel.ReliefPressure = ScenarioReliefPressure(dbProtectedSystemFile).ToString();
                db.Update(sModel, Session);
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
            UOMLib.UOMEnum uomEnum = new UOMEnum(dbPlantFile);
            outletModel.MaxPressure = uc.Convert(uomEnum.UserSetTemperature, UOMLib.UOMEnum.Pressure.ToString(), outletModel.MaxPressure);
            outletModel.MaxStreamRate = uc.Convert(uomEnum.UserWeightFlow, UOMLib.UOMEnum.WeightFlow.ToString(), outletModel.MaxStreamRate);
            outletModel.NormalFlashDuty = uc.Convert(uomEnum.UserEnthalpyDuty, UOMLib.UOMEnum.EnthalpyDuty.ToString(), outletModel.NormalFlashDuty);
            return outletModel;
        }

        public double PfeedUpstream(string dbProtectedSystemFile)
        {
            dbStream stream = new dbStream();
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                var streamModel = stream.GetAllList(Session).FirstOrDefault();
                if (streamModel != null)
                {
                    if (!string.IsNullOrEmpty(streamModel.Pressure))
                        return double.Parse(streamModel.Pressure);
                }
            }
            return 0;
        }
        public double ScenarioReliefPressure(string dbProtectedSystemFile)
        {
            dbPSV psv = new dbPSV();
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                var psvModel = psv.GetAllList(Session).FirstOrDefault();
                if (psvModel != null)
                {
                    if (!string.IsNullOrEmpty(psvModel.Pressure))
                        return double.Parse(psvModel.Pressure) * double.Parse(psvModel.ReliefPressureFactor);
                }
            }
            return 0;
        }
        public double PSet(string dbProtectedSystemFile)
        {
            dbPSV psv = new dbPSV();
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                var psvModel = psv.GetAllList(Session).FirstOrDefault();
                if (psvModel != null)
                {
                    if (!string.IsNullOrEmpty(psvModel.Pressure))
                        return double.Parse(psvModel.Pressure);
                }
            }
            return 0;
        }

    }
}
