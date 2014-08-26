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

        public virtual string DetailID_Color { get; set; }
        public virtual string ScenarioID_Color { get; set; }
        public virtual string DutyLost_Color { get; set; }
        public virtual string DutyCalcFactor_Color { get; set; }
        public virtual string IsPinch_Color { get; set; }
        public virtual string PinchFactor_Color { get; set; }
        public virtual string DetailName_Color { get; set; }
        public virtual string HeaterType_Color { get; set; }
        public virtual string Medium_Color { get; set; }
    }
}
