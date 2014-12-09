using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.Compressors
{
    public class CentrifugalBlockedOutlet
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual double Scale { get; set; }
        public virtual double InletLoad { get; set; }
        public virtual double SurgeLoad { get; set; }
        public virtual double OutletPressure { get; set; }
        public virtual double Reliefload { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefPressure { get; set; }
        public virtual double ReliefZ { get; set; }
        public virtual double ReliefCpCv { get; set; }

        public virtual string ScenarioID_Color { get; set; }
        public virtual string Scale_Color { get; set; }
        public virtual string InletLoad_Color { get; set; }
        public virtual string SurgeLoad_Color { get; set; }
        public virtual string OutletPressure_Color { get; set; }
        public virtual string Reliefload_Color { get; set; }
        public virtual string ReliefMW_Color { get; set; }
        public virtual string ReliefTemperature_Color { get; set; }
        public virtual string ReliefPressure_Color { get; set; }

        public virtual double KNormal { get; set; }
        public virtual double DeltPowY { get; set; }
        public virtual string KNormal_Color { get; set; }
        public virtual string DeltPowY_Color { get; set; }

    }
}
