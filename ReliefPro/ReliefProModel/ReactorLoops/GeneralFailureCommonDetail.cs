using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.ReactorLoops
{
    public class GeneralFailureCommonDetail
    {
        public virtual int ID { get; set; }
        public virtual int GeneralFailureCommonID { get; set; }
        public virtual string HXName { get; set; }
        public virtual bool Stop { get; set; }
        public virtual double? DutyFactor { get; set; }

        public virtual string GeneralFailureCommonID_Color { get; set; }
        public virtual string HXName_Color { get; set; }
        public virtual string Stop_Color { get; set; }
        public virtual string DutyFactor_Color { get; set; }
    }
}
