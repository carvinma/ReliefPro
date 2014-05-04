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
        public virtual string DutyPercentage { get; set; }
        public virtual string Duty { get; set; }
        public virtual int HXID { get; set; }
    }
}
