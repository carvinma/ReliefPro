using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class TowerFireColumnDetail
    {
        public virtual int ID { get; set; }
        public virtual double? Internal { get; set; }
        public virtual int Trays { get; set; }
        public virtual double? Height { get; set; }
        public virtual double? Diameter { get; set; }
        public virtual int ColumnID { get; set; }
        public virtual int Segment { get; set; }
    }
}
