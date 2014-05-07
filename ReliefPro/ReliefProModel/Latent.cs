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
        public virtual string ReliefTemperature { get; set; }
        public virtual string LatestEnthalpy { get; set; }
        public virtual string ReliefOHWeightFlow { get; set; }
        public virtual string ReliefPressure { get; set; }
    }
}
