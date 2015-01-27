using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using NHibernate;

using ReliefProMain.Commands;
using ReliefProMain.Models.Compressors;
using UOMLib;
using ReliefProDAL;
using ReliefProModel;
using ReliefProBLL;
using System.Windows;
using ReliefProMain.View.Compressors;

namespace ReliefProMain.ViewModel.Compressors
{
    public class CentrifugalVM : ViewModelBase
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        public ICommand SettingCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        private int DriverType;
        public CentrifugalBlockedOutletModel model { get; set; }
        private CompressorBlockedBLL blockBLL;
        public CentrifugalVM(int ScenarioID, ISession SessionPS, ISession SessionPF, int DriverType)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            this.DriverType = DriverType;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);
            SettingCMD = new DelegateCommand<object>(Setting);
            blockBLL = new CompressorBlockedBLL(SessionPS, SessionPF);
            var BlockedModel = blockBLL.GetCentrifugalModel(ScenarioID);
            if (BlockedModel.ID == 0)
            {
                CustomStreamDAL csDAL = new CustomStreamDAL();
                if (DriverType == 0)
                {
                    IList<CustomStream> inletList = csDAL.GetAllList(SessionPS, false);
                    BlockedModel.Scale = 1.05;
                    BlockedModel.DeltPowY = 2;
                    BlockedModel.KNormal = -0.4675;
                    BlockedModel.InletLoad = inletList[0].WeightFlow / inletList[0].BulkDensityAct;
                    IList<CustomStream> outletList = csDAL.GetAllList(SessionPS, true);
                    BlockedModel.OutletPressure = outletList[0].Pressure;
                }
                else
                {
                    PSVDAL psvDAL = new PSVDAL();
                    PSV psv = psvDAL.GetModel(SessionPS);
                    IList<CustomStream> outletList = csDAL.GetAllList(SessionPS, true);
                    BlockedModel.Reliefload = outletList[0].WeightFlow;
                    if (BlockedModel.Reliefload < 0)
                        BlockedModel.Reliefload = 0;
                    BlockedModel.ReliefMW = outletList[0].BulkMwOfPhase;
                    BlockedModel.ReliefPressure = psv.Pressure * psv.ReliefPressureFactor;
                    BlockedModel.ReliefTemperature = outletList[0].Temperature;
                    BlockedModel.ReliefZ = outletList[0].VaporZFmKVal;
                    BlockedModel.ReliefCpCv = outletList[0].BulkCPCVRatio;
                }
            }
            BlockedModel = blockBLL.ReadConvertCentrifugalModel(BlockedModel);//读取并转换为用户单位
            model = new CentrifugalBlockedOutletModel(BlockedModel);
            model.dbmodel.ScenarioID = ScenarioID;

            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.ReliefloadUnit = uomEnum.UserMassRate;
            model.ReliefTempUnit = uomEnum.UserTemperature;
            model.ReliefPressureUnit = uomEnum.UserPressure;
            model.SurgeLoadUnit = uomEnum.UserVolumeRate;
            model.OutletPressureUnit = uomEnum.UserPressure;
            model.InletLoadUnit = uomEnum.UserVolumeRate;
        }
        private void ReadConvert()
        {
            //model.Reliefload = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(), model.ReliefloadUnit, model.dbmodel.Reliefload);
            //model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTempUnit, model.dbmodel.ReliefTemperature);
            //model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressureUnit, model.dbmodel.ReliefPressure);
            //model.InletLoad = UnitConvert.Convert(UOMLib.UOMEnum.VolumeRate.ToString(), model.InletLoadUnit, model.dbmodel.InletLoad);
            //model.OutletPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(), model.OutletPressureUnit, model.OutletPressure);
            //model.SurgeLoad = UnitConvert.Convert(UOMLib.UOMEnum.VolumeRate.ToString(), model.SurgeLoadUnit, model.dbmodel.SurgeLoad);
        }
        private void WriteConvert()
        {
            model.dbmodel.Scale = model.Scale;
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.Reliefload = UnitConvert.Convert(model.ReliefloadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.Reliefload);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTempUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
            model.dbmodel.InletLoad = UnitConvert.Convert(model.InletLoadUnit, UOMLib.UOMEnum.VolumeRate.ToString(), model.InletLoad);
            model.dbmodel.OutletPressure = UnitConvert.Convert(model.OutletPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.OutletPressure);
            model.dbmodel.SurgeLoad = UnitConvert.Convert(model.SurgeLoadUnit, UOMLib.UOMEnum.VolumeRate.ToString(), model.SurgeLoad);
            model.dbmodel.ReliefCpCv = model.ReliefCpCv;
            model.dbmodel.ReliefZ = model.ReliefZ;
            model.dbmodel.KNormal = model.KNormal;
            model.dbmodel.DeltPowY = model.DeltPowY;
            //model.dbmodel.InletLoad = UnitConvert.Convert(model.InletLoad, UOMLib.UOMEnum.Viscosity.ToString(), model.ReliefPressure);
            //model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
            //model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
        }
        private void CalcResult(object obj)
        {
            //计算时压力单位统一为Mpa
            CustomStreamDAL csdal = new CustomStreamDAL();
            PSVDAL psvDAL = new PSVDAL();
            PSV psv = psvDAL.GetModel(SessionPS);
            double PDesign = UnitConvert.Convert(UOMEnum.Pressure, "Mpa", psv.Pressure);
            double QNormal = UnitConvert.Convert(model.InletLoadUnit, UOMLib.UOMEnum.VolumeRate.ToString(), model.InletLoad);
            double PressureNormal = UnitConvert.Convert(model.OutletPressureUnit, "Mpa", model.OutletPressure);
            double QSurgeNormal = UnitConvert.Convert(model.SurgeLoadUnit, UOMLib.UOMEnum.VolumeRate.ToString(), model.SurgeLoad);
            double PSurgeNormal = 0;
            double bNormal = 0; //y=-0.4675x+b
            double kNormal = model.KNormal;
            double powY = model.DeltPowY;
            IList<CustomStream> feeds = csdal.GetAllList(SessionPS, false);
            double PressureNormalIn = feeds[0].Pressure ;
            for (int i = 1; i < feeds.Count; i++)
            {
                if (PressureNormalIn > feeds[i].Pressure)
                {
                    PressureNormalIn = feeds[i].Pressure;
                }
            }
            PressureNormalIn = UnitConvert.Convert(UOMEnum.Pressure, "Mpa", PressureNormalIn);
            double deltPressureNormal = PressureNormal - PressureNormalIn;

            double Ratio = model.Scale; // double Ratio = 1.05; //   max speed/normal speed
            double QMax = Ratio * QNormal;
            double deltPressureMax = Math.Pow(Ratio, powY) * deltPressureNormal;
            double PressureMax = deltPressureMax + PressureNormalIn;
            double bMax = 0;

            double K2 = deltPressureNormal / QNormal;
            if (model.SurgeLoad<=0)
            {
                model.Reliefload = 0;
                model.ReliefMW = 0;
                model.ReliefTemperature = 0;
                model.ReliefPressure = psv.Pressure * psv.ReliefPressureFactor; ;
                model.ReliefCpCv = 0;
                model.ReliefZ = 0;
                model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, model.ReliefPressure);
                MessageBox.Show("Surge Load must be greater than zero.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (PDesign < model.OutletPressure)
            {
                model.Reliefload = 0;
                model.ReliefMW = 0;
                model.ReliefTemperature = 0;
                model.ReliefPressure = psv.Pressure * psv.ReliefPressureFactor; ;
                model.ReliefCpCv = 0;
                model.ReliefZ = 0;
                model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, model.ReliefPressure);
                MessageBox.Show("PSV set pressure must be greater than outlet pressure.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Ratio <= 1 || Ratio > 2)
            {
                model.Reliefload = 0;
                model.ReliefMW = 0;
                model.ReliefTemperature = 0;
                model.ReliefPressure = psv.Pressure * psv.ReliefPressureFactor; ;
                model.ReliefCpCv = 0;
                model.ReliefZ = 0;
                model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, model.ReliefPressure);
                MessageBox.Show("Error: Max speed/Normal speed ratio is not within range of 1.0 to 2.0 ", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);                  
                return;
            }
            if (QSurgeNormal * 1.1 >= QNormal)
            {
                model.Reliefload = 0;
                model.ReliefMW = 0;
                model.ReliefTemperature = 0;
                model.ReliefPressure = psv.Pressure * psv.ReliefPressureFactor; 
                model.ReliefCpCv = 0;
                model.ReliefZ = 0;
                model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, model.ReliefPressure);
                MessageBox.Show(" Error: Nornal flow rate/Surge flow rate is less than 1.1.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bNormal = deltPressureNormal - kNormal * K2 * QNormal;
            bMax = deltPressureMax - kNormal * K2 * QMax;
            double deltPSurgeNormal = kNormal * K2 * QSurgeNormal + bNormal;
            PSurgeNormal= deltPSurgeNormal+ PressureNormalIn;

            double rate = deltPSurgeNormal/ QSurgeNormal;
            double Fsa = bMax / (rate - kNormal * K2);
            double deltPsa = kNormal * K2 * Fsa + bMax;
            double Psa = deltPsa + PressureNormalIn;

            //控制线 y=rate（x-1.1)
            double Fs = (bMax + 1.1 * rate) / (rate - kNormal * K2);
            double deltPs = kNormal * K2 * Fs + bMax;
            double Ps=deltPs+ PressureNormalIn;


            double v = 0;
            if (PDesign > Psa)
            {
                v = 0;               //model.Reliefload=0;
                model.Reliefload = 0;
                model.ReliefMW = 0;
                model.ReliefTemperature = 0;
                model.ReliefPressure = psv.Pressure * psv.ReliefPressureFactor; 
                model.ReliefCpCv = 0;
                model.ReliefZ = 0;
                model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, model.ReliefPressure);
                MessageBox.Show("Design pressure is larger than Max discharge pressure, no relief will occur.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (PDesign > Ps)
            {
                v = Fs;
            }
            else
            {
                v = (bMax + PressureNormalIn - PDesign) / (-1*kNormal * K2);
            }

            if (v < 0)
            {
                model.Reliefload = 0;
                model.ReliefMW = 0;
                model.ReliefTemperature = 0;
                model.ReliefPressure = psv.Pressure * psv.ReliefPressureFactor; ;
                model.ReliefCpCv = 0;
                model.ReliefZ = 0;
                v = 0;
                model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, model.ReliefPressure);
                MessageBox.Show("Design pressure is larger than Max discharge pressure, no relief will occur.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //读取压缩机出口物料的密度
            IList<CustomStream> csList = csdal.GetAllList(SessionPS, false);
            if (csList.Count > 0)
            {
                CustomStream cs = csList[0];
                double density = cs.BulkDensityAct;
                model.Reliefload = density * v;
                model.ReliefMW = cs.BulkMwOfPhase;
                model.ReliefTemperature = cs.Temperature;
                model.ReliefPressure = psv.Pressure * psv.ReliefPressureFactor;
                model.ReliefCpCv = cs.BulkCPCVRatio;
                model.ReliefZ = cs.VaporZFmKVal;
                if (model.Reliefload < 0)
                    model.Reliefload = 0;

                model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, model.ReliefPressure);
                model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTempUnit, model.ReliefTemperature);
                model.Reliefload = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefloadUnit, model.Reliefload);
            }


        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvert();
                    blockBLL.SaveCentrifugal(model.dbmodel);
                    wd.DialogResult = true;
                }
            }
        }


        private void Setting(object obj)
        {
            BlockedSettingView view = new BlockedSettingView();
            BlockedSettingVM vm = new BlockedSettingVM(model);
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            view.DataContext = vm;
            if (view.ShowDialog()==true)
            {
                model = vm.model;
            }
        }
    }
}
