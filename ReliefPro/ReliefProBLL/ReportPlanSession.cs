using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace ReliefProBLL
{
    public class ReportPlanSession
    {

        public static ISession PlantSession { get; set; }
        static ReportPlanSession()
        {
        }
        private static ReportPlanSession _instance;

        public static ReportPlanSession Instance()
        {
            if (_instance == null)
                _instance = new ReportPlanSession();
            return _instance;
        }
    }
}
