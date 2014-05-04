using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class BasicUnit
    {
        public virtual int ID { get; set; }
        public virtual string UnitName { get; set; }
        public virtual int IsDefault { get; set; }
    }
}
