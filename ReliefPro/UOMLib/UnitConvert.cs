using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;
using ReliefPro;

namespace UOMLib
{
    public class UnitConvert
    {
        private ILookup<string, SystemUnit> lkpSystemUnit;
        private ILookup<string, UnitType> lkpUnitType;
        private SystemUnitInfo systemUnitInfo;
        private UnitTypeInfo unitTypeInfo;

        public UnitConvert()
        {
            var lstSystemUnit = new List<SystemUnit>();
            systemUnitInfo = new SystemUnitInfo();
            unitTypeInfo = new UnitTypeInfo();
            GetALLUnitInfo();
        }

        private void GetALLUnitInfo()
        {
            var tmpSystemUnit = systemUnitInfo.GetSystemUnit();
            if (null != tmpSystemUnit)
                lkpSystemUnit = tmpSystemUnit.ToLookup(p => p.Name.ToLower());

            var tmpUnitType = unitTypeInfo.GetUnitType();
            if (null != tmpUnitType)
                lkpUnitType = tmpUnitType.ToLookup(p => p.ShortName.ToLower());
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
        public double Convert(string UnitType, string OriginUnit, string TargetUnit, double value)
        {
            var unitTypeModel = lkpUnitType[UnitType.ToLower()];
            if (null == unitTypeModel)
                throw new Exception("The UnitType is not Exists!");
            return Convert(OriginUnit, TargetUnit, value);
        }
        public double Convert(string OriginUnit, string TargetUnit, double value)
        {
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

