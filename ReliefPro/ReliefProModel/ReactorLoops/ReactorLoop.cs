using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.ReactorLoops
{
    public class ReactorLoop
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual string EffluentStream { get; set; }
        public virtual string ColdReactorFeedStream { get; set; }
        public virtual string HotHighPressureSeparator { get; set; }
        public virtual string ColdHighPressureSeparator { get; set; }
        public virtual string HXNetworkColdStream { get; set; }
        public virtual string InjectionWaterStream { get; set; }
    }
}
