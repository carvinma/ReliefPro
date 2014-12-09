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
        public virtual string SourceFile { get; set; }
        public virtual string Driver { get; set; } 

        public virtual string CompressorName_Color { get; set; }
        public virtual string CompressorType_Color { get; set; }
        public virtual string SourceFile_Color { get; set; }
        public virtual string Driver_Color { get; set; } 
    }
}
