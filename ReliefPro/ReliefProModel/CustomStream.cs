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
        public virtual string ProdType { get; set; }
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


        public virtual string StreamName_Color { get; set; }
        public virtual string TotalMolarRate_Color { get; set; }
        public virtual string TotalComposition_Color { get; set; }
        public virtual string CompIn_Color { get; set; }
        public virtual string Componentid_Color { get; set; }
        public virtual string PrintNumber_Color { get; set; }
        public virtual string ProdType_Color { get; set; }
        public virtual string Pressure_Color { get; set; }
        public virtual string Temperature_Color { get; set; }
        public virtual string IsProduct_Color { get; set; }
        public virtual string WeightFlow_Color { get; set; }
        public virtual string SpEnthalpy_Color { get; set; }
        public virtual string Tray_Color { get; set; }
        public virtual string BulkDensityAct_Color { get; set; }
        public virtual string VaporFraction_Color { get; set; }
        public virtual string BulkMwOfPhase_Color { get; set; }
        public virtual string BulkViscosity_Color { get; set; }
        public virtual string BulkCPCVRatio_Color { get; set; }
        public virtual string VaporZFmKVal_Color { get; set; }
        public virtual string BulkCP_Color { get; set; }
        public virtual string BulkThermalCond_Color { get; set; }
        public virtual string BulkSurfTension_Color { get; set; }
        public virtual string TotalMolarEnthalpy_Color { get; set; }
        public virtual string InertWeightEnthalpy_Color { get; set; }
        public virtual string InertWeightRate_Color { get; set; }
        public virtual string Description_Color { get; set; }
    }
}
