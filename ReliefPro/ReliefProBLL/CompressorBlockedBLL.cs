using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.Compressors;
using ReliefProModel;
using ReliefProModel.Compressors;
using UOMLib;

namespace ReliefProLL
{
    public class CompressorBlockedBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private CentrifugalBlockedOutletDAL dbcentrifugal = new CentrifugalBlockedOutletDAL();
        private PistonBlockedOutletDAL dbpiston = new PistonBlockedOutletDAL();
        public CompressorBlockedBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public CentrifugalBlockedOutlet GetCentrifugalModel(int ScenarioID)
        {
            var model = dbcentrifugal.GetModelByScenarioID(SessionPS, ScenarioID);
            if (model == null)
                model = new CentrifugalBlockedOutlet();
            return model;
        }
        public PistonBlockedOutlet GetPistonModel(int ScenarioID)
        {
            var model = dbpiston.GetModelByScenarioID(SessionPS, ScenarioID);
            if (model == null)
                model = new PistonBlockedOutlet();
            return model;
        }

        public CentrifugalBlockedOutlet ReadConvertCentrifugalModel(CentrifugalBlockedOutlet model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            CentrifugalBlockedOutlet Model = new CentrifugalBlockedOutlet();
            UnitConvert uc = new UnitConvert();
            Model = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            Model.Reliefload = uc.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.Reliefload);
            Model.ReliefTemperature = uc.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.ReliefTemperature);
            Model.ReliefPressure = uc.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, Model.ReliefPressure);
            return Model;
        }
        public PistonBlockedOutlet ReadConvertPistonModel(PistonBlockedOutlet model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            PistonBlockedOutlet Model = new PistonBlockedOutlet();
            UnitConvert uc = new UnitConvert();
            Model = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            Model.Reliefload = uc.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.Reliefload);
            Model.ReliefTemperature = uc.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.ReliefTemperature);
            Model.ReliefPressure = uc.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, Model.ReliefPressure);
            return Model;
        }

        public void SaveCentrifugal(CentrifugalBlockedOutlet model)
        {
            dbcentrifugal.Save(SessionPS, model);
            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            sModel.ReliefLoad = model.Reliefload.ToString();
            sModel.ReliefPressure = model.ReliefPressure.ToString();
            sModel.ReliefTemperature = model.ReliefTemperature.ToString();
            sModel.ReliefMW = model.ReliefMW.ToString();
            db.Update(sModel, SessionPS);
        }

        public void SavePiston(PistonBlockedOutlet model)
        {
            dbpiston.Save(SessionPS, model);
            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            sModel.ReliefLoad = model.Reliefload.ToString();
            sModel.ReliefPressure = model.ReliefPressure.ToString();
            sModel.ReliefTemperature = model.ReliefTemperature.ToString();
            sModel.ReliefMW = model.ReliefMW.ToString();
            db.Update(sModel, SessionPS);
        }
    }
}
