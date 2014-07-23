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
        public virtual double? Diameter { get; set; }
        public virtual double? Length { get; set; }
        public virtual double? NormalLiquidLevel { get; set; }

        public virtual string AccumulatorName_Color { get; set; }
        public virtual string Orientation_Color { get; set; }
        public virtual string Diameter_Color { get; set; }
        public virtual string Length_Color { get; set; }
        public virtual string NormalLiquidLevel_Color { get; set; }
        
    }
}
