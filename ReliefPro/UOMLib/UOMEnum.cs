using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProModel;

namespace UOMLib
{
    public class UOMEnum
    {
        public const string Temperature = "C";
        public const string Pressure = "MPag";
        public const string Viscosity = "cP";
        public const string HeatCapacity = "kJ/kg-K";
        public const string SurfaceTension = "N/m";
        public const string Volume = "m3";
        public const string Length = "m";
        public const string Time = "min";
        public const string FlowConductance = "(kg/sec)/sqrt(kPa-kg/m3)";
        public const string FineLength = "in";
        public const string MassRate = "Kg/hr";
        public const string EnthalpyDuty = "KJ/hr";
        public const string SpecificEnthalpy = "KJ/kg";
        public const string Density = "kg/m3";
        public const string Area = "m2";
        public const string ThermalConductivity = "w/m2-C";
        public const string HeatTransCoeffcient = "w/m-C";

        public string UserTemperature { get; private set; }
        //public readonly string UserWeightFlow;
        public string UserPressure { get; private set; }
        public string UserEnthalpyDuty { get; private set; }
        public string UserMassRate { get; private set; }
        public string UserArea { get; private set; }
        public string UserSpecificEnthalpy { get; private set; }
        public string UserDensity { get; private set; }
        public string UserVolume { get; private set; }
        public string UserTime { get; private set; }
        public string UserLength { get; private set; }
        public string UserThermalConductivity { get; private set; }
        public string UserHeatTransCoeffcient { get; private set; }

        public string SessionDBPath;

        public IList<BasicUnitDefault> lstBasicUnitDefault;
        public IList<BasicUnitCurrent> lstBasicUnitCurrent;
        public IList<SystemUnit> lstSystemUnit;
        public int BasicUnitID;
        public bool UnitFormFlag = true;//true 从Current赋值下拉框，否则是原来系统默认值
        public UOMEnum(ISession SessionPlant)
        {
            SessionDBPath = SessionPlant.Connection.ConnectionString;
            initInfo(SessionPlant);

            UserTemperature = GetUnit(UnitTypeEnum.Temperature);
            UserPressure = GetUnit(UnitTypeEnum.Pressure);
            UserEnthalpyDuty = GetUnit(UnitTypeEnum.EnthalpyDuty);
            UserMassRate = GetUnit(UnitTypeEnum.MassRate);
            UserArea = GetUnit(UnitTypeEnum.Aera);
            UserSpecificEnthalpy = GetUnit(UnitTypeEnum.SpecificEnthalpy);
            UserDensity = GetUnit(UnitTypeEnum.Density);
            UserVolume = GetUnit(UnitTypeEnum.Volume);
            UserTime = GetUnit(UnitTypeEnum.Time);
            UserLength = GetUnit(UnitTypeEnum.Length);
            UserThermalConductivity = GetUnit(UnitTypeEnum.ThermalConductivity);
            UserHeatTransCoeffcient = GetUnit(UnitTypeEnum.HeatTransCoeffcient);
        }
        private string GetUnit(UnitTypeEnum unitTypeEnum)
        {
            if (UnitFormFlag && lstBasicUnitCurrent != null && lstBasicUnitCurrent.Count > 0)
            {
                return GetCurrentUnit(unitTypeEnum);
            }
            else
            {
                return GetDefalutUnit(unitTypeEnum);
            }
        }
        private string GetCurrentUnit(UnitTypeEnum unitTypeEnum)
        {
            var currentUnit = lstBasicUnitCurrent.FirstOrDefault(p => p.UnitTypeID == int.Parse(unitTypeEnum.ToString("d")));
            if (currentUnit != null)
                return lstSystemUnit.FirstOrDefault(p => p.ID == currentUnit.SystemUnitID).Name;
            return "";
        }
        private void initInfo(ISession SessionPlant)
        {
            UnitInfo unitInfo = new UnitInfo();
            var basicUnit = unitInfo.GetBasicUnitUOM(SessionPlant);
            BasicUnitID = basicUnit.ID;

            if (lstBasicUnitDefault == null || lstBasicUnitDefault.Count == 0)
            {
                lstBasicUnitDefault = unitInfo.GetBasicUnitDefault(SessionPlant);
            }
            if (lstSystemUnit == null || lstSystemUnit.Count == 0)
            {
                lstSystemUnit = unitInfo.GetSystemUnit(SessionPlant);
            }
            if (lstBasicUnitCurrent == null || lstBasicUnitCurrent.Count > 0)
            {
                lstBasicUnitCurrent = unitInfo.GetBasicUnitCurrent(SessionPlant);
            }
        }
        private string GetDefalutUnit(UnitTypeEnum unitTypeEnum)
        {
            var basicUnitDefault = lstBasicUnitDefault.FirstOrDefault(p => p.BasicUnitID == BasicUnitID && p.UnitTypeID == int.Parse(unitTypeEnum.ToString("d")));
            return lstSystemUnit.FirstOrDefault(p => p.ID == basicUnitDefault.SystemUnitID).Name;
        }
        public enum UnitTypeEnum
        {
            Temperature = 1,
            Pressure = 2,
            Weight = 3,
            Molar = 4,
            StandardVolumeRate = 5,
            Viscosity = 6,
            HeatCapacity = 7,
            ThermalConductivity = 8,
            HeatTransCoeffcient = 9,
            SurfaceTension = 10,
            MachineSpeed = 12,
            Volume = 13,
            Length = 14,
            Aera = 15,
            Energy = 16,
            Time = 17,
            FlowConductance = 18,
            MassRate = 19,
            VolumeRate = 20,
            Density = 21,
            SpecificEnthalpy = 22,
            FineLength = 23,
            EnthalpyDuty = 24,
        }
    }
}
