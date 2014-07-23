﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public  class InletValveOpen
    {
        public virtual int ID { get; set; }
        public virtual string VesselName { get; set; }
        public virtual string UpStameName { get; set; }
        public virtual string OperatingPhase { get; set; }
        public virtual double? MaxOperatingPressure { get; set; }
        public virtual string DownStreamName { get; set; }
        public virtual double? CV { get; set; }
        public virtual double? ReliefLoad { get; set; }
        public virtual double? ReliefTemperature { get; set; }
        public virtual double? ReliefPressure { get; set; }
        public virtual double? ReliefMW { get; set; }
    }
}
