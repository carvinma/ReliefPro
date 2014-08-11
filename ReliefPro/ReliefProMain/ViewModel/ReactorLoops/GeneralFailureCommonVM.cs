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
using ReliefProDAL.ReactorLoops;
using UOMLib;

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
            UOMLib.UOMEnum uomEnum = new UOMEnum(SessionPF);
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
            else
            {
                model.lstUtilityHX = GetInitUtilityHXs();
            }
        }
        private List<UtilityHXModel> GetInitUtilityHXs()
        {
            List<UtilityHXModel> list=new List<UtilityHXModel>();
             ReactorLoopDAL dal = new ReliefProDAL.ReactorLoops.ReactorLoopDAL();
             IList<ReactorLoopDetail> details = dal.GetReactorLoopDetail(SessionPS, 1);
             foreach (ReactorLoopDetail d in details)
             {
                 UtilityHXModel m = new UtilityHXModel();
                 m.HXName = d.DetailInfo;
                 m.Stop = false;
                 m.DutyFactor = 1;
                 list.Add(m);
             }
             return list;
        }
        

        private void CoolingRunCaseSimulation(object obj)
        { }
        private void CoolingLaunchSimulator(object obj)
        { }
        private void ElectricRunCaseSimulation(object obj)
        { }
        private void ElectricLaunchSimulator(object obj)
        { }
        private void WriteConvert()
        {
            model.dbModel.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMEnum.MassRate, model.dbModel.ReliefLoad.Value);
            model.dbModel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMEnum.MassRate, model.dbModel.ReliefPressure.Value);
            model.dbModel.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMEnum.MassRate, model.dbModel.ReliefTemperature.Value);
        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvert();
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

        /// <summary>
        /// 获取新的hx的信息。
        /// </summary>
        /// <param name="hxName"></param>
        private string GetNewHXInfo(string hxName)
        {
            string hxInfo = string.Empty;

            return hxInfo;
        }
    }
}
