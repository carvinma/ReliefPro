using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.ReactorLoops
{
    public class ReactorLoopBlockedOutlet
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual int ReactorType { get; set; }   //0-ReactorLoopBlockedOutlet,1-LossOfReactorQuench,2-LossOfColdFeed
        public virtual double EffluentTemperature { get; set; }
        public virtual double MaxGasRate { get; set; }
        public virtual double TotalPurgeRate { get; set; }
        public virtual double ReliefLoad { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefCpCv { get; set; }
        public virtual double ReliefZ { get; set; }
    }
}
