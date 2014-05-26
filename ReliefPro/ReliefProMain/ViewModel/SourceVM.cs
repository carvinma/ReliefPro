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
using UOMLib;
using NHibernate;
using ReliefProMain.View;
namespace ReliefProMain.ViewModel
{
    public class SourceVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string PrzFile;
        public string PressureUnit { get; set; }
        private string pressureUnit;
        public int ID { get; set; }
        public string SourceName { get; set; }
        public string Description { get; set; }
        public string SourceType { get; set; }
        public string MaxPossiblePressure { get; set; }
        public bool IsMaintained { get; set; }
        public string SourceName_Color { get; set; }
        public string Description_Color { get; set; }
        public string SourceType_Color { get; set; }
        public string MaxPossiblePressure_Color { get; set; }
        public string IsMaintained_Color { get; set; }
        public string StreamName { get; set; }

        public bool _IsSteam;
        public bool IsSteam
        {
            get
            {
                return this._IsSteam;
            }
            set
            {
                this._IsSteam = value;
                OnPropertyChanged("IsSteam");
            }
        }
        public bool _IsHeatSource;
        public bool IsHeatSource
        {
            get
            {
                return this._IsHeatSource;
            }
            set
            {
                this._IsHeatSource = value;
                OnPropertyChanged("IsHeatSource");
            }
        }
        public List<string> SourceTypes { get; set; }
        public Source CurrentSource { get; set; }
        public List<string> GetSourceTypes()
        {
            List<string> list = new List<string>();
            list.Add("Compressor(Motor)");
            list.Add("Compressor(Steam Turbine Driven)");
            list.Add("Pump(Steam Turbine Driven)");
            list.Add("Pump(Motor)");
            list.Add("Pressurized Vessel");
            return list;
        }




        public SourceVM(string name,string PrzFile, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            this.PrzFile = PrzFile;
            SourceTypes = GetSourceTypes();
            SourceName = name;
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            BasicUnit BU;
            dbBasicUnit dbBU = new dbBasicUnit();
            IList<BasicUnit> list = dbBU.GetAllList(sessionPlant);
            BU = list.Where(s => s.IsDefault == 1).Single();

            UnitConvert uc = new UnitConvert();

            dbSource db = new dbSource();
            Source source = db.GetModel(SessionProtectedSystem, SourceName);
            SourceType = source.SourceType;
            MaxPossiblePressure = uc.BasicConvert("P", "StInternal", BU.UnitName, out pressureUnit, double.Parse(source.MaxPossiblePressure)).ToString();
            Description = source.Description;
            IsMaintained = source.IsMaintained;
            PressureUnit = pressureUnit;
            IsSteam = source.IsSteam;
            IsHeatSource = source.IsHeatSource;
            ID = source.ID;

        }

        private ICommand _Update;
        
        public ICommand Update
        {
            get { return _Update ?? (_Update = new RelayCommand(OKClick)); }
        }


        private void OKClick(object window)
        {
            SourceName.Trim();
            if (SourceName == "")
            {
                throw new ArgumentException("Please type in a name for the Source.");
            }
            BasicUnit BU;

            dbBasicUnit dbBU = new dbBasicUnit();
            IList<BasicUnit> list = dbBU.GetAllList(SessionPlant);
            BU = list.Where(s => s.IsDefault == 1).Single();


            UnitConvert uc = new UnitConvert();
            CurrentSource = new Source();

            dbSource db = new dbSource();
            CurrentSource = db.GetModel(SessionProtectedSystem, SourceName);
            CurrentSource.SourceName = SourceName;
            CurrentSource.SourceType = SourceType;
            CurrentSource.MaxPossiblePressure = uc.BasicConvert("P", BU.UnitName, "StInternal", out pressureUnit, double.Parse(MaxPossiblePressure)).ToString();
            CurrentSource.Description = Description;
            CurrentSource.IsMaintained = IsMaintained;
            CurrentSource.IsSteam = IsSteam;
            CurrentSource.IsHeatSource = IsHeatSource;
            db.Update(CurrentSource, SessionProtectedSystem);
            SessionProtectedSystem.Flush();  //update必须带着它。 之所以没写入基类，是为了日后transaction

            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

        private ICommand _ShowHeatSourceListCommand;
        public ICommand ShowHeatSourceListCommand
        {
            get
            {
                if (_ShowHeatSourceListCommand == null)
                {
                    _ShowHeatSourceListCommand = new DelegateCommand(ShowHeatSourceList);

                }
                return _ShowHeatSourceListCommand;
            }
        }

        public void ShowHeatSourceList()
        {
            HeatSourceListView v = new HeatSourceListView();
            HeatSourceListVM vm = new HeatSourceListVM(ID,PrzFile, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            v.ShowDialog();
        }

    }
}
