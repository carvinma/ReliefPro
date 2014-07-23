using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.ReactorLoops
{
    public class GeneralFailureCommon
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual int GeneralType { get; set; } //0-GeneralCoolingWaterFailure,1-GeneralElectricPowerFailure

        public virtual bool RecycleCompressorFailure { get; set; }
        public virtual bool CalcInjectionWaterStream { get; set; }
        public virtual double? InjectionWaterStream { get; set; }

        public virtual bool CalcHXNetworkColdStream { get; set; }
        public virtual double? HXNetworkColdStream { get; set; }

        public virtual double? ReliefLoad { get; set; }
        public virtual double? ReliefMW { get; set; }
        public virtual double? ReliefTemperature { get; set; }
        public virtual double? ReliefPressure { get; set; }
    }
}
