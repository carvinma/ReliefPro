using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL.ReactorLoops;
using ReliefProModel;
using ReliefProModel.ReactorLoops;
using UOMLib;

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

        public GeneralFailureCommon ReadConvert(GeneralFailureCommon model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            UOMLib.UOMEnum uomEnum = new UOMEnum(SessionPF);
            model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, uomEnum.UserMassRate, model.ReliefLoad);
            model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, uomEnum.UserPressure, model.ReliefPressure);
            model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature, uomEnum.UserTemperature, model.ReliefTemperature);
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
