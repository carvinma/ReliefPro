using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Commands;
using ReliefProMain.Model;
using UOMLib;

namespace ReliefProMain.ViewModel.Drums
{
    public class DrumSizeVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public DrumSizeModel model { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public List<string> lstOrientation { get; set; }
        public List<string> lstHeadType { get; set; }

        private string selectedOrientation = "Vertical";
        public string SelectedOrientation
        {
            get { return selectedOrientation; }
            set
            {
                selectedOrientation = value;
                OnPropertyChanged("SelectedOrientation");
            }
        }
        private string selectedHeadType = "Eclipse";
        public string SelectedHeadType
        {
            get { return selectedHeadType; }
            set
            {
                selectedHeadType = value;
                OnPropertyChanged("SelectedHeadType");
            }
        }
        public DrumSizeVM(int DrumFireCalcID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            lstOrientation = new List<string> { "Vertical", "Horizontal", "Spherical", "Non-standard" };
            lstHeadType = new List<string> { "Eclipse", "Sphere" };
            DrumSizeBLL fluidBll = new DrumSizeBLL(SessionPS, SessionPF);

            var sizeModel = fluidBll.GetSizeModel(DrumFireCalcID);
            sizeModel = fluidBll.ReadConvertModel(sizeModel);
            model = new DrumSizeModel(sizeModel);
            //if(string.IsNullOrEmpty(model.Orientation))

            UOMLib.UOMEnum uomEnum = new UOMLib.UOMEnum(SessionPF);
            model.ElevationUnit = uomEnum.UserLength;
            model.DiameterUnit = uomEnum.UserLength;
            model.LengthUnit = uomEnum.UserLength;
            model.NormalLiquidLevelUnit = uomEnum.UserLength;
            model.BootDiameterUnit = uomEnum.UserLength;
            model.BootHeightUnit = uomEnum.UserLength;
            OKCMD = new DelegateCommand<object>(Save);
        }
        private void WriteConvertModel()
        {
            model.dbmodel.Orientation = selectedOrientation;
            model.dbmodel.HeadNumber = model.Headnumber;
            model.dbmodel.HeadType = selectedHeadType;

            model.dbmodel.Elevation = UnitConvert.Convert(model.ElevationUnit, UOMLib.UOMEnum.Length.ToString(), model.Elevation);
            model.dbmodel.Diameter = UnitConvert.Convert(model.DiameterUnit, UOMLib.UOMEnum.Length.ToString(), model.Diameter);
            model.dbmodel.Length = UnitConvert.Convert(model.LengthUnit, UOMLib.UOMEnum.Length.ToString(), model.Length);
            model.dbmodel.NormalLiquidLevel = UnitConvert.Convert(model.NormalLiquidLevelUnit, UOMLib.UOMEnum.Length.ToString(), model.NormalLiquidLevel);
            model.dbmodel.BootDiameter = UnitConvert.Convert(model.BootDiameterUnit, UOMLib.UOMEnum.Length.ToString(), model.BootDiameter);
            model.dbmodel.BootHeight = UnitConvert.Convert(model.BootHeightUnit, UOMLib.UOMEnum.Length.ToString(), model.BootHeight);
        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    wd.DialogResult = true;
                }
            }
        }
    }
}
