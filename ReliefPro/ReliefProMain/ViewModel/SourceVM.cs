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
        UnitConvert unitConvert;
        UOMLib.UOMEnum uomEnum;

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
        #region 单位-字段
        private string _PressureUnit;
        public string PressureUnit
        {
            get
            {
                return this._PressureUnit;
            }
            set
            {
                this._PressureUnit = value;
                OnPropertyChanged("PressureUnit");
            }
        }
        private string _DrumPressureUnit;
        public string DrumPressureUnit
        {
            get
            {
                return this._DrumPressureUnit;
            }
            set
            {
                this._DrumPressureUnit = value;
                OnPropertyChanged("DrumPressureUnit");
            }
        }
        #endregion

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

        public SourceVM(string name, string PrzFile, ISession SessionPlant, ISession SessionProtectedSystem)
        {
            this.PrzFile = PrzFile;
            SourceTypes = GetSourceTypes();
            SourceName = name;
            this.SessionPlant = SessionPlant;
            this.SessionProtectedSystem = SessionProtectedSystem;
            unitConvert = new UnitConvert();
            uomEnum = new UOMLib.UOMEnum(SessionPlant);
            SourceDAL db = new SourceDAL();
            Source source = db.GetModel(SessionProtectedSystem, SourceName);
            SourceType = source.SourceType;
            MaxPossiblePressure = source.MaxPossiblePressure;
            Description = source.Description;
            IsMaintained = source.IsMaintained;
            PressureUnit = pressureUnit;
            IsSteam = source.IsSteam;
            IsHeatSource = source.IsHeatSource;
            ID = source.ID;
            ReadConvert();
        }
        private void ReadConvert()
        {
            //if (!string.IsNullOrEmpty(_PressureUnit))
            //    _Pressure = unitConvert.Convert(UOMEnum.Length, _DiameterUnit, double.Parse(_Diameter)).ToString();
            //if (!string.IsNullOrEmpty(_Length))
            //    _Length = unitConvert.Convert(UOMEnum.Length, _LengthUnit, double.Parse(_Length)).ToString();
           
        }
        private void WriteConvert()
        {
            if (!string.IsNullOrEmpty(_PressureUnit))
                _PressureUnit = unitConvert.Convert(_PressureUnit, uomEnum.UserPressure, double.Parse(_PressureUnit)).ToString();
           
           
        }
        private void InitUnit()
        {
            this._PressureUnit = uomEnum.UserPressure;
            this._DrumPressureUnit = uomEnum.UserPressure;
           
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
            
            UnitConvert uc = new UnitConvert();
            CurrentSource = new Source();

            SourceDAL db = new SourceDAL();
            CurrentSource = db.GetModel(SessionProtectedSystem, SourceName);
            CurrentSource.SourceName = SourceName;
            CurrentSource.SourceType = SourceType;
            CurrentSource.MaxPossiblePressure = MaxPossiblePressure;
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
