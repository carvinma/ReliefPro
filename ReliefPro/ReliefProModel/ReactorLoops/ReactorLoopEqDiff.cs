using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.ReactorLoops
{
    public class ReactorLoopEqDiff
    {
        public virtual int ID { get; set; }
        public virtual int ReactorLoopID { get; set; }
        public virtual string EqName { get; set; }
        public virtual string EqType { get; set; }
        public virtual double OrginDuty { get; set; }
        public virtual double CurrentDuty { get; set; }
        public virtual double Diff { get; set; }
    }
}
