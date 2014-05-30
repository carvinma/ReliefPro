using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using System.Net;
using System.Net.Mail;
namespace ReliefProCommon
{
    public partial class CommonHelper
    {
        #region Methods
        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="Email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public static bool IsValidEmail(string Email)
        {
            if (String.IsNullOrEmpty(Email))
                return false;
            return Regex.IsMatch(Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// Gets query string value by name
        /// </summary>
        /// <param name="Name">Parameter name</param>
        /// <returns>Query string value</returns>
        public static string QueryString(string Name)
        {
            string result = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request.QueryString[Name] != null)
                result = HttpContext.Current.Request.QueryString[Name].ToString();
            return result;
        }

        /// <summary>
        /// Gets boolean value from query string 
        /// </summary>
        /// <param name="Name">Parameter name</param>
        /// <returns>Query string value</returns>
        public static bool QueryStringBool(string Name)
        {
            string resultStr = QueryString(Name).ToUpperInvariant();
            return (resultStr == "YES" || resultStr == "TRUE" || resultStr == "1");
        }

        /// <summary>
        /// Gets integer value from query string 
        /// </summary>
        /// <param name="Name">Parameter name</param>
        /// <returns>Query string value</returns>
        public static int QueryStringInt(string Name)
        {
            string resultStr = QueryString(Name).ToUpperInvariant();
            int result;
            Int32.TryParse(resultStr, out result);
            return result;
        }

        /// <summary>
        /// Gets integer value from query string 
        /// </summary>
        /// <param name="Name">Parameter name</param>
        /// <param name="DefaultValue">Default value</param>
        /// <returns>Query string value</returns>
        public static int QueryStringInt(string Name, int DefaultValue)
        {
            string resultStr = QueryString(Name).ToUpperInvariant();
            if (resultStr.Length > 0)
            {
                return Int32.Parse(resultStr);
            }
            return DefaultValue;
        }

        /// <summary>
        /// Gets GUID value from query string 
        /// </summary>
        /// <param name="Name">Parameter name</param>
        /// <returns>Query string value</returns>
        public static Guid? QueryStringGUID(string Name)
        {
            string resultStr = QueryString(Name).ToUpperInvariant();
            Guid? result = null;
            try
            {
                result = new Guid(resultStr);
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// Selects item
        /// </summary>
        /// <param name="List">List</param>
        /// <param name="Value">Value to select</param>
        public static void SelectListItem(DropDownList List, object Value)
        {
            if (List.Items.Count != 0)
            {
                ListItem selectedItem = List.SelectedItem;
                if (selectedItem != null)
                    selectedItem.Selected = false;
                if (Value != null)
                {
                    selectedItem = List.Items.FindByValue(Value.ToString());
                    if (selectedItem != null)
                        selectedItem.Selected = true;
                }
            }
        }

        /// <summary>
        /// Gets server variable by name
        /// </summary>
        /// <param name="Name">Name</param>
        /// <returns>Server variable</returns>
        public static string ServerVariables(string Name)
        {
            string tmpS = String.Empty;
            try
            {
                if (HttpContext.Current.Request.ServerVariables[Name] != null)
                {

                    tmpS = HttpContext.Current.Request.ServerVariables[Name].ToString();

                }
            }
            catch
            {
                tmpS = String.Empty;
            }
            return tmpS;
        }                                                     

        /// <summary>
        /// Modifies query string
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryStringModification">Query string modification</param>
        /// <param name="targetLocationModification">Target location modification</param>
        /// <returns>New url</returns>
        public static string ModifyQueryString(string url, string queryStringModification, string targetLocationModification)
        {
            string str = string.Empty;
            string str2 = string.Empty;
            if (url.Contains("#"))
            {
                str2 = url.Substring(url.IndexOf("#") + 1);
                url = url.Substring(0, url.IndexOf("#"));
            }
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryStringModification))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    foreach (string str4 in queryStringModification.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str4))
                        {
                            string[] strArray2 = str4.Split(new char[] { '=' });
                            if (strArray2.Length == 2)
                            {
                                dictionary[strArray2[0]] = strArray2[1];
                            }
                            else
                            {
                                dictionary[str4] = null;
                            }
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
                else
                {
                    str = queryStringModification;
                }
            }
            if (!string.IsNullOrEmpty(targetLocationModification))
            {
                str2 = targetLocationModification;
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)) + (string.IsNullOrEmpty(str2) ? "" : ("#" + str2)));
        }

        /// <summary>
        /// Remove query string from url
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryString">Query string to remove</param>
        /// <returns>New url</returns>
        public static string RemoveQueryString(string url, string queryString)
        {
            string str = string.Empty;
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryString))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    dictionary.Remove(queryString);

                    StringBuilder builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)));
        }        

        /// <summary>
        /// Sets cookie
        /// </summary>
        /// <param name="cookieName">Cookie name</param>
        /// <param name="cookieValue">Cookie value</param>
        /// <param name="ts">Timespan</param>
        public static void SetCookie(String cookieName, string cookieValue, TimeSpan ts)
        {
            try
            {
                HttpCookie cookie = new HttpCookie(cookieName);
                cookie.Value = HttpContext.Current.Server.UrlEncode(cookieValue);
                DateTime dt = DateTime.Now;
                cookie.Expires = dt.Add(ts);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
        }

        /// <summary>
        /// Gets cookie string
        /// </summary>
        /// <param name="cookieName">Cookie name</param>
        /// <param name="decode">Decode cookie</param>
        /// <returns>Cookie string</returns>
        public static String GetCookieString(String cookieName, bool decode)
        {
            if (HttpContext.Current.Request.Cookies[cookieName] == null)
            {
                return String.Empty;
            }
            try
            {
                string tmp = HttpContext.Current.Request.Cookies[cookieName].Value.ToString();
                if (decode)
                    tmp = HttpContext.Current.Server.UrlDecode(tmp);
                return tmp;
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetStoreLocation()
        {
            bool useSSL = false;
            if (HttpContext.Current != null)
                useSSL = HttpContext.Current.Request.IsSecureConnection;
            return GetStoreLocation(useSSL);
        }

        /// <summary>
        /// Gets store location
        /// </summary>
        /// <param name="UseSSL">Use SSL</param>
        /// <returns>Store location</returns>
        public static string GetStoreLocation(bool UseSSL)
        {
            string result = GetStoreHost(UseSSL);
            if (result.EndsWith("/"))
                result = result.Substring(0, result.Length - 1);
            result = result + HttpContext.Current.Request.ApplicationPath;
            if (!result.EndsWith("/"))
                result += "/";

            return result;
        }

        /// <summary>
        /// Gets store host location
        /// </summary>
        /// <param name="UseSSL">Use SSL</param>
        /// <returns>Store host location</returns>
        public static string GetStoreHost(bool UseSSL)
        {
            string result = "http://" + ServerVariables("HTTP_HOST");
            if (!result.EndsWith("/"))
                result += "/";

            if (UseSSL)
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SharedSSL"]))
                {
                    result = ConfigurationManager.AppSettings["SharedSSL"];
                }
                else
                {
                    result = result.Replace("http:/", "https:/");
                }
            }

            if (!result.EndsWith("/"))
                result += "/";

            return result;
        }

        /// <summary>
        /// Gets boolean value from cookie
        /// </summary>
        /// <param name="cookieName">Cookie name</param>
        /// <returns>Result</returns>
        public static bool GetCookieBool(String cookieName)
        {
            string str1 = GetCookieString(cookieName, true).ToUpperInvariant();
            return (str1 == "TRUE" || str1 == "YES" || str1 == "1");
        }

        /// <summary>
        /// Gets integer value from cookie
        /// </summary>
        /// <param name="cookieName">Cookie name</param>
        /// <returns>Result</returns>
        public static int GetCookieInt(String cookieName)
        {
            string str1 = GetCookieString(cookieName, true);
            if (!String.IsNullOrEmpty(str1))
                return Convert.ToInt32(str1);
            else
                return 0;
        }                

        /// <summary>
        /// Write XML to response
        /// </summary>
        /// <param name="xml">XML</param>
        /// <param name="Filename">Filename</param>
        public static void WriteResponseXML(string xml, string Filename)
        {
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                ((XmlDeclaration)document.FirstChild).Encoding = "utf-8";
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.Charset = "utf-8";
                response.ContentType = "text/xml";
                response.AddHeader("content-disposition", string.Format("attachment; filename={0}", Filename));
                response.BinaryWrite(Encoding.UTF8.GetBytes(document.InnerXml));
                response.End();
            }
        }

        /// <summary>
        /// Write XLS file to response
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="targetFileName">Target file name</param>
        public static void WriteResponseXLS(string filePath, string targetFileName)
        {
            if (!String.IsNullOrEmpty(filePath))
            {
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.Charset = "utf-8";
                response.ContentType = "text/xls";
                response.AddHeader("content-disposition", string.Format("attachment; filename={0}", targetFileName));
                response.BinaryWrite(File.ReadAllBytes(filePath));
                response.End();
            }
        }

        /// <summary>
        /// Write PDF file to response
        /// </summary>
        /// <param name="filePath">File napathme</param>
        /// <param name="targetFileName">Target file name</param>
        /// <remarks>For BeatyStore project</remarks>
        public static void WriteResponsePDF(string filePath, string targetFileName)
        {
            if (!String.IsNullOrEmpty(filePath))
            {
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.Charset = "utf-8";
                response.ContentType = "text/pdf";
                response.AddHeader("content-disposition", string.Format("attachment; filename={0}", targetFileName));
                response.BinaryWrite(File.ReadAllBytes(filePath));
                response.End();
            }
        }

        /// <summary>
        /// Generate random digit code
        /// </summary>
        /// <param name="Length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int Length)
        {
            Random random = new Random();
            string s = "";
            for (int i = 0; i < Length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }

        /// <summary>
        /// Convert enum for front-end
        /// </summary>
        /// <param name="s">Input string</param>
        /// <returns>Covnerted string</returns>
        public static string ConvertEnum(string s)
        {
            string result = string.Empty;
            char[] letters = s.ToCharArray();
            foreach (char c in letters)
                if (c.ToString() != c.ToString().ToLower())
                    result += " " + c.ToString();
                else
                    result += c.ToString();
            return result;
        }

        /// <summary>
        /// Fills drop down list with values of enumaration
        /// </summary>
        /// <param name="List">Dropdownlist</param>
        /// <param name="enumType">Enumeration</param>
        //public static void FillDropDownWithEnum(DropDownList List, Type enumType)
        //{
        //    if (List == null)
        //    {
        //        throw new ArgumentNullException("List");
        //    }
        //    if (enumType == null)
        //    {
        //        throw new ArgumentNullException("enumType");
        //    }
        //    if (!enumType.IsEnum)
        //    {
        //        throw new ArgumentException("enumType must be enum type");
        //    }

        //    List.Items.Clear();
        //    string[] strArray = Enum.GetNames(enumType);
        //    foreach (string str2 in strArray)
        //    {
        //        int enumValue = (int)Enum.Parse(enumType, str2, true);
        //        ListItem ddlItem = new ListItem(CommonHelper.ConvertEnum(str2), enumValue.ToString());
        //        List.Items.Add(ddlItem);
        //    }
        //}

        public static string CompactListString(IList<string> list, string separator, bool singleQuoted)
        {
            string compacted = string.Empty;
            for (int i = 0; i < list.Count; ++i)
            {
                string item = list[i];

                if(singleQuoted)
                {
                    item+="'"+item+"'";
                }

                compacted += (i == 0) ? item : separator + item;
            }

            return compacted;
        }

        /// <summary>
        /// 发送电邮
        /// </summary>
        /// <param name="strSendAddress">发送人的地址</param>
        /// <param name="strReceiveAddress">接受人的地址.注(多人邮件以分号分隔)</param>
        /// <param name="strTitle">邮件标题</param>
        /// <param name="strBody">邮件正文</param>
        /// <returns>bool</returns>
        public static bool SendEmail(string strReceiveAddress, string strTitle, string strBody)
        {
            try
            {
                //   包含用于 SMTP 事务的主机的名称或 IP 地址//mail.gmail.com.cn
                string MailStpHost = ConfigurationManager.AppSettings["smtpServerHost"].ToString();
                string MailStpName = ConfigurationManager.AppSettings["smtpServerName"].ToString();
                string MailStpPass = ConfigurationManager.AppSettings["smtpServerPassword"].ToString();
                if (string.IsNullOrEmpty(MailStpName) || string.IsNullOrEmpty(MailStpName) || string.IsNullOrEmpty(MailStpPass) || string.IsNullOrEmpty(strReceiveAddress))
                    return false;
                MailAddress from = new MailAddress(MailStpName);
                MailAddress to = new MailAddress(strReceiveAddress);
                MailMessage message = new MailMessage(from, to);
                message.Subject = strTitle;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.Body = strBody;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                // 初始化 SmtpClient 类的新实例，让其使用指定的 SMTP 服务器发送电子邮件。
                SmtpClient client = new SmtpClient(MailStpHost);
                client.Credentials = new NetworkCredential(MailStpName, MailStpPass);
                string Port = ConfigurationManager.AppSettings["smtpServerPort"].ToString();
                if (!string.IsNullOrEmpty(Port))
                    client.Port =int.Parse(Port);
                client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
