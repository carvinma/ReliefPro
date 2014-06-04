using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class TowerScenarioHX
    {
        public virtual int ID { get; set; }
        public virtual int DetailID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual bool DutyLost { get; set; }
        public virtual double DutyCalcFactor { get; set; }
        public virtual bool IsPinch { get; set; }
        public virtual double PinchFactor { get; set; }
        public virtual string DetailName{ get; set; }
        public virtual int HeaterType { get; set; }
        public virtual string Medium { get; set; }
        //public virtual int HXID { get; set; }
    }
}
