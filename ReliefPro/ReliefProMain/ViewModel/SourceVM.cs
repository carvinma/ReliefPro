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
using System.Windows;

namespace ReliefProMain.ViewModel
{
    public class SourceVM : ViewModelBase
    {
        public ISession SessionPlant { set; get; }
        public ISession SessionProtectedSystem { set; get; }
        public SourceFile SourceFileInfo;
        private string pressureUnit;
        public string PressureUnit
        {
            get { return pressureUnit; }
            set
            {
                pressureUnit = value;
                this.OnPropertyChanged("PressureUnit");
            }
        }
        public int ID { get; set; }
        public string SourceName { get; set; }
        public string Description { get; set; }

        private string _SourceType;
        public string SourceType
        {
            get
            {
                return this._SourceType;
            }
            set
            {
                this._SourceType = value;
                OnPropertyChanged("SourceType");
            }
        }
        private double? maxPossiblePressure;
        public double? MaxPossiblePressure
        {
            get { return maxPossiblePressure; }
            set
            {
                maxPossiblePressure = value;
                this.OnPropertyChanged("MaxPossiblePressure");
            }
        }
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
                if (this._IsSteam)
                {
                    SourceType = "Pressurized Vessel";
                }
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
        UOMLib.UOMEnum uomEnum;
        public SourceVM(string name, SourceFile sourceFileInfo, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SourceTypes = GetSourceTypes();
            SourceName = name;
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            SourceFileInfo = sourceFileInfo;
            BasicUnit BU;
            BasicUnitDAL dbBU = new BasicUnitDAL();
            IList<BasicUnit> list = dbBU.GetAllList(sessionPlant);
            BU = list.Where(s => s.IsDefault == 1).Single();

            uomEnum = new UOMLib.UOMEnum(SessionPlant);
            InitUnit();
            SourceDAL db = new SourceDAL();
            Source source = db.GetModel(SessionProtectedSystem, SourceName);
            SourceType = source.SourceType;
            ReadConvert(source);
            Description = source.Description;
            IsMaintained = source.IsMaintained;
            PressureUnit = pressureUnit;
            IsSteam = source.IsSteam;
            IsHeatSource = source.IsHeatSource;
            ID = source.ID;

            //ReadConvert();
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

            BasicUnitDAL dbBU = new BasicUnitDAL();
            IList<BasicUnit> list = dbBU.GetAllList(SessionPlant);
            BU = list.Where(s => s.IsDefault == 1).Single();


            CurrentSource = new Source();

            SourceDAL db = new SourceDAL();
            CurrentSource = db.GetModel(SessionProtectedSystem, SourceName);
            CurrentSource.SourceName = SourceName;
            CurrentSource.SourceType = SourceType;
            // CurrentSource.MaxPossiblePressure = uc.BasicConvert("P", BU.UnitName, "StInternal", out pressureUnit, double.Parse(MaxPossiblePressure)).ToString();
            CurrentSource.Description = Description;
            CurrentSource.IsMaintained = IsMaintained;
            CurrentSource.IsSteam = IsSteam;
            CurrentSource.IsHeatSource = IsHeatSource;
            WriteConvert();
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
            HeatSourceListVM vm = new HeatSourceListVM(ID, SourceFileInfo, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.ShowDialog();
        }
        private void ReadConvert(Source source)
        {
            if (source != null)
                MaxPossiblePressure = UnitConvert.Convert(UOMEnum.Pressure, pressureUnit, source.MaxPossiblePressure);
        }
        private void WriteConvert()
        {
            if (MaxPossiblePressure != null)
                CurrentSource.MaxPossiblePressure = UnitConvert.Convert(pressureUnit, UOMEnum.Pressure, MaxPossiblePressure.Value);
        }
        private void InitUnit()
        {
            this.pressureUnit = uomEnum.UserPressure;
        }
    }
}
