﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.HXs;
using ReliefProModel;
using ReliefProModel.HXs;
using UOMLib;
using ReliefProCommon.Enum;

namespace ReliefProBLL
{
    public class HXBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private HXBlockedInDAL dbBlock = new HXBlockedInDAL();
        private AirCooledHXFireSizeDAL dbAir = new AirCooledHXFireSizeDAL();
        private HXFireSizeDAL dbFire = new HXFireSizeDAL();
        private ScenarioDAL dbScenario = new ScenarioDAL();
        public HXBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public HXBlockedIn GetHXBlockedOutletModel(int ScenarioID)
        {
            var model = dbBlock.GetModelByScenarioID(SessionPS, ScenarioID);
            var sModel = dbScenario.GetModel(ScenarioID, SessionPS);
            if (model == null)
                model = new HXBlockedIn();
            else
            {
                if (sModel != null)
                {
                    model.ReliefLoad = sModel.ReliefLoad;
                    model.ReliefPressure = sModel.ReliefPressure;
                    model.ReliefTemperature = sModel.ReliefTemperature;
                    model.ReliefMW = sModel.ReliefMW;
                    model.ReliefCpCv = sModel.ReliefCpCv;
                    model.ReliefZ = sModel.ReliefZ;
                }
            }
            return model;
        }
        public HXBlockedIn ReadConvertHXBlockedOutletModel(HXBlockedIn model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            HXBlockedIn Model = new HXBlockedIn();
            Model = model;
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
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
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);

            Model.WettedBundle = UnitConvert.Convert(UOMLib.UOMEnum.Area.ToString(), uomEnum.UserArea, Model.WettedBundle);
            return Model;
        }

        public HXFireSize GetHXFireSizeModel(int ScenarioID)
        {
            var model = dbFire.GetModelByScenarioID(SessionPS, ScenarioID);
            if (model == null)
            {
                PSVDAL psvdal = new PSVDAL();
                PSV psv = psvdal.GetModel(SessionPS);
                model = new HXFireSize();
                model.PipingContingency = 10;
                model.ExposedToFire = psv.LocationDescription;
                model.Type = "Fixed";                
                model.Elevation_Color = ColorBorder.green.ToString();
                model.ExposedToFire_Color = ColorBorder.green.ToString();
                model.Length_Color = ColorBorder.green.ToString();
                model.OD_Color = ColorBorder.green.ToString();
                model.PipingContingency_Color = ColorBorder.green.ToString();
                model.Type_Color = ColorBorder.green.ToString();
            }
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
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);

            Model.OD = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, Model.OD);
            Model.Length = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, Model.Length);
            Model.Elevation = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, Model.Elevation);
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
            dbBlock.Save(SessionPS, model as HXBlockedIn);
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
