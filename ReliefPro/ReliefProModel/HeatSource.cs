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
        public virtual string Duty { get; set; }
        public virtual int SourceID { get; set; }
    }
}
