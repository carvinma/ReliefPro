using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class TowerHX
    {
        public virtual int ID { get; set; }
        public virtual string HeaterName { get; set; }
        public virtual string Description { get; set; }
        public virtual string HeaterDuty { get; set; }
        public virtual string HeaterName_Color { get; set; }
        public virtual string Description_Color { get; set; }
        public virtual string HeaterDuty_Color { get; set; }
        public virtual int HeaterType { get; set; }
        
        
    }
}
