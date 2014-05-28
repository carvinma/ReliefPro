using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class Compressor
    {
        public virtual int ID { get; set; }
        public virtual string CompressorName { get; set; }
        public virtual string CompressorType { get; set; }      
        public virtual string PrzFile { get; set; }
    }
}
