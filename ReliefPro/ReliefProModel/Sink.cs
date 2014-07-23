using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class Sink
    {
        public virtual int ID { get; set; }
        public virtual string SinkName { get; set; }
        public virtual string Description { get; set; }
        public virtual string SinkType { get; set; }
        public virtual double? MaxPossiblePressure { get; set; }    
        public virtual string StreamName { get; set; }

        public virtual string SinkName_Color { get; set; }
        public virtual string Description_Color { get; set; }
        public virtual string SinkType_Color { get; set; }
        public virtual string MaxPossiblePressure_Color { get; set; }
        public virtual string StreamName_Color { get; set; }     
    }
}
