using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using NHibernate;
using ReliefProBLL.Common;
using ReliefProDAL;

namespace ReliefProMain.Models
{
    public class TowerHXDetailModel : ModelBase
    {
        public  TowerHXDetailModel()
        {
            _ProcessSideFlowSources=GetProcessSideFlowSources();
            _Mediums=GetMediums();
        }

        public TowerHXModel _Parent;
        public TowerHXModel Parent
        {
            get
            {
                return this._Parent;
            }
            set
            {
                this._Parent = value;
                NotifyPropertyChanged("Parent");
            }
        }

        private int _SeqNumber;
        public int SeqNumber
        {
            get
            {
                return this._SeqNumber;
            }
            set
            {
                this._SeqNumber = value;
                NotifyPropertyChanged("SeqNumber");
            }
        }

        private int _ID;
        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
                NotifyPropertyChanged("ID");
            }
        }

        private string _DetailName;
        public string DetailName
        {
            get
            {
                return this._DetailName;
            }
            set
            {
                this._DetailName = value;
                NotifyPropertyChanged("DetailName");
            }
        }
        private string _ProcessSideFlowSource;
        public string ProcessSideFlowSource
        {
            get
            {
                return this._ProcessSideFlowSource;
            }
            set
            {
                this._ProcessSideFlowSource = value;
                NotifyPropertyChanged("ProcessSideFlowSource");
            }
        }
        private string _Medium;
        public string Medium
        {
            get
            {
                return this._Medium;
            }
            set
            {
                this._Medium = value;
                this.MediumSideFlowSources = GetMediumSideFlowSources(_Medium);
                this.MediumSideFlowSource = MediumSideFlowSources[0];
                NotifyPropertyChanged("Medium");                
            }
        }

        private string _MediumSideFlowSource;
        public string MediumSideFlowSource
        {
            get
            {
                return this._MediumSideFlowSource;
            }
            set
            {
                this._MediumSideFlowSource = value;
                NotifyPropertyChanged("MediumSideFlowSource");
            }
        }

        private double _DutyPercentage;
        public double DutyPercentage
        {
            get
            {
                return this._DutyPercentage;
            }
            set
            {
                this._DutyPercentage = value;
                this.Duty = (_DutyPercentage * Parent.HeaterDuty/100);
                NotifyPropertyChanged("DutyPercentage");
            }
        }

        private double _Duty;
        public double Duty
        {
            get
            {
                return this._Duty;
            }
            set
            {
                this._Duty = value;
                NotifyPropertyChanged("Duty");
            }
        }

        private int _HXID;
        public int HXID
        {
            get
            {
                return this._HXID;
            }
            set
            {
                this._HXID = value;
                NotifyPropertyChanged("HXID");
            }
        }


        private ObservableCollection<string> _ProcessSideFlowSources;
        public ObservableCollection<string> ProcessSideFlowSources
        {
            get
            {
                return _ProcessSideFlowSources;
            }
            set
            {
                this._ProcessSideFlowSources = value;
                NotifyPropertyChanged("ProcessSideFlowSources");
            }
        }

        private ObservableCollection<string> _Mediums;
        public ObservableCollection<string> Mediums
        {
            get
            {
                return _Mediums; 
            }
            set
            {
                this._Mediums = value;
                this.Medium = this._Mediums[0];
                NotifyPropertyChanged("Mediums");
            }
        }

        private ObservableCollection<string> _MediumSideFlowSources;
        public ObservableCollection<string> MediumSideFlowSources
        {
            get
            {
                return _MediumSideFlowSources;
            }
            set
            {
                this._MediumSideFlowSources = value;
                this._MediumSideFlowSource = this._MediumSideFlowSources[0];
                NotifyPropertyChanged("MediumSideFlowSources");
            }
        }


        public ObservableCollection<string> GetProcessSideFlowSources()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Pressure Driven");
            list.Add("Pump(Motor)");
            list.Add("Pump(Turbine)");
            list.Add("Compressor(Turbine)");
            list.Add("Compressor(Motor)");
            return list;
        }
        public ObservableCollection<string> GetMediums()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Cooling Water");
            list.Add("Air");
            list.Add("Steam");
            list.Add("Hot Oil/Hot Water");
            list.Add("Process Stream");
            list.Add("Fired Heater");
            return list;
        }
        public ObservableCollection<string> GetMediumSideFlowSources(string medium)
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
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
        

    }
}
