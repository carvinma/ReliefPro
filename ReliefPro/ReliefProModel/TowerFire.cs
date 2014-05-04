using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class TowerFire
    {
        public virtual int ID { get; set; }
        public virtual string HeatInputModel { get; set; }
        public virtual bool IsExist { get; set; }
        public virtual string ReliefLoad { get; set; }
        public virtual string ReliefPressure { get; set; }
        public virtual string ReliefTemperature { get; set; }
        public virtual string ReliefMW { get; set; }
        public virtual string ReliefCpCv { get; set; }
        public virtual string ReliefZ { get; set; }
        public virtual int ScenarioID { get; set; }
    }
}
