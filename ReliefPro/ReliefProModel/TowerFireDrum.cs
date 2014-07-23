using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class TowerFireDrum
    {
        public virtual int ID { get; set; }
        public virtual double? Diameter { get; set; }
        public virtual double? Orientation { get; set; }
        public virtual double? Length { get; set; }
        public virtual double? NormalLiquidLevel { get; set; }
        public virtual double? HeadNumber { get; set; }
        public virtual string HeadType { get; set; }
        public virtual double? BootDiameter { get; set; }
        public virtual double? BootHeight { get; set; }
        public virtual double? PipingContingency { get; set; }
        public virtual int EqID { get; set; }
        public virtual double? Elevation { get; set; }
    }
}
