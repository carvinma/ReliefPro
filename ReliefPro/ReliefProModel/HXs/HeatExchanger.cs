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
        public virtual double? Duty { get; set; }
        public virtual string PrzFile { get; set; }
    }
}
