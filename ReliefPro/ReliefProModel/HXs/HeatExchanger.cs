using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel.HXs
{
    public class HeatExchanger
    {
        public virtual int ID { get; set; }
        public virtual string HXName { get; set; }
        public virtual string HXType { get; set; }
        public virtual double Duty { get; set; }
        public virtual string SourceFile { get; set; }

        public virtual string HXName_Color { get; set; }
        public virtual string HXType_Color { get; set; }
        public virtual string Duty_Color { get; set; }
        public virtual string SourceFile_Color { get; set; }


        public virtual double Pressure { get; set; }
        public virtual double Temperature { get; set; }
        public virtual string FirstFeed { get; set; }
        public virtual string FirstProduct { get; set; }
        public virtual string LastFeed { get; set; }
        public virtual string LastProduct { get; set; }

    }
}
