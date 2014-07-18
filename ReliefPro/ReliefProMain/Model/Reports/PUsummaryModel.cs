﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;
using ReliefProModel.Reports;

namespace ReliefProMain.Model.Reports
{
    public class PUsummaryModel : ModelBase
    {
        public List<PUsummaryGridDS> listGrid { get; set; }

        private PUsummary pu;
        public PUsummaryModel()
        {
            pu = new PUsummary();
        }

        public string PlantName
        {
            get { return pu.PlantName; }
            set
            {
                pu.PlantName = value;
                this.NotifyPropertyChanged("PlantName");
            }
        }

        public string ProcessUnitName
        {
            get { return pu.ProcessUnitName; }
            set
            {
                pu.ProcessUnitName = value;
                this.NotifyPropertyChanged("ProcessUnitName");
            }
        }
        public string Description
        {
            get { return pu.Description; }
            set
            {
                pu.Description = value;
                this.NotifyPropertyChanged("Description");
            }
        }
    }
}
