using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProMain.Models.Reports
{
    public class PlantReprotHead : ModelBase
    {
        private string summationFun;
        public string SummationFun
        {
            get { return summationFun; }
            set
            {
                summationFun = value;
                NotifyPropertyChanged("SummationFun");
            }
        }
        private string plantFlare;
        public string PlantFlare
        {
            get { return plantFlare; }
            set
            {
                plantFlare = value;
                NotifyPropertyChanged("PlantFlare");
            }
        }
        private string dischargeTo;
        public string DischargeTo
        {
            get { return dischargeTo; }
            set
            {
                dischargeTo = value;
                NotifyPropertyChanged("DischargeTo");
            }
        }
    }
}
