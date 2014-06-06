using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class SystemScenarioFactor
    {
        public virtual int ID { get; set; }
        public virtual string Category { get; set; }
        public virtual string CategoryValue { get; set; }
        public virtual string BlockedOutlet { get; set; }
        public virtual string RefluxFailure { get; set; }
        public virtual string GeneralElectricPowerFailure { get; set; }
        public virtual string CoolingWaterFailure { get; set; }
        public virtual string RefrigerantFailure { get; set; }
        public virtual string PumpAroundFailure { get; set; }
        public virtual string AbnormalHeatInput { get; set; }
        public virtual string ColdFeedStops { get; set; }
        public virtual string InletValveFailsOpen { get; set; }
        public virtual string Fire { get; set; }
        public virtual string SteamFailure { get; set; }
        public virtual string AutomaticControlsFailure { get; set; }
    }
}
