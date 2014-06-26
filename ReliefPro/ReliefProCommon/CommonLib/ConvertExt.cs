namespace ReliefProCommon.CommonLib
{
    using System;
    using System.Reflection;
    using System.ComponentModel;

    /// <summary>
    /// utility add by vivid 
    /// </summary>
    public static class ConvertExt
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// db -> decimal to page view
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToViewString(this decimal? d)
        {
            return d.HasValue ? d.ToString() : string.Empty;
        }
        /// <summary>
        /// db -> int to page view
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToViewString(this int? d)
        {
            return d.HasValue ? d.ToString() : string.Empty;
        }
        /// <summary>
        /// db -> int to page bool
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool ToBool(this int? d)
        {
            if (d.HasValue)
                if (d.Value != 0)
                    return true;

            return false;
        }

        /// <summary>
        /// db -> date to page view
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToViewString(this DateTime? d)
        {
            return d.HasValue ? d.Value.ToString("yyyy-MM-dd") : string.Empty;
        }

        /// <summary>
        /// page -> bool to db int
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int? ToDBInt32(this bool s)
        {
            return s ? 1 : 0;
        }
        /// <summary>
        /// page -> view string to int
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int? ToInt32(this string s)
        {
            int i;
            if (int.TryParse(s.Trim(), out i))
                return i;
            else
                return null;
        }
        /// <summary>
        /// page -> view string to date
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime? ToDate(this string s)
        {
            DateTime dt;
            if (DateTime.TryParse(s.Trim(), out dt))
                return dt;
            else
                return null;
        }

        /// <summary>
        /// page -> view string to decimal
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static decimal? ToDecimal(this string d)
        {
            decimal ds;
            if (decimal.TryParse(d.Trim(), out ds))
                return ds;
            else
                return null;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
              (DescriptionAttribute[])fi.GetCustomAttributes(
              typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        public static string ObjectToString(object obj)
        {
            string rs = string.Empty;
            if (obj is Array)
            {
                object[] objdata = (System.Object[])obj;
                foreach (object s in objdata)
                {
                    if (s != null)
                    {
                        string v = s.ToString();
                        if (v!= "")
                        {
                            rs = rs + "," + v;
                        }
                    }
                }
                rs = rs.Substring(1);
            }
            else if (obj == null)
            {
                rs = "";
            }
            else
                rs = obj.ToString();

            return rs;
        }
    }
}
