using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.Reports
{
    public class PUsummaryGridDS
    {
        public PSV psv { get; set; }
        public Scenario SingleDS { get; set; }
        public Scenario PowerDS { get; set; }
        public Scenario WaterDS { get; set; }
        public Scenario AirDS { get; set; }
        public Scenario SteamDS { get; set; }
        public Scenario FireDS { get; set; }
    }
}
