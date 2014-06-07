﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.Drums;
using ReliefProModel;
using ReliefProModel.Drums;
using UOMLib;

namespace ReliefProLL
{
    public class DrumFireBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private DrumFireDAL dbdrumFire = new DrumFireDAL();
        public DrumFireBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public DrumFireCalc GetDrumFireModel(int ScenarioID)
        {
            DrumFireCalc firemodel = new DrumFireCalc();
            DrumFireDAL drumFire = new DrumFireDAL();
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
            ScenarioDAL db = new ScenarioDAL();
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
            fireModel = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            fireModel.WettedArea = UnitConvert.Convert(UOMLib.UOMEnum.Area.ToString(), uomEnum.UserArea, fireModel.WettedArea);
            fireModel.LatentHeat = UnitConvert.Convert(UOMLib.UOMEnum.SpecificEnthalpy.ToString(), uomEnum.UserSpecificEnthalpy, fireModel.LatentHeat);
            fireModel.CrackingHeat = UnitConvert.Convert(UOMLib.UOMEnum.SpecificEnthalpy.ToString(), uomEnum.UserSpecificEnthalpy, fireModel.CrackingHeat);
            fireModel.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserWeightFlow, fireModel.ReliefLoad);
            fireModel.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, fireModel.ReliefPressure);
            fireModel.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, fireModel.ReliefTemperature);
            return fireModel;
        }

        public void SaveData(DrumFireCalc model, DrumFireFluid fluidModel, DrumSize sizeModel, ISession SessionPS)
        {

            dbdrumFire.SaveDrumFireCalc(SessionPS, model);
            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            if (fluidModel != null)
            {
                fluidModel.DrumFireCalcID = model.ID;
                DrumFireFluidDAL dbFluid = new DrumFireFluidDAL();
                dbFluid.SaveDrumFireFluid(SessionPS, fluidModel);
            }
            if (sizeModel != null)
            {
                sizeModel.DrumFireCalcID = model.ID;
                DrumSizeDAL dbSize = new DrumSizeDAL();
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
