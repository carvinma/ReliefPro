using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class FlashCalcResult
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual double Latent { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefPressure { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefCpCv { get; set; }
        public virtual double ReliefZ { get; set; }
        
    }
}
