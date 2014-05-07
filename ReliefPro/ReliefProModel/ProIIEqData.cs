using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public  class ProIIEqData
    {
        public virtual int ID { get; set; }
        public virtual string EqType { get; set; }
        public virtual string EqName { get; set; }
        public virtual string SourceFile { get; set; }
        public virtual string FeedData { get; set; }
        public virtual string ProductData { get; set; }
        public virtual string PressureDrop { get; set; }
        public virtual string Duty { get; set; }
        public virtual string MolarFlow { get; set; }
        public virtual string LiquidVolumeFlow { get; set; }
        public virtual string GasVolumeFlow { get; set; }
        public virtual string Efficiency { get; set; }
        public virtual string Head { get; set; }
        public virtual string Work { get; set; }
        public virtual string HotsideTemperature { get; set; }
        public virtual string ColdsideTemperature { get; set; }
        public virtual string HotsidePressureDrop { get; set; }
        public virtual string ColdsidePressureDrop { get; set; }
        public virtual string Area { get; set; }
        public virtual string Diameter { get; set; }
        public virtual string CondenserDuty { get; set; }
        public virtual string ReboilerDuty { get; set; }
        public virtual string DutyCalc { get; set; }
        public virtual string WorkActualCalc { get; set; }
        public virtual string NumberOfTrays { get; set; }
        public virtual string FeedTrays { get; set; }
        public virtual string ProdTrays { get; set; }
        public virtual string ProdType { get; set; }
        public virtual string HeaterNames { get; set; }
        public virtual string HeaterDuties { get; set; }
        public virtual string HeaterNumber { get; set; }
        public virtual string HeaterPANumberfo { get; set; }
        public virtual string HeaterRegOrPAFlag { get; set; }
        public virtual string HeaterTrayLoc { get; set; }
        public virtual string HeaterTrayNumber { get; set; }
    }
}
