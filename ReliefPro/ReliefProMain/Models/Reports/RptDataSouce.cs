using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Reporting.WinForms;

namespace ReliefProMain.Model.Reports
{
    public class RptDataSouce
    {
        public static List<ReportDataSource> ReportDS { get; set; }

        static RptDataSouce()
        {
            ReportDS = new List<ReportDataSource>();
        }
        private static RptDataSouce _instance;

        public static RptDataSouce Instance()
        {
            if (_instance == null)
                _instance = new RptDataSouce();
            return _instance;
        }
    }
}
