using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class Critical
    {
        public virtual int ID { get; set; }
        public virtual double CriticalPressure { get; set; }

        public virtual string CriticalPressure_Color { get; set; } 
    }
}
