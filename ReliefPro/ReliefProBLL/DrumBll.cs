using System;
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

            sModel.ReliefLoad = reliefLoad;
            sModel.ReliefMW = reliefMW;
            sModel.ReliefTemperature = reliefT;
            sModel.ReliefPressure = ScenarioReliefPressure(SessionPS);
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
                Model.NormalFlashDuty =lstDrum[0].Duty;
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
                if (listSource.First().MaxPossiblePressure!=null)
                {
                    Model.MaxPressure = listSource.First().MaxPossiblePressure.Value;
                }
            }

            List<CustomStream> liststream = dbsteam.GetAllList(SessionPS, false).ToList();
            if (liststream.Count() > 0)
            {
                if (liststream.First().WeightFlow!=null)
                {
                    Model.MaxStreamRate = liststream.First().WeightFlow.Value;
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
            outletModel = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(SessionPlan);
            outletModel.MaxPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserPressure, outletModel.MaxPressure.Value);
            outletModel.MaxStreamRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserWeightFlow, outletModel.MaxStreamRate.Value);
            outletModel.NormalFlashDuty = UnitConvert.Convert(UOMLib.UOMEnum.EnthalpyDuty.ToString(), uomEnum.UserEnthalpyDuty, outletModel.NormalFlashDuty.Value);
            return outletModel;
        }

        public double PfeedUpstream(ISession SessionPS)
        {
            StreamDAL stream = new StreamDAL();

            var streamModel = stream.GetAllList(SessionPS).FirstOrDefault();
            if (streamModel != null)
            {
                if (streamModel.Pressure!=null)
                    return streamModel.Pressure.Value;
            }

            return 0;
        }
        public double ScenarioReliefPressure(ISession SessionPS)
        {
            PSVDAL psv = new PSVDAL();
            var psvModel = psv.GetAllList(SessionPS).FirstOrDefault();
            if (psvModel != null)
            {
                if (psvModel.Pressure!=null)
                    return psvModel.Pressure.Value * psvModel.ReliefPressureFactor.Value;
            }
            return 0;
        }
        public double PSet(ISession SessionPS)
        {
            PSVDAL psv = new PSVDAL();

            var psvModel = psv.GetAllList(SessionPS).FirstOrDefault();
            if (psvModel != null)
            {
                if (psvModel.Pressure!=null)
                    return psvModel.Pressure.Value;
            }
            return 0;
        }

    }
}
