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
        public CustomStream VaporStream;
        private dbDrumBlockedOutlet dbBlockedOutlet = new dbDrumBlockedOutlet();
        public int GetDrumID(ISession SessionPS)
        {
            dbDrum db = new dbDrum();
            Drum model = db.GetModel(SessionPS);
            if (model != null)
                return model.ID;
            return 0;
        }
        public void SaveDrumBlockedOutlet(DrumBlockedOutlet model, ISession SessionPS, double reliefLoad, double reliefMW, double reliefT)
        {

            dbBlockedOutlet.SaveDrumBlockedOutlet(SessionPS, model);
            dbScenario db = new dbScenario();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);

            sModel.ReliefLoad = reliefLoad.ToString();
            sModel.ReliefMW = reliefMW.ToString();
            sModel.ReliefTemperature = reliefT.ToString();
            sModel.ReliefPressure = ScenarioReliefPressure(SessionPS).ToString();
            db.Update(sModel, SessionPS);

        }
        public DrumBlockedOutlet GetBlockedOutletModel(ISession SessionPS)
        {
            dbStream dbsteam = new dbStream();
            DrumBlockedOutlet Model = new DrumBlockedOutlet();
            dbDrum dbdrum = new dbDrum();
            List<Drum> lstDrum = dbdrum.GetAllList(SessionPS).ToList();
            if (lstDrum.Count() > 0)
            {
                Model.DrumType = lstDrum[0].DrumType;
                Model.NormalFlashDuty = double.Parse(lstDrum[0].Duty);
                Model.DrumID = lstDrum[0].ID;
            }

            List<CustomStream> listvaporstream = dbsteam.GetAllList(SessionPS, true).ToList();
            if (listvaporstream.Count() > 0)
            {
                foreach(CustomStream cs in listvaporstream)
                {
                    if (cs.ProdType == "1")
                    {
                        VaporStream = cs;
                    }
                }
            }

            var tmpModel = dbBlockedOutlet.GetModelByDrumID(SessionPS, Model.DrumID);
            if (tmpModel != null)
            {
                Model = tmpModel;
                return Model;
            }
            dbSource dbSource = new dbSource();
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
            outletModel.MaxPressure = uc.Convert(uomEnum.UserPressure, UOMLib.UOMEnum.Pressure.ToString(), outletModel.MaxPressure);
            outletModel.MaxStreamRate = uc.Convert(uomEnum.UserWeightFlow, UOMLib.UOMEnum.WeightFlow.ToString(), outletModel.MaxStreamRate);
            outletModel.NormalFlashDuty = uc.Convert(uomEnum.UserEnthalpyDuty, UOMLib.UOMEnum.EnthalpyDuty.ToString(), outletModel.NormalFlashDuty);
            return outletModel;
        }

        public double PfeedUpstream(ISession SessionPS)
        {
            dbStream stream = new dbStream();

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
            dbPSV psv = new dbPSV();
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
            dbPSV psv = new dbPSV();

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
