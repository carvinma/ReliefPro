using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using NHibernate;

using ReliefProMain.Commands;
using ReliefProMain.Models.Compressors;
using UOMLib;
using ReliefProModel.Compressors;
using ReliefProDAL;
using ReliefProModel;
using ReliefProBLL;

namespace ReliefProMain.ViewModel.Compressors
{
    public class PistonVM : ViewModelBase
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public PistonBlockedOutletModel model { get; set; }
        private CompressorBlockedBLL blockBLL;
        public PistonVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);

            blockBLL = new CompressorBlockedBLL(SessionPS, SessionPF);
            var pistonModel = blockBLL.GetPistonModel(ScenarioID);
            pistonModel = blockBLL.ReadConvertPistonModel(pistonModel);

            model = new PistonBlockedOutletModel(pistonModel);
            model.dbmodel.ScenarioID = ScenarioID;


            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.ReliefloadUnit = uomEnum.UserMassRate;
            model.ReliefTempUnit = uomEnum.UserTemperature;
            model.ReliefPressureUnit = uomEnum.UserPressure;
        }
        private void WriteConvertModel()
        {
            model.dbmodel.RatedCapacity = model.RatedCapacity;
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefCpCv = model.ReliefCpCv;
            model.dbmodel.ReliefZ = model.ReliefZ;
            model.dbmodel.Reliefload = UnitConvert.Convert(model.ReliefloadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.Reliefload);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTempUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
        }
        private void CalcResult(object obj)
        {
            CustomStreamDAL customStreamDAL = new CustomStreamDAL();
            IList<CustomStream> csList = customStreamDAL.GetAllList(SessionPS, true);
            if (csList.Count > 0)
            {
                CustomStream cs = csList[0];
                model.ReliefMW = cs.BulkMwOfPhase;
                model.ReliefPressure = cs.Pressure;
                model.Reliefload = cs.WeightFlow * model.RatedCapacity;
                model.ReliefTemperature = cs.Temperature;
                if (model.Reliefload < 0)
                    model.Reliefload = 0;
                model.ReliefCpCv = cs.BulkCPCVRatio;
                model.ReliefZ = cs.VaporZFmKVal;
            }
        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    blockBLL.SavePiston(model.dbmodel);

                    wd.DialogResult = true;
                }
            }
        }
    }
}
