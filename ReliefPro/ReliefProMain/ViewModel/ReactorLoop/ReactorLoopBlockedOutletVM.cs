using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Model.ReactorLoop;
using UOMLib;

namespace ReliefProMain.ViewModel.ReactorLoop
{
    public class ReactorLoopBlockedOutletVM
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public ReactorLoopBlockedOutletModel model { get; set; }
        private ReactorLoopBLL reactorBLL;
        public ReactorLoopBlockedOutletVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);

            reactorBLL = new ReactorLoopBLL(SessionPS, SessionPF);
            var blockModel = reactorBLL.GetBlockedOutletModel(ScenarioID);
            blockModel = reactorBLL.ReadConvertBlockedOutletModel(blockModel);

            model = new ReactorLoopBlockedOutletModel(blockModel);
            model.dbmodel.ScenarioID = ScenarioID;

            UOMLib.UOMEnum uomEnum = new UOMLib.UOMEnum(SessionPF);
            model.MaxGasRateUnit = uomEnum.UserMassRate;
            model.TotalPurgeRateUnit = uomEnum.UserMassRate;
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
        }
        private void WriteConvertModel()
        {
            UnitConvert uc = new UnitConvert();
            model.dbmodel.MaxGasRate = uc.Convert(model.MaxGasRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.MaxGasRate);
            model.dbmodel.TotalPurgeRate = uc.Convert(model.TotalPurgeRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.TotalPurgeRate);
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefLoad = uc.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad);
            model.dbmodel.ReliefTemperature = uc.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.ReliefCpCv = model.ReliefCpCv;
            model.dbmodel.ReliefZ = model.ReliefZ;
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
