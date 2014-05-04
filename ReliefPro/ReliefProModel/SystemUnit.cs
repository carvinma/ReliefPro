using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class SystemUnit
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual double Constant { get; set; }
        public virtual double ScaleFactor { get; set; }
        public virtual int UnitType { get; set; }
    }
}
