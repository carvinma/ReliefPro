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

        public string ScenarioReliefRate { get; set; }
        public string ScenarioPhase { get; set; }
        public string ScenarioMWorSpGr { get; set; }
        public string ScenarioT { get; set; }
        public string ScenarioZ { get; set; }
        public string ScenarioName { get; set; }

        public string PowerReliefRate { get; set; }
        public string PowerPhase { get; set; }
        public string PowerMWorSpGr { get; set; }
        public string PowerT { get; set; }
        public string PowerZ { get; set; }

        public string WaterReliefRate { get; set; }
        public string WaterPhase { get; set; }
        public string WaterMWorSpGr { get; set; }
        public string WaterT { get; set; }
        public string WaterZ { get; set; }


        public string AirReliefRate { get; set; }
        public string AirPhase { get; set; }
        public string AirMWorSpGr { get; set; }
        public string AirT { get; set; }
        public string AirZ { get; set; }

        public string SteamReliefRate { get; set; }
        public string SteamPhase { get; set; }
        public string SteamMWorSpGr { get; set; }
        public string SteamT { get; set; }
        public string SteamZ { get; set; }

        public string FireReliefRate { get; set; }
        public string FirePhase { get; set; }
        public string FireMWorSpGr { get; set; }
        public string FireT { get; set; }
        public string FireZ { get; set; }
    }
}
