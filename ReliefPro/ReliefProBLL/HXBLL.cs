using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.HX;
using ReliefProModel;
using ReliefProModel.HX;
using UOMLib;

namespace ReliefProLL
{
    public class HXBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private dbHXBlockedOutlet dbBlock = new dbHXBlockedOutlet();
        private dbAirCooledHXFire dbAir = new dbAirCooledHXFire();
        private dbHXFire dbFire = new dbHXFire();
        public HXBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public HXBlockedOutlet GetCentrifugalModel(int ScenarioID)
        {
            var model = dbBlock.GetModelByScenarioID(SessionPS, ScenarioID);
            if (model == null)
                model = new HXBlockedOutlet();
            return model;
        }
        public HXBlockedOutlet ReadConvertCentrifugalModel(HXBlockedOutlet model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            HXBlockedOutlet Model = new HXBlockedOutlet();
            UnitConvert uc = new UnitConvert();
            Model = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            Model.NormalDuty = uc.Convert(UOMLib.UOMEnum.EnthalpyDuty.ToString(), uomEnum.UserEnthalpyDuty, Model.NormalDuty);
            Model.NormalHotTemperature = uc.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.NormalHotTemperature);
            Model.NormalColdInletTemperature = uc.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.NormalColdInletTemperature);
            Model.NormalColdOutletTemperature = uc.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.NormalColdOutletTemperature);

            Model.LatentPoint = uc.Convert(UOMLib.UOMEnum.SpecificEnthalpy.ToString(), uomEnum.UserSpecificEnthalpy, Model.LatentPoint);
            Model.ReliefLoad = uc.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.ReliefLoad);
            Model.ReliefTemperature = uc.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.ReliefTemperature);
            Model.ReliefPressure = uc.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, Model.ReliefPressure);
            return Model;
        }
        private void SaveScenario(IScenarioModel model)
        {
            dbScenario db = new dbScenario();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            sModel.ReliefLoad = model.ReliefLoad.ToString();
            sModel.ReliefPressure = model.ReliefPressure.ToString();
            sModel.ReliefTemperature = model.ReliefTemperature.ToString();
            sModel.ReliefMW = model.ReliefMW.ToString();
            db.Update(sModel, SessionPS);
        }
        public void SaveHXBlockedOutlet(IScenarioModel model)
        {
            dbBlock.Save(SessionPS, model as HXBlockedOutlet);
            SaveScenario(model);
        }
        public void SaveAirCooledHXFire(IScenarioModel model)
        {
            dbBlock.Save(SessionPS, model as HXBlockedOutlet);
            SaveScenario(model);
        }
    }
}
