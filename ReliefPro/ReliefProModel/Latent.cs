using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class Latent
    {
        public virtual int ID { get; set; }
        public virtual double? ReliefTemperature { get; set; }
        public virtual double? LatentEnthalpy { get; set; }
        public virtual double? ReliefOHWeightFlow { get; set; }
        public virtual double? ReliefPressure { get; set; }
    }
}
