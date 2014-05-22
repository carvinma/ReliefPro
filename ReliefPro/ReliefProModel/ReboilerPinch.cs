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
        public virtual string Coldtin { get; set; }
        public virtual string Coldtout { get; set; }
        public virtual string HeatTin { get; set; }
        public virtual string HeatTout { get; set; }
        public virtual string SourceType { get; set; }
        public virtual string StreamName { get; set; }
        public virtual string SupplyHeadPressure { get; set; }
        public virtual bool IsUseFlowStop { get; set; }
        public virtual bool IsHotStreamInProII { get; set; }
        public virtual string UDesign { get; set; }
        public virtual string UClean { get; set; }
        public virtual string CodetoutRelief { get; set; }
        public virtual string HeatTinRelief { get; set; }
        public virtual string Duty { get; set; }
        public virtual string ReliefDuty { get; set; }
        public virtual string TotalCount { get; set; }
        public virtual string QRQN { get; set; }
        public virtual bool IsPinch { get; set; }
    }
}
