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
        public virtual string Duty { get; set; }
        public virtual string Pressure { get; set; }
        public virtual string Temperature { get; set; }
        public virtual string PrzFile { get; set; }
    }
}
