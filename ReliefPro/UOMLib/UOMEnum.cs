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
        public const string Time = "hr";
        public const string FlowConductance = "(kg/sec)/sqrt(kPa-kg/m3)";
        public const string FineLength = "in";
        public const string MassRate = "Kg/hr";
        public const string EnthalpyDuty = "KJ/hr";
        public const string SpecificEnthalpy = "KJ/kg";
        public const string Density = "kg/m3";
        public const string Area = "m2";
        public const string ThermalConductivity = "w/m2-C";
        public const string HeatTransCoeffcient = "w/m-C";

        public readonly string UserTemperature;
        public readonly string UserWeightFlow;
        public readonly string UserPressure;
        public readonly string UserEnthalpyDuty;
        public readonly string UserMassRate;
        public readonly string UserArea;
        public readonly string UserSpecificEnthalpy;
        public readonly string UserDensity;
        public readonly string UserVolume;
        public readonly string UserTime;
        public readonly string UserLength;
        public readonly string UserThermalConductivity;
        public readonly string UserHeatTransCoeffcient;
        public static IList<BasicUnitDefault> lstBasicUnitDefault;
        public static int BasicUnitID;
        public UOMEnum(ISession SessionPlant)
        {
            //UnitInfo unitInfo = new UnitInfo();
            //var basicUnit = unitInfo.GetBasicUnitUOM(SessionPlant);
            //lstBasicUnitDefault = unitInfo.GetBasicUnitDefaultUserSet(SessionPlant);
            UserTemperature = GetDefalutUnit(UnitTypeEnum.Temperature, BasicUnitID);
            UserPressure = GetDefalutUnit(UnitTypeEnum.Pressure, BasicUnitID);
            UserEnthalpyDuty = GetDefalutUnit(UnitTypeEnum.EnthalpyDuty, BasicUnitID);
            UserMassRate = GetDefalutUnit(UnitTypeEnum.MassRate, BasicUnitID);
            UserArea = GetDefalutUnit(UnitTypeEnum.Aera, BasicUnitID);
            UserSpecificEnthalpy = GetDefalutUnit(UnitTypeEnum.SpecificEnthalpy, BasicUnitID);
            UserDensity = GetDefalutUnit(UnitTypeEnum.Density, BasicUnitID);
            UserVolume = GetDefalutUnit(UnitTypeEnum.Volume, BasicUnitID);
            UserTime = GetDefalutUnit(UnitTypeEnum.Time, BasicUnitID);
            UserLength = GetDefalutUnit(UnitTypeEnum.Length, BasicUnitID);
            UserThermalConductivity = GetDefalutUnit(UnitTypeEnum.ThermalConductivity, BasicUnitID);
            UserHeatTransCoeffcient = GetDefalutUnit(UnitTypeEnum.HeatTransCoeffcient, BasicUnitID);
        }
        private string GetDefalutUnit(UnitTypeEnum unitTypeEnum, int basicUnitID)
        {
            var basicUnitDefault = lstBasicUnitDefault.FirstOrDefault(p => p.BasicUnitID == basicUnitID && p.UnitTypeID == int.Parse(unitTypeEnum.ToString("d")));
            if (basicUnitDefault != null && basicUnitDefault.SystemUnitInfo != null)
                return basicUnitDefault.SystemUnitInfo.Name;
            return "";
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
