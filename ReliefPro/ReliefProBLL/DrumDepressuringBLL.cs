using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.Drums;
using ReliefProModel;
using ReliefProModel.Drums;
using UOMLib;

namespace ReliefProBLL
{
    public class DrumDepressuringBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private DrumDepressuringDAL dbdrum = new DrumDepressuringDAL();
        public CustomStream DrumVaporStream=null;
        public DrumDepressuringBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            DrumVaporStream = GetVaporStream();
        }
        public DrumDepressuring GetDrumPressuring(int ScenarioID)
        {
            DrumDepressuring drumModel = new DrumDepressuring();
            List<DrumDepressuring> lstDrum = dbdrum.GetAllList(SessionPS).ToList();
            if (lstDrum.Count() > 0)
            {
                drumModel = lstDrum.Where(p => p.ScenarioID == ScenarioID).FirstOrDefault();
            }
            if (drumModel != null && drumModel.ID > 0)
            {
                return drumModel;
            }
            else
            {
                drumModel = new DrumDepressuring();
                drumModel.VaporDensity = DrumVaporStream.BulkDensityAct;
                drumModel.ScenarioID = ScenarioID;
            }
            return drumModel;
        }
        public DrumDepressuring ReadConvertModel(DrumDepressuring model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            DrumDepressuring drumModel = new DrumDepressuring();
            drumModel = model;
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            drumModel.InitialPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, drumModel.InitialPressure);
            drumModel.VaporDensity = UnitConvert.Convert(UOMLib.UOMEnum.Density.ToString(), uomEnum.UserDensity, drumModel.VaporDensity);
            drumModel.TotalVaporVolume = UnitConvert.Convert(UOMLib.UOMEnum.Volume.ToString(), uomEnum.UserVolume, drumModel.TotalVaporVolume);
            drumModel.Vesseldesignpressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, drumModel.Vesseldesignpressure);
            drumModel.TotalWettedArea = UnitConvert.Convert(UOMLib.UOMEnum.Area.ToString(), uomEnum.UserArea, drumModel.TotalWettedArea);
            // drumModel.ValveConstantforSonicFlow = uc.Convert(uomEnum.UserPressure, UOMLib.UOMEnum.Pressure.ToString(), drumModel.ValveConstantforSonicFlow);
            drumModel.InitialDepressuringRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, drumModel.InitialDepressuringRate);
            drumModel.Timespecify = UnitConvert.Convert(UOMLib.UOMEnum.Time.ToString(), uomEnum.UserTime, drumModel.Timespecify);
            drumModel.CalculatedVesselPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, drumModel.CalculatedVesselPressure);
            drumModel.CalculatedDepressuringRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, drumModel.CalculatedDepressuringRate);
            return drumModel;
        }

        public void SaveData(DrumDepressuring model, ISession SessionPS)
        {
            dbdrum.SaveDrumPressuring(SessionPS, model);
        }

        private CustomStream GetVaporStream()
        {
            CustomStream cs = null;
            StreamDAL dbs = new StreamDAL();
            var lstStream = dbs.GetAllList(SessionPS).Where(p => p.IsProduct == true && p.ProdType == "1").ToList();
            if (lstStream.Count > 0)
            {
                cs = lstStream[0];
            }
            return cs;
        }
    }
}
