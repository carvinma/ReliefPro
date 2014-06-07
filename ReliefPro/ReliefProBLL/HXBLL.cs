using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.HXs;
using ReliefProModel;
using ReliefProModel.HXs;
using UOMLib;

namespace ReliefProLL
{
    public class HXBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private HXBlockedOutletDAL dbBlock = new HXBlockedOutletDAL();
        private AirCooledHXFireDAL dbAir = new AirCooledHXFireDAL();
        private HXFireDAL dbFire = new HXFireDAL();
        public HXBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public HXBlockedOutlet GetHXBlockedOutletModel(int ScenarioID)
        {
            var model = dbBlock.GetModelByScenarioID(SessionPS, ScenarioID);
            if (model == null)
                model = new HXBlockedOutlet();
            return model;
        }
        public HXBlockedOutlet ReadConvertHXBlockedOutletModel(HXBlockedOutlet model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            HXBlockedOutlet Model = new HXBlockedOutlet();
            Model = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            Model.NormalDuty = UnitConvert.Convert(UOMLib.UOMEnum.EnthalpyDuty.ToString(), uomEnum.UserEnthalpyDuty, Model.NormalDuty);
            Model.NormalHotTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.NormalHotTemperature);
            Model.NormalColdInletTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.NormalColdInletTemperature);
            Model.NormalColdOutletTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.NormalColdOutletTemperature);

            Model.LatentPoint = UnitConvert.Convert(UOMLib.UOMEnum.SpecificEnthalpy.ToString(), uomEnum.UserSpecificEnthalpy, Model.LatentPoint);
            Model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.ReliefLoad);
            Model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.ReliefTemperature);
            Model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, Model.ReliefPressure);
            return Model;
        }

        public AirCooledHXFire GetAirCooledHXFireModel(int ScenarioID)
        {
            var model = dbAir.GetModelByScenarioID(SessionPS, ScenarioID);
            if (model == null)
                model = new AirCooledHXFire();
            return model;
        }
        public AirCooledHXFire ReadConvertAirCooledHXFireModel(AirCooledHXFire model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            AirCooledHXFire Model = new AirCooledHXFire();
            Model = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);

            Model.WettedBundle = UnitConvert.Convert(UOMLib.UOMEnum.Area.ToString(), uomEnum.UserArea, Model.WettedBundle);
            Model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.ReliefLoad);
            Model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.ReliefTemperature);
            Model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, Model.ReliefPressure);
            return Model;
        }

        public HXFire GetHXFireModel(int ScenarioID)
        {
            var model = dbFire.GetModelByScenarioID(SessionPS, ScenarioID);
            if (model == null)
                model = new HXFire();
            return model;
        }
        public HXFire ReadConvertHXFireModel(HXFire model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            HXFire Model = new HXFire();
            Model = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);

            Model.OD = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, Model.OD);
            Model.Length = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, Model.Length);
            Model.Elevation = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, Model.Elevation);
            return Model;
        }

        private void SaveScenario(IScenarioModel model)
        {
            ScenarioDAL db = new ScenarioDAL();
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
            dbAir.Save(SessionPS, model as AirCooledHXFire);
            SaveScenario(model);
        }
        public void SaveHXFire(HXFire model)
        {
            dbFire.Save(SessionPS, model);
        }
    }
}
