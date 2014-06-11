using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class AbnormalHeaterDetail
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual double Duty { get; set; }
        public virtual double DutyFactor { get; set; }
        public virtual string HeaterName { get; set; }        
        public virtual int HeaterID { get; set; }
        public virtual int AbnormalType { get; set; }  //1:heatsource  2 reboiler 3 pump
    }
}
