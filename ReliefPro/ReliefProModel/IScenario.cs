using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel
{
    public interface IScenarioModel
    {
        int ScenarioID { get; set; }
        double ReliefLoad { get; set; }
        double ReliefMW { get; set; }
        double ReliefTemperature { get; set; }
        double ReliefPressure { get; set; }
    }
}
