using System;
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

            //将输入的值的单位制，从系统单位转为用户指定单位
            ReadConvertModel();

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

        private void ReadConvertModel()
        {
            model.InletGasUpstreamMaxPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure,model.InletGasUpstreamMaxPressureUnit,  model.InletGasUpstreamMaxPressure);
            model.InletAbsorbentUpstreamMaxPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, model.InletAbsorbentUpstreamMaxPressureUnit, model.InletAbsorbentUpstreamMaxPressure);
            model.NormalGasFeedWeightRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate,model.NormalGasFeedWeightRateUnit,  model.NormalGasFeedWeightRate);
            model.NormalGasProductWeightRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate,model.NormalGasProductWeightRateUnit,  model.NormalGasProductWeightRate);

            model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, model.ReliefLoadUnit, model.ReliefLoad);
            model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, model.ReliefPressureUnit,model.ReliefPressure);
            model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature,model.ReliefTemperatureUnit,  model.ReliefTemperature);
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

            double sysInletGasUpstreamMaxPressure = UnitConvert.Convert(model.InletGasUpstreamMaxPressureUnit, UOMLib.UOMEnum.Pressure, model.InletGasUpstreamMaxPressure);
            double sysInletAbsorbentUpstreamMaxPressure = UnitConvert.Convert(model.InletAbsorbentUpstreamMaxPressureUnit, UOMLib.UOMEnum.Pressure, model.InletAbsorbentUpstreamMaxPressure);
            double sysNormalGasFeedWeightRate = UnitConvert.Convert(model.NormalGasFeedWeightRateUnit, UOMLib.UOMEnum.MassRate, model.NormalGasFeedWeightRate);
            double sysNormalGasProductWeightRate = UnitConvert.Convert(model.NormalGasProductWeightRateUnit, UOMLib.UOMEnum.MassRate, model.NormalGasProductWeightRate);
            if (model.dbmodel.OutletType == 0)
            {
                if (sysInletGasUpstreamMaxPressure > pSet)
                {
                    if (sysInletAbsorbentUpstreamMaxPressure > pSet)
                    {
                        model.ReliefLoad = sysNormalGasProductWeightRate;
                    }
                    else
                    {
                        model.ReliefLoad = sysNormalGasFeedWeightRate;
                    }
                }
                else
                {
                    model.ReliefLoad = 0;
                }
                
            }
            else
            {
                if (sysInletGasUpstreamMaxPressure > pSet)
                {
                    model.ReliefLoad =sysNormalGasFeedWeightRate - sysNormalGasProductWeightRate;
                }
                else
                {
                    model.ReliefLoad = 0;
                }
                
            }
            if (model.ReliefLoad < 0)
                model.ReliefLoad = 0;

            //计算出来的结果都是系统单位制，需要转换下
            //只有泄放压力和泄放需要计算，温度和其他值都在界面加载时计算好了
            model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, model.ReliefLoadUnit, model.ReliefLoad);
            model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, model.ReliefPressureUnit, model.ReliefPressure);
           
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
