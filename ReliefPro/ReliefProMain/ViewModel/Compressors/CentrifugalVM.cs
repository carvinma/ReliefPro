using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Commands;
using ReliefProMain.Model.Compressors;
using UOMLib;
using ReliefProDAL;
using ReliefProModel;

namespace ReliefProMain.ViewModel.Compressors
{
    public class CentrifugalVM : ViewModelBase
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public CentrifugalBlockedOutletModel model { get; set; }
        private CompressorBlockedBLL blockBLL;
        public CentrifugalVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);

            blockBLL = new CompressorBlockedBLL(SessionPS, SessionPF);
            var BlockedModel = blockBLL.GetCentrifugalModel(ScenarioID);
            if (BlockedModel.Scale == 0)
            {
                CustomStreamDAL csDAL = new CustomStreamDAL();
                IList<CustomStream> inletList = csDAL.GetAllList(SessionPS, false);
                BlockedModel.Scale = 1.05;
                BlockedModel.InletLoad = double.Parse(inletList[0].WeightFlow) / double.Parse(inletList[0].BulkDensityAct);

                IList<CustomStream> outletList = csDAL.GetAllList(SessionPS, true);
                BlockedModel.OutletPressure = double.Parse(outletList[0].Pressure);
            }
            BlockedModel = blockBLL.ReadConvertCentrifugalModel(BlockedModel);
            
            model = new CentrifugalBlockedOutletModel(BlockedModel);
            model.dbmodel.ScenarioID = ScenarioID;

            UOMLib.UOMEnum uomEnum = new UOMEnum(SessionPF);
            model.ReliefloadUnit = uomEnum.UserMassRate;
            model.ReliefTempUnit = uomEnum.UserTemperature;
            model.ReliefPressureUnit = uomEnum.UserPressure;
        }
        private void WriteConvertModel()
        {
            model.dbmodel.Scale = model.Scale;
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.Reliefload = UnitConvert.Convert(model.ReliefloadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.Reliefload);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTempUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemp);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
            model.dbmodel.InletLoad = model.InletLoad;
            model.dbmodel.OutletPressure = model.OutletPressure;
            model.dbmodel.SurgeLoad = model.SurgeLoad;
            //model.dbmodel.InletLoad = UnitConvert.Convert(model.InletLoad, UOMLib.UOMEnum.Viscosity.ToString(), model.ReliefPressure);
            //model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
            //model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
        }
        private void CalcResult(object obj)
        {
            PSVDAL psvDAL = new PSVDAL();
            PSV psv = psvDAL.GetModel(SessionPS);
            double PDesign = double.Parse(psv.ReliefPressureFactor) * double.Parse(psv.Pressure);
            double QNormal = model.InletLoad;
            double PressureNormal = model.OutletPressure;
            double QSurgeNormal = model.SurgeLoad;
            double PSurgeNormal = 1;
            double bNormal = 0; //y=-0.25x+b

            double Ratio = 1.05; //   max speed/normal speed
            double QMax = Ratio*QNormal;
            double PressureMax = Math.Pow(Ratio, 2) * PressureNormal ;
            //double QSurgeMax = 1;
            //double PSurgeMax = 1;
            double bMax = 0;

            double K2 = PressureNormal / QNormal;


            if (Ratio <= 1 || Ratio > 2)
            {
                return;
            }
            if (QSurgeNormal * 1.1 >= QNormal)
            {
                return;
            }

            bNormal = PressureNormal + 0.25*K2 * QNormal;
            bMax = PressureMax + 0.25 * K2 * QMax;
            PSurgeNormal = -0.25 * K2 * QSurgeNormal + bNormal;

            double rate = PSurgeNormal / QSurgeNormal;
            double Fsa = bMax / (rate + 0.25 * K2);
            double Psa = -0.25 * K2 * Fsa + bMax;
            
            //控制线 y=rate（x-1.1)
            double Fs = (bMax + 1.1 * rate) / (rate + 0.25 * K2);
            double Ps = -0.25 * K2 * Fs + bMax;

            double v=0;
            if (PDesign < Psa)
                model.Reliefload = 0;
            else if (PDesign < Ps)
                v = Fs;
            else
                v = (bMax - PDesign) / (0.25 * K2);

            if (v < 0)
                v = 0;
            
            //读取压缩机出口物料的密度
             CustomStreamDAL customStreamDAL = new CustomStreamDAL();
            IList<CustomStream> csList = customStreamDAL.GetAllList(SessionPS, true);
            if (csList.Count > 0)
            {
                CustomStream cs = csList[0];
                double density = double.Parse(cs.BulkDensityAct);
                model.Reliefload = density * v;
                model.ReliefMW = double.Parse(cs.BulkMwOfPhase);
                model.ReliefTemp = double.Parse(cs.Temperature);
                model.ReliefPressure = PDesign;

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
                    blockBLL.SaveCentrifugal(model.dbmodel);
                    wd.DialogResult = true;
                }
            }
        }
    }
}
