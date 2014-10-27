using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.HXs
{
    public class HXBlockedIn : IScenarioModel
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }

        public virtual string ColdStream { get; set; }
        public virtual double NormalDuty { get; set; }
        public virtual double NormalHotTemperature { get; set; }
        public virtual double NormalColdInletTemperature { get; set; }
        public virtual double NormalColdOutletTemperature { get; set; }

        public virtual double LatentPoint { get; set; }
        public virtual double ReliefLoad { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefPressure { get; set; }
        public virtual double ReliefCpCv { get; set; }
        public virtual double ReliefZ { get; set; }

        public virtual string ScenarioID_Color { get; set; }
        public virtual string ColdStream_Color { get; set; }
        public virtual string NormalDuty_Color { get; set; }
        public virtual string NormalHotTemperature_Color { get; set; }
        public virtual string NormalColdInletTemperature_Color { get; set; }
        public virtual string NormalColdOutletTemperature_Color { get; set; }
        public virtual string LatentPoint_Color { get; set; }
        public virtual string ReliefLoad_Color { get; set; }
        public virtual string ReliefMW_Color { get; set; }
        public virtual string ReliefTemperature_Color { get; set; }
        public virtual string ReliefPressure_Color { get; set; }
    }
}
