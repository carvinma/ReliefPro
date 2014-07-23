using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class CondenserCalc
    {
        public virtual int ID { get; set; }     
        public virtual bool Flooding { get; set; }
        public virtual string SurgeTime { get; set; }
        public virtual bool IsSurgeTime { get; set; }

        public virtual string Flooding_Color { get; set; }
        public virtual string SurgeTime_Color { get; set; }
        public virtual string IsSurgeTime_Color { get; set; }
    }
}
