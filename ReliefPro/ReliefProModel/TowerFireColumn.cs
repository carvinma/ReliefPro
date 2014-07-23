using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class TowerFireColumn
    {
        public virtual int ID { get; set; }
        public virtual int NumberOfSegment { get; set; }
        public virtual double? Elevation { get; set; }
        public virtual double? BNLL { get; set; }
        public virtual double? LiquidHoldup { get; set; }
        public virtual double? PipingContingency { get; set; }
        public virtual int EqID { get; set; }

        public virtual string NumberOfSegment_Color { get; set; }
        public virtual string Elevation_Color { get; set; }
        public virtual string BNLL_Color { get; set; }
        public virtual string LiquidHoldup_Color { get; set; }
        public virtual string PipingContingency_Color { get; set; }
        public virtual string EqID_Color { get; set; }
       
    }
}
