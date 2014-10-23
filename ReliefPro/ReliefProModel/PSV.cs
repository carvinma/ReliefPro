using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class PSV
    {
        public virtual int ID { get; set; }
        public virtual string PSVName { get; set; }
        public virtual string Description { get; set; }
        public virtual int ValveNumber { get; set; }
        public virtual string ValveType { get; set; }
        public virtual double Pressure { get; set; }
        public virtual double ReliefPressureFactor { get; set; }
        public virtual string Location { get; set; }
        public virtual string LocationDescription { get; set; }
        public virtual string DrumPSVName { get; set; }
        public virtual double DrumPressure { get; set; }
        public virtual string MWAP { get; set; }
        public virtual string DischargeTo { get; set; }
        public virtual string dbPath { get; set; }
        public virtual double CriticalPressure { get; set; }
        public virtual string CriticalPressure_Color { get; set; }
        public virtual double CriticalTemperature { get; set; }
        public virtual string CriticalTemperature_Color { get; set; }

        public virtual double CricondenbarPress { get; set; }
        public virtual string CricondenbarPress_Color { get; set; }
        public virtual double CricondenbarTemp { get; set; }
        public virtual string CricondenbarTemp_Color { get; set; } 

        //color
        public virtual string PSVName_Color { get; set; }
        public virtual string Description_Color { get; set; }
        public virtual string ValveNumber_Color { get; set; }
        public virtual string ValveType_Color { get; set; }
        public virtual string Pressure_Color { get; set; }
        public virtual string ReliefPressureFactor_Color { get; set; }
        public virtual string Location_Color { get; set; }
        public virtual string LocationDescription_Color { get; set; }
        public virtual string DrumPSVName_Color { get; set; }
        public virtual string DrumPressure_Color { get; set; }
        public virtual string MWAP_Color { get; set; }
        public virtual string DischargeTo_Color { get; set; }
        public virtual string dbPath_Color { get; set; }
    }
}
