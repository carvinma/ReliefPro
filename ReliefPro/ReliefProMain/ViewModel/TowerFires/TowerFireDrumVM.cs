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
        UnitConvert unitConvert;
        UOMLib.UOMEnum uomEnum;
        public TowerFireDrumVM(int EqID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            unitConvert = new UnitConvert();
            uomEnum = new UOMLib.UOMEnum(sessionPlant);
            InitUnit();
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
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


            double elevation = double.Parse(m.Elevation);
            double diameter = double.Parse(m.Diameter);
            double length = double.Parse(m.Length);
            double NLL = double.Parse(m.NormalLiquidLevel);
            double bootheight = double.Parse(m.BootHeight);
            double bootdiameter = double.Parse(m.BootDiameter);

            Area = Algorithm.GetDrumArea(m.Orientation, model.HeadType, elevation, diameter, length, NLL, bootheight, bootdiameter);
            Area = Area + Area * double.Parse(model.PipingContingency) / 100;

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
            if (!string.IsNullOrEmpty(model.Elevation))
                model.Elevation = unitConvert.Convert(UOMEnum.Length, elevationUnit, double.Parse(model.Elevation)).ToString();
            if (!string.IsNullOrEmpty(model.Diameter))
                model.Diameter = unitConvert.Convert(UOMEnum.Length, diameterUnit, double.Parse(model.Diameter)).ToString();
            if (!string.IsNullOrEmpty(model.Length))
                model.Length = unitConvert.Convert(UOMEnum.Length, lengthUnit, double.Parse(model.Length)).ToString();
            if (!string.IsNullOrEmpty(model.NormalLiquidLevel))
                model.NormalLiquidLevel = unitConvert.Convert(UOMEnum.Length, normalLiquidLevelUnit, double.Parse(model.NormalLiquidLevel)).ToString();
            if (!string.IsNullOrEmpty(model.BootDiameter))
                model.BootDiameter = unitConvert.Convert(UOMEnum.Length, bootDiameterUnit, double.Parse(model.BootDiameter)).ToString();
            if (!string.IsNullOrEmpty(model.BootHeight))
                model.BootHeight = unitConvert.Convert(UOMEnum.Length, bootHeightUnit, double.Parse(model.BootHeight)).ToString();
        }
        private void WriteConvert()
        {
            if (!string.IsNullOrEmpty(model.Elevation))
                model.Elevation = unitConvert.Convert(elevationUnit, UOMEnum.Length, double.Parse(model.Elevation)).ToString();
            if (!string.IsNullOrEmpty(model.Diameter))
                model.Diameter = unitConvert.Convert(diameterUnit, UOMEnum.Length, double.Parse(model.Diameter)).ToString();
            if (!string.IsNullOrEmpty(model.Length))
                model.Length = unitConvert.Convert(lengthUnit, UOMEnum.Length, double.Parse(model.Length)).ToString();
            if (!string.IsNullOrEmpty(model.NormalLiquidLevel))
                model.NormalLiquidLevel = unitConvert.Convert(normalLiquidLevelUnit, UOMEnum.Length, double.Parse(model.NormalLiquidLevel)).ToString();
            if (!string.IsNullOrEmpty(model.BootDiameter))
                model.BootDiameter = unitConvert.Convert(bootDiameterUnit, UOMEnum.Length, double.Parse(model.BootDiameter)).ToString();
            if (!string.IsNullOrEmpty(model.BootHeight))
                model.BootHeight = unitConvert.Convert(bootHeightUnit, UOMEnum.Length, double.Parse(model.BootHeight)).ToString();
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
