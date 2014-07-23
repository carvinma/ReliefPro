﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ReactorLoopCommonDAL dbBlock = new ReactorLoopCommonDAL();
        private ReactorLoopDAL reactorLoopDAL = new ReactorLoopDAL();
        public ReactorLoopBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public ReactorLoopCommon GetBlockedOutletModel(int ScenarioID, int ReactorType)
        {
            var model = dbBlock.GetModelByScenarioID(SessionPS, ScenarioID, ReactorType);
            if (model == null)
                model = new ReactorLoopCommon();
            return model;
        }
        public ReactorLoopCommon ReadConvertBlockedOutletModel(ReactorLoopCommon model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            ReactorLoopCommon Model = new ReactorLoopCommon();
            Model = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            Model.EffluentTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.EffluentTemperature.Value);
            Model.MaxGasRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.MaxGasRate.Value);
            Model.TotalPurgeRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.TotalPurgeRate.Value);
            Model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), uomEnum.UserMassRate, Model.ReliefLoad.Value);
            Model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), uomEnum.UserTemperature, Model.ReliefTemperature.Value);
            return Model;
        }
        public void SaveBlockedOutlet(ReactorLoopCommon model)
        {
            dbBlock.Save(SessionPS, model);
            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            sModel.ReliefLoad = model.ReliefLoad;
            sModel.ReliefTemperature = model.ReliefTemperature;
            sModel.ReliefMW = model.ReliefMW;
            db.Update(sModel, SessionPS);
        }

        public ReactorLoop GetReactorLoopModel(int ScenarioID)
        {
            var model = reactorLoopDAL.GetModelByScenarioID(SessionPS, ScenarioID);
            if (model == null)
                model = new ReactorLoop();
            return model;
        }

        public ObservableCollection<ReactorLoopDetail> GetProcessHX(int ReactorLoopID)
        {
            var lst = reactorLoopDAL.GetReactorLoopDetail(SessionPS, ReactorLoopID, 0);
            ObservableCollection<ReactorLoopDetail> tObject = new ObservableCollection<ReactorLoopDetail>(lst);
            return tObject;
        }
        public ObservableCollection<ReactorLoopDetail> GetUtilityHX(int ReactorLoopID)
        {
            var lst = reactorLoopDAL.GetReactorLoopDetail(SessionPS, ReactorLoopID, 1);
            ObservableCollection<ReactorLoopDetail> tObject = new ObservableCollection<ReactorLoopDetail>(lst);
            return tObject;
        }
        public ObservableCollection<ReactorLoopDetail> GetMixerSplitter(int ReactorLoopID)
        {
            var lst = reactorLoopDAL.GetReactorLoopDetail(SessionPS, ReactorLoopID, 2);
            ObservableCollection<ReactorLoopDetail> tObject = new ObservableCollection<ReactorLoopDetail>(lst);
            return tObject;
        }

        public void Save(ReactorLoop model, ObservableCollection<ReactorLoopDetail> obcReactorLoopDetail)
        {
            IList<ReactorLoopDetail> lst = new List<ReactorLoopDetail>(obcReactorLoopDetail);
            reactorLoopDAL.Save(SessionPS, model, lst);
        }
    }
}
