using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Model.ReactorLoops;
using UOMLib;

namespace ReliefProMain.ViewModel.ReactorLoops
{
    public class ReactorLoopBlockedOutletVM
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }

        public ICommand RunCaseSimulationCMD { get; set; }
        public ICommand LaunchSimulatorCMD { get; set; }

        private ISession SessionPS;
        private ISession SessionPF;
        public ReactorLoopBlockedOutletModel model { get; set; }
        private ReactorLoopBLL reactorBLL;
        private int reactorType;
        /// <summary>
        /// 0-ReactorLoopBlockedOutlet,1-LossOfReactorQuench,2-LossOfColdFeed
        /// </summary>
        /// <param name="ScenarioID"></param>
        /// <param name="SessionPS"></param>
        /// <param name="SessionPF"></param>
        /// <param name="ReactorType"> 0-ReactorLoopBlockedOutlet,1-LossOfReactorQuench,2-LossOfColdFeed</param>
        public ReactorLoopBlockedOutletVM(int ScenarioID, ISession SessionPS, ISession SessionPF, int ReactorType)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            reactorType = ReactorType;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);
            RunCaseSimulationCMD = new DelegateCommand<object>(RunCaseSimulation);
            LaunchSimulatorCMD = new DelegateCommand<object>(LaunchSimulator);

            reactorBLL = new ReactorLoopBLL(SessionPS, SessionPF);
            var blockModel = reactorBLL.GetBlockedOutletModel(ScenarioID, reactorType);
            blockModel = reactorBLL.ReadConvertBlockedOutletModel(blockModel);

            model = new ReactorLoopBlockedOutletModel(blockModel);
            model.dbmodel.ScenarioID = ScenarioID;

            UOMLib.UOMEnum uomEnum = new UOMLib.UOMEnum(SessionPF);
            model.EffluentTemperatureUnit = uomEnum.UserTemperature;
            model.MaxGasRateUnit = uomEnum.UserMassRate;
            model.TotalPurgeRateUnit = uomEnum.UserMassRate;
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
        }
        private void WriteConvertModel()
        {
            UnitConvert uc = new UnitConvert();
            model.dbmodel.EffluentTemperature = uc.Convert(model.EffluentTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.EffluentTemperature);
            model.dbmodel.MaxGasRate = uc.Convert(model.MaxGasRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.MaxGasRate);
            model.dbmodel.TotalPurgeRate = uc.Convert(model.TotalPurgeRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.TotalPurgeRate);
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefLoad = uc.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad);
            model.dbmodel.ReliefTemperature = uc.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
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
