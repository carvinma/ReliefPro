using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel.Drums
{
    public class DrumFireFluid
    {
        public virtual int ID { get; set; }
        public virtual int DrumFireCalcID { get; set; }
        public virtual double GasVaporMW { get; set; }
        public virtual double ExposedVesse { get; set; }
        public virtual double NormaTemperature { get; set; }
        public virtual double NormalPressure { get; set; }
        public virtual double NormalCpCv { get; set; }
        public virtual double PSVPressure { get; set; }
        public virtual double TW { get; set; }
        public virtual string DrumFireCalcID_Color { get; set; }
        public virtual string GasVaporMW_Color { get; set; }
        public virtual string ExposedVesse_Color { get; set; }
        public virtual string NormaTemperature_Color { get; set; }
        public virtual string NormalPressure_Color { get; set; }
        public virtual string PSVPressure_Color { get; set; }
        public virtual string TW_Color { get; set; }
        public virtual string NormalCpCv_Color { get; set; }
    }
}
