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

        public static int? BaseUnitSelectedID { get; set; }//当前选中系统单位ID
        public static ORDesignerPlantDataContext templatePlantContext { get; set; }//模板数据库
        public static ORDesignerPlantDataContext currentPlantContext { get; set; }//当前plant数据库
        public static int currentPlantId { get; set; }//当前plantID
        public static List<PlantInfo> plantsInfo { get; set; }//多个plant时，记录plantContext

        static UOMSingle()
        {
            templatePlantContext = new ORDesignerPlantDataContext(dbConnectPath);
            currentPlantContext = templatePlantContext;
            /*程序启动时，当前plant数据库默认为模板数据库，在App.cs中初始化单位转换所需基础数据
             * 新建或打开Plant后，currentPlantContext需切换为当前路径下
             */
            plantsInfo = new List<PlantInfo>(); 
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
