using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pcitc.Model.Session;
using System.Web;

namespace ReliefProCommon.CommonLib
{
    public class UserSession
    {
        [ThreadStatic]
        public static SessionModel UserThreadStatic;

        public static SessionModel CurrentUser()
        {
            SessionModel user = null;
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                user = CommonFunc.GetCurrentUser(HttpContext.Current);


            }
            else
            {
                user = UserThreadStatic;
            }

            return user;
        }
    }
}
