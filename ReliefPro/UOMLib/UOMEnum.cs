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


        public static IList<BasicUnitDefault> lstBasicUnitDefault;
        public static IList<BasicUnitCurrent> lstBasicUnitCurrent;
        public static IList<SystemUnit> lstSystemUnit;
        public static int BasicUnitID;
        public static int BasicUnitCuurentID;
        public static bool UnitFormFlag = true;//true 从Current赋值下拉框，否则是原来系统默认值
        public string TestUnit { get; set; }
        public UOMEnum(ISession SessionPlant)
        {
            UnitInfo unitInfo = new UnitInfo();
            //var basicUnit = unitInfo.GetBasicUnitUOM(SessionPlant);
            //lstBasicUnitDefault = unitInfo.GetBasicUnitDefaultUserSet(SessionPlant);
            // lstBasicUnitCurrent = unitInfo.GetBasicUnitCurrentUserSet(SessionPlant);

            UserTemperature = GetUnit(UnitTypeEnum.Temperature, BasicUnitID);
            UserPressure = GetUnit(UnitTypeEnum.Pressure, BasicUnitID);
            UserEnthalpyDuty = GetUnit(UnitTypeEnum.EnthalpyDuty, BasicUnitID);
            UserMassRate = GetUnit(UnitTypeEnum.MassRate, BasicUnitID);
            UserArea = GetUnit(UnitTypeEnum.Aera, BasicUnitID);
            UserSpecificEnthalpy = GetUnit(UnitTypeEnum.SpecificEnthalpy, BasicUnitID);
            UserDensity = GetUnit(UnitTypeEnum.Density, BasicUnitID);
            UserVolume = GetUnit(UnitTypeEnum.Volume, BasicUnitID);
            UserTime = GetUnit(UnitTypeEnum.Time, BasicUnitID);
            UserLength = GetUnit(UnitTypeEnum.Length, BasicUnitID);
            UserThermalConductivity = GetUnit(UnitTypeEnum.ThermalConductivity, BasicUnitID);
            UserHeatTransCoeffcient = GetUnit(UnitTypeEnum.HeatTransCoeffcient, BasicUnitID);
        }
        private string GetUnit(UnitTypeEnum unitTypeEnum, int basicUnitID)
        {
            if (UOMEnum.UnitFormFlag && lstBasicUnitCurrent != null && lstBasicUnitCurrent.Count > 0)
            {
                return GetCurrentUnit(unitTypeEnum);
            }
            else
            {
                return GetDefalutUnit(unitTypeEnum, basicUnitID);
            }
        }
        private string GetCurrentUnit(UnitTypeEnum unitTypeEnum)
        {
            var currentUnit = lstBasicUnitCurrent.FirstOrDefault(p => p.UnitTypeID == int.Parse(unitTypeEnum.ToString("d")));
            if (currentUnit != null)
                return lstSystemUnit.FirstOrDefault(p => p.ID == currentUnit.SystemUnitID).Name;
            return "";
        }
        private string GetDefalutUnit(UnitTypeEnum unitTypeEnum, int basicUnitID)
        {
            var basicUnitDefault = lstBasicUnitDefault.FirstOrDefault(p => p.BasicUnitID == basicUnitID && p.UnitTypeID == int.Parse(unitTypeEnum.ToString("d")));
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
