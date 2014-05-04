using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class Tower
    {
        public virtual int ID { get; set; }
        public virtual string TowerName { get; set; }
        public virtual string Description { get; set; }
        public virtual string StageNumber { get; set; }
        public virtual string TowerName_Color { get; set; }
        public virtual string Description_Color { get; set; }
        public virtual string StageNumber_Color { get; set; }
        public virtual string PrzFile { get; set; }
    }
}
