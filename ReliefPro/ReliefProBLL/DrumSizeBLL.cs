using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.Drums;
using ReliefProModel;
using ReliefProModel.Drums;
using UOMLib;
using ReliefProCommon.Enum;

namespace ReliefProBLL
{
    public class DrumSizeBLL
    {
        private ISession SessionPS;
        private ISession SessionPF;
        public DrumSizeBLL(ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
        }
        public DrumSize GetSizeModel(int DrumFireCalcID)
        {
            DrumSize Model = new DrumSize();
            DrumSizeDAL dbSize = new DrumSizeDAL();
            List<DrumSize> lstDrumSize = dbSize.GetAllList(SessionPS).ToList();
            if (DrumFireCalcID > 0)
            {
                var findModel = lstDrumSize.Where(p => p.DrumFireCalcID == DrumFireCalcID).FirstOrDefault();
                if (findModel != null)
                    return findModel;
            }
            else
            {
                Model.Orientation = "Vertical";
                Model.HeadType = "Eclipse";
                Model.HeadNumber = 2;
                Model.BootDiameter_Color = ColorBorder.green.ToString();
                Model.BootHeight_Color = ColorBorder.green.ToString();
                Model.Diameter_Color = ColorBorder.green.ToString();
                Model.Elevation_Color = ColorBorder.green.ToString();
                Model.HeadNumber_Color = ColorBorder.green.ToString();
                Model.HeadType_Color = ColorBorder.green.ToString();
                Model.Length_Color = ColorBorder.green.ToString();
                Model.NormalLiquidLevel_Color = ColorBorder.green.ToString();
                Model.Orientation_Color = ColorBorder.green.ToString();
            }
            return Model;
        }
        public DrumSize ReadConvertModel(DrumSize model)
        {
            UnitInfo unitInfo = new UnitInfo();
            BasicUnit basicUnit = unitInfo.GetBasicUnitUOM(this.SessionPF);
            if (basicUnit.UnitName == "StInternal")
            {
                return model;
            }
            DrumSize sizeModel = new DrumSize();
            sizeModel = model;
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            sizeModel.Elevation = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.Elevation);
            sizeModel.Diameter = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.Diameter);
            sizeModel.Length = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.Length);
            sizeModel.NormalLiquidLevel = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.NormalLiquidLevel);

            sizeModel.BootDiameter = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.BootDiameter);
            sizeModel.BootHeight = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.BootHeight);
            return sizeModel;
        }

    }
}
