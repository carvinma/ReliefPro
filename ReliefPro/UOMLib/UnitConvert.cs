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
        public static ILookup<string, systbSystemUnit> lkpSystemUnit;
        public static ILookup<int, systbSystemUnit> lkpSystemUnitByUnitType;

        public static ILookup<string, systbUnitType> lkpUnitType;

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

            double? innerValue = (value.Value + originUnitModel.First().Constant) * originUnitModel.First().ScaleFactor;
            if (targetUnitModel.First().ScaleFactor != 0)
            {
                double? resultValue = innerValue / targetUnitModel.First().ScaleFactor - targetUnitModel.First().Constant;
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

            double innerValue = (value + originUnitModel.First().Constant.Value) * originUnitModel.First().ScaleFactor.Value;
            if (targetUnitModel.First().ScaleFactor != 0)
            {
                double resultValue = innerValue / targetUnitModel.First().ScaleFactor.Value - targetUnitModel.First().Constant.Value;
                return resultValue;
            }
            return value;
        }
    }
}
