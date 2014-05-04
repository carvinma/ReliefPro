using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class LatentProduct
    {
        public virtual int ID { get; set; }
        public virtual string StreamName { get; set; }
        public virtual string TotalMolarRate { get; set; }
        public virtual string TotalComposition { get; set; }
        public virtual string CompIn { get; set; }
        public virtual string Componentid { get; set; }
        public virtual string ProdType { get; set; }
        public virtual string Pressure { get; set; }
        public virtual string Temperature { get; set; }
        public virtual string Tray { get; set; }
        public virtual string WeightFlow { get; set; }
        public virtual string SpEnthalpy { get; set; }
        public virtual string BulkDensityAct { get; set; }
    }
}
