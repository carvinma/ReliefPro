using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.CompressorBlocked;
using ReliefProModel;
using ReliefProModel.CompressorBlocked;
using UOMLib;

namespace ReliefProLL
{
    public class CompressorBlockedBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private dbCentrifugal dbcentrifugal = new dbCentrifugal();
        private dbPiston dbpiston = new dbPiston();
        public CompressorBlockedBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public Centrifugal GetCentrifugalModel(int ScenarioID)
        {
            var model = dbcentrifugal.GetModelByScenarioID(SessionPS, ScenarioID);
            if (model == null)
                model = new Centrifugal();
            return model;
        }
        public Piston GetPistonModel(int ScenarioID)
        {
            var model = dbpiston.GetModelByScenarioID(SessionPS, ScenarioID);
            if (model == null)
                model = new Piston();
            return model;
        }

        public Centrifugal ReadConvertCentrifugalModel(Centrifugal model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            Centrifugal Model = new Centrifugal();
            UnitConvert uc = new UnitConvert();
            Model = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            Model.Reliefload = uc.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.Reliefload);
            Model.ReliefTemperature = uc.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.ReliefTemperature);
            Model.ReliefPressure = uc.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, Model.ReliefPressure);
            return Model;
        }
        public Piston ReadConvertPistonModel(Piston model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            Piston Model = new Piston();
            UnitConvert uc = new UnitConvert();
            Model = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            Model.Reliefload = uc.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.Reliefload);
            Model.ReliefTemperature = uc.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.ReliefTemperature);
            Model.ReliefPressure = uc.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, Model.ReliefPressure);
            return Model;
        }

        public void SaveCentrifugal(Centrifugal model)
        {
            dbcentrifugal.Save(SessionPS, model);
            dbScenario db = new dbScenario();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            sModel.ReliefLoad = model.Reliefload.ToString();
            sModel.ReliefPressure = model.ReliefPressure.ToString();
            sModel.ReliefTemperature = model.ReliefTemperature.ToString();
            sModel.ReliefMW = model.ReliefMW.ToString();
            db.Update(sModel, SessionPS);
        }

        public void SavePiston(Piston model)
        {
            dbpiston.Save(SessionPS, model);
            dbScenario db = new dbScenario();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            sModel.ReliefLoad = model.Reliefload.ToString();
            sModel.ReliefPressure = model.ReliefPressure.ToString();
            sModel.ReliefTemperature = model.ReliefTemperature.ToString();
            sModel.ReliefMW = model.ReliefMW.ToString();
            db.Update(sModel, SessionPS);
        }
    }
}
