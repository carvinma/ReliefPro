using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class CustomStream
    {
        public virtual int ID { get; set; }
        public virtual string StreamName { get; set; }
        public virtual string TotalMolarRate { get; set; }
        public virtual string TotalComposition { get; set; }
        public virtual string CompIn { get; set; }
        public virtual string Componentid { get; set; }
        public virtual string PrintNumber { get; set; }
        public virtual string ProdType { get; set; }
        public virtual string Pressure { get; set; }
        public virtual string Temperature { get; set; }
        public virtual bool IsProduct { get; set; }
        public virtual string WeightFlow { get; set; }
        public virtual string SpEnthalpy { get; set; }
        public virtual string Tray { get; set; }
        public virtual string BulkDensityAct { get; set; }

        public virtual string VaporFraction { get; set; }
        public virtual string BulkMwOfPhase { get; set; }
        public virtual string BulkViscosity { get; set; }
        public virtual string BulkCPCVRatio { get; set; }
        public virtual string VaporZFmKVal { get; set; }
        public virtual string BulkCP { get; set; }
        public virtual string BulkThermalCond { get; set; }
        public virtual string BulkSurfTension { get; set; }
        public virtual string TotalMolarEnthalpy { get; set; }
        public virtual string InertWeightEnthalpy { get; set; }
        public virtual string InertWeightRate { get; set; }
        public virtual string Description { get; set; }

    }
}
