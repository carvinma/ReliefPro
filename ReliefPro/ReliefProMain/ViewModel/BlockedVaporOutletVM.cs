﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProMain.Models;
using ReliefProModel.Towers;
using UOMLib;
using ReliefProDAL;
using ReliefProModel;
using ReliefProBLL;

namespace ReliefProMain.ViewModel
{
    public class BlockedVaporOutletVM : ViewModelBase
    {
        private BlockedVaporOutletBLL blockBLL;
        public BlockedVaporOutletModel model { get; set; }
        private ISession SessionPF;
        private ISession SessionPS;
        public int OutletType;
        public ICommand OKCMD { get; set; }
        public ICommand CalculateCommand { get; set; }

        public BlockedVaporOutletVM(ISession SessionPF, ISession SessinPS, int ScenarioID, int OutletType)
        {
            OKCMD = new DelegateCommand<object>(Save);
            CalculateCommand = new DelegateCommand<object>(Calculate);
            this.SessionPF = SessionPF;
            this.SessionPS = SessinPS;
            blockBLL = new BlockedVaporOutletBLL(SessionPF, SessinPS);

            var BlockedModel = blockBLL.GeModel(ScenarioID, OutletType);
            var ScenarioModel = blockBLL.GetScenarioModel(ScenarioID);

            CustomStreamDAL csdal = new CustomStreamDAL();
            SourceDAL srdal = new SourceDAL();
            if (BlockedModel == null)
            {
                BlockedModel = new BlockedVaporOutlet();
                IList<CustomStream> feeds = csdal.GetAllList(this.SessionPS,false);
                IList<CustomStream> products = csdal.GetAllList(this.SessionPS,true);
                foreach (CustomStream cs in feeds)
                {
                    if (cs.VaporFraction == 1)
                    {
                        Source sr = srdal.GetModel(cs.StreamName, SessionPS);
                        BlockedModel.InletGasUpstreamMaxPressure = sr.MaxPossiblePressure;
                        BlockedModel.NormalGasFeedWeightRate = cs.WeightFlow;
                        ScenarioModel.ReliefMW = cs.BulkMwOfPhase;
                        
                        ScenarioModel.ReliefTemperature = cs.Temperature;
                        ScenarioModel.ReliefCpCv = cs.BulkCPCVRatio;
                        ScenarioModel.ReliefZ = cs.VaporZFmKVal;
                    }
                    else
                    {
                        Source sr = srdal.GetModel(cs.StreamName, SessionPS);
                        BlockedModel.InletAbsorbentUpstreamMaxPressure = sr.MaxPossiblePressure;
                    }
                    
                }
                foreach (CustomStream cs in products)
                {
                    if (cs.VaporFraction == 1)
                    {
                        BlockedModel.NormalGasProductWeightRate = cs.WeightFlow;
                        ScenarioModel.ReliefMW = cs.BulkMwOfPhase;
                        
                        ScenarioModel.ReliefTemperature = cs.Temperature;
                        ScenarioModel.ReliefCpCv = cs.BulkCPCVRatio;
                        ScenarioModel.ReliefZ = cs.VaporZFmKVal;             
                    }
                }
            }
            BlockedModel = blockBLL.ReadConvertBlockedVaporOutletModel(BlockedModel);
            ScenarioModel = blockBLL.ReadConvertScenarioModel(ScenarioModel);
            model = new BlockedVaporOutletModel(BlockedModel, ScenarioModel);
            model.dbmodel.ScenarioID = ScenarioID;
            model.dbmodel.OutletType = OutletType;
            InitUnit();
        }
        private void InitUnit()
        {
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
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
        private void Calculate(object obj)
        {
            if (!model.CheckData()) return;
            PSVDAL psvdal = new PSVDAL();
            PSV psv = psvdal.GetModel(SessionPS);
            double pSet = psv.Pressure ;//和定压比较。不是ReliefPressure
            model.ReliefPressure = pSet;
            if (model.dbmodel.OutletType == 0)
            {
                if (model.InletGasUpstreamMaxPressure > pSet)
                {
                    if (model.InletAbsorbentUpstreamMaxPressure > pSet)
                    {
                        model.ReliefLoad = model.NormalGasProductWeightRate;
                    }
                    else
                    {
                        model.ReliefLoad = model.NormalGasFeedWeightRate;
                    }
                }
                else
                {
                    model.ReliefLoad = 0;
                }
                
            }
            else
            {
                if (model.InletGasUpstreamMaxPressure > pSet)
                {
                    model.ReliefLoad = model.NormalGasFeedWeightRate - model.NormalGasProductWeightRate;
                }
                else
                {
                    model.ReliefLoad = 0;
                }
                
            }
            if (model.ReliefLoad < 0)
                model.ReliefLoad = 0;

        }
        private void Save(object obj)
        {
            if (!model.CheckData()) return;
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
