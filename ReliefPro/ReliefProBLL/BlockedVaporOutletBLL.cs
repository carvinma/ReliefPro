using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.Towers;
using ReliefProModel;
using ReliefProModel.Towers;

namespace ReliefProLL
{
    public class BlockedVaporOutletBLL
    {
        private ISession SessionPS;
        private BlockedVaporOutletDAL dbBlockedVaporOutlet = new BlockedVaporOutletDAL();
        public BlockedVaporOutletBLL(ISession SessionPS)
        {
            this.SessionPS = SessionPS;
        }
        public BlockedVaporOutlet GeModel(int ScenarioID, int OutletType)
        {
            var list = dbBlockedVaporOutlet.GetBlockedVaporOutlet(SessionPS, ScenarioID, OutletType);
            if (list.Count > 0) return list[0];
            return null;
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
