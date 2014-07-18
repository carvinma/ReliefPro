using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.Reports;

namespace ReliefProMain.Model.Reports
{
    public class PlantSummaryModel : ModelBase
    {
        private PUsummary pu;
        public PlantSummaryModel()
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
