using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class HeatSource
    {
        public virtual int ID { get; set; }
        public virtual string HeatSourceName { get; set; }
        public virtual string HeatSourceType { get; set; }
        public virtual double? Duty { get; set; }
        public virtual int SourceID { get; set; }

        public virtual string HeatSourceName_Color { get; set; }
        public virtual string HeatSourceType_Color { get; set; }
        public virtual string Duty_Color { get; set; }
        public virtual string SourceID_Color { get; set; }
    }
}
