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

namespace ReliefProLL
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
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            sizeModel.Elevation = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.Elevation.Value);
            sizeModel.Diameter = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.Diameter.Value);
            sizeModel.Length = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.Length.Value);
            sizeModel.NormalLiquidLevel = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.NormalLiquidLevel.Value);

            sizeModel.BootDiameter = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.BootDiameter.Value);
            sizeModel.BootHeight = UnitConvert.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.BootHeight.Value);
            return sizeModel;
        }

    }
}
