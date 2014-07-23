﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel.Drums
{
    public class DrumSize
    {
        public virtual int ID { get; set; }
        public virtual int DrumFireCalcID { get; set; }
        public virtual string Orientation { get; set; }
        public virtual string HeadType { get; set; }
        public virtual double? Elevation { get; set; }
        public virtual double? Diameter { get; set; }
        public virtual double? Length { get; set; }
        public virtual double? NormalLiquidLevel { get; set; }
        public virtual double? HeadNumber { get; set; }
        public virtual double? BootDiameter { get; set; }
        public virtual double? BootHeight { get; set; }

        public virtual string DrumFireCalcID_Color { get; set; }
        public virtual string Orientation_Color { get; set; }
        public virtual string HeadType_Color { get; set; }
        public virtual string Elevation_Color { get; set; }
        public virtual string Diameter_Color { get; set; }
        public virtual string Length_Color { get; set; }
        public virtual string NormalLiquidLevel_Color { get; set; }
        public virtual string HeadNumber_Color { get; set; }
        public virtual string BootDiameter_Color { get; set; }
        public virtual string BootHeight_Color { get; set; }
    }
}
