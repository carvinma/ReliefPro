using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public const string WeightFlow = "Kg/hr";
        public const string EnthalpyDuty = "KJ/hr";
        public const string SpecificEnthalpy = "KJ/kg";
        public const string Density = "kg/m3";

        public readonly string UserSetTemperature;
        public readonly string UserWeightFlow;
        public readonly string UserPressure;
        public readonly string UserEnthalpyDuty;

        public UOMEnum(string dbPlantFile)
        {
            UnitInfo unitInfo = new UnitInfo();
            var basicUnit = unitInfo.GetBasicUnitUOM(dbPlantFile);
            IList<BasicUnitDefault> lstBasicUnitDefault = unitInfo.GetBasicUnitDefaultUserSet(dbPlantFile);

            UserSetTemperature = GetDefalutUnit(lstBasicUnitDefault, UnitTypeEnum.Temperature, basicUnit.ID);
            UserWeightFlow = GetDefalutUnit(lstBasicUnitDefault, UnitTypeEnum.WeightFlow, basicUnit.ID);
            UserPressure = GetDefalutUnit(lstBasicUnitDefault, UnitTypeEnum.Pressure, basicUnit.ID);
            UserEnthalpyDuty = GetDefalutUnit(lstBasicUnitDefault, UnitTypeEnum.EnthalpyDuty, basicUnit.ID);
        }
        private string GetDefalutUnit(IList<BasicUnitDefault> lstBasicUnitDefault, UnitTypeEnum unitTypeEnum, int basicUnitID)
        {
            var basicUnitDefault = lstBasicUnitDefault.Where(p => p.BasicUnitID == basicUnitID && p.UnitTypeID == int.Parse(unitTypeEnum.ToString("d"))).FirstOrDefault();
            if (basicUnitDefault != null)
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
            MachineSpeed = 11,
            Volume = 12,
            Length = 13,
            Aera = 14,
            Energy = 15,
            Time = 16,
            FlowConductance = 17,
            MassRate = 18,
            VolumeRate = 19,
            Density = 20,
            SpecificEnthalpy = 21,
            EnthalpyDuty = 22,
            FineLength = 23,
            WeightFlow = 24
        }
    }
}
