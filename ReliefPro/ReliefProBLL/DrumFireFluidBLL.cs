using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.Drum;
using ReliefProModel;
using ReliefProModel.Drum;
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
            dbDrumFireFluid dbfire = new dbDrumFireFluid();
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
                Model.NormalPressure = info.Item2;
                Model.NormaTemperature = info.Item3;
                Model.PSVPressure = info.Item4;
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
            UnitConvert uc = new UnitConvert();
            fireModel = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            fireModel.ExposedVesse = uc.Convert(UOMLib.UOMEnum.Area.ToString(), uomEnum.UserArea, fireModel.ExposedVesse);
            fireModel.NormaTemperature = uc.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, fireModel.NormaTemperature);
            fireModel.NormalPressure = uc.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, fireModel.NormalPressure);
            fireModel.PSVPressure = uc.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, fireModel.PSVPressure);
            return fireModel;
        }
        private Tuple<double, double, double, double> GetFluidInfo()
        {
            double s = 0, drumt = 0, drump = 0, psv = 0;
            dbStream dbs = new dbStream();
            var lstStream = dbs.GetAllList(SessionPS).Where(p => p.IsProduct == true && p.ProdType == "1").ToList();
            if (lstStream.Count > 0)
            {
                double.TryParse(lstStream[0].BulkMwOfPhase, out s);
            }
            dbDrum dbd = new dbDrum();
            var lstDrum = dbd.GetAllList(SessionPS);
            if (lstDrum.Count > 0)
            {
                double.TryParse(lstDrum[0].Temperature, out drumt);
                double.TryParse(lstDrum[0].Temperature, out drump);
            }
            dbPSV dbpsv = new dbPSV();
            var lstPsv = dbpsv.GetAllList(SessionPS);
            if (lstPsv.Count > 0)
            {
                double.TryParse(lstPsv[0].Pressure, out psv);
            }
            return Tuple.Create(s, drumt, drump, psv);
        }
    }
}
