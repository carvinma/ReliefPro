using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Model.ReactorLoops;
using ReliefProModel.ReactorLoops;

namespace ReliefProMain.ViewModel.ReactorLoops
{
    public class GeneralFailureCommonVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public ICommand CoolingRunCaseSimulationCMD { get; set; }
        public ICommand CoolingLaunchSimulatorCMD { get; set; }

        public ICommand ElectricRunCaseSimulationCMD { get; set; }
        public ICommand ElectricLaunchSimulatorCMD { get; set; }

        private ISession SessionPS;
        private ISession SessionPF;

        private int reactorLoopID;
        public GeneralFailureCommonModel model { get; set; }
        private GeneralFailureCommonBLL generalBLL;

        private void InitCMD()
        {
            OKCMD = new DelegateCommand<object>(Save);
            CoolingRunCaseSimulationCMD = new DelegateCommand<object>(CoolingRunCaseSimulation);
            CoolingLaunchSimulatorCMD = new DelegateCommand<object>(CoolingLaunchSimulator);

            ElectricRunCaseSimulationCMD = new DelegateCommand<object>(ElectricRunCaseSimulation);
            ElectricLaunchSimulatorCMD = new DelegateCommand<object>(ElectricLaunchSimulator);
        }
        private void InitPage()
        {
            UOMLib.UOMEnum uomEnum = new UOMLib.UOMEnum(SessionPF);
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefPressureUnit = uomEnum.UserPressure;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ScenarioID"></param>
        /// <param name="SessionPS"></param>
        /// <param name="SessionPF"></param>
        /// <param name="GeneralType">0-GeneralCoolingWaterFailure,1-GeneralElectricPowerFailure</param>
        public GeneralFailureCommonVM(int ScenarioID, ISession SessionPS, ISession SessionPF, int GeneralType)
        {
            model = new GeneralFailureCommonModel();
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            InitCMD();
            InitPage();

            generalBLL = new GeneralFailureCommonBLL(SessionPS, SessionPF);
            GeneralFailureCommon commonModel = new GeneralFailureCommon();
            if (GeneralType == 0)
                commonModel = generalBLL.GetGeneralCoolingWaterFailureModel(ScenarioID);
            else if (GeneralType == 1)
                commonModel = generalBLL.GetGeneralElectricPowerFailureModel(ScenarioID);

            model.dbModel = generalBLL.ReadConvert(commonModel);
            if (commonModel.ID > 0)
            {
                model.lstUtilityHX = new List<UtilityHXModel>();
                IList<GeneralFailureCommonDetail> lstCommonDetail = generalBLL.GetGeneralFailureCommonDetail(commonModel.ID);
                model.lstUtilityHX = lstCommonDetail.Select(p => new UtilityHXModel { HXName = p.HXName, Stop = p.Stop, DutyFactor = p.DutyFactor }).ToList();
            }
        }
        private void CoolingRunCaseSimulation(object obj)
        { }
        private void CoolingLaunchSimulator(object obj)
        { }
        private void ElectricRunCaseSimulation(object obj)
        { }
        private void ElectricLaunchSimulator(object obj)
        { }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    var lstCommonDetail = model.lstUtilityHX.Select(p => new GeneralFailureCommonDetail
                    {
                        ID = 0,
                        HXName = p.HXName,
                        Stop = p.Stop,
                        DutyFactor = p.DutyFactor
                    }).ToList(); ;
                    generalBLL.Save(model.dbModel, lstCommonDetail);
                    wd.DialogResult = true;
                }
            }
        }
    }
}
