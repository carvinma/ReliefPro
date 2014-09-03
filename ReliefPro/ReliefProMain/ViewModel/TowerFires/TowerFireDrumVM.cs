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
using ReliefProMain.Models;
using ReliefProCommon.Enum;

namespace ReliefProMain.ViewModel.TowerFires
{
    public class TowerFireDrumVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public double Area { get; set; }
        public TowerFireDrumModel model { get; set; }
        public List<string> Orientations { get; set; }
        public List<string> HeadTypes { get; set; }
        UOMLib.UOMEnum uomEnum;
        public TowerFireDrumVM(int EqID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            InitUnit();

            Orientations = getOrientations();
            HeadTypes = getHeadTypes();

            TowerFireDrumDAL db = new TowerFireDrumDAL();
            TowerFireDrum sizemodel = db.GetModel(SessionProtectedSystem, EqID);
            if (sizemodel == null)
            {
                sizemodel = new TowerFireDrum();               
                sizemodel.EqID = EqID;
                sizemodel.PipingContingency = 10;
                sizemodel.Orientation = "Horizon";
                sizemodel.HeadType = "Eclipse";
                sizemodel.BootDiameter_Color = ColorBorder.green.ToString();
                sizemodel.BootHeight_Color = ColorBorder.green.ToString();
                sizemodel.Diameter_Color = ColorBorder.green.ToString();
                sizemodel.Elevation_Color = ColorBorder.green.ToString();
                sizemodel.HeadNumber_Color = ColorBorder.green.ToString();
                sizemodel.Length_Color = ColorBorder.green.ToString();
                sizemodel.NormalLiquidLevel_Color = ColorBorder.green.ToString();
                sizemodel.PipingContingency_Color = ColorBorder.green.ToString(); 
               
                db.Add(sizemodel, SessionProtectedSystem);                
            }
            model = new TowerFireDrumModel(sizemodel);
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
            TowerFireDrumDAL db = new TowerFireDrumDAL();
            WriteConvert();           
            try
            {
                model.dbmodel.PipingContingency = model.PipingContingency;
                model.dbmodel.Orientation=model.Orientation;
                model.dbmodel.HeadNumber=model.Headnumber;;
                model.dbmodel.HeadType = model.HeadType;
               
                model.dbmodel.BootDiameter_Color=model.BootDiameter_Color;
                model.dbmodel.BootHeight_Color=model.BootHeight_Color;
                model.dbmodel.PipingContingency_Color = model.PipingContingency_Color;
                model.dbmodel.Elevation_Color = model.Elevation_Color;
                model.dbmodel.HeadNumber_Color = model.Headnumber_Color;
                model.dbmodel.HeadType_Color = model.Headnumber_Color;
                model.dbmodel.Diameter_Color = model.Diameter_Color;
                model.dbmodel.Length_Color = model.Length_Color;
                model.dbmodel.NormalLiquidLevel_Color = model.NormalLiquidLevel_Color;
                model.dbmodel.Orientation_Color = model.Orientation_Color;
                db.Update(model.dbmodel, SessionProtectedSystem);

                SessionProtectedSystem.Flush();
            }
            catch (Exception ex)
            {
            }


            double elevation = model.dbmodel.Elevation;
            double diameter = model.dbmodel.Diameter;
            double length = model.dbmodel.Length;
            double NLL = model.dbmodel.NormalLiquidLevel;
            double bootheight = model.dbmodel.BootHeight;
            double bootdiameter = model.dbmodel.BootDiameter;

            Area = Algorithm.GetDrumArea(model.Orientation, model.HeadType, elevation, diameter, length, NLL, bootheight, bootdiameter);
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
            list.Add("Horizon");
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
                model.Elevation = UnitConvert.Convert(UOMEnum.Length, elevationUnit, model.dbmodel.Elevation);
            if (model.Diameter != null)
                model.Diameter = UnitConvert.Convert(UOMEnum.Length, diameterUnit, model.dbmodel.Diameter);
            if (model.Length != null)
                model.Length = UnitConvert.Convert(UOMEnum.Length, lengthUnit, model.dbmodel.Length);
            if (model.NormalLiquidLevel != null)
                model.NormalLiquidLevel = UnitConvert.Convert(UOMEnum.Length, normalLiquidLevelUnit, model.dbmodel.NormalLiquidLevel);
            if (model.BootDiameter != null)
                model.BootDiameter = UnitConvert.Convert(UOMEnum.Length, bootDiameterUnit, model.dbmodel.BootDiameter);
            if (model.BootHeight != null)
                model.BootHeight = UnitConvert.Convert(UOMEnum.Length, bootHeightUnit, model.dbmodel.BootHeight);
        }
        private void WriteConvert()
        {
            if (model.Elevation != null)
                model.dbmodel.Elevation = UnitConvert.Convert(elevationUnit, UOMEnum.Length, model.Elevation);
            if (model.Diameter != null)
                model.dbmodel.Diameter = UnitConvert.Convert(diameterUnit, UOMEnum.Length, model.Diameter);
            if (model.Length != null)
                model.dbmodel.Length = UnitConvert.Convert(lengthUnit, UOMEnum.Length, model.Length);
            if (model.NormalLiquidLevel != null)
                model.dbmodel.NormalLiquidLevel = UnitConvert.Convert(normalLiquidLevelUnit, UOMEnum.Length, model.NormalLiquidLevel);
            if (model.BootDiameter != null)
                model.dbmodel.BootDiameter = UnitConvert.Convert(bootDiameterUnit, UOMEnum.Length, model.BootDiameter);
            if (model.BootHeight != null)
                model.dbmodel.BootHeight = UnitConvert.Convert(bootHeightUnit, UOMEnum.Length, model.BootHeight);
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
