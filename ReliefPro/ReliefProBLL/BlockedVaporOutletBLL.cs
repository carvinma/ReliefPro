using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.Towers;
using ReliefProModel;
using ReliefProModel.Towers;
using UOMLib;

namespace ReliefProLL
{
    public class BlockedVaporOutletBLL
    {
        private ISession SessionPF;
        private ISession SessionPS;
        private BlockedVaporOutletDAL dbBlockedVaporOutlet = new BlockedVaporOutletDAL();
        private ScenarioDAL scenarioDAL = new ScenarioDAL();
        private UOMLib.UOMEnum uomEnum;
        public BlockedVaporOutletBLL(ISession SessionPF, ISession SessionPS)
        {
            this.SessionPF = SessionPF;
            this.SessionPS = SessionPS;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionDBPath == this.SessionPF.Connection.ConnectionString);
        }
        public BlockedVaporOutlet GeModel(int ScenarioID, int OutletType)
        {
            var list = dbBlockedVaporOutlet.GetBlockedVaporOutlet(SessionPS, ScenarioID, OutletType);
            if (list.Count > 0) return list[0];
            return null;
        }
        public Scenario GetScenarioModel(int ScenarioID)
        {
            return scenarioDAL.GetModel(ScenarioID, SessionPS);
        }
        public BlockedVaporOutlet ReadConvertBlockedVaporOutletModel(BlockedVaporOutlet model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            BlockedVaporOutlet Model = new BlockedVaporOutlet();
            Model = model;
            Model.InletGasUpstreamMaxPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, uomEnum.UserPressure, Model.InletGasUpstreamMaxPressure);
            Model.InletAbsorbentUpstreamMaxPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, uomEnum.UserPressure, Model.InletAbsorbentUpstreamMaxPressure);
            Model.NormalGasFeedWeightRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, uomEnum.UserMassRate, Model.NormalGasFeedWeightRate);
            Model.NormalGasProductWeightRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, uomEnum.UserMassRate, Model.NormalGasProductWeightRate);
            return Model;
        }
        public Scenario ReadConvertScenarioModel(Scenario model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            Scenario Model = new Scenario();
            Model = model;
            Model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, uomEnum.UserMassRate, Model.ReliefLoad);
            Model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature, uomEnum.UserTemperature, Model.ReliefTemperature);
            Model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, uomEnum.UserPressure, Model.ReliefPressure);
            return Model;
        }
        public void Save(BlockedVaporOutlet model, Scenario smodel)
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

        }
    }
}
