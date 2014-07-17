using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.Reports
{
    public class PUsummaryReportSource
    {
        public string Device { get; set; }
        public string ProtectedSystem { get; set; }
        public string DeviceType { get; set; }
        public string SetPressure { get; set; }
        public string DischargeTo { get; set; }

        public double? ScenarioReliefRate { get; set; }
        public double? ScenarioVolumeRate { get; set; }
        public string ScenarioPhase { get; set; }
        public double? ScenarioMWorSpGr { get; set; }
        public double? ScenarioT { get; set; }
        public double? ScenarioZ { get; set; }
        public string ScenarioName { get; set; }

        public double? PowerReliefRate { get; set; }
        public double? PowerVolumeRate { get; set; }
        public string PowerPhase { get; set; }
        public double? PowerMWorSpGr { get; set; }
        public double? PowerT { get; set; }
        public double? PowerZ { get; set; }
        public double? PowerCpCv { get; set; }


        public double? WaterReliefRate { get; set; }
        public double? WaterVolumeRate { get; set; }
        public string WaterPhase { get; set; }
        public double? WaterMWorSpGr { get; set; }
        public double? WaterT { get; set; }
        public double? WaterZ { get; set; }
        public double? WaterCpCv { get; set; }


        public double? AirReliefRate { get; set; }
        public double? AirVolumeRate { get; set; }
        public string AirPhase { get; set; }
        public double? AirMWorSpGr { get; set; }
        public double? AirT { get; set; }
        public double? AirZ { get; set; }
        public double? AirCpCv { get; set; }

        public double? SteamReliefRate { get; set; }
        public double? SteamVolumeRate { get; set; }
        public string SteamPhase { get; set; }
        public double? SteamMWorSpGr { get; set; }
        public double? SteamT { get; set; }
        public double? SteamZ { get; set; }
        public double? SteamCpCv { get; set; }

        public double? FireReliefRate { get; set; }
        public double? FireVolumeRate { get; set; }
        public string FirePhase { get; set; }
        public double? FireMWorSpGr { get; set; }
        public double? FireT { get; set; }
        public double? FireZ { get; set; }
        public double? FireCpCv { get; set; }
    }
}
