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
    }
}
