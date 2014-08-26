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
        public virtual double VaporDensity { get; set; }
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


        public virtual string ScenarioID_Color { get; set; }
        public virtual string ShortCut_Color { get; set; }
        public virtual string FireHeatInput_Color { get; set; }
        public virtual string InitialPressure_Color { get; set; }
        public virtual string VaporDensity_Color { get; set; }
        public virtual string TotalVaporVolume_Color { get; set; }
        public virtual string Vesseldesignpressure_Color { get; set; }
        public virtual string DepressuringRequirements_Color { get; set; }
        public virtual string TotalWettedArea_Color { get; set; }
        public virtual string HeatInputModel_Color { get; set; }
        public virtual string ValveConstantforSonicFlow_Color { get; set; }

        public virtual string InitialDepressuringRate_Color { get; set; }
        public virtual string Timespecify_Color { get; set; }
        public virtual string CalculatedVesselPressure_Color { get; set; }
        public virtual string CalculatedDepressuringRate_Color { get; set; }

        public virtual string DeltaP_Color { get; set; }
        public virtual string DeltaPTime_Color { get; set; }
        public virtual string TimeStep_Color { get; set; }
    }
}
