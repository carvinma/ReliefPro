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
    public class DrumDepressuringBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private DrumDepressuringDAL dbdrum = new DrumDepressuringDAL();
        public DrumDepressuringBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public DrumDepressuring GetDrumPressuring(int ScenarioID)
        {
            DrumDepressuring drumModel = new DrumDepressuring();
            List<DrumDepressuring> lstDrum = dbdrum.GetAllList(SessionPS).ToList();
            if (lstDrum.Count() > 0)
            {
                drumModel = lstDrum.Where(p => p.ScenarioID == ScenarioID).FirstOrDefault();
            }
            if (drumModel.ID > 0)
                return drumModel;
            else
            {
                drumModel = new DrumDepressuring();
                drumModel.VaporDensity = GetStreamVaporDensity();
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
            UnitConvert uc = new UnitConvert();
            drumModel = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            drumModel.InitialPressure = uc.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, drumModel.InitialPressure);
            if (drumModel.VaporDensity != null)
                drumModel.VaporDensity = uc.Convert(UOMLib.UOMEnum.Density.ToString(), uomEnum.UserDensity, drumModel.VaporDensity.Value);
            drumModel.TotalVaporVolume = uc.Convert(UOMLib.UOMEnum.Volume.ToString(), uomEnum.UserVolume, drumModel.TotalVaporVolume);
            drumModel.Vesseldesignpressure = uc.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, drumModel.Vesseldesignpressure);
            drumModel.TotalWettedArea = uc.Convert(UOMLib.UOMEnum.Area.ToString(), uomEnum.UserArea, drumModel.TotalWettedArea);
            // drumModel.ValveConstantforSonicFlow = uc.Convert(uomEnum.UserPressure, UOMLib.UOMEnum.Pressure.ToString(), drumModel.ValveConstantforSonicFlow);
            drumModel.InitialDepressuringRate = uc.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserWeightFlow, drumModel.InitialDepressuringRate);
            drumModel.Timespecify = uc.Convert(UOMLib.UOMEnum.Time.ToString(), uomEnum.UserTime, drumModel.Timespecify);
            drumModel.CalculatedVesselPressure = uc.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, drumModel.CalculatedVesselPressure);
            drumModel.CalculatedDepressuringRate = uc.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserWeightFlow, drumModel.CalculatedDepressuringRate);
            return drumModel;
        }

        public void SaveData(DrumDepressuring model, ISession SessionPS)
        {
            dbdrum.SaveDrumPressuring(SessionPS, model);
        }

        private double? GetStreamVaporDensity()
        {
            StreamDAL dbs = new StreamDAL();
            var lstStream = dbs.GetAllList(SessionPS).Where(p => p.IsProduct == true && p.ProdType == "1").ToList();
            if (lstStream.Count > 0)
            {
                double s = 0;
                double.TryParse(lstStream[0].BulkDensityAct, out s);
                return s;
            }
            return null;
        }
    }
}
