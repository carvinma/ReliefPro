using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;

namespace UOMLib
{
    public class UnitConvert
    {
        private ILookup<string, SystemUnit> lkpSystemUnit;
        private ILookup<string, UnitType> lkpUnitType;
        private IList<SystemUnit> tmpSystemUnit;
        private IList<UnitType> tmpUnitType;
        private IList<BasicUnit> lstBasicUnit;
        private IList<BasicUnitDefault> lstBasicUnitDefault;
        private UnitInfo unitInfo;
        public UnitConvert()
        {
            var lstSystemUnit = new List<SystemUnit>();
            GetALLUnitInfo();
        }
        private void GetALLUnitInfo()
        {
            unitInfo = new UnitInfo();
            tmpSystemUnit = unitInfo.GetSystemUnit();
            if (null != tmpSystemUnit)
                lkpSystemUnit = tmpSystemUnit.ToLookup(p => p.Name.ToLower());

            tmpUnitType = unitInfo.GetUnitType();
            if (null != tmpUnitType)
                lkpUnitType = tmpUnitType.ToLookup(p => p.ShortName.ToLower());

            lstBasicUnit = unitInfo.GetBasicUnit();
            lstBasicUnitDefault = unitInfo.GetBasicUnitDefault();
        }

        public List<double> Convert(string UnitType, string OriginUnit, string TargetUnit, List<double> lstValue)
        {
            List<double> lst = new List<double>();
            try
            {
                foreach (var value in lstValue)
                {
                    var result = Convert(UnitType, OriginUnit, TargetUnit, value);
                    lst.Add(result);
                }
            }
            catch (Exception ex)
            {
                lst.Clear();
                throw ex;
            }
            return lst;
        }
        public List<double> BasicConvert(string UnitType, string OriginBasic, string TargetBasic, out string TargetUnit, List<double> lstValue)
        {
            var unitTypeModel = lkpUnitType[UnitType.ToLower()];
            if (null == unitTypeModel)
                throw new Exception("The UnitType is not Exists!");
            int unitTypeID = unitTypeModel.First().ID;
            int originBasicID = lstBasicUnit.Where(p => p.UnitName.ToLower() == OriginBasic.ToLower()).FirstOrDefault().ID;
            int targetBasicID = lstBasicUnit.Where(p => p.UnitName.ToLower() == TargetBasic.ToLower()).FirstOrDefault().ID;

            int originSystemUnitID = 0;
            var originModel = lstBasicUnitDefault.Where(p => p.BasicUnitID == originBasicID && p.UnitTypeID == unitTypeID).FirstOrDefault();
            if (originModel != null)
                originSystemUnitID = originModel.ID;
            int targetSystemUnitID = 0;
            var targetModel = lstBasicUnitDefault.Where(p => p.BasicUnitID == targetBasicID && p.UnitTypeID == unitTypeID).FirstOrDefault();
            if (targetModel != null)
                targetSystemUnitID = targetModel.ID;

            if (originSystemUnitID > 0 && targetSystemUnitID > 0)
            {
                string originUnit = tmpSystemUnit.Where(p => p.ID == originSystemUnitID).First().Name;
                string targetUnit = tmpSystemUnit.Where(p => p.ID == targetSystemUnitID).First().Name;
                TargetUnit = targetUnit;
                return Convert(UnitType, originUnit, targetUnit, lstValue);
            }
            throw new Exception("Baisic Cant't find Default Unit!");
        }

        public double BasicConvert(string UnitType, string OriginBasic, string TargetBasic, out string TargetUnit, double Value)
        {
            var unitTypeModel = lkpUnitType[UnitType.ToLower()];
            if (null == unitTypeModel)
                throw new Exception("The UnitType is not Exists!");
            int unitTypeID = unitTypeModel.First().ID;
            int originBasicID = lstBasicUnit.Where(p => p.UnitName.ToLower() == OriginBasic.ToLower()).FirstOrDefault().ID;
            int targetBasicID = lstBasicUnit.Where(p => p.UnitName.ToLower() == TargetBasic.ToLower()).FirstOrDefault().ID;

            int originSystemUnitID = 0;
            var originModel = lstBasicUnitDefault.Where(p => p.BasicUnitID == originBasicID && p.UnitTypeID == unitTypeID).FirstOrDefault();
            if (originModel != null)
                originSystemUnitID = originModel.SystemUnitID;
            int targetSystemUnitID = 0;
            var targetModel = lstBasicUnitDefault.Where(p => p.BasicUnitID == targetBasicID && p.UnitTypeID == unitTypeID).FirstOrDefault();
            if (targetModel != null)
                targetSystemUnitID = targetModel.SystemUnitID;


            if (originSystemUnitID > 0 && targetSystemUnitID > 0)
            {
                string originUnit = tmpSystemUnit.Where(p => p.ID == originSystemUnitID).First().Name;
                string targetUnit = tmpSystemUnit.Where(p => p.ID == targetSystemUnitID).First().Name;
                TargetUnit = targetUnit;
                return Convert(UnitType, originUnit, targetUnit, Value);
            }
            throw new Exception("Baisic Cant't find Default Unit!");
        }
        public double Convert(string UnitType, string OriginUnit, string TargetUnit, double value)
        {
            var unitTypeModel = lkpUnitType[UnitType.ToLower()];
            if (null == unitTypeModel)
                throw new Exception("The UnitType is not Exists!");
            return Convert(OriginUnit, TargetUnit, value);
        }
        public double Convert(string OriginUnit, string TargetUnit, double value)
        {
            if (OriginUnit.ToLower() == TargetUnit.ToLower())
                return value;
            var originUnitModel = lkpSystemUnit[OriginUnit.ToLower()];
            var targetUnitModel = lkpSystemUnit[TargetUnit.ToLower()];
            if (null == originUnitModel)
                throw new Exception("the Origin Unit is not Exists!");
            if (null == targetUnitModel)
                throw new Exception("the Target Unit is not Exists!");
            if (originUnitModel.First().UnitType != targetUnitModel.First().UnitType)
                throw new Exception("the Origin Unit Translate to Target Unit is error!");

            double innerValue = (value + originUnitModel.First().Constant) * originUnitModel.First().ScaleFactor;
            if (targetUnitModel.First().ScaleFactor != 0)
            {
                double resultValue = innerValue / targetUnitModel.First().ScaleFactor - targetUnitModel.First().Constant;
                return resultValue;
            }
            return value;
        }
    }
}
