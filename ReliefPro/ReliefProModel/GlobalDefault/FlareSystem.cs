using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.GlobalDefault
{
    public class FlareSystem
    {
        public virtual int ID { get; set; }
        public virtual Guid RowGuid { get; set; }
        public virtual String FlareName { get; set; }
        public virtual double DesignBackPressure { get; set; }
        public virtual bool isDel { get; set; }



    }
}
