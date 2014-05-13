using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public  class FeedBottomHX
    {
        public virtual int ID { get; set; }
        public virtual string FeedNormaltin { get; set; }
        public virtual string FeedNormalENTHALPYin { get; set; }
        public virtual string StreamName { get; set; }
        public virtual string BottomNormalTin { get; set; }
        public virtual string BottomNormalTout { get; set; }
        public virtual string BottomReliefTin { get; set; }
        public virtual string QAQNGuess { get; set; }
        public virtual string TotalCount { get; set; }
        public virtual string FeedTemperature { get; set; }
        public virtual string FeedEnthalpy { get; set; }
        public virtual string QRQN { get; set; }
    }
}
