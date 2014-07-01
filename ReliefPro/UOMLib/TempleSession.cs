using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace UOMLib
{
    public class TempleSession
    {
        private static readonly string dbConnectPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"Template\plant.mdb";

        public static ISession Session { get; private set; }
        static TempleSession()
        {
            using (var helper = new UOMLNHibernateHelper(dbConnectPath))
            {
                Session = helper.GetCurrentSession();
            }
        }
        private static TempleSession _instance;

        public static TempleSession Instance()
        {
            if (_instance == null)
                _instance = new TempleSession();
            return _instance;
        }

    }
}
