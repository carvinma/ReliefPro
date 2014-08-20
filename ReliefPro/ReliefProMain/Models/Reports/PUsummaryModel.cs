using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;
using ReliefProModel.Reports;

namespace ReliefProMain.Models.Reports
{
    public class PUsummaryModel : ModelBase
    {
        public List<PUsummaryGridDS> listGrid { get; set; }

        public PUsummary pu;
        public PUsummaryModel(PUsummary pu)
        {
            this.pu = pu;
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
