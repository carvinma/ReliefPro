using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel.Drums
{
    public class DrumBlockedOutlet
    {
        public virtual int ID { get; set; }
        public virtual int DrumID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual double MaxPressure { get; set; }
        public virtual double MaxStreamRate { get; set; }
        public virtual string DrumType { get; set; }
        public virtual double NormalFlashDuty { get; set; }
        public virtual double FDReliefCondition { get; set; }
        public virtual bool Feed { get; set; }
        public virtual bool ReboilerPinch { get; set; }
        public virtual string MixProductName { get; set; }

        public virtual string DrumID_Color { get; set; }
        public virtual string ScenarioID_Color { get; set; }
        public virtual string MaxPressure_Color { get; set; }
        public virtual string MaxStreamRate_Color { get; set; }
        public virtual string DrumType_Color { get; set; }
        public virtual string NormalFlashDuty_Color { get; set; }
        public virtual string FDReliefCondition_Color { get; set; }
        public virtual string Feed_Color { get; set; }
        public virtual string ReboilerPinch_Color { get; set; }
    }
}
