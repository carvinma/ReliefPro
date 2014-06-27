using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL.GlobalDefault;
using ReliefProModel;
using ReliefProModel.GlobalDefault;
using UOMLib;

namespace ReliefProLL
{
    public class GlobalDefaultBLL
    {
        private GlobalDefaultDAL globalDefaultDAL = new GlobalDefaultDAL();
        private ISession SessionPS;
        private ISession SessionPF;
        public GlobalDefaultBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public void DelFlareSystemByID(int id)
        {
            globalDefaultDAL.DelFlareSystemByID(id, SessionPS);
        }
        public void Save(List<FlareSystem> lstFlarem, ConditionsSettings conditionsSettings)
        {
            globalDefaultDAL.SaveGlobalDefault(SessionPS, lstFlarem, conditionsSettings);
        }
        public IList<FlareSystem> GetFlareSystem()
        {
            return globalDefaultDAL.GetFlareSystem(SessionPS);
        }
        public ConditionsSettings GetConditionsSettings()
        {
            return globalDefaultDAL.GetConditionsSettings(SessionPS);
        }
        public ConditionsSettings ReadConvertModel(ConditionsSettings model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            ConditionsSettings settingsModel = new ConditionsSettings();
            settingsModel = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(SessionPF);
            if (settingsModel.LatentHeatSettings != null)
                settingsModel.LatentHeatSettings = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), uomEnum.UserTime, settingsModel.LatentHeatSettings.Value);
            if (settingsModel.DrumSurgeTimeSettings != null)
                settingsModel.DrumSurgeTimeSettings = UnitConvert.Convert(UOMLib.UOMEnum.Time.ToString(), uomEnum.UserTime, settingsModel.DrumSurgeTimeSettings.Value);
            return settingsModel;
        }

    }
}
