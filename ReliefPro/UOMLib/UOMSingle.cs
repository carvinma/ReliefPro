using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProModel;

namespace UOMLib
{
    public class UOMSingle
    {
        private static readonly string dbConnectPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"Template\plant.mdb";

        public static ORDesignerPlantDataContext templatePlantContext { get; set; }//模板数据库
        public static ORDesignerPlantDataContext currentPlantContext { get; set; }//当前plant数据库
        public static Dictionary<string,ALinq.DataContext> plantsContext { get; set; }//多个plant时，记录plantContext,方便切换
        public static List<UOMEnum> UomEnums;
        static UOMSingle()
        {
            UomEnums = new List<UOMEnum>();
            templatePlantContext = new ORDesignerPlantDataContext(dbConnectPath);
            currentPlantContext = templatePlantContext;
            plantsContext = new Dictionary<string, ALinq.DataContext>(); 
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
