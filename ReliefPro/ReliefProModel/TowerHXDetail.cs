using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class TowerHXDetail
    {
        public virtual int ID { get; set; }
        public virtual string DetailName { get; set; }
        public virtual string ProcessSideFlowSource { get; set; }
        public virtual string Medium { get; set; }
        public virtual string MediumSideFlowSource { get; set; }
        public virtual double? DutyPercentage { get; set; }
        public virtual double? Duty { get; set; }
        public virtual int HXID { get; set; }
    }
}
