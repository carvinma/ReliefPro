using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.CommonLib
{
   public class StringUtil
    {

        /// <summary>
        /// 将字符串转换成集合
        /// </summary>
        /// <param name="oldstr">要转换的字符串</param>
        /// <param name="stringBy">以什么样的字符串截取</param>
        /// <returns></returns>
        public static List<String> stringToList(String oldstr, String stringBy)
        {
            if (String.IsNullOrEmpty(oldstr)) return new List<String>();
            String[] str = oldstr.Split(new string[] { stringBy }, StringSplitOptions.None);
            if (str != null) return str.ToList<String>();
            else return new List<String>();
        }

        /// <summary>
        /// 将字符串转换成集合
        /// 默认用“,”来截取字符串
        /// </summary>
        /// <param name="oldstr">要转换的字符串</param>
        /// <returns></returns>
        public static List<String> stringToList(String oldstr)
        {
            if (String.IsNullOrEmpty(oldstr)) return new List<String>();
            String[] str = oldstr.Split(new string[] { "," }, StringSplitOptions.None);
            if (str != null) return str.ToList<String>();
            else return new List<String>();
        }

        /// <summary>
        /// 将字符串转换成Set集合,默认用“,”来截取字符串
        /// </summary>
        /// <param name="oldstr"></param>
        /// <returns></returns>
        public static HashSet<String> stringToHashSet(String oldstr)
        {
            HashSet<String> hs = new HashSet<string>();
            if (String.IsNullOrEmpty(oldstr)) return hs;
            String[] str = oldstr.Split(new string[] { "," }, StringSplitOptions.None);

            if (str != null)
            {
                foreach (String s in str)
                {
                    if (!"".Equals(s)) hs.Add(s);
                }
            }
            return hs;
        }

        /// <summary>
        /// 合并两个HashSet集合
        /// 如果两个集合都为空，则返回 一个空的集合。
        /// </summary>
        /// <param name="newhs"></param>
        /// <param name="oldhs"></param>
        /// <returns></returns>
        public static HashSet<String> hashSetAddToHashSet(HashSet<String> newhs, HashSet<String> oldhs)
        {
            if (newhs == null || oldhs == null)
                return (newhs == null) ? (oldhs == null ? new HashSet<String>() : oldhs) : newhs;
            foreach (String s in oldhs)
            {
                if (!"".Equals(s)) newhs.Add(s);
            }
            return newhs;
        }



        /// <summary>
        /// 将集合转换成字符串 并用“,”连接集合是中的每一个元素
        /// </summary>
        /// <param name="strList"></param>
        /// <returns></returns>
        public static String listToString(List<String> strList)
        {
            if (strList == null) return null;
            String str = null;
            for (int i = 0; i < strList.Count; i++)
            {
                if (str == null)
                {
                    str = strList[i];
                }
                else
                {
                    str = str + "," + strList[i];
                }
            }
            return str;
        }



        /// <summary>
        /// 将HashSet 转换成String，并将各个元素用“,”隔开
        /// </summary>
        /// <param name="strSet"></param>
        /// <returns></returns>
        public static String SetToString(HashSet<String> strSet)
        {
            if (strSet == null) return null;

            String str = null;

            foreach (String s in strSet)
            {
                if (str == null)
                {
                    if (!String.IsNullOrEmpty(s)) str = s;
                }
                else
                {
                    if (!String.IsNullOrEmpty(s))
                        str = str + "," + s;
                }
            }
            return str;
        }

        /// <summary>
        /// 将HashSet 转换成String，并将各个元素用“,”隔开
        /// 并给每个元素加入单引号"'"
        /// </summary>
        /// <param name="strSet"></param>
        /// <returns></returns>
        public static String SetToStringMarks(HashSet<String> strSet)
        {
            if (strSet == null) return null;

            String str = null;

            foreach (String s in strSet)
            {
                if (str == null)
                {
                    if (!String.IsNullOrEmpty(s)) str = "'" + s + "'";
                }
                else
                {
                    if (!String.IsNullOrEmpty(s))
                        str = str + ",'" + s + "'";
                }
            }
            return str;
        }



        /// <summary>
        /// 将集合转换成字符串 并用“,”连接集合是中的每一个元素
        /// 并且每个字符带单引号‘'’
        /// </summary>
        /// <returns></returns>
        public static String listToStringMarks(List<String> strList)
        {
            if (strList == null) return null;
            String str = null;
            for (int i = 0; i < strList.Count; i++)
            {
                if (str == null)
                {
                    str = "'" + strList[i] + "'";
                }
                else
                {
                    str = str + ",'" + strList[i] + "'";
                }
            }
            return str;

        }

        /// <summary>
        ///  将字符串转换成INT32
        ///  如果字符串为空，则返回NULL,
        ///  如果转换出错，则返回NULL
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int? StringToInt32(String str)
        {
            if (String.IsNullOrEmpty(str)) return null;
            try
            {
                return Convert.ToInt32(str);
            }
            catch (Exception e)
            {

            }
            return null;
        }

        /// <summary>
        ///  将字符串转换成INT32
        ///  如果字符串为空，则返回-1,
        ///  如果转换出错，则返回-1
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static System.Nullable<long> StringToLong(String str)
        {
            if (String.IsNullOrEmpty(str)) return null;
            try
            {
                return Convert.ToInt64(str);
            }
            catch (Exception e)
            {

            }
            return null;
        }

        /// <summary>
        ///  将字符串转换成Decimal
        ///  如果字符串为空，则返回0,
        ///  如果转换出错，则返回-1
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>decimal
        public static decimal StringToDecimal(Object str)
        {
            if (str == null) return 0;
            try
            {
                return Convert.ToDecimal(str);
            }
            catch (Exception e)
            {

            }
            return -1;
        }


        /// <summary>
        ///  将字符串转换成Double
        ///  如果字符串为空，则返回0,
        ///  如果转换出错，则返回-1.0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>decimal
        public static Double StringToDouble(Object str)
        {
            if (str == null) return 0.0;
            try
            {
                return Convert.ToDouble(str);
            }
            catch (Exception e)
            {

            }
            return -1.0;
        }



        /// <summary>
        ///  将object转换成INT32
        ///  如果object为空，则返回-1,
        ///  如果转换出错，则返回-1
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ObjectToInt32(object obj)
        {
            if (obj == null) return -1;
            int? i = StringToInt32(obj.ToString());
            if (i == null)
            {
                return -1;
            }
            else
            {
                return (int)i;
            }
        }


        /// <summary>
        /// 将字符串用‘,’截断 并且个值都加上单引
        /// </summary>
        /// <param name="marks"></param>
        /// <returns></returns>
        public static String stringToMarksString(String marks)
        {
            if (String.IsNullOrEmpty(marks)) return null;
            String[] str = marks.Split(',');
            String s = "";
            foreach (String i in str)
            {
                if ("".Equals(s))
                {
                    s = "'" + i.ToString() + "'";
                }
                else
                {
                    s = s + "," + "'" + i.ToString() + "'";
                }

            }
            return s;

        }
        /// <summary>
        /// 判断字符串是否为空
        ///  如果不为空，则去前后空格
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String stringTransToNull(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return null;
            }
            else
            {
                return s.Trim();
            }
        }

        /// <summary>
        /// 判断两个字符串是否相同
        /// 字符串去空格后再比较
        /// 如果两个同时为空，则返回TRUE
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool stringEquals(String str1, String str2)
        {
            bool b = false;

            if (!String.IsNullOrEmpty(str1))
            {
                b = str1.Equals(stringTransToNull(str2));
            }
            else
            {
                if (String.IsNullOrEmpty(stringTransToNull(str2)))
                {
                    b = true;
                }
                else
                {
                    b = false;
                }

            }

            return b;
        }



        /// <summary>
        /// 将Object 转换成 字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static String objectToString(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else
            {
                return obj.ToString();
            }

        }

        /// <summary>
        /// 若string字符串是NULL或String.Empty，则转换成""// Yanjun.hou 2012.8.6
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string NoNullString(object obj)
        {
            if (obj == null)
            {
                return "";
            }

            return obj.ToString();
        }

        public static bool IsSteam(string VaporFraction,string CompIn,string TotalComposition)
        {
            if (VaporFraction == "1")
                return true;
            else if(!CompIn.ToLower().Contains("h2o"))
            {
                return false;
            }
            else
            {
                string[] arrCompIn = CompIn.Split(',');
                string[] arrTotalComposition = TotalComposition.Split(',');
                int len=arrCompIn.Length;
                for (int i = 0; i < len; i++)
                {
                    if (arrCompIn[i].ToLower() == "h2o" && arrTotalComposition[i] == "1")
                    {
                        return true;
                    }
                   
                }
            }
            return false;
        }
    }
}
