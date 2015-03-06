using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using UOMLib;

namespace ReliefProBLL
{
    public class aBlockedVaporOutletBLL
    {
        private ORDesignerPlantDataContext plantContext = new ORDesignerPlantDataContext();
        private UOMLib.UOMEnum uomEnum;

        public aBlockedVaporOutletBLL()
        {
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
        }
        public tbBlockedVaporOutlet GeModel(int scenarioID, int outletType)
        {
            return plantContext.tbBlockedVaporOutlet.FirstOrDefault(p => p.ScenarioID == scenarioID && p.OutletType == outletType);
        }
        public tbScenario GetScenarioModel(int scenarioID)
        {
            return plantContext.tbScenario.FirstOrDefault(p => p.Id == scenarioID);
        }
        public tbBlockedVaporOutlet ReadConvertBlockedVaporOutletModel(tbBlockedVaporOutlet model)
        {
            UnitInfo unitInfo = new UnitInfo();
            systbBasicUnit basicUnit = unitInfo.GetBasicUnitUOM();
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            tbBlockedVaporOutlet Model = new tbBlockedVaporOutlet();
            Model = model;
            Model.InletGasUpstreamMaxPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, uomEnum.UserPressure, Model.InletGasUpstreamMaxPressure);
            Model.InletAbsorbentUpstreamMaxPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, uomEnum.UserPressure, Model.InletAbsorbentUpstreamMaxPressure);
            Model.NormalGasFeedWeightRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, uomEnum.UserMassRate, Model.NormalGasFeedWeightRate);
            Model.NormalGasProductWeightRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, uomEnum.UserMassRate, Model.NormalGasProductWeightRate);
            return Model;
        }
        public tbScenario ReadConvertScenarioModel(tbScenario model)
        {
            UnitInfo unitInfo = new UnitInfo();
            systbBasicUnit basicUnit = unitInfo.GetBasicUnitUOM();
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            tbScenario Model = new tbScenario();
            Model = model;
            Model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, uomEnum.UserMassRate, Model.ReliefLoad);
            Model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature, uomEnum.UserTemperature, Model.ReliefTemperature);
            Model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, uomEnum.UserPressure, Model.ReliefPressure);
            return Model;
        }
        public void Save(tbBlockedVaporOutlet model, tbScenario smodel)
        {

            dbBlockedVaporOutlet.Save(SessionPS, model);
            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);

            sModel.ReliefLoad = smodel.ReliefLoad;
            sModel.ReliefMW = smodel.ReliefMW;
            sModel.ReliefTemperature = smodel.ReliefTemperature;
            sModel.ReliefPressure = smodel.ReliefPressure;
            sModel.ReliefCpCv = smodel.ReliefCpCv;
            smodel.ReliefZ = smodel.ReliefZ;
            db.Update(sModel, SessionPS);
            SessionPS.Flush();
        }
    }
}
