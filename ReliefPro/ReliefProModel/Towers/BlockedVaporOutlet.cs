﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.Towers
{
    public class BlockedVaporOutlet
    {
        public virtual int ID { get; set; }
        public virtual int OutletType { get; set; } //0-BlockedVaporOutlet,1-AbsorbentStops
        public virtual double InletGasUpstreamMaxPressure { get; set; }
        public virtual double InletAbsorbentUpstreamMaxPressure { get; set; }
        public virtual double NormalGasFeedWeightRate { get; set; }
        public virtual double NormalGasProductWeightRate { get; set; }
    }
}