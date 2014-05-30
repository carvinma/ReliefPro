using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProDAL.Drum;
using ReliefProModel;
using ReliefProModel.Drum;
using UOMLib;


namespace ReliefProBLL
{
    public class DrumBll
    {
        public IList<CustomStream> Feeds;
        private DrumBlockedOutletDAL dbBlockedOutlet = new DrumBlockedOutletDAL();
        public int GetDrumID(ISession SessionPS)
        {
            DrumDAL db = new DrumDAL();
            Drum model = db.GetModel(SessionPS);
            if (model != null)
                return model.ID;
            return 0;
        }
        public void SaveDrumBlockedOutlet(DrumBlockedOutlet model, ISession SessionPS, double reliefLoad, double reliefMW, double reliefT)
        {

            dbBlockedOutlet.SaveDrumBlockedOutlet(SessionPS, model);
            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);

            sModel.ReliefLoad = reliefLoad.ToString();
            sModel.ReliefMW = reliefMW.ToString();
            sModel.ReliefTemperature = reliefT.ToString();
            sModel.ReliefPressure = ScenarioReliefPressure(SessionPS).ToString();
            db.Update(sModel, SessionPS);

        }
        public DrumBlockedOutlet GetBlockedOutletModel(ISession SessionPS)
        {
            StreamDAL dbsteam = new StreamDAL();
            DrumBlockedOutlet Model = new DrumBlockedOutlet();
            DrumDAL dbdrum = new DrumDAL();
            List<Drum> lstDrum = dbdrum.GetAllList(SessionPS).ToList();
            if (lstDrum.Count() > 0)
            {
                Model.DrumType = lstDrum[0].DrumType;
                Model.NormalFlashDuty = double.Parse(lstDrum[0].Duty);
                Model.DrumID = lstDrum[0].ID;
            }

            Feeds = dbsteam.GetAllList(SessionPS, true);
            
            var tmpModel = dbBlockedOutlet.GetModelByDrumID(SessionPS, Model.DrumID);
            if (tmpModel != null)
            {
                Model = tmpModel;
                return Model;
            }
            SourceDAL dbSource = new SourceDAL();
            List<Source> listSource = dbSource.GetAllList(SessionPS).ToList();
            if (listSource.Count() > 0)
            {
                double MaxPressure = 0;
                if (double.TryParse(listSource.First().MaxPossiblePressure, out MaxPressure))
                {
                    Model.MaxPressure = MaxPressure;
                }
            }

            List<CustomStream> liststream = dbsteam.GetAllList(SessionPS, false).ToList();
            if (liststream.Count() > 0)
            {
                double MaxStreamRate = 0;
                if (double.TryParse(liststream.First().WeightFlow, out MaxStreamRate))
                {
                    Model.MaxStreamRate = MaxStreamRate;
                }
            }
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
            UnitConvert uc = new UnitConvert();
            outletModel = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(SessionPlan);
            outletModel.MaxPressure = uc.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, outletModel.MaxPressure);
            outletModel.MaxStreamRate = uc.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserWeightFlow, outletModel.MaxStreamRate);
            outletModel.NormalFlashDuty = uc.Convert(UOMLib.UOMEnum.EnthalpyDuty.ToString(), uomEnum.UserEnthalpyDuty, outletModel.NormalFlashDuty);
            return outletModel;
        }

        public double PfeedUpstream(ISession SessionPS)
        {
            StreamDAL stream = new StreamDAL();

            var streamModel = stream.GetAllList(SessionPS).FirstOrDefault();
            if (streamModel != null)
            {
                if (!string.IsNullOrEmpty(streamModel.Pressure))
                    return double.Parse(streamModel.Pressure);
            }

            return 0;
        }
        public double ScenarioReliefPressure(ISession SessionPS)
        {
            PSVDAL psv = new PSVDAL();
            var psvModel = psv.GetAllList(SessionPS).FirstOrDefault();
            if (psvModel != null)
            {
                if (!string.IsNullOrEmpty(psvModel.Pressure))
                    return double.Parse(psvModel.Pressure) * double.Parse(psvModel.ReliefPressureFactor);
            }
            return 0;
        }
        public double PSet(ISession SessionPS)
        {
            PSVDAL psv = new PSVDAL();

            var psvModel = psv.GetAllList(SessionPS).FirstOrDefault();
            if (psvModel != null)
            {
                if (!string.IsNullOrEmpty(psvModel.Pressure))
                    return double.Parse(psvModel.Pressure);
            }
            return 0;
        }

    }
}
