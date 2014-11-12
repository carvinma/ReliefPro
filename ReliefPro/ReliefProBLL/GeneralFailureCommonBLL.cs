using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL.ReactorLoops;
using ReliefProModel;
using ReliefProModel.ReactorLoops;
using UOMLib;
using ReliefProDAL;

namespace ReliefProBLL
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
        public GeneralFailureCommon GetLossofLiquidFeedModel(int ScenarioID)
        {
            var model = generalDAL.GetModelByScenarioID(SessionPS, ScenarioID, 2);
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
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, uomEnum.UserMassRate, model.ReliefLoad);
            model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, uomEnum.UserPressure, model.ReliefPressure);
            model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature, uomEnum.UserTemperature, model.ReliefTemperature);
            return model;
        }
        public IList<GeneralFailureCommonDetail> GetGeneralFailureCommonDetail(int GeneralFailureCommonID,int ReactorType)
        {
            var list = generalDAL.GetGeneralFailureCommonDetail(SessionPS, GeneralFailureCommonID, ReactorType);
            return list;
        }
        public void Save(GeneralFailureCommon model, IList<GeneralFailureCommonDetail> lstDetail)
        {
            ScenarioDAL db = new ScenarioDAL();
           
            Scenario sModel;
            if(model.ScenarioID==0)
             sModel=new Scenario();
            else 
              sModel= db.GetModel(model.ScenarioID, SessionPS);            
            sModel.ReliefLoad = model.ReliefLoad;
            sModel.ReliefTemperature = model.ReliefTemperature;
            sModel.ReliefMW = model.ReliefMW;
            sModel.ReliefPressure = model.ReliefPressure;
            sModel.ReliefCpCv = model.ReliefCpCv;
            sModel.ReliefZ = model.ReliefZ;
            db.AddOrUpdate(sModel, SessionPS);
            model.ScenarioID = sModel.ID;
            generalDAL.Save(SessionPS, model, lstDetail);
        }
    }
}
