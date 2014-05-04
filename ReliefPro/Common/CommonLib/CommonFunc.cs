using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using Pcitc.Model.Session;

/// <summary>
/// 定义通用功能函数
/// </summary>
namespace ReliefProCommon
{
    public class CommonFunc
    {

        /// <summary>
        /// 获取系统日期时间,返回日期时间类型
        /// </summary>
        public static DateTime GetSysDateAsDate()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// 获取系统日期,返回字符串类型
        /// </summary>
        public static string GetSysDateAsYMDString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 去时间字符串，无分隔符
        /// </summary>
        /// <returns></returns>
        public static string GetSysDateAsYMDStringNoSeparator()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 获取这个月的第一天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetDateAsMonthFirstDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// 获取当前日期偏移dayOffset天数后的日期，返回字符串
        /// 例如 如果想返回30天前的日期，就输入-30
        /// </summary>
        /// <param name="dayOffset"></param>
        /// <returns></returns>
        public static string GetSysDateAsYMDString(double dayOffset)
        {
            return DateTime.Now.AddDays(dayOffset).ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 获取系统日期时间,返回字符串类型 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string GetSysDateAsString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string GetSysDateAsYMDString(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string GetSysDateAsString(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string GetSysDateAsString(DateTime? datetime)
        {
            string d = datetime == null ? "" : ((DateTime)datetime).ToString("yyyy-MM-dd HH:mm:ss");
            return d;
        }
        /// <summary>
        /// 将时间对象转换为时间格式字符串
        /// </summary>
        /// <param name="strDateTime">时间对象</param>
        /// <param name="strFormat">字符串格式（""表示显示全部,"d"短日期,"g"年月日时分,"CH"中文的年月日,"CHg"中文的年月日时分,"allCH"全中文年月日,"NoYear"中文的月日时分,"NYRS"中文年月日时,"NY"中文年月）</param>
        /// <returns></returns>
        public static string ToDateTime(Object strDateTime, string strFormat)
        {
            string str = "";
            if (Convert.IsDBNull(strDateTime) || strDateTime == null)
            {
                return str;
            }
            DateTime DT = DateTime.Parse("1900-1-1");
            string strtemp = Convert.ToString(strDateTime);
            try
            {
                DT = DateTime.Parse(strtemp);
            }
            catch
            {
                if (strtemp.IndexOf("年") > 0)
                {
                    strtemp = strtemp.Replace("年", "-").Replace("月", "-").Replace("日", " ").Replace("时", ":").Replace("分", ":").Replace("秒", "");
                    strtemp = strtemp.Trim(new char[] { ':', ' ' });
                    try
                    {
                        DT = DateTime.Parse(strtemp);
                    }
                    catch
                    {
                        return "";
                    }
                }
            }
            if (DT < Convert.ToDateTime("1901-1-1"))
            {
                return str;
            }

            if (strFormat == "CH")
            {
                str = DT.Year.ToString() + "年" + DT.Month.ToString() + "月" + DT.Day.ToString() + "日";
            }
            else if (strFormat == "CHg")
            {
                str = DT.Year.ToString() + "年" + DT.Month.ToString() + "月" + DT.Day.ToString() + "日" + DT.Hour.ToString() + "时" + DT.Minute.ToString() + "分";
            }
            else if (strFormat == "NoYear") //wyj 2009-10-27 添加NoYear参数
            {
                str = DT.Month.ToString() + "月" + DT.Day.ToString() + "日" + DT.Hour.ToString() + "时" + DT.Minute.ToString() + "分";
            }
            else if (strFormat == "NYRS")//wyj 2009-10-27 添加NYRS参数
            {
                str = DT.Year.ToString() + "年" + DT.Month.ToString() + "月" + DT.Day.ToString() + "日" + DT.Hour.ToString() + "时";
            }
            else if (strFormat == "HHMM")
            {
                str = DT.Hour.ToString() + "时" + DT.Minute + "分";
            }
            else if (strFormat == "en_hm")
            {
                if (DT.Hour < 10)
                {
                    str += "0" + DT.Hour.ToString();
                }
                else
                {
                    str += DT.Hour.ToString();
                }
                if (DT.Minute < 10)
                {
                    str += ":0" + DT.Minute.ToString();
                }
                else
                {
                    str += ":" + DT.Minute.ToString();
                }
            }
            else if (strFormat == "NY")
            {
                str = DT.Year.ToString() + "年" + DT.Month.ToString() + "月";
            }
            else if (strFormat == "en_hhmmss")
            {
                if (DT.Hour < 10)
                {
                    str += "0" + DT.Hour.ToString();
                }
                else
                {
                    str += DT.Hour.ToString();
                }
                if (DT.Minute < 10)
                {
                    str += ":0" + DT.Minute.ToString();
                }
                else
                {
                    str += ":" + DT.Minute.ToString();
                }
                if (DT.Second < 10)
                {
                    str += ":0" + DT.Second.ToString();
                }
                else
                {
                    str += ":" + DT.Second.ToString();
                }
            }
            else if (strFormat == "")
                str = DT.ToString();
            else str = DT.ToString(strFormat);
            return str.Replace("/", "-");
        }

        /// <summary>
        /// 获取当前时间偏移dayOffset天数后的时间，返回字符串
        /// 例如 如果想返回30天前的时间，就输入-30
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="dayOffset"></param>
        /// <returns></returns>
        public static string GetSysDateAsString(double dayOffset)
        {
            return DateTime.Now.AddDays(dayOffset).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// startOrEnd为true则offset时间为当天的开始，false则为当天结束
        /// </summary>
        /// <param name="dayOffset"></param>
        /// <param name="startOrEnd"></param>
        /// <returns></returns>
        public static string GetSysDateAsString(double dayOffset, bool startOrEnd)
        {
            string time = string.Empty;
            if (startOrEnd)
            {
                time = "00:00:00";
            }
            else
            {
                time = "23:59:59";
            }
            return DateTime.Now.AddDays(dayOffset).ToString("yyyy-MM-dd " + time);
        }


        /// <summary>
        /// 将字符串转成日期格式
        /// 如果转换失败，默认为当前系统时间
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime? stringTranToDataTime(String str)
        {
            DateTime dt = System.DateTime.Now;

            if (String.IsNullOrEmpty(str)) return dt;

            try
            {
                dt = DateTime.Parse(str);
            }
            catch
            {
            }
            return dt;
        }


        /// 计算两个日期的时间间隔
        /// </summary>
        /// <param name="DateTime1">第一个日期和时间</param>
        /// <param name="DateTime2">第二个日期和时间</param>        
        /// <returns>时间间隔</returns>
        public static string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;

            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();

            dateDiff = ts.TotalDays.ToString();

            return dateDiff;
        }



        /// <summary>
        /// 获取页面session中的当前登录用户信息
        /// </summary>
        /// <param name="pPage">页面对象</param>
        /// 左春刚 201108 编写

        public static SessionModel GetCurrentUser(Page pPage)
        {
            SessionModel _objUser = null;

            if (pPage != null)
            {
                _objUser = (SessionModel)pPage.Session["LoginUserInfo"];
            }
            return _objUser;
        }
        /// <summary>
        /// add by st 
        /// 2013/08/28
        /// </summary>
        /// <returns></returns>
        public static SessionModel GetSessionModelForSystemAuto()
        {

            return new SessionModel();

        }


        /// <summary>
        /// 获取用户控件session中的当前登录用户信息
        /// </summary>
        /// <param name="pUC">用户自定义对象</param>  
        ///  左春刚 201108 编写
        public static SessionModel GetCurrentUser(UserControl pUC)
        {
            SessionModel _objUser = null;

            if (pUC != null)
            {
                _objUser = (SessionModel)pUC.Session["LoginUserInfo"];
            }
            return _objUser;
        }

        /// <summary>
        ///  获取页面session中的当前登录用户信息 ---标题用
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        ///  闫瑞君 20110906 编写
        public static SessionModel GetCurrentUser(HttpContext content)
        {
            SessionModel _objUser = null;

            if (content != null)
            {
                try
                {
                    _objUser = (SessionModel)content.Session["LoginUserInfo"];
                }
                catch
                {
                    return null;
                }
            }
            return _objUser;
        }

        /// <summary>
        /// 在客户端弹出消息框
        /// </summary>
        /// <param name="pUC">用户自定义对象</param>
        /// <param name="pMessage">消息内容</param>
        public static void PageMessageBox(UserControl pUC, string pMessage)
        {
            if (pUC != null)
            {
                Page page = pUC.Page;
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "script", "<script type='text/javascript'>alert('" + pMessage + "');</script>");
            }
        }


        /// <summary>
        /// 在客户端弹出消息框
        /// </summary>
        /// <param name="pPage">页面对象</param>
        /// <param name="pMessage">消息内容</param>
        public static void PageMessageBox(Page pPage, string pMessage)
        {
            if (pPage != null)
            {
                pPage.ClientScript.RegisterClientScriptBlock(pPage.GetType(), "script", "<script type='text/javascript'>alert('" + pMessage + "');</script>");
            }
        }


        /// <summary>
        /// 在客户端弹出消息框，并跳转到另一页面
        /// </summary>
        /// <param name="pPage">页面对象</param>
        /// <param name="pMessage">消息内容</param>
        public static void PageMessageBoxGoTo(Page pPage, string pMessage, string pGoToUrl)
        {
            if (pPage != null)
            {
                pPage.ClientScript.RegisterStartupScript(pPage.GetType(), "", "<script type='text/javascript'>alert('" + pMessage + "');location.href='" + pGoToUrl + "';</script>");
            }
        }

        //用于从一个页面设置 下一个页面的跳转的url
        //就是通过session来使2个页面传递数据
        //返回一个guid，用来从session取回数据
        public static string SetDirection(HttpSessionState session, string url)
        {
            string guid = Guid.NewGuid().ToString("N");
            string keyName = "direction_" + guid;

            session[keyName] = url;

            return guid;
        }

        //要跳转的页面的url从session取出来以后，立刻从session里删掉这个数据
        //如果session里没这个数据，则返回null
        public static string FetchDirection(HttpSessionState session, string guid)
        {
            string keyName = "direction_" + guid;
            string url = null;
            if ((string)session[keyName] != null)
            {
                url = (string)session[keyName];
                session.Remove(keyName);
            }

            return url;
        }

        public static void RemoveLoginUserSess(HttpSessionState session, String sessName)
        {
            if (session[sessName] != null)
            {
                session.Remove(sessName);
            }
        }

        /// <summary>
        /// 跳转到统一信息提示页面，并显示相应的信息
        /// </summary>
        /// <param name="pPage">页面对象</param>
        /// <param name="pMsgInfo">要显示的信息内容</param>
        public static void CommMsgInfoPage(Page pPage, string pMsgInfo)
        {
            const string strUrl = "/CommonInfoMeg.aspx";
            pPage.Response.Redirect(strUrl + "?MsgInfo=" + pMsgInfo);
        }

        /// <summary>
        /// 跳转到统一信息提示页面，并显示相应的信息
        /// </summary>
        /// <param name="pPage">页面对象</param>
        /// <param name="pMsgInfo">要显示的信息内容</param>
        public static void CommMsgInfoPage(UserControl pUserControl, string pMsgInfo)
        {
            const string strUrl = "/CommonInfoMeg.aspx";
            pUserControl.Response.Redirect(strUrl + "?MsgInfo=" + pMsgInfo);
        }


        /// <summary>
        /// add by st 2011/09/21
        /// 字符串转换
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static String ObjectToString(Object obj)
        {
            if (obj == null) return "";
            else return obj.ToString();
        }

        /// <summary>
        /// 将数量转换成统一格式
        /// </summary>
        /// <param name="Quantity"></param>
        /// <returns></returns>
        public static String TransferToQuantity(object Quantity)
        {
            decimal d = 0M;
            if (Quantity != null)
            {
                try { d = decimal.Parse(Quantity.ToString()); }
                catch { }
            }
            return String.Format("{0:f2}", d);
        }
        /// <summary>
        /// 将价格转换成统一格式
        /// </summary>
        /// <param name="UnitPrice"></param>
        /// <returns></returns>
        public static String TransferToPrice(object UnitPrice)
        {
            decimal d = 0M;
            if (UnitPrice != null)
            {
                try { d = decimal.Parse(UnitPrice.ToString()); }
                catch { }
            }
            return String.Format("{0:f2}", d);
        }


        /// <summary>
        /// sha1
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Hash(string text, HashAlgorithm ha, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(text);

            byte[] encrypted = ha.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in encrypted)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// unicode sha1
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Sha1(string text)
        {
            SHA1 sha1 = SHA1.Create();
            return Hash(text, sha1, Encoding.UTF8);
        }

        public static string Md5(string text)
        {
            MD5 md5 = MD5.Create();
            return Hash(text, md5, Encoding.UTF8);
        }

        public static string EncryptString(string text)
        {
            MD5 md5 = MD5.Create();
            return Hash(text, md5, Encoding.UTF8);
        }


        /// <summary>
        /// 防止SQL注入及非法输入
        /// 检查参数stringThatMayContainSql是否含有sql有关的字符串。
        /// 如果含有sql有关的字符串则函数返回true.
        /// 会被检查的字符串有：
        /// backup, and, >, <, like, kill, join, declare, truncate, or, master, mid,
        /// chr, select, update, delete, drop, alter, execute, exec, create, sp_executesql,
        /// --, //, /*, */, ;, '
        /// 大小写不敏感
        /// </summary>
        /// <param name="stringThatMayContainSql"></param>
        /// <returns></returns>
        public static bool ValidateInput(String stringThatMayContainSql)
        {
            Regex regex = new Regex(@"
                (\bbackup\b)|(\band\b)|(>)|(<)|(\blike\b)|(=)|(\bkill\b)|
                (\bjoin\b)|(\bdeclare\b)|(\btruncate\b)|(\bor\b)|(\bmaster\b)|(\bmid\b)|
                (\bchr\b)|(')|(\bselect\b)|(\bupdate\b)|(\bdelete\b)|(\bdrop\b)|(\balter\b)|
                (\bexecute\b)|(\bexec\b)|(\bcreate\b)|(\bsp_executesql\b)|(--+)|(;)|(//+)|(/\*)|(\*/)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return regex.IsMatch(stringThatMayContainSql);
        }
        /// <summary>
        /// 修改DataTable列名称
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="diction">keys:需要修改的列名
        ///                       Value：修改之后的列明
        ///                       </param>
        /// <returns></returns>
        public static DataTable ModifyColumn(DataTable ds, Dictionary<string, string> diction)
        {
            foreach (KeyValuePair<string, string> pair in diction)
            {
                if (ds.Columns.Contains(pair.Key.ToString()))
                {
                    ds.Columns[pair.Key.ToString()].ColumnName = pair.Value.ToString();
                }
            }
            return ds;
        }
        /// <summary>
        /// 检查字符串是否为数字
        /// </summary>
        /// <param name="strString"></param>
        /// <returns></returns>
        public static bool CheckNumber(string strString)
        {
            Regex reg = new Regex(@"^([+-]?)\d*[.]?\d*$");
            Match ma = reg.Match(strString);
            if (ma.Success)
                return true;
            else
                return false;
        }
    }
}
