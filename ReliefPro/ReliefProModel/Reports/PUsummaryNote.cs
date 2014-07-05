using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.Reports
{
    public class PUsummaryNote
    {
        public virtual int ID { get; set; }
        public virtual int PUsummaryID { get; set; }
        public virtual string Note { get; set; }
    }
}
