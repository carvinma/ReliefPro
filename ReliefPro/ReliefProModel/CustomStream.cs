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
        public virtual double? TotalMolarRate { get; set; }
        public virtual string TotalComposition { get; set; }
        public virtual string CompIn { get; set; }
        public virtual string Componentid { get; set; }
        public virtual string PrintNumber { get; set; }
        public virtual int ProdType { get; set; }
        public virtual double? Pressure { get; set; }
        public virtual double? Temperature { get; set; }
        public virtual bool IsProduct { get; set; }
        public virtual double? WeightFlow { get; set; }
        public virtual double? SpEnthalpy { get; set; }
        public virtual double? Tray { get; set; }
        public virtual double? BulkDensityAct { get; set; }

        public virtual double? VaporFraction { get; set; }
        public virtual double? BulkMwOfPhase { get; set; }
        public virtual double? BulkViscosity { get; set; }
        public virtual double? BulkCPCVRatio { get; set; }
        public virtual double? VaporZFmKVal { get; set; }
        public virtual double? BulkCP { get; set; }
        public virtual double? BulkThermalCond { get; set; }
        public virtual double? BulkSurfTension { get; set; }
        public virtual double? TotalMolarEnthalpy { get; set; }
        public virtual double? InertWeightEnthalpy { get; set; }
        public virtual double? InertWeightRate { get; set; }
        public virtual string Description { get; set; }

    }
}
