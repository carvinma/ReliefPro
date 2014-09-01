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

namespace ReliefProMain.ViewModel.TowerFires
{
    public class TowerFireDrumVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public double Area { get; set; }
        public TowerFireDrum model { get; set; }
        public List<string> Orientations { get; set; }
        public List<string> HeadTypes { get; set; }
        UOMLib.UOMEnum uomEnum;
        public TowerFireDrumVM(int EqID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionDBPath == this.SessionPlant.Connection.ConnectionString);
            InitUnit();

            Orientations = getOrientations();
            HeadTypes = getHeadTypes();

            TowerFireDrumDAL db = new TowerFireDrumDAL();
            model = db.GetModel(SessionProtectedSystem, EqID);
            if (model == null)
            {
                model = new TowerFireDrum();
                model.EqID = EqID;
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

            TowerFireDrumDAL db = new TowerFireDrumDAL();
            TowerFireDrum m = db.GetModel(model.ID, SessionProtectedSystem);
            WriteConvert();
            m.Elevation = model.Elevation;
            m.BootDiameter = model.BootDiameter;
            m.BootHeight = model.BootHeight;
            m.Diameter = model.Diameter;
            m.HeadNumber = model.HeadNumber;
            m.HeadType = model.HeadType;
            m.Length = model.Length;
            m.NormalLiquidLevel = model.NormalLiquidLevel;
            m.Orientation = model.Orientation;
            m.PipingContingency = model.PipingContingency;
            try
            {
                db.Update(m, SessionProtectedSystem);

                SessionProtectedSystem.Flush();
            }
            catch (Exception ex)
            {
            }


            double elevation = m.Elevation;
            double diameter = m.Diameter;
            double length = m.Length;
            double NLL = m.NormalLiquidLevel;
            double bootheight = m.BootHeight;
            double bootdiameter = m.BootDiameter;

            Area = Algorithm.GetDrumArea(m.Orientation, model.HeadType, elevation, diameter, length, NLL, bootheight, bootdiameter);
            Area = Area + Area * model.PipingContingency / 100;

            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private List<string> getOrientations()
        {
            List<string> list = new List<string>();
            list.Add("Horiz");
            list.Add("Vertical");
            return list;
        }
        private List<string> getHeadTypes()
        {
            List<string> list = new List<string>();
            list.Add("Eclipse");
            list.Add("Flat");
            return list;
        }

        private void ReadConvert()
        {
            if (model.Elevation != null)
                model.Elevation = UnitConvert.Convert(UOMEnum.Length, elevationUnit, model.Elevation);
            if (model.Diameter != null)
                model.Diameter = UnitConvert.Convert(UOMEnum.Length, diameterUnit, model.Diameter);
            if (model.Length != null)
                model.Length = UnitConvert.Convert(UOMEnum.Length, lengthUnit, model.Length);
            if (model.NormalLiquidLevel != null)
                model.NormalLiquidLevel = UnitConvert.Convert(UOMEnum.Length, normalLiquidLevelUnit, model.NormalLiquidLevel);
            if (model.BootDiameter != null)
                model.BootDiameter = UnitConvert.Convert(UOMEnum.Length, bootDiameterUnit, model.BootDiameter);
            if (model.BootHeight != null)
                model.BootHeight = UnitConvert.Convert(UOMEnum.Length, bootHeightUnit, model.BootHeight);
        }
        private void WriteConvert()
        {
            if (model.Elevation != null)
                model.Elevation = UnitConvert.Convert(elevationUnit, UOMEnum.Length, model.Elevation);
            if (model.Diameter != null)
                model.Diameter = UnitConvert.Convert(diameterUnit, UOMEnum.Length, model.Diameter);
            if (model.Length != null)
                model.Length = UnitConvert.Convert(lengthUnit, UOMEnum.Length, model.Length);
            if (model.NormalLiquidLevel != null)
                model.NormalLiquidLevel = UnitConvert.Convert(normalLiquidLevelUnit, UOMEnum.Length, model.NormalLiquidLevel);
            if (model.BootDiameter != null)
                model.BootDiameter = UnitConvert.Convert(bootDiameterUnit, UOMEnum.Length, model.BootDiameter);
            if (model.BootHeight != null)
                model.BootHeight = UnitConvert.Convert(bootHeightUnit, UOMEnum.Length, model.BootHeight);
        }
        private void InitUnit()
        {
            this.elevationUnit = uomEnum.UserLength;
            this.diameterUnit = uomEnum.UserLength;
            this.lengthUnit = uomEnum.UserLength;
            this.normalLiquidLevelUnit = uomEnum.UserLength;
            this.bootDiameterUnit = uomEnum.UserLength;
            this.bootHeightUnit = uomEnum.UserLength;
        }
        #region 单位
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
        private string diameterUnit;
        public string DiameterUnit
        {
            get { return diameterUnit; }
            set
            {
                diameterUnit = value;
                this.OnPropertyChanged("DiameterUnit");
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
        private string normalLiquidLevelUnit;
        public string NormalLiquidLevelUnit
        {
            get { return normalLiquidLevelUnit; }
            set
            {
                normalLiquidLevelUnit = value;
                this.OnPropertyChanged("NormalLiquidLevelUnit");
            }
        }
        private string bootDiameterUnit;
        public string BootDiameterUnit
        {
            get { return bootDiameterUnit; }
            set
            {
                bootDiameterUnit = value;
                this.OnPropertyChanged("BootDiameterUnit");
            }
        }
        private string bootHeightUnit;
        public string BootHeightUnit
        {
            get { return bootHeightUnit; }
            set
            {
                bootHeightUnit = value;
                this.OnPropertyChanged("BootHeightUnit");
            }
        }
        #endregion
    }
}
