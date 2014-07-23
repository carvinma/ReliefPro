using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public  class ReboilerPinch
    {
        public virtual int ID { get; set; }
        public virtual int TowerScenarioHXID { get; set; }
        public virtual double? Coldtin { get; set; }
        public virtual double? Coldtout { get; set; }
        public virtual double? HeatTin { get; set; }
        public virtual double? HeatTout { get; set; }
        public virtual string SourceType { get; set; }
        public virtual string StreamName { get; set; }
        public virtual double? SupplyHeadPressure { get; set; }
        public virtual bool IsUseFlowStop { get; set; }
        public virtual bool IsHotStreamInProII { get; set; }
        public virtual double? UDesign { get; set; }
        public virtual double? UClean { get; set; }
        public virtual double? ReliefColdtout { get; set; }
        public virtual double? ReliefHeatTin { get; set; }
        public virtual double? Duty { get; set; }
        public virtual double? ReliefDuty { get; set; }
        public virtual int TotalCount { get; set; }
        public virtual double? QRQN { get; set; }
        public virtual double? Factor { get; set; }
        public virtual bool IsPinch { get; set; }
        public virtual double? Area { get; set; }
    }
}
