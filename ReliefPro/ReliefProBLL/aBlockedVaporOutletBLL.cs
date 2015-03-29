using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using UOMLib;
using Alinq;

namespace ReliefProBLL
{
    public class aBlockedVaporOutletBLL
    {
        private UOMLib.UOMEnum uomEnum;

        public aBlockedVaporOutletBLL(int plantId)
        {
            uomEnum = UOMSingle.plantsInfo.FirstOrDefault(p => p.Id == plantId).UnitInfo;
        }
        public tbBlockedVaporOutlet GeModel(int scenarioID, int outletType)
        {
            return UOMSingle.currentPlant.DataContext.tbBlockedVaporOutlet.FirstOrDefault(p => p.ScenarioID == scenarioID && p.OutletType == outletType);
        }
        public tbScenario GetScenarioModel(int scenarioID)
        {
            return UOMSingle.currentPlant.DataContext.tbScenario.FirstOrDefault(p => p.Id == scenarioID);
        }
        public tbBlockedVaporOutlet ReadConvertBlockedVaporOutletModel(tbBlockedVaporOutlet model)
        {
            UnitInfo unitInfo = new UnitInfo();
            systbBasicUnit basicUnit = unitInfo.GetBasicUnitUOM();
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            tbBlockedVaporOutlet Model = new tbBlockedVaporOutlet();
            Model = model;
            Model.InletGasUpstreamMaxPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, uomEnum.UserPressure, Model.InletGasUpstreamMaxPressure);
            Model.InletAbsorbentUpstreamMaxPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, uomEnum.UserPressure, Model.InletAbsorbentUpstreamMaxPressure);
            Model.NormalGasFeedWeightRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, uomEnum.UserMassRate, Model.NormalGasFeedWeightRate);
            Model.NormalGasProductWeightRate = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, uomEnum.UserMassRate, Model.NormalGasProductWeightRate);
            return Model;
        }
        public tbScenario ReadConvertScenarioModel(tbScenario model)
        {
            UnitInfo unitInfo = new UnitInfo();
            systbBasicUnit basicUnit = unitInfo.GetBasicUnitUOM();
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            tbScenario Model = new tbScenario();
            Model = model;
            Model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate, uomEnum.UserMassRate, Model.ReliefLoad);
            Model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature, uomEnum.UserTemperature, Model.ReliefTemperature);
            Model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure, uomEnum.UserPressure, Model.ReliefPressure);
            return Model;
        }
        public void Save(tbBlockedVaporOutlet model, tbScenario smodel)
        {
            if (model.Id == 0)
            {
                UOMSingle.currentPlant.DataContext.tbBlockedVaporOutlet.Insert(model);
            }
            else
            {
                UOMSingle.currentPlant.DataContext.tbBlockedVaporOutlet.Update(p => model, p => p.Id == model.Id);
            }

            UOMSingle.currentPlant.DataContext.tbScenario.Update<tbScenario>(p => smodel, p => p.Id == smodel.Id);
            UOMSingle.currentPlant.DataContext.SubmitChanges();
        }
    }
}
