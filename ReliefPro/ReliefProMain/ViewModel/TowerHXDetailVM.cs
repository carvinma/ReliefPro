using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
        
namespace ReliefProMain.ViewModel
{
    public class TowerHXDetailVM : ViewModelBase
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
        public List<string> ProcessSideFlowSources { get; set; }
        public List<string> Mediums { get; set; }
        public List<string> MediumSideFlowSources { get; set; }

        public List<string> GetProcessSideFlowSources()
        {
            List<string> list = new List<string>();
            list.Add("Pressure Driven");
            list.Add("Pump(Motor)");
            list.Add("Pump(Turbine)");
            list.Add("Compressor(Turbine)");
            list.Add("Compressor(Motor)");
            return list;
        }
        public List<string> GetMediums()
        {
            List<string> list = new List<string>();
            list.Add("Cooling Water");
            list.Add("Air");
            list.Add("Steam");
            list.Add("Hot Oil/Hot Water");
            list.Add("Process Stream");
            list.Add("Fired Heater");
            return list;
        }
        public List<string> GetMediumSideFlowSources(string medium)
        {
            List<string> list = new List<string>();
            switch (medium)
            {
                case "Cooling Water":
                    list.Add("Pump(Motor)");
                    list.Add("Pump(Turbine)");
                    list.Add("Pressurized Vessel");
                    list.Add("Supply Header");
                    break;
                case "Air":
                    list.Add("Fan(Motor)");
                    break;
                case "Steam":
                    list.Add("Pressurized Vessel");
                    list.Add("Supply Header");
                    break;
                case "Hot Oil/Hot Water":
                    list.Add("Pump(Motor)");
                    list.Add("Pump(Turbine)");
                    list.Add("Pressurized Vessel");
                    list.Add("Supply Header");
                    break;
                case "Process Stream":
                    list.Add("Pump(Motor)");
                    list.Add("Pump(Turbine)");
                    list.Add("Compressor(Turbine)");
                    list.Add("Compressor(Pump)");

                    list.Add("Pressurized Vessel");
                    list.Add("Supply Header");
                    break;
                case "Fired Heater":
                    list.Add("Pressurized Vessel");
                    list.Add("Supply Header");
                    break;
                default:
                    list.Add("Fan(Motor)");
                    list.Add("Pump(Motor)");
                    list.Add("Pump(Turbine)");
                    list.Add("Compressor(Turbine)");
                    list.Add("Compressor(Pump)");
                    list.Add("Pressurized Vessel");
                    list.Add("Supply Header");
                    break;
            }
            return list;
        }
        
        public TowerHXDetail CurrentTowerHXDetail { get; set; }

        private TowerHXDetail originalValue;

        public TowerHXVM ObjTowerHXVM
        {
            get;
            set;
        }
        public int ID { get; set; }
        public string DetailName { get; set; }
        public string ProcessSideFlowSource { get; set; }
        public string MediumSideFlowSource { get; set; }
        private string medium;
        public string Medium 
        {
            get { return medium; }
            set {
                medium = value;
                MediumSideFlowSources = GetMediumSideFlowSources(medium);
            }
        }
        public string DutyPercentage { get; set; }
        public string Duty { get; set; }
        public int HXID { get; set; }

        public Mode Mode
        {
            get;
            set;
        }     
        internal TowerHXDetailVM(TowerHXDetail obj)
        {
            ID = obj.ID;
            DetailName = obj.DetailName;
            ProcessSideFlowSource = obj.ProcessSideFlowSource;
            Medium = obj.Medium;
            MediumSideFlowSource = obj.MediumSideFlowSource;
            DutyPercentage = obj.DutyPercentage;
            Duty = obj.Duty;
            HXID = obj.HXID;
            ProcessSideFlowSources = GetProcessSideFlowSources();
            Mediums = GetMediums();
            MediumSideFlowSources = GetMediumSideFlowSources(Medium);
            //this.originalValue = (OrderViewModel)this.MemberwiseClone();
        }

        internal TowerHXDetailVM(TowerHXVM hx)
        {
            dbPlantFile = hx.dbPlantFile;
            dbProtectedSystemFile = hx.dbProtectedSystemFile;
            ObjTowerHXVM = hx;
            ID = 0;
            DetailName = string.Empty;
            ProcessSideFlowSource = string.Empty;
            ProcessSideFlowSource = string.Empty;
            Medium = string.Empty;
            MediumSideFlowSource = string.Empty;
            DutyPercentage = string.Empty;
            Duty = string.Empty;
            HXID = hx.ID;
            ProcessSideFlowSources = GetProcessSideFlowSources();
            Mediums = GetMediums();
            MediumSideFlowSources = GetMediumSideFlowSources(Medium);
        }

        private ICommand _AddTowerHXDetail;
        public ICommand AddTowerHXDetail
        {           
            get
            {
                if (_AddTowerHXDetail == null)
                {
                    _AddTowerHXDetail = new RelayCommand(OKClick);
                    
                }

                return _AddTowerHXDetail;
            }
        }

        private void OKClick(object window)
        {
            DetailName.Trim();
            if (DetailName == "")
            {
                throw new ArgumentException("Please type in a name for the HX.");
            }
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                CurrentTowerHXDetail = new TowerHXDetail();
                var Session = helper.GetCurrentSession();
                dbTowerHXDetail db = new dbTowerHXDetail();
                CurrentTowerHXDetail.HXID = ObjTowerHXVM.ID;
                CurrentTowerHXDetail.ProcessSideFlowSource = ProcessSideFlowSource;
                CurrentTowerHXDetail.Medium = Medium;
                CurrentTowerHXDetail.MediumSideFlowSource = MediumSideFlowSource;
                CurrentTowerHXDetail.DetailName = DetailName;
                CurrentTowerHXDetail.DutyPercentage = DutyPercentage;
                double duty = double.Parse(DutyPercentage) * double.Parse(ObjTowerHXVM.HeaterDuty) / 100;
                CurrentTowerHXDetail.Duty = duty.ToString();
                db.Add(CurrentTowerHXDetail, Session);
                Session.Flush();
            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

       
    }
}
