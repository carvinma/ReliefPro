using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL.GlobalDefault;
using ReliefProModel;
using ReliefProModel.GlobalDefault;
using UOMLib;

namespace ReliefProBLL
{
    public class GlobalDefaultBLL
    {
        private GlobalDefaultDAL globalDefaultDAL = new GlobalDefaultDAL();
        private ISession SessionPF;
        public GlobalDefaultBLL(ISession SessionPF)
        {
            this.SessionPF = SessionPF;
        }
        public void DelFlareSystemByID(int id)
        {
            globalDefaultDAL.DelFlareSystemByID(id, SessionPF);
        }
        public void Save(List<FlareSystem> lstFlarem, ConditionsSettings conditionsSettings)
        {
            globalDefaultDAL.SaveGlobalDefault(SessionPF, lstFlarem, conditionsSettings);
        }
        public List<FlareSystem> GetFlareSystem()
        {
            return globalDefaultDAL.GetFlareSystem(SessionPF).ToList();
        }
        public ConditionsSettings GetConditionsSettings()
        {
            return globalDefaultDAL.GetConditionsSettings(SessionPF);
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
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            settingsModel.LatentHeatSettings = UnitConvert.Convert(UOMLib.UOMEnum.SpecificEnthalpy.ToString(), uomEnum.UserSpecificEnthalpy, settingsModel.LatentHeatSettings);
            settingsModel.DrumSurgeTimeSettings = UnitConvert.Convert(UOMLib.UOMEnum.Time.ToString(), uomEnum.UserTime, settingsModel.DrumSurgeTimeSettings);
            return settingsModel;
        }

    }
}
