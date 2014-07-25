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

namespace ReliefProBLL
{
    public class HXBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private HXBlockedOutletDAL dbBlock = new HXBlockedOutletDAL();
        private AirCooledHXFireSizeDAL dbAir = new AirCooledHXFireSizeDAL();
        private HXFireSizeDAL dbFire = new HXFireSizeDAL();
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
            Model.NormalDuty = UnitConvert.Convert(UOMLib.UOMEnum.EnthalpyDuty.ToString(), uomEnum.UserEnthalpyDuty, Model.NormalDuty.Value);
            Model.NormalHotTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.NormalHotTemperature.Value);
            Model.NormalColdInletTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.NormalColdInletTemperature.Value);
            Model.NormalColdOutletTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.NormalColdOutletTemperature.Value);

            Model.LatentPoint = UnitConvert.Convert(UOMLib.UOMEnum.SpecificEnthalpy.ToString(), uomEnum.UserSpecificEnthalpy, Model.LatentPoint.Value);
            Model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.ReliefLoad.Value);
            Model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.ReliefTemperature.Value);
            Model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, Model.ReliefPressure.Value);
            return Model;
        }

        public AirCooledHXFireSize GetAirCooledHXFireSizeModel(int ScenarioID)
        {
            var model = dbAir.GetModelByScenarioID(SessionPS, ScenarioID);
            if (model == null)
                model = new AirCooledHXFireSize();
            return model;
        }
        public AirCooledHXFireSize ReadConvertAirCooledHXFireSizeModel(AirCooledHXFireSize model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            AirCooledHXFireSize Model = new AirCooledHXFireSize();
            Model = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);

            Model.WettedBundle = UnitConvert.Convert(UOMLib.UOMEnum.Area.ToString(), uomEnum.UserArea, Model.WettedBundle.Value);          
            return Model;
        }

        public HXFireSize GetHXFireSizeModel(int ScenarioID)
        {
            var model = dbFire.GetModelByScenarioID(SessionPS, ScenarioID);
            if (model == null)
                model = new HXFireSize();
            return model;
        }
        public HXFireSize ReadConvertHXFireSizeModel(HXFireSize model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            HXFireSize Model = new HXFireSize();
            Model = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);

            Model.OD = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, Model.OD.Value);
            Model.Length = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, Model.Length.Value);
            Model.Elevation = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, Model.Elevation.Value);
            return Model;
        }

        private void SaveScenario(IScenarioModel model)
        {
            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            sModel.ReliefLoad = model.ReliefLoad;
            sModel.ReliefPressure = model.ReliefPressure;
            sModel.ReliefTemperature = model.ReliefTemperature;
            sModel.ReliefMW = model.ReliefMW;
            db.Update(sModel, SessionPS);
        }

        public void SaveHXBlockedOutlet(IScenarioModel model)
        {
            dbBlock.Save(SessionPS, model as HXBlockedOutlet);
            SaveScenario(model);
        }
        public void SaveAirCooledHXFireSize(IScenarioModel model)
        {
            dbAir.Save(SessionPS, model as AirCooledHXFireSize);
            SaveScenario(model);
        }
        public void SaveHXFireSize(HXFireSize model)
        {
            dbFire.Save(SessionPS, model);
        }
    }
}
