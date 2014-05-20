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

namespace ReliefProMain.ViewModel
{
    public class SourceVM : ViewModelBase
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
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




        public SourceVM(string name, string dbPSFile, string dbPFile)
        {

            SourceTypes = GetSourceTypes();
            SourceName = name;
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            BasicUnit BU;
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbBasicUnit dbBU = new dbBasicUnit();
                IList<BasicUnit> list = dbBU.GetAllList(Session);
                BU = list.Where(s => s.IsDefault == 1).Single();
            }



            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                UnitConvert uc = new UnitConvert();
                var Session = helper.GetCurrentSession();


                dbSource db = new dbSource();
                Source source = db.GetModel(Session, SourceName);
                SourceType = source.SourceType;
                MaxPossiblePressure = uc.BasicConvert("P", "StInternal", BU.UnitName, out pressureUnit, double.Parse(source.MaxPossiblePressure)).ToString();
                Description = source.Description;
                IsMaintained = source.IsMaintained;
                PressureUnit = pressureUnit;
            }

        }

        private ICommand _Update;
        //public ICommand Update
        //{
        //    get
        //    {
        //        if (_Update == null)
        //        {
        //            _Update = new DelegateCommand(delegate()
        //            {


        //            });
        //        }
        //        //CloseAction();
        //        return _Update;
        //    }
        //}

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
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbBasicUnit dbBU = new dbBasicUnit();
                IList<BasicUnit> list = dbBU.GetAllList(Session);
                BU = list.Where(s => s.IsDefault == 1).Single();
            }
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                UnitConvert uc = new UnitConvert();
                CurrentSource = new Source();
                var Session = helper.GetCurrentSession();
                dbSource db = new dbSource();
                CurrentSource = db.GetModel(Session, SourceName);
                CurrentSource.SourceName = SourceName;
                CurrentSource.SourceType = SourceType;
                CurrentSource.MaxPossiblePressure = uc.BasicConvert("P", BU.UnitName, "StInternal", out pressureUnit, double.Parse(MaxPossiblePressure)).ToString();
                CurrentSource.Description = Description;
                CurrentSource.IsMaintained = IsMaintained;
                db.Update(CurrentSource, Session);
                Session.Flush();  //update必须带着它。 之所以没写入基类，是为了日后transaction
            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

        //public Action CloseAction { get; set; }
    }
}
