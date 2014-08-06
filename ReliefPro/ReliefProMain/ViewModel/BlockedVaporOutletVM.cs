﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Model;
using ReliefProModel.Towers;
using UOMLib;

namespace ReliefProMain.ViewModel
{
    public class BlockedVaporOutletVM : ViewModelBase
    {
        private BlockedVaporOutletBLL blockBLL;
        public BlockedVaporOutletModel model { get; set; }
        private ISession SessionPF;

        public ICommand OKCMD { get; set; }

        public BlockedVaporOutletVM(ISession SessionPF, ISession SessinPS, int ScenarioID, int OutletType)
        {
            OKCMD = new DelegateCommand<object>(Save);
            this.SessionPF = SessionPF;
            blockBLL = new BlockedVaporOutletBLL(SessionPF, SessinPS);

            var BlockedModel = blockBLL.GeModel(ScenarioID, OutletType);
            var ScenarioModel = blockBLL.GetScenarioModel(ScenarioID);

            BlockedModel = blockBLL.ReadConvertBlockedVaporOutletModel(BlockedModel);
            ScenarioModel = blockBLL.ReadConvertScenarioModel(ScenarioModel);
            model = new BlockedVaporOutletModel(BlockedModel, ScenarioModel);
            model.dbmodel.ScenarioID = ScenarioID;
            model.dbmodel.OutletType = OutletType;
            InitUnit();
        }
        private void InitUnit()
        {
            UOMLib.UOMEnum uomEnum = new UOMEnum(SessionPF);
            model.InletGasUpstreamMaxPressureUnit = uomEnum.UserPressure;
            model.InletAbsorbentUpstreamMaxPressureUnit = uomEnum.UserPressure;
            model.NormalGasFeedWeightRateUnit = uomEnum.UserMassRate;
            model.NormalGasProductWeightRateUnit = uomEnum.UserMassRate;

            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefPressureUnit = uomEnum.UserPressure;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
        }
        private void WriteConvertModel()
        {
            model.dbmodel.InletGasUpstreamMaxPressure = UnitConvert.Convert(model.InletGasUpstreamMaxPressureUnit, UOMLib.UOMEnum.Pressure, model.InletGasUpstreamMaxPressure);
            model.dbmodel.InletAbsorbentUpstreamMaxPressure = UnitConvert.Convert(model.InletAbsorbentUpstreamMaxPressureUnit, UOMLib.UOMEnum.Pressure, model.InletAbsorbentUpstreamMaxPressure);
            model.dbmodel.NormalGasFeedWeightRate = UnitConvert.Convert(model.NormalGasFeedWeightRateUnit, UOMLib.UOMEnum.MassRate, model.NormalGasFeedWeightRate);
            model.dbmodel.NormalGasProductWeightRate = UnitConvert.Convert(model.NormalGasProductWeightRateUnit, UOMLib.UOMEnum.MassRate, model.NormalGasProductWeightRate);

            model.dbScenario.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate, model.ReliefLoad);
            model.dbScenario.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure, model.ReliefPressure);
            model.dbScenario.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature, model.ReliefTemperature);
        }
        private void Save(object obj)
        {
            WriteConvertModel();
            blockBLL.Save(model.dbmodel, model.dbScenario);
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    wd.DialogResult = true;
                }
            }
        }
    }
}