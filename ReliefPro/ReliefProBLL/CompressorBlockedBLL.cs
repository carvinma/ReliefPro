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

namespace ReliefProBLL
{
    public class CompressorBlockedBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private CentrifugalBlockedOutletDAL dbcentrifugal = new CentrifugalBlockedOutletDAL();
        private PistonBlockedOutletDAL dbpiston = new PistonBlockedOutletDAL();
        private UOMLib.UOMEnum uomEnum;

        public CompressorBlockedBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
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
            Model = model;
            Model.Reliefload = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.Reliefload);
            Model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.ReliefTemperature);
            Model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, Model.ReliefPressure);

            Model.InletLoad = UnitConvert.Convert(UOMLib.UOMEnum.VolumeRate.ToString(), uomEnum.UserVolumeRate, Model.InletLoad);
            Model.OutletPressure = UnitConvert.Convert(UOMLib.UOMEnum.VolumeRate.ToString(), uomEnum.UserPressure, Model.OutletPressure);
            Model.SurgeLoad = UnitConvert.Convert(UOMLib.UOMEnum.VolumeRate.ToString(), uomEnum.UserVolumeRate, Model.SurgeLoad);
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
            Model = model;
            Model.Reliefload = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.Reliefload);
            Model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.ReliefTemperature);
            Model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, Model.ReliefPressure);
            return Model;
        }

        public void SaveCentrifugal(CentrifugalBlockedOutlet model)
        {
            dbcentrifugal.Save(SessionPS, model);
            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            sModel.ReliefLoad = model.Reliefload;
            sModel.ReliefPressure = model.ReliefPressure;
            sModel.ReliefTemperature = model.ReliefTemperature;
            sModel.ReliefMW = model.ReliefMW;
            sModel.ReliefCpCv = model.ReliefCpCv;
            sModel.ReliefZ = model.ReliefZ;
            db.Update(sModel, SessionPS);
            SessionPS.Flush();
        }

        public void SavePiston(PistonBlockedOutlet model)
        {
            dbpiston.Save(SessionPS, model);
            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            sModel.ReliefLoad = model.Reliefload;
            sModel.ReliefPressure = model.ReliefPressure;
            sModel.ReliefTemperature = model.ReliefTemperature;
            sModel.ReliefMW = model.ReliefMW;
            sModel.ReliefCpCv = model.ReliefCpCv;
            sModel.ReliefZ = model.ReliefZ;
            db.Update(sModel, SessionPS);
            SessionPS.Flush();
        }
    }
}
