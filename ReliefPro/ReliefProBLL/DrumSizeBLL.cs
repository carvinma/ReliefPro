using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProDAL.Drum;
using ReliefProModel;
using ReliefProModel.Drum;
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
            dbDrumSize dbSize = new dbDrumSize();
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
            UnitConvert uc = new UnitConvert();
            sizeModel = model;
            UOMLib.UOMEnum uomEnum = new UOMEnum(this.SessionPF);
            sizeModel.Elevation = uc.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.Elevation);
            sizeModel.Diameter = uc.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.Diameter);
            sizeModel.Length = uc.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.Length);
            sizeModel.NormalLiquidLevel = uc.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.NormalLiquidLevel);

            sizeModel.BootDiameter = uc.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.BootDiameter);
            sizeModel.BootHeight = uc.Convert(UOMLib.UOMEnum.Length.ToString(), uomEnum.UserLength, sizeModel.BootHeight);
            return sizeModel;
        }

    }
}
