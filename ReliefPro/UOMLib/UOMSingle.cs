using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProModel.GlobalDefault;

namespace UOMLib
{
    public class UOMSingle
    {
        private static readonly string dbConnectPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"Template\plant.mdb";

        public static ISession Session { get; private set; }
        public static int BaseUnitSelectedID { get; set; }
        public static Dictionary<string,ISession> listUnitSession { get; set; }
        public static List<UOMEnum> UomEnums;
        static UOMSingle()
        {
            UomEnums = new List<UOMEnum>();
            using (var helper = new UOMLNHibernateHelper(dbConnectPath))
            {
                Session = helper.GetCurrentSession();
            }
            listUnitSession = new Dictionary<string, ISession>(); 
        }
        private static UOMSingle _instance;

        public static UOMSingle Instance()
        {
            if (_instance == null)
                _instance = new UOMSingle();
            return _instance;
        }
    }
}
