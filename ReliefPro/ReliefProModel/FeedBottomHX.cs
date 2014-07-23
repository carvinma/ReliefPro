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
        public virtual double? FeedTin { get; set; }
        public virtual double? FeedTout { get; set; }
        public virtual double? FeedMassRate { get; set; }
        public virtual double? FeedSpEin { get; set; }
        public virtual double? FeedSpEout { get; set; }
        public virtual double? BottomTin { get; set; }
        public virtual double? BottomTout { get; set; }
        public virtual double? BottomReliefTin { get; set; }
        public virtual double? BottomMassRate { get; set; }
        public virtual double? Duty { get; set; }
        public virtual int HeatSourceID { get; set; }
        public virtual double? FeedReliefTout { get; set; }
        public virtual double? FeedReliefSpEout { get; set; }
        public virtual double? Factor { get; set; }
    }
}
