using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel.Drums
{
    public class DrumFireCalc
    {
        public virtual int ID { get; set; }
        public virtual int DrumID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual string HeatInputModel { get; set; }
        public virtual double WettedArea { get; set; }
        public virtual double LatentHeat { get; set; }
        public virtual double CrackingHeat { get; set; }
        public virtual double DesignPressure { get; set; }
        public virtual double ReliefLoad { get; set; }
        public virtual double ReliefPressure { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefCpCv { get; set; }
        public virtual double ReliefZ { get; set; }

        public virtual bool HeavyOilFluid { get; set; }
        public virtual bool AllGas { get; set; }
        public virtual bool EquipmentExist { get; set; }
    }
}
