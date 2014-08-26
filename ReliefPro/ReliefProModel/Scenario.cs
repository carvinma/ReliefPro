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
        public virtual double ReliefLoad { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefPressure { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefCpCv { get; set; }
        public virtual double ReliefZ { get; set; }
        public virtual double ReliefVolumeRate { get; set; }
        public virtual bool Flooding { get; set; }
        public virtual bool IsSurgeCalculation { get; set; }
        public virtual double SurgeTime { get; set; }
        public virtual string dbPath { get; set; }
        private string phase = "V";
        public virtual string Phase { get { return phase; } set { phase = value; } }

        //color
        public virtual string ScenarioName_Color { get; set; }
        public virtual string ReliefLoad_Color { get; set; }
        public virtual string ReliefTemperature_Color { get; set; }
        public virtual string ReliefPressure_Color { get; set; }
        public virtual string ReliefMW_Color { get; set; }
        public virtual string ReliefCpCv_Color { get; set; }
        public virtual string ReliefZ_Color { get; set; }
        public virtual string ReliefVolumeRate_Color { get; set; }
        public virtual string Flooding_Color { get; set; }
        public virtual string IsSurgeCalculation_Color { get; set; }
        public virtual string SurgeTime_Color { get; set; }
        public virtual string dbPath_Color { get; set; }
        
        public virtual string Phase_Color { get { return phase; } set { phase = value; } }
    }
}
