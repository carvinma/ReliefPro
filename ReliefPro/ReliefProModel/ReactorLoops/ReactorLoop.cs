using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.ReactorLoops
{
    public class ReactorLoop
    {
        public virtual int ID { get; set; }
        public virtual string EffluentStream { get; set; }
        public virtual string ColdReactorFeedStream { get; set; }
        public virtual string EffluentStream2 { get; set; }
        public virtual string ColdReactorFeedStream2 { get; set; }
        public virtual string HotHighPressureSeparator { get; set; }
        public virtual string ColdHighPressureSeparator { get; set; }
        public virtual string HXNetworkColdStream { get; set; }
        public virtual string InjectionWaterStream { get; set; }
        public virtual string CompressorH2Stream { get; set; }
        public virtual string SourceFile { get; set; }
        
        public virtual string EffluentStream_Color { get; set; }
        public virtual string ColdReactorFeedStream_Color { get; set; }
        public virtual string EffluentStream2_Color { get; set; }
        public virtual string ColdReactorFeedStream2_Color { get; set; }
        public virtual string HotHighPressureSeparator_Color { get; set; }
        public virtual string ColdHighPressureSeparator_Color { get; set; }
        public virtual string HXNetworkColdStream_Color { get; set; }
        public virtual string InjectionWaterStream_Color { get; set; }
        public virtual string CompressorH2Stream_Color { get; set; }
    }
}
