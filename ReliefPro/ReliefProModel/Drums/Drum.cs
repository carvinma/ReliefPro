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
        public virtual double Duty { get; set; }
        public virtual double Pressure { get; set; }
        public virtual double Temperature { get; set; }
        public virtual string SourceFile { get; set; }

        public virtual string DrumName_Color { get; set; }
        public virtual string DrumType_Color { get; set; }
        public virtual string Duty_Color { get; set; }
        public virtual string Pressure_Color { get; set; }
        public virtual string Temperature_Color { get; set; }
        public virtual string SourceFile_Color { get; set; }
    }
}
