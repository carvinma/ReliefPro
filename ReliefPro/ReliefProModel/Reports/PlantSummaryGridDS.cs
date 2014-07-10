using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.Reports
{
    public class PlantSummaryGridDS
    {
        public string ProcessUnit { get; set; }
        public Scenario ControllingDS { get; set; }
        public Scenario PowerDS { get; set; }
        public Scenario WaterDS { get; set; }
        public Scenario AirDS { get; set; }
    }
}
