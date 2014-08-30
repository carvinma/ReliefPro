﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Models.ReactorLoops;
using UOMLib;
using ReliefProModel;

namespace ReliefProMain.ViewModel.ReactorLoops
{
    public class ReactorLoopCommonVM
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }

        public ICommand RunCaseSimulationCMD { get; set; }
        public ICommand LaunchSimulatorCMD { get; set; }

        private ISession SessionPS;
        private ISession SessionPF;
        private SourceFile SourceFileInfo;
        private string DirPlant;
        private string DirProtectedSystem;
        public ReactorLoopCommonModel model { get; set; }
        private ReactorLoopBLL reactorBLL;
        private int reactorType;
        /// <summary>
        /// 0-ReactorLoopBlockedOutlet,1-LossOfReactorQuench,2-LossOfColdFeed
        /// </summary>
        /// <param name="ScenarioID"></param>
        /// <param name="SessionPS"></param>
        /// <param name="SessionPF"></param>
        /// <param name="ReactorType"> 0-ReactorLoopBlockedOutlet,1-LossOfReactorQuench,2-LossOfColdFeed</param>
        public ReactorLoopCommonVM(int ScenarioID, SourceFile SourceFileInfo, ISession SessionPS, ISession SessionPF, string DirPlant, string DirProtectedSystem, int ReactorType)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            this.SourceFileInfo = SourceFileInfo;
            this.DirPlant = DirPlant;
            this.DirProtectedSystem = DirProtectedSystem;
            reactorType = ReactorType;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);
            RunCaseSimulationCMD = new DelegateCommand<object>(RunCaseSimulation);
            LaunchSimulatorCMD = new DelegateCommand<object>(LaunchSimulator);

            reactorBLL = new ReactorLoopBLL(SessionPS, SessionPF);
            var blockModel = reactorBLL.GetBlockedOutletModel(ScenarioID, reactorType);
            blockModel = reactorBLL.ReadConvertBlockedOutletModel(blockModel);

            model = new ReactorLoopCommonModel(blockModel);
            model.dbmodel.ScenarioID = ScenarioID;

            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionDBPath == SessionPF.Connection.ConnectionString);
            model.EffluentTemperatureUnit = uomEnum.UserTemperature;
            model.MaxGasRateUnit = uomEnum.UserMassRate;
            model.TotalPurgeRateUnit = uomEnum.UserMassRate;
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
        }
        private void WriteConvertModel()
        {
            model.dbmodel.EffluentTemperature = UnitConvert.Convert(model.EffluentTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.EffluentTemperature);
            model.dbmodel.MaxGasRate = UnitConvert.Convert(model.MaxGasRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.MaxGasRate);
            model.dbmodel.TotalPurgeRate = UnitConvert.Convert(model.TotalPurgeRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.TotalPurgeRate);
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.ReliefCpCv = model.ReliefCpCv;
            model.dbmodel.ReliefZ = model.ReliefZ;
            model.dbmodel.ReactorType = reactorType;
        }
        private void RunCaseSimulation(object obj)
        { }
        private void LaunchSimulator(object obj)
        {
        }
        private void CalcResult(object obj)
        {
            switch (reactorType)
            {
                case 0:
                    CalcBlocket();
                    break;


            }

        }
        private void CalcBlocket()
        {
            model.ReliefLoad = model.TotalPurgeRate - model.MaxGasRate;

        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    reactorBLL.SaveBlockedOutlet(model.dbmodel);
                    wd.DialogResult = true;
                }
            }
        }
    }
}
