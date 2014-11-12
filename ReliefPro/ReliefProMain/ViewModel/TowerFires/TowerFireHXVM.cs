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
using ReliefProMain.Models;
using ReliefProCommon.Enum;

namespace ReliefProMain.ViewModel.TowerFires
{
    public class TowerFireHXVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public double Area { get; set; }
        public TowerFireHXModel model { get; set; }
        public List<string> ExposedToFires { get; set; }
        public List<string> Types { get; set; }
        public UOMLib.UOMEnum uomEnum { get; set; }
        public TowerFireHXVM(int EqID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            InitUnit();
            ExposedToFires = GetExposedToFires();
            Types = GetTypes();

            TowerFireHXDAL db = new TowerFireHXDAL();
            TowerFireHX sizemodel = db.GetModel(SessionProtectedSystem, EqID);
            if (sizemodel == null)
            {
                sizemodel = new TowerFireHX();
                sizemodel.EqID = EqID;
                sizemodel.Length_Color = ColorBorder.green.ToString();
                sizemodel.Elevation_Color = ColorBorder.green.ToString();
                sizemodel.ExposedToFire = "Shell";
                sizemodel.Type = "Fixed";
                sizemodel.OD_Color = ColorBorder.green.ToString();
                sizemodel.PipingContingency = 10;
                db.Add(sizemodel, SessionProtectedSystem);
            }
            model = new TowerFireHXModel(sizemodel);
            ReadConvert();
            
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
            WriteConvert();
            
            db.Update(model.dbmodel, SessionProtectedSystem);
            SessionProtectedSystem.Flush();

            double length = model.dbmodel.Length;
            double pipingContingency = model.dbmodel.PipingContingency;
            double od = model.dbmodel.OD;
            Area = Algorithm.GetHXArea(model.ExposedToFire, model.Type, length,model.Elevation, od);
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
            model.OD = UnitConvert.Convert(UOMEnum.Length, oDUnit, model.dbmodel.OD);
            //if (model.Length!=null)
            model.Length = UnitConvert.Convert(UOMEnum.Length, lengthUnit, model.dbmodel.Length);
            //if (model.Elevation!=null)
            model.Elevation = UnitConvert.Convert(UOMEnum.Length, elevationUnit, model.dbmodel.Elevation);
        }
        private void WriteConvert()
        {
            //if (model.OD!=null)
            model.dbmodel.OD = UnitConvert.Convert(oDUnit, UOMEnum.Length, model.OD);
            //if (model.Length!=null)
            model.dbmodel.Length = UnitConvert.Convert(lengthUnit, UOMEnum.Length, model.Length);
            //if (model.Elevation!=null)
            model.dbmodel.Elevation = UnitConvert.Convert(elevationUnit, UOMEnum.Length, model.Elevation);
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
