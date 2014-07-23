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
        public virtual double? Latent { get; set; }
        public virtual double? ReliefTemperature { get; set; }
        public virtual double? ReliefPressure { get; set; }
        public virtual double? ReliefMW { get; set; }
        public virtual double? ReliefCpCv { get; set; }
        public virtual double? ReliefZ { get; set; }

        public virtual string ScenarioID_Color { get; set; }
        public virtual string Latent_Color { get; set; }
        public virtual string ReliefTemperature_Color { get; set; }
        public virtual string ReliefPressure_Color { get; set; }
        public virtual string ReliefMW_Color { get; set; }
        public virtual string ReliefCpCv_Color { get; set; }
        public virtual string ReliefZ_Color { get; set; }
        
    }
}
