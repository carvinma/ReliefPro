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
        public static ILookup<string, SystemUnit> lkpSystemUnit;
        public static ILookup<string, UnitType> lkpUnitType;
        public static IList<BasicUnit> lstBasicUnit;
        public static IList<BasicUnitDefault> lstBasicUnitDefault;

        public static IList<SystemUnit> tmpSystemUnit;
        public static IList<UnitType> tmpUnitType;

        public static List<double> Convert(string UnitType, string OriginUnit, string TargetUnit, List<double> lstValue)
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
        public static List<double> BasicConvert(string UnitType, string OriginBasic, string TargetBasic, out string TargetUnit, List<double> lstValue)
        {
            var unitTypeModel = lkpUnitType[UnitType.ToLower()];
            if (null == unitTypeModel)
                throw new Exception("The UnitType Not Exists!");
            int unitTypeID = unitTypeModel.First().ID;
            int originBasicID = lstBasicUnit.FirstOrDefault(p => p.UnitName.ToLower() == OriginBasic.ToLower()).ID;
            int targetBasicID = lstBasicUnit.FirstOrDefault(p => p.UnitName.ToLower() == TargetBasic.ToLower()).ID;

            int originSystemUnitID = 0;
            var originModel = lstBasicUnitDefault.FirstOrDefault(p => p.BasicUnitID == originBasicID && p.UnitTypeID == unitTypeID);
            if (originModel != null)
                originSystemUnitID = originModel.ID;
            int targetSystemUnitID = 0;
            var targetModel = lstBasicUnitDefault.FirstOrDefault(p => p.BasicUnitID == targetBasicID && p.UnitTypeID == unitTypeID);
            if (targetModel != null)
                targetSystemUnitID = targetModel.ID;

            if (originSystemUnitID > 0 && targetSystemUnitID > 0)
            {
                string originUnit = tmpSystemUnit.Where(p => p.ID == originSystemUnitID).First().Name;
                string targetUnit = tmpSystemUnit.Where(p => p.ID == targetSystemUnitID).First().Name;
                TargetUnit = targetUnit;
                return Convert(UnitType, originUnit, targetUnit, lstValue);
            }
            throw new Exception("Baisic Cant't Find Default Unit!");
        }

        public static double BasicConvert(string UnitType, string OriginBasic, string TargetBasic, out string TargetUnit, double Value)
        {
            var unitTypeModel = lkpUnitType[UnitType.ToLower()];
            if (null == unitTypeModel)
                throw new Exception("The UnitType Not Exists!");
            int unitTypeID = unitTypeModel.First().ID;
            int originBasicID = lstBasicUnit.FirstOrDefault(p => p.UnitName.ToLower() == OriginBasic.ToLower()).ID;
            int targetBasicID = lstBasicUnit.FirstOrDefault(p => p.UnitName.ToLower() == TargetBasic.ToLower()).ID;

            int originSystemUnitID = 0;
            var originModel = lstBasicUnitDefault.FirstOrDefault(p => p.BasicUnitID == originBasicID && p.UnitTypeID == unitTypeID);
            if (originModel != null)
                originSystemUnitID = originModel.SystemUnitID;
            int targetSystemUnitID = 0;
            var targetModel = lstBasicUnitDefault.FirstOrDefault(p => p.BasicUnitID == targetBasicID && p.UnitTypeID == unitTypeID);
            if (targetModel != null)
                targetSystemUnitID = targetModel.SystemUnitID;


            if (originSystemUnitID > 0 && targetSystemUnitID > 0)
            {
                string originUnit = tmpSystemUnit.Where(p => p.ID == originSystemUnitID).First().Name;
                string targetUnit = tmpSystemUnit.Where(p => p.ID == targetSystemUnitID).First().Name;
                TargetUnit = targetUnit;
                return Convert(UnitType, originUnit, targetUnit, Value);
            }
            throw new Exception("Baisic Cant't Find Default Unit!");
        }
        public static double Convert(string UnitType, string OriginUnit, string TargetUnit, double value)
        {
            var unitTypeModel = lkpUnitType[UnitType.ToLower()];
            if (null == unitTypeModel)
                throw new Exception("The UnitType Not Exists!");
            return Convert(OriginUnit, TargetUnit, value);
        }
        public static double? Convert(string OriginUnit, string TargetUnit, double? value)
        {
            if (value == null) return null;
            if (string.IsNullOrEmpty(OriginUnit) || string.IsNullOrEmpty(TargetUnit))
                return value;
            if (OriginUnit.ToLower() == TargetUnit.ToLower())
                return value;
            var originUnitModel = lkpSystemUnit[OriginUnit.ToLower()];
            var targetUnitModel = lkpSystemUnit[TargetUnit.ToLower()];
            if (null == originUnitModel)
                throw new Exception("the Origin Unit Not Exists!");
            if (null == targetUnitModel)
                throw new Exception("the Target Unit Not Exists!");
            if (originUnitModel.First().UnitType != targetUnitModel.First().UnitType)
                throw new Exception("the Origin Unit Translate to Target Unit is error!");

            double innerValue = (value.Value + originUnitModel.First().Constant) * originUnitModel.First().ScaleFactor;
            if (targetUnitModel.First().ScaleFactor != 0)
            {
                double resultValue = innerValue / targetUnitModel.First().ScaleFactor - targetUnitModel.First().Constant;
                return resultValue;
            }
            return value;
        }
        public static double Convert(string OriginUnit, string TargetUnit, double value)
        {
            //if (value == 0) return 0;
            if (string.IsNullOrEmpty(OriginUnit) || string.IsNullOrEmpty(TargetUnit))
                return value;
            if (OriginUnit.ToLower() == TargetUnit.ToLower())
                return value;
            var originUnitModel = lkpSystemUnit[OriginUnit.ToLower()];
            var targetUnitModel = lkpSystemUnit[TargetUnit.ToLower()];
            if (null == originUnitModel)
                throw new Exception("the Origin Unit Not Exists!");
            if (null == targetUnitModel)
                throw new Exception("the Target Unit Not Exists!");
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
