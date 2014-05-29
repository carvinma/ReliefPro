﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.HX
{
    public class AirCooledHXFire
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual double WettedBundle { get; set; }
        public virtual double PipingContingency { get; set; }

        public virtual double ReliefLoad { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefPressure { get; set; }
    }
}
