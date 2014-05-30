using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProCommon.Enum
{
    public static class ContractScoreTypeManger
    {
        private static List<ScoreType> scoreTypeList = null;

        public static List<ScoreType> GetScoreTypeList()
        {
            List<ScoreType> list = new List<ScoreType>();
            list.Add(ScoreType.国际物流运输单);
            list.Add(ScoreType.合同);
            return list;
        }

        public static string GetScoreTypeNameByID(int id)
        {
            scoreTypeList = GetScoreTypeList();
            string strResult = string.Empty;
            foreach (var item in scoreTypeList)
            {
                int conID = (int)item;
                if (conID == id)
                {
                    strResult = item.ToString();
                }
            }
            return strResult;
        }

        public static ScoreType? GetScoreTypeByID(int id)
        {
            ScoreType scoreType;
            scoreTypeList = GetScoreTypeList();
            foreach (var item in scoreTypeList)
            {
                int conID = (int)item;
                if (conID == id)
                {
                    scoreType = item;
                    return scoreType;
                }
            }
            return null ;
        }

    }

    public enum ScoreType
    {
        /// <summary>
        /// 国际物流运输单
        /// </summary>
        国际物流运输单 = 1,

        /// <summary>
        /// 合同
        /// </summary>
        合同 = 2,
    }
}
