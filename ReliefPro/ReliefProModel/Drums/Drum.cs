using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel.Drums
{
    public class Drum
    {
        public virtual int ID { get; set; }
        public virtual string DrumName { get; set; }
        public virtual string DrumType { get; set; }
        public virtual double? Duty { get; set; }
        public virtual double? Pressure { get; set; }
        public virtual double? Temperature { get; set; }
        public virtual string PrzFile { get; set; }
    }
}
