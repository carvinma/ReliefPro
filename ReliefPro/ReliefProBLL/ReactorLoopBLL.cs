using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.ReactorLoops;
using ReliefProModel;
using ReliefProModel.ReactorLoops;
using UOMLib;

namespace ReliefProLL
{
    public class ReactorLoopBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        private ReactorLoopBlockedOutletDAL dbBlock = new ReactorLoopBlockedOutletDAL();
        public ReactorLoopBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public ReactorLoopBlockedOutlet GetBlockedOutletModel(int ScenarioID, int ReactorType)
        {
            var model = dbBlock.GetModelByScenarioID(SessionPS, ScenarioID, ReactorType);
            if (model == null)
                model = new ReactorLoopBlockedOutlet();
            return model;
        }
        public ReactorLoopBlockedOutlet ReadConvertBlockedOutletModel(ReactorLoopBlockedOutlet model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            ReactorLoopBlockedOutlet Model = new ReactorLoopBlockedOutlet();
            Model = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            Model.EffluentTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.EffluentTemperature);
            Model.MaxGasRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.MaxGasRate);
            Model.TotalPurgeRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.TotalPurgeRate);
            Model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.ReliefLoad);
            Model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.ReliefTemperature);
            return Model;
        }
        public void SaveBlockedOutlet(ReactorLoopBlockedOutlet model)
        {
            dbBlock.Save(SessionPS, model);
            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            sModel.ReliefLoad = model.ReliefLoad.ToString();
            sModel.ReliefTemperature = model.ReliefTemperature.ToString();
            sModel.ReliefMW = model.ReliefMW.ToString();
            db.Update(sModel, SessionPS);
        }
    }
}
