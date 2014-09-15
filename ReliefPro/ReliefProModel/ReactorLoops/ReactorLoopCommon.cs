using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.ReactorLoops
{
    public class ReactorLoopCommon
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual int ReactorType { get; set; }   //0-ReactorLoopBlockedOutlet,1-LossOfReactorQuench,2-LossOfColdFeed
        public virtual double EffluentTemperature { get; set; }
        public virtual double EffluentTemperature2 { get; set; }
        public virtual double MaxGasRate { get; set; }
        public virtual double TotalPurgeRate { get; set; }
        public virtual double ReliefLoad { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefPressure { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefCpCv { get; set; }
        public virtual double ReliefZ { get; set; }

        public virtual string ScenarioID_Color { get; set; }
        public virtual string ReactorType_Color { get; set; }   //0-ReactorLoopBlockedOutlet,1-LossOfReactorQuench,2-LossOfColdFeed
        public virtual string EffluentTemperature_Color { get; set; }
        public virtual string EffluentTemperature2_Color { get; set; }
        public virtual string MaxGasRate_Color { get; set; }
        public virtual string TotalPurgeRate_Color { get; set; }
        public virtual string ReliefLoad_Color { get; set; }
        public virtual string ReliefTemperature_Color { get; set; }
        public virtual string ReliefPressure_Color { get; set; }
        public virtual string ReliefMW_Color { get; set; }
        public virtual string ReliefCpCv_Color { get; set; }
        public virtual string ReliefZ_Color { get; set; }
    }
}
