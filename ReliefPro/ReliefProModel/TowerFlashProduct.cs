﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public  class TowerFlashProduct
    {
        public virtual int ID { get; set; }
        public virtual string StreamName { get; set; }
        public virtual double TotalMolarRate { get; set; }
        public virtual string TotalComposition { get; set; }
        public virtual string CompIn { get; set; }
        public virtual string Componentid { get; set; }
        public virtual string ProdType { get; set; }
        public virtual double Pressure { get; set; }
        public virtual double Temperature { get; set; }
        public virtual int Tray { get; set; }
        public virtual double WeightFlow { get; set; }
        public virtual double SpEnthalpy { get; set; }

        public virtual string StreamName_Color { get; set; }
        public virtual string TotalMolarRate_Color { get; set; }
        public virtual string TotalComposition_Color { get; set; }
        public virtual string CompIn_Color { get; set; }
        public virtual string Componentid_Color { get; set; }
        public virtual string ProdType_Color { get; set; }
        public virtual string Pressure_Color { get; set; }
        public virtual string Temperature_Color { get; set; }
        public virtual string Tray_Color { get; set; }
        public virtual string WeightFlow_Color { get; set; }
        public virtual string SpEnthalpy_Color { get; set; }
    }
}