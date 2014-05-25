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

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumSizeVM
    {
        public ICommand OKCMD { get; set; }
        public DrumSizeModel model { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public DrumSizeVM(int DrumFireCalcID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;

            DrumSizeBLL fluidBll = new DrumSizeBLL(SessionPS, SessionPF);

            var sizeModel = fluidBll.GetSizeModel(DrumFireCalcID);
            sizeModel = fluidBll.ReadConvertModel(sizeModel);
            model = new DrumSizeModel(sizeModel);

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
            UnitConvert uc = new UnitConvert();
            model.dbmodel.Orientation = model.Orientation;
            model.dbmodel.HeadNumber = model.Headnumber;
            model.dbmodel.HeadType = model.HeadType;

            model.dbmodel.Elevation = uc.Convert(model.ElevationUnit, UOMLib.UOMEnum.Length.ToString(), model.Elevation);
            model.dbmodel.Diameter = uc.Convert(model.DiameterUnit, UOMLib.UOMEnum.Length.ToString(), model.Diameter);
            model.dbmodel.Length = uc.Convert(model.LengthUnit, UOMLib.UOMEnum.Length.ToString(), model.Length);
            model.dbmodel.NormalLiquidLevel = uc.Convert(model.NormalLiquidLevelUnit, UOMLib.UOMEnum.Length.ToString(), model.NormalLiquidLevel);
            model.dbmodel.BootDiameter = uc.Convert(model.BootDiameterUnit, UOMLib.UOMEnum.Length.ToString(), model.BootDiameter);
            model.dbmodel.BootHeight = uc.Convert(model.BootHeightUnit, UOMLib.UOMEnum.Length.ToString(), model.BootHeight);
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
