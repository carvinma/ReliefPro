using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel.Drum
{
    public class DrumAllGas
    {
        public virtual int ID { get; set; }
        public virtual int DrumCalcID { get; set; }
        public virtual double GasVaporMW { get; set; }
        public virtual double ExposedVesse { get; set; }
        public virtual double NormaTemperature { get; set; }
        public virtual double NormalPressure { get; set; }
        public virtual double PSVPressure { get; set; }
        public virtual double TW { get; set; }
    }
}
