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
        public virtual double InjectionWaterStream { get; set; }

        public virtual bool CalcHXNetworkColdStream { get; set; }
        public virtual double HXNetworkColdStream { get; set; }

        public virtual double ReliefLoad { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefPressure { get; set; }
        public virtual double ReliefCpCv { get; set; }
        public virtual double ReliefZ { get; set; }
        public virtual bool IsSolved { get; set; }
        public virtual string ScenarioID_Color { get; set; }
        public virtual string GeneralType_Color { get; set; } //0-GeneralCoolingWaterFailure,1-GeneralElectricPowerFailure
        public virtual string RecycleCompressorFailure_Color { get; set; }
        public virtual string CalcInjectionWaterStream_Color { get; set; }
        public virtual string InjectionWaterStream_Color { get; set; }
        public virtual string CalcHXNetworkColdStream_Color { get; set; }
        public virtual string HXNetworkColdStream_Color { get; set; }
        public virtual string ReliefLoad_Color { get; set; }
        public virtual string ReliefMW_Color { get; set; }
        public virtual string ReliefTemperature_Color { get; set; }
        public virtual string ReliefPressure_Color { get; set; }
        public virtual string ReliefCpCv_Color { get; set; }
        public virtual string ReliefZ_Color { get; set; }
        public virtual string IsSolved_Color { get; set; }
    }
}
