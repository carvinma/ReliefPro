using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.Drum;
using ReliefProModel;
using ReliefProModel.Drum;
using UOMLib;

namespace ReliefProLL
{
    public class DrumFireBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private dbDrumFire dbdrumFire = new dbDrumFire();
        public DrumFireBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public DrumFireCalc GetDrumFireModel(int ScenarioID)
        {
            DrumFireCalc firemodel = new DrumFireCalc();
            dbDrumFire drumFire = new dbDrumFire();
            List<DrumFireCalc> lstDrumFire = drumFire.GetAllList(SessionPS).ToList();
            if (lstDrumFire.Count() > 0)
            {
                firemodel = lstDrumFire.Where(p => p.ScenarioID == ScenarioID).FirstOrDefault();
            }
            if (firemodel.ID > 0)
                return firemodel;
            else
            {
                firemodel = GetScenarioInfo(ScenarioID, firemodel);
                firemodel.ScenarioID = ScenarioID;
            }
            return firemodel;
        }
        private DrumFireCalc GetScenarioInfo(int ScenarioID, DrumFireCalc firemodel)
        {
            dbScenario db = new dbScenario();
            var sModel = db.GetModel(ScenarioID, SessionPS);
            if (sModel != null)
            {
                if (!string.IsNullOrEmpty(sModel.ReliefLoad))
                    firemodel.ReliefLoad = double.Parse(sModel.ReliefLoad);
                if (!string.IsNullOrEmpty(sModel.ReliefPressure))
                    firemodel.ReliefPressure = double.Parse(sModel.ReliefPressure);
                if (!string.IsNullOrEmpty(sModel.ReliefTemperature))
                    firemodel.ReliefTemperature = double.Parse(sModel.ReliefTemperature);
                if (!string.IsNullOrEmpty(sModel.ReliefMW))
                    firemodel.ReliefMW = double.Parse(sModel.ReliefMW);
                if (!string.IsNullOrEmpty(sModel.ReliefCpCv))
                    firemodel.ReliefCpCv = double.Parse(sModel.ReliefCpCv);
                if (!string.IsNullOrEmpty(sModel.ReliefZ))
                    firemodel.ReliefZ = double.Parse(sModel.ReliefZ);
            }
            return firemodel;
        }

        public DrumFireCalc ReadConvertModel(DrumFireCalc model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            DrumFireCalc fireModel = new DrumFireCalc();
            UnitConvert uc = new UnitConvert();
            fireModel = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            fireModel.WettedArea = uc.Convert(UOMLib.UOMEnum.Area.ToString(), uomEnum.UserArea, fireModel.WettedArea);
            fireModel.LatentHeat = uc.Convert(UOMLib.UOMEnum.SpecificEnthalpy.ToString(), uomEnum.UserSpecificEnthalpy, fireModel.LatentHeat);
            fireModel.CrackingHeat = uc.Convert(UOMLib.UOMEnum.SpecificEnthalpy.ToString(), uomEnum.UserSpecificEnthalpy, fireModel.CrackingHeat);
            fireModel.ReliefLoad = uc.Convert(UOMLib.UOMEnum.WeightFlow.ToString(), uomEnum.UserWeightFlow, fireModel.ReliefLoad);
            fireModel.ReliefPressure = uc.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, fireModel.ReliefPressure);
            fireModel.ReliefTemperature = uc.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, fireModel.ReliefTemperature);
            return fireModel;
        }

        public void SaveData(DrumFireCalc model, DrumFireFluid fluidModel, DrumSize sizeModel, ISession SessionPS)
        {

            dbdrumFire.SaveDrumFireCalc(SessionPS, model);
            dbScenario db = new dbScenario();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            if (fluidModel != null)
            {
                fluidModel.DrumFireCalcID = model.ID;
                dbDrumFireFluid dbFluid = new dbDrumFireFluid();
                dbFluid.SaveDrumFireFluid(SessionPS, fluidModel);
            }
            if (sizeModel != null)
            {
                dbDrumSize dbSize = new dbDrumSize();
                dbSize.SaveDrumSize(SessionPS, sizeModel);
            }
            sModel.ReliefLoad = model.ReliefLoad.ToString();
            sModel.ReliefPressure = model.ReliefPressure.ToString();
            sModel.ReliefTemperature = model.ReliefTemperature.ToString();
            sModel.ReliefMW = model.ReliefMW.ToString();
            sModel.ReliefCpCv = model.ReliefCpCv.ToString();
            sModel.ReliefZ = model.ReliefZ.ToString();
            db.Update(sModel, SessionPS);

        }
    }
}
