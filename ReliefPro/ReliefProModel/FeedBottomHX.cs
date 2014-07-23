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

        public virtual string StreamName_Color { get; set; }
        public virtual string FeedTin_Color { get; set; }
        public virtual string FeedTout_Color { get; set; }
        public virtual string FeedMassRate_Color { get; set; }
        public virtual string FeedSpEin_Color { get; set; }
        public virtual string FeedSpEout_Color { get; set; }
        public virtual string BottomTin_Color { get; set; }
        public virtual string BottomTout_Color { get; set; }
        public virtual string BottomReliefTin_Color { get; set; }
        public virtual string BottomMassRate_Color { get; set; }
        public virtual string Duty_Color { get; set; }
        public virtual string HeatSourceID_Color { get; set; }
        public virtual string FeedReliefTout_Color { get; set; }
        public virtual string FeedReliefSpEout_Color { get; set; }
        public virtual string Factor_Color { get; set; }
    }
}
