using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.CompressorBlocked
{
    public class Centrifugal
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual double Scale { get; set; }
        public virtual double Reliefload { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefPressure { get; set; }
    }
}
