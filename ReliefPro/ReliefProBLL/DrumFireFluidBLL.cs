using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.Drums;
using ReliefProModel;
using ReliefProModel.Drums;
using UOMLib;

namespace ReliefProBLL
{
    public class DrumFireFluidBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        public DrumFireFluidBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public DrumFireFluid GetFireFluidModel(int DrumFireCalcID)
        {
            DrumFireFluid Model = new DrumFireFluid();
            DrumFireFluidDAL dbfire = new DrumFireFluidDAL();
            List<DrumFireFluid> lstDrumFireFluid = dbfire.GetAllList(SessionPS).ToList();
            if (DrumFireCalcID > 0)
            {
                var findModel = lstDrumFireFluid.Where(p => p.DrumFireCalcID == DrumFireCalcID).FirstOrDefault();
                if (findModel != null)
                    return findModel;
            }
            else
            {
                var info = GetFluidInfo();
                Model.GasVaporMW = info.Item1;
                Model.NormalPressure = info.Item3;
                Model.NormaTemperature = info.Item2;
                Model.PSVPressure = info.Item4;
                Model.TW = 593;
            }
            return Model;
        }
        public DrumFireFluid ReadConvertModel(DrumFireFluid model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            DrumFireFluid fireModel = new DrumFireFluid();
            fireModel = model;
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionDBPath == SessionPF.Connection.ConnectionString);
            fireModel.ExposedVesse = UnitConvert.Convert(UOMLib.UOMEnum.Area.ToString(), uomEnum.UserArea, fireModel.ExposedVesse);
            fireModel.NormaTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, fireModel.NormaTemperature);
            fireModel.NormalPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, fireModel.NormalPressure);
            fireModel.PSVPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, fireModel.PSVPressure);
            fireModel.TW = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, fireModel.TW);
            return fireModel;
        }
        private Tuple<double, double, double, double> GetFluidInfo()
        {
            double s = 0, drumt = 0, drump = 0, psv = 0;
            StreamDAL dbs = new StreamDAL();
            var lstStream = dbs.GetAllList(SessionPS).Where(p => p.IsProduct == true && p.ProdType == "1").ToList();
            if (lstStream.Count > 0)
            {
                s = lstStream[0].BulkMwOfPhase;
            }
            DrumDAL dbd = new DrumDAL();
            var lstDrum = dbd.GetAllList(SessionPS);
            if (lstDrum.Count > 0)
            {
                drumt = lstDrum[0].Temperature;
                drump = lstDrum[0].Pressure;
            }
            PSVDAL dbpsv = new PSVDAL();
            var lstPsv = dbpsv.GetAllList(SessionPS);
            if (lstPsv.Count > 0)
            {
                psv = lstPsv[0].Pressure;
            }
            return Tuple.Create(s, drumt, drump, psv);
        }
    }
}
