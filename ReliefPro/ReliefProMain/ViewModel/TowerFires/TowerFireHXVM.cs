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
        public UOMLib.UOMEnum uomEnum { get; set; }
        public TowerFireHXVM(int EqID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionDBPath == sessionPlant.Connection.ConnectionString);
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
                model.PipingContingency = 10;
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

            double length = m.Length;
            double pipingContingency = m.PipingContingency;
            double od = m.OD;
            Area = Algorithm.GetHXArea(m.ExposedToFire, m.Type, length, od);
            Area = Area + Area * model.PipingContingency / 100;


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
            //if (model.OD!=null)
            model.OD = UnitConvert.Convert(UOMEnum.Length, oDUnit, model.OD);
            //if (model.Length!=null)
            model.Length = UnitConvert.Convert(UOMEnum.Length, lengthUnit, model.Length);
            //if (model.Elevation!=null)
            model.Elevation = UnitConvert.Convert(UOMEnum.Length, elevationUnit, model.Elevation);
        }
        private void WriteConvert()
        {
            //if (model.OD!=null)
            model.OD = UnitConvert.Convert(oDUnit, UOMEnum.Length, model.OD);
            //if (model.Length!=null)
            model.Length = UnitConvert.Convert(lengthUnit, UOMEnum.Length, model.Length);
            //if (model.Elevation!=null)
            model.Elevation = UnitConvert.Convert(elevationUnit, UOMEnum.Length, model.Elevation);
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
