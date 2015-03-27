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

        public static int? BaseUnitSelectedID { get; set; }//当前选中系统单位默认ID（切换单位制转换）
        public static ORDesignerPlantDataContext templatePlantContext { get; set; }//模板数据库
        public static PlantInfo currentPlant { get; set; }
        public static List<PlantInfo> plantsInfo { get; set; }//多个plant时，记录plantContext

        static UOMSingle()
        {
            plantsInfo = new List<PlantInfo>();
            /*程序启动时，当前plant数据库默认为模板数据库，在App.cs中初始化单位转换所需基础数据
            * 新建或打开Plant后，currentPlant Context需切换为当前路径下
            */
            templatePlantContext = new ORDesignerPlantDataContext(dbConnectPath);
            currentPlant = new PlantInfo();
            currentPlant.DataContext = templatePlantContext;
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
