using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Configuration;
using System.Reflection;

namespace ReliefProCommon.SessionShare
{   
        public class CrossDomainCookie : IHttpModule
        {
            private string m_RootDomain = string.Empty;

            #region IHttpModule Members

            public void Dispose()
            {

            }

            public void Init(HttpApplication context)
            {
                m_RootDomain = ConfigurationManager.AppSettings["RootDomain"];

                Type stateServerSessionProvider = typeof(HttpSessionState).Assembly.GetType("System.Web.SessionState.OutOfProcSessionStateStore");
                FieldInfo uriField = stateServerSessionProvider.GetField("s_uribase", BindingFlags.Static | BindingFlags.NonPublic);

                if (uriField == null)
                    throw new ArgumentException("UriField was not found");

                uriField.SetValue(null, m_RootDomain);

                context.EndRequest += new System.EventHandler(context_EndRequest);

                //发布EC时请将下面代码注释；为其他解决方案提供common.dll时请去掉注释
                //context.PreRequestHandlerExecute += new EventHandler(context_AcquireRequestState);
            }

            void context_EndRequest(object sender, System.EventArgs e)
            {
                HttpApplication app = sender as HttpApplication;

                for (int i = 0; i < app.Context.Response.Cookies.Count; i++)
                {                    
                    if (app.Context.Response.Cookies[i].Name == "ASP.NET_SessionId")
                    {
                        app.Context.Response.Cookies[i].Domain = m_RootDomain;
                    }  
                }
                
            }

            public void context_AcquireRequestState(object sender, EventArgs e)
            {
                HttpApplication application = sender as HttpApplication;
                string strLoginURL = ConfigurationManager.AppSettings["LoginURL"].ToString();
                try
                {
                    if (application.Context.Session == null)
                        return;
                    string path = application.Context.Request.Url.ToString();
                    int n = path.ToLower().IndexOf("login.aspx");
                    if (n == -1)
                    {
                        if (CommonFunc.GetCurrentUser(application.Context) == null)
                        {
                            //application.Context.Response.Redirect(strLoginURL);

                            string strMsg = "获取用户信息失败，请重新登录!";
                            //string strLoginURL = ConfigurationManager.AppSettings["LoginURL"].ToString();
                            application.Context.Response.Write("<script type='text/javascript'>alert('" + strMsg + "');location.href='" + strLoginURL + "';</script>");
                        }
                    }
                }
                catch
                {


                }
            }

            #endregion
        }
    }

