/*
drum size界面  单位转换都已经有了
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using NHibernate;

using ReliefProMain.Commands;
using ReliefProMain.Models;
using UOMLib;
using ReliefProBLL;
using ReliefProDAL.Drums;
using System.Windows;

namespace ReliefProMain.ViewModel.Drums
{
    public class DrumSizeVM : ViewModelBase
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
            //if(string.IsNullOrEmpty(model.Orientation))

            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
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
            model.dbmodel.Orientation = model.Orientation;
            model.dbmodel.HeadNumber = model.Headnumber;
            model.dbmodel.HeadType = model.HeadType;

            model.dbmodel.Elevation = UnitConvert.Convert(model.ElevationUnit, UOMLib.UOMEnum.Length.ToString(), model.Elevation);
            model.dbmodel.Diameter = UnitConvert.Convert(model.DiameterUnit, UOMLib.UOMEnum.Length.ToString(), model.Diameter);
            model.dbmodel.Length = UnitConvert.Convert(model.LengthUnit, UOMLib.UOMEnum.Length.ToString(), model.Length);
            model.dbmodel.NormalLiquidLevel = UnitConvert.Convert(model.NormalLiquidLevelUnit, UOMLib.UOMEnum.Length.ToString(), model.NormalLiquidLevel);
            model.dbmodel.BootDiameter = UnitConvert.Convert(model.BootDiameterUnit, UOMLib.UOMEnum.Length.ToString(), model.BootDiameter);
            model.dbmodel.BootHeight = UnitConvert.Convert(model.BootHeightUnit, UOMLib.UOMEnum.Length.ToString(), model.BootHeight);
        }
        private void Save(object obj)
        {
            //if (!model.CheckData()) return;           
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    if (model.dbmodel.Orientation == "Vertical")
                    {
                        if (model.dbmodel.NormalLiquidLevel > model.dbmodel.Length)
                        {
                            MessageBox.Show("Normal Liquid Level could not higher than length", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else
                    {
                        if (model.dbmodel.NormalLiquidLevel > model.dbmodel.Diameter)
                        {
                            MessageBox.Show("Normal Liquid Level could not higher than diameter", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    if (model.dbmodel.ID > 0)
                    {
                        DrumSizeDAL dal = new DrumSizeDAL();
                        dal.SaveDrumSize(SessionPS, model.dbmodel);
                    }
                    wd.DialogResult = true;
                }
            }
        }
    }
}
