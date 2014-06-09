using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL.ReactorLoops;
using ReliefProModel.ReactorLoops;

namespace ReliefProLL
{
    public class GeneralFailureCommonBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private GeneralFailureCommonDAL generalDAL = new GeneralFailureCommonDAL();
        public GeneralFailureCommonBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public GeneralFailureCommon GetGeneralCoolingWaterFailureModel(int ScenarioID)
        {
            var model = generalDAL.GetModelByScenarioID(SessionPS, ScenarioID, 0);
            if (model == null)
                model = new GeneralFailureCommon();
            return model;
        }
        public GeneralFailureCommon GetGeneralElectricPowerFailureModel(int ScenarioID)
        {
            var model = generalDAL.GetModelByScenarioID(SessionPS, ScenarioID, 1);
            if (model == null)
                model = new GeneralFailureCommon();
            return model;
        }

        public IList<GeneralFailureCommonDetail> GetGeneralFailureCommonDetail(int GeneralFailureCommonID)
        {
            var list = generalDAL.GetGeneralFailureCommonDetail(SessionPS, GeneralFailureCommonID);
            return list;
        }
        public void Save(GeneralFailureCommon model, IList<GeneralFailureCommonDetail> lstDetail)
        {
            generalDAL.Save(SessionPS, model, lstDetail);
        }
    }
}
