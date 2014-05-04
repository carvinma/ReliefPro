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
        public virtual string Internal { get; set; }
        public virtual string Trays { get; set; }
        public virtual string Height { get; set; }
        public virtual string Diameter { get; set; }
        public virtual int ColumnID { get; set; }
    }
}
