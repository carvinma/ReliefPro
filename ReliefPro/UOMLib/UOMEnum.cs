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
        public const string Weight = "kg";
        public const string Molar = "kg-mol";
        public const string StandardVolumeRate = "nm3";
        public const string Viscosity = "cP";
       
        public const string HeatCapacity = "kJ/kg-K";
        public const string ThermalConductivity = "W/m-K";
        public const string HeatTransCoeffcient = "W/m2-C";
        public const string VolumeRate = "m3/hr";
        public const string MassRate = "Kg/hr";
        public const string Density = "kg/m3";

        public const string SurfaceTension = "N/m";
        public const string FineLength = "in";
        public const string MachineSpeed = "rpm";
        public const string Volume = "m3";
        public const string Length = "m";
        public const string Area = "m2";

        public const string Energy = "kJ";
        public const string Time = "min";
        public const string FlowConductance = "(kg/sec)/sqrt(kPa-kg/m3)";
        public const string SpecificEnthalpy = "KJ/kg";
        public const string EnthalpyDuty = "KJ/hr";

        public string UserTemperature { get; private set; }
        public string UserPressure { get; private set; }
        public string UserWeight { get; private set; }
        public string UserMolar { get; private set; }
        public string UserStandardVolumeRate { get; private set; }
        public string UserViscosity { get; private set; }

        public string UserHeatCapacity { get; private set; }
        public string UserThermalConductivity { get; private set; }
        public string UserHeatTransCoeffcient { get; private set; }
        public string UserVolumeRate { get; private set; }
        public string UserMassRate { get; private set; }
        public string UserDensity { get; private set; }

        public string UserSurfaceTension { get; private set; }
        public string UserFineLength { get; private set; }
        public string UserMachineSpeed { get; private set; }
        public string UserVolume { get; private set; }
        public string UserLength { get; private set; }
        public string UserArea { get; private set; }

        public string UserEnergy { get; private set; }
        public string UserTime { get; private set; }
        public string UserFlowConductance { get;private set; }
        public string UserSpecificEnthalpy { get; private set; }
        public string UserEnthalpyDuty { get; private set; }
        
        public string SessionDBPath;

        public IList<BasicUnitDefault> lstBasicUnitDefault;
        public IList<BasicUnitCurrent> lstBasicUnitCurrent;
        public IList<SystemUnit> lstSystemUnit;
        public int BasicUnitID;
        public bool UnitFromFlag = true;//true 从Current赋值下拉框，否则是原来系统默认值
        public ISession SessionPlant;
        public UOMEnum(ISession SessionPlant)
        {
            SessionDBPath = SessionPlant.Connection.ConnectionString;
            SessionPlant.Flush();
            this.SessionPlant = SessionPlant;
            initInfo(SessionPlant);

            UserTemperature = GetUnit(UnitTypeEnum.Temperature);
            UserPressure = GetUnit(UnitTypeEnum.Pressure);
            UserWeight = GetUnit(UnitTypeEnum.Weight);
            UserMolar = GetUnit(UnitTypeEnum.Molar);
            UserStandardVolumeRate = GetUnit(UnitTypeEnum.StandardVolumeRate);
            UserViscosity = GetUnit(UnitTypeEnum.Viscosity);

            UserHeatCapacity = GetUnit(UnitTypeEnum.HeatCapacity);
            UserThermalConductivity = GetUnit(UnitTypeEnum.ThermalConductivity);
            UserHeatTransCoeffcient = GetUnit(UnitTypeEnum.HeatTransCoeffcient);
            UserVolumeRate = GetUnit(UnitTypeEnum.VolumeRate);
            UserMassRate = GetUnit(UnitTypeEnum.MassRate);
            UserDensity = GetUnit(UnitTypeEnum.Density);

            UserSurfaceTension = GetUnit(UnitTypeEnum.SurfaceTension);
            UserFineLength = GetUnit(UnitTypeEnum.FineLength);
            UserMachineSpeed = GetUnit(UnitTypeEnum.MachineSpeed);
            UserVolume = GetUnit(UnitTypeEnum.Volume);
            UserLength = GetUnit(UnitTypeEnum.Length);
            UserArea = GetUnit(UnitTypeEnum.Aera);

            UserEnergy = GetUnit(UnitTypeEnum.Energy);
            UserTime = GetUnit(UnitTypeEnum.Time);
            UserFlowConductance = GetUnit(UnitTypeEnum.FlowConductance);
            UserSpecificEnthalpy = GetUnit(UnitTypeEnum.SpecificEnthalpy);
            UserEnthalpyDuty = GetUnit(UnitTypeEnum.EnthalpyDuty);
        }
        private string GetUnit(UnitTypeEnum unitTypeEnum)
        {
            if (lstBasicUnitCurrent != null && lstBasicUnitCurrent.Count > 0)
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
            if (lstBasicUnitCurrent == null || lstBasicUnitCurrent.Count== 0)
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
