using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class Scenario
    {
        public virtual int ID { get; set; }
        public virtual string ScenarioName { get; set; }
        public virtual string ReliefLoad { get; set; }
        public virtual string ReliefTemperature { get; set; }
        public virtual string ReliefPressure { get; set; }
        public virtual string ReliefMW { get; set; }
        public virtual string ReliefCpCv { get; set; }
        public virtual string ReliefZ { get; set; }
        public virtual string ReliefVolumeRate { get; set; }
        public virtual bool Flooding { get; set; }
        public virtual bool IsSurgeCalculation { get; set; }
        public virtual string SurgeTime { get; set; }
        public virtual string dbPath { get; set; }
        private string phase = "V";
        public virtual string Phase { get { return phase; } set { phase = value; } }
    }
}
