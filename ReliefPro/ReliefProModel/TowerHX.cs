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
        public virtual double HeaterDuty { get; set; }

        public virtual int HeaterName_Color { get; set; }
        public virtual int Description_Color { get; set; }
        public virtual int HeaterDuty_Color { get; set; }
        public virtual int HeaterType { get; set; }
        
        
    }
}
