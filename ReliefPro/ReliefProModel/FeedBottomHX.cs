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
        public virtual string StreamName { get; set; }
        public virtual string FeedTin { get; set; }
        public virtual string FeedTout { get; set; }
        public virtual string FeedMassRate { get; set; }
        public virtual string FeedSpEin { get; set; }
        public virtual string FeedSpEout { get; set; }      
        public virtual string BottomTin { get; set; }
        public virtual string BottomTout { get; set; }
        public virtual string BottomReliefTin { get; set; }
        public virtual string BottomMassRate { get; set; }
        public virtual string Duty { get; set; }
        public virtual int HeatSourceID { get; set; }
        public virtual string FeedReliefTout { get; set; }
        public virtual string FeedReliefSpEout { get; set; }
        public virtual string Factor { get; set; }
    }
}
