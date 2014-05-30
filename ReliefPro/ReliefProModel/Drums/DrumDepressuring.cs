using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.Drums
{
    public class DrumDepressuring
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual string ShortCut { get; set; }
        public virtual bool FireHeatInput { get; set; }
        public virtual double InitialPressure { get; set; }
        public virtual double? VaporDensity { get; set; }
        public virtual double TotalVaporVolume { get; set; }
        public virtual double Vesseldesignpressure { get; set; }
        public virtual string DepressuringRequirements { get; set; }
        public virtual double TotalWettedArea { get; set; }
        public virtual string HeatInputModel { get; set; }
        public virtual double ValveConstantforSonicFlow { get; set; }

        public virtual double InitialDepressuringRate { get; set; }
        public virtual double Timespecify { get; set; }
        public virtual double CalculatedVesselPressure { get; set; }
        public virtual double CalculatedDepressuringRate { get; set; }

        public virtual double DeltaP { get; set; }
        public virtual double DeltaPTime { get; set; }
        public virtual double TimeStep { get; set; }
    }
}
