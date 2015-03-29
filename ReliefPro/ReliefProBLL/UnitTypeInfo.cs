using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProModel;

namespace ReliefProBLL
{
    public class UnitTypeInfo
    {
        string dbPlant = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\plant.mdb";
             
        public IList<systbUnitType> GetUnitType()
        {
            IList<systbUnitType> lstUnitType;
            UnitTypeDAL db = new UnitTypeDAL();
            using (var helper = new NHibernateHelper(dbPlant))
            {
                lstUnitType = db.GetAllList(helper.GetCurrentSession());
            }
            return lstUnitType;
        }

        public void Save(IList<systbUnitType> lstUnitType)
        {
            UnitTypeDAL db = new UnitTypeDAL();
            using (var helper = new NHibernateHelper(dbPlant))
            {
                var Session = helper.GetCurrentSession();
                using (ITransaction tx = Session.BeginTransaction())
                {
                    try
                    {
                        foreach (var unitType in lstUnitType)
                        {
                            var model = Session.Get<systbUnitType>(unitType.Id);
                            //model.Custom = unitType.Custom;
                            Session.Update(model);
                        }
                        Session.Flush();
                        tx.Commit();
                    }
                    catch (HibernateException)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
