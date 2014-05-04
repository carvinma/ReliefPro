using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProModel;

namespace ReliefProBLL
{
    public class SystemUnitInfo
    {
        public IList<SystemUnit> GetSystemUnit()
        {
            IList<SystemUnit> lstSystemUnit;
            dbSystemUnit db = new dbSystemUnit();
            string dbPlant = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\plant.mdb";
                    
            using (var helper = new NHibernateHelper(dbPlant))
            {
                lstSystemUnit = db.GetAllList(helper.GetCurrentSession());
            }
            return lstSystemUnit;
        }
    }
}
