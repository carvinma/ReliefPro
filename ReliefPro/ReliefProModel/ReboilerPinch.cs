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
        public virtual double Coldtin { get; set; }
        public virtual double Coldtout { get; set; }
        public virtual double HeatTin { get; set; }
        public virtual double HeatTout { get; set; }
        public virtual string SourceType { get; set; }
        public virtual string StreamName { get; set; }
        public virtual double SupplyHeadPressure { get; set; }
        public virtual bool IsUseFlowStop { get; set; }
        public virtual bool IsHotStreamInProII { get; set; }
        public virtual double UDesign { get; set; }
        public virtual double UClean { get; set; }
        public virtual double ReliefColdtout { get; set; }
        public virtual double ReliefHeatTin { get; set; }
        public virtual double Duty { get; set; }
        public virtual double ReliefDuty { get; set; }
        public virtual int TotalCount { get; set; }
        public virtual double QRQN { get; set; }
        public virtual double Factor { get; set; }
        public virtual bool IsPinch { get; set; }
        public virtual double Area { get; set; }

        public virtual string TowerScenarioHXID_Color { get; set; }
        public virtual string Coldtin_Color { get; set; }
        public virtual string Coldtout_Color { get; set; }
        public virtual string HeatTin_Color { get; set; }
        public virtual string HeatTout_Color { get; set; }
        public virtual string SourceType_Color { get; set; }
        public virtual string StreamName_Color { get; set; }
        public virtual string SupplyHeadPressure_Color { get; set; }
        public virtual string IsUseFlowStop_Color { get; set; }
        public virtual string IsHotStreamInProII_Color { get; set; }
        public virtual string UDesign_Color { get; set; }
        public virtual string UClean_Color { get; set; }
        public virtual string ReliefColdtout_Color { get; set; }
        public virtual string ReliefHeatTin_Color { get; set; }
        public virtual string Duty_Color { get; set; }
        public virtual string ReliefDuty_Color { get; set; }
        public virtual string TotalCount_Color { get; set; }
        public virtual string QRQN_Color { get; set; }
        public virtual string Factor_Color { get; set; }
        public virtual string IsPinch_Color { get; set; }
        public virtual string Area_Color { get; set; }
    }
}
