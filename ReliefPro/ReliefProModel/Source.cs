using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class Source
    {
        public virtual int ID { get; set; }
        public virtual string SourceName { get; set; }
        public virtual string Description { get; set; }
        public virtual string SourceType { get; set; }
        public virtual double MaxPossiblePressure { get; set; }
        public virtual bool IsMaintained { get; set; }

        public virtual string SourceName_Color { get; set; }
        public virtual string Description_Color { get; set; }
        public virtual string SourceType_Color { get; set; }
        public virtual string MaxPossiblePressure_Color { get; set; }
        public virtual string IsMaintained_Color { get; set; }
        public virtual string StreamName { get; set; }
        public virtual bool IsSteam { get; set; }
        public virtual bool IsHeatSource { get; set; }
    }
}
