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
        public virtual double ReliefTemperature { get; set; }
        public virtual double LatentEnthalpy { get; set; }
        public virtual double ReliefWeightFlow { get; set; }
        public virtual double ReliefPressure { get; set; }
        public virtual double ReliefCpCv { get; set; }
        public virtual double ReliefZ { get; set; }
        public virtual int LatentType { get; set; }

        public virtual string ReliefTemperature_Color { get; set; }
        public virtual string LatentEnthalpy_Color { get; set; }
        public virtual string ReliefWeightFlow_Color { get; set; }
        public virtual string ReliefPressure_Color { get; set; }
       
    }
}
