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
using ReliefProMain;
using UOMLib;
using NHibernate;

namespace ReliefProMain.ViewModel.TowerFires
{
    public class TowerFireHXVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public double Area { get; set; }
        public TowerFireHX model { get; set; }
        public List<string> ExposedToFires { get; set; }
        public List<string> Types { get; set; }
        UnitConvert unitConvert;
        UOMLib.UOMEnum uomEnum;
        public TowerFireHXVM(int EqID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            unitConvert = new UnitConvert();
            uomEnum = new UOMLib.UOMEnum(sessionPlant);
            InitUnit();
            ExposedToFires = GetExposedToFires();
            Types = GetTypes();
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;

            TowerFireHXDAL db = new TowerFireHXDAL();
            model = db.GetModel(SessionProtectedSystem, EqID);
            if (model == null)
            {
                model = new TowerFireHX();
                model.EqID = EqID;
                model.PipingContingency = "10";
                db.Add(model, SessionProtectedSystem);
            }
            else
            {
                ReadConvert();
            }
        }

        private ICommand _OKClick;
        public ICommand OKClick
        {
            get
            {
                if (_OKClick == null)
                {
                    _OKClick = new RelayCommand(Update);

                }
                return _OKClick;
            }
        }

        private void Update(object window)
        {

            TowerFireHXDAL db = new TowerFireHXDAL();
            TowerFireHX m = db.GetModel(model.ID, SessionProtectedSystem);
            WriteConvert();
            m.ExposedToFire = model.ExposedToFire;
            m.Length = model.Length;
            m.OD = model.OD;
            m.Type = model.Type;
            m.Elevation = model.Elevation;
            m.PipingContingency = model.PipingContingency;
            db.Update(m, SessionProtectedSystem);
            SessionProtectedSystem.Flush();

            double length = double.Parse(m.Length);
            double pipingContingency = double.Parse(m.PipingContingency);
            double od = double.Parse(m.OD);
            double D = double.Parse(m.Elevation);
            Area = Algorithm.GetHXArea(m.ExposedToFire, m.Type, length, od, D);
            Area = Area + Area * double.Parse(model.PipingContingency) / 100;


            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }


        public List<string> GetExposedToFires()
        {
            List<string> list = new List<string>();
            list.Add("Shell");
            list.Add("Tube");
            return list;
        }
        public List<string> GetTypes()
        {
            List<string> list = new List<string>();
            list.Add("Fixed");
            list.Add("U-Tube");
            list.Add("Floating head");
            return list;
        }
        private void ReadConvert()
        {
            if (!string.IsNullOrEmpty(model.OD))
                model.OD = unitConvert.Convert(UOMEnum.Length, oDUnit, double.Parse(model.OD)).ToString();
            if (!string.IsNullOrEmpty(model.OD))
                model.Length = unitConvert.Convert(UOMEnum.Length, lengthUnit, double.Parse(model.Length)).ToString();
            if (!string.IsNullOrEmpty(model.OD))
                model.Elevation = unitConvert.Convert(UOMEnum.Length, elevationUnit, double.Parse(model.Elevation)).ToString();
        }
        private void WriteConvert()
        {
            if (!string.IsNullOrEmpty(model.OD))
                model.OD = unitConvert.Convert(oDUnit, UOMEnum.Length, double.Parse(model.OD)).ToString();
            if (!string.IsNullOrEmpty(model.OD))
                model.Length = unitConvert.Convert(lengthUnit, UOMEnum.Length, double.Parse(model.Length)).ToString();
            if (!string.IsNullOrEmpty(model.OD))
                model.Elevation = unitConvert.Convert(elevationUnit, UOMEnum.Length, double.Parse(model.Elevation)).ToString();
        }
        private void InitUnit()
        {
            this.oDUnit = uomEnum.UserLength;
            this.lengthUnit = uomEnum.UserLength;
            this.elevationUnit = uomEnum.UserLength;
        }
        #region 单位-字段
        private string oDUnit;
        public string ODUnit
        {
            get { return oDUnit; }
            set
            {
                oDUnit = value;
                this.OnPropertyChanged("ODUnit");
            }
        }

        private string lengthUnit;
        public string LengthUnit
        {
            get { return lengthUnit; }
            set
            {
                lengthUnit = value;
                this.OnPropertyChanged("LengthUnit");
            }
        }

        private string elevationUnit;
        public string ElevationUnit
        {
            get { return elevationUnit; }
            set
            {
                elevationUnit = value;
                this.OnPropertyChanged("ElevationUnit");
            }
        }
        #endregion
    }
}
