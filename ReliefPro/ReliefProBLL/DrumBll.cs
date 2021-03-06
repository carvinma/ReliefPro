﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProDAL.Drums;
using ReliefProModel;
using ReliefProModel.Drums;
using UOMLib;
using ReliefProDAL.HXs;
using ReliefProModel.HXs;


namespace ReliefProBLL
{
    public class DrumBLL
    {
        public List<CustomStream> Feeds=new List<CustomStream>();
        private DrumBlockedOutletDAL dbBlockedOutlet = new DrumBlockedOutletDAL();
        public int GetDrumID(ISession SessionPS)
        {
            DrumDAL db = new DrumDAL();
            Drum model = db.GetModel(SessionPS);
            if (model != null)
                return model.ID;
            return 0;
        }
        public string EqName;
        public void SaveDrumBlockedOutlet(DrumBlockedOutlet model, ISession SessionPS, double reliefLoad, double reliefMW, double reliefT, double reliefCpCv, double reliefZ)
        {

            dbBlockedOutlet.SaveDrumBlockedOutlet(SessionPS, model);
            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);

            sModel.ReliefLoad = reliefLoad;
            sModel.ReliefMW = reliefMW;
            sModel.ReliefTemperature = reliefT;
            sModel.ReliefPressure = ScenarioReliefPressure(SessionPS);
            sModel.ReliefCpCv = reliefCpCv;
            sModel.ReliefZ = reliefZ;
            db.Update(sModel, SessionPS);
            SessionPS.Flush();
        }
        public DrumBlockedOutlet GetBlockedOutletModel(ISession SessionPS, int ScenarioID, int EqType)
        {
            StreamDAL dbsteam = new StreamDAL();
            DrumBlockedOutlet Model = new DrumBlockedOutlet();

            if (EqType == 0)
            {
                DrumDAL dbdrum = new DrumDAL();
                List<Drum> lstDrum = dbdrum.GetAllList(SessionPS).ToList();
                if (lstDrum.Count() > 0)
                {
                    Model.DrumType = lstDrum[0].DrumType;
                    Model.NormalFlashDuty = lstDrum[0].Duty;
                    Model.DrumID = lstDrum[0].ID;
                    Model.FDReliefCondition = Model.NormalFlashDuty;
                    EqName = lstDrum[0].DrumName;
                }
                Feeds = dbsteam.GetAllList(SessionPS, false).ToList();
                
            }
            else
            {
                HeatExchangerDAL hxdal = new HeatExchangerDAL();
                HeatExchanger hx = hxdal.GetModel(SessionPS);
                if (hx != null)
                {
                    Model.NormalFlashDuty = hx.Duty;
                    Model.DrumID = hx.ID;
                    Model.FDReliefCondition = Model.NormalFlashDuty;
                    EqName = hx.HXName;
                }
                PSVDAL psv = new PSVDAL();

                var psvModel = psv.GetAllList(SessionPS).FirstOrDefault();
                if (psvModel != null)
                {
                    if (psvModel.LocationDescription == "Shell")
                    {
                        string[] shells = hx.ShellFeedStreams.Split(',');
                        foreach (string s in shells)
                        {
                            CustomStream cs = dbsteam.GetModel(SessionPS, s);
                            Feeds.Add(cs);
                        }
                    }
                    else
                    {
                        string[] tubes = hx.TubeFeedStreams.Split(',');
                        foreach (string s in tubes)
                        {
                            CustomStream cs = dbsteam.GetModel(SessionPS, s);
                            Feeds.Add(cs);
                        }
                    }
                }
            }



            var tmpModel = dbBlockedOutlet.GetModelByScenarioID(SessionPS, ScenarioID);
            if (tmpModel != null)
            {
                Model = tmpModel;
                return Model;
            }

            SourceDAL dbSource = new SourceDAL();
            if (Feeds.Count == 1)
            {
                Source sr = dbSource.GetModel(Feeds[0].StreamName, SessionPS);
                Model.MaxPressure = sr.MaxPossiblePressure;
                Model.MaxStreamRate = Feeds[0].WeightFlow;
            }

            Model.ScenarioID = ScenarioID;
            return Model;
        }

        public DrumBlockedOutlet ReadConvertModel(DrumBlockedOutlet model, ISession SessionPlan)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(SessionPlan);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            DrumBlockedOutlet outletModel = new DrumBlockedOutlet();
            outletModel = model;
            var SessionPT = SessionPlan;
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionDBPath == SessionPT.Connection.ConnectionString);
            outletModel.MaxPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, outletModel.MaxPressure);
            outletModel.MaxStreamRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, outletModel.MaxStreamRate);
            outletModel.NormalFlashDuty = UnitConvert.Convert(UOMLib.UOMEnum.EnthalpyDuty.ToString(), uomEnum.UserEnthalpyDuty, outletModel.NormalFlashDuty);
            outletModel.FDReliefCondition = UnitConvert.Convert(UOMLib.UOMEnum.EnthalpyDuty.ToString(), uomEnum.UserEnthalpyDuty, outletModel.FDReliefCondition);
            SessionPT.Flush();




            return outletModel;
        }

        
        public double ScenarioReliefPressure(ISession SessionPS)
        {
            PSVDAL psv = new PSVDAL();
            var psvModel = psv.GetAllList(SessionPS).FirstOrDefault();
            if (psvModel != null)
            {
                return psvModel.Pressure * psvModel.ReliefPressureFactor;
            }
            return 0;
        }
        public double PSet(ISession SessionPS)
        {
            PSVDAL psv = new PSVDAL();

            var psvModel = psv.GetAllList(SessionPS).FirstOrDefault();
            if (psvModel != null)
            {
                if (psvModel.Pressure != null)
                    return psvModel.Pressure;
            }
            return 0;
        }

    }
}
