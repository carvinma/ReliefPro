﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class TowerScenarioHeatSource
    {
        public virtual int ID { get; set; }
        public virtual int HeatSourceID { get; set; }
        public virtual int DutyFactor { get; set; }
    }
}
