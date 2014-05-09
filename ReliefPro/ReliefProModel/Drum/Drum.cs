using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel.Drum
{
    public class Drum
    {
        public virtual int ID { get; set; }
        public virtual string DrumName { get; set; }
        public virtual string DrumType { get; set; }
    }
}
