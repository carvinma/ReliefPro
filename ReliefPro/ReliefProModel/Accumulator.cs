using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class Accumulator
    {
        public virtual int ID { get; set; }
        public virtual string AccumulatorName { get; set; }
        public virtual bool Orientation { get; set; }
        public virtual string Diameter { get; set; }
        public virtual string Length { get; set; }
        public virtual string NormalLiquidLevel { get; set; }
       
    }
}
