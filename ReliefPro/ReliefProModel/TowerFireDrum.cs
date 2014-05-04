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
        public virtual string Diameter { get; set; }
        public virtual string Orientation { get; set; }
        public virtual string Length { get; set; }
        public virtual string NormalLiquidLevel { get; set; }
        public virtual string HeadNumber { get; set; }
        public virtual string HeadType { get; set; }
        public virtual string BootDiameter { get; set; }
        public virtual string BootHeight { get; set; }
        public virtual string PipingContingency { get; set; }
        public virtual int EqID { get; set; }
        public virtual string Elevation { get; set; }
    }
}
