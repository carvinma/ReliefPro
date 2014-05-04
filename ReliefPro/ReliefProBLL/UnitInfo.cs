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
    public class UnitInfo
    {
        private readonly string dbConnectPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\plant.mdb";
        public IList<BasicUnit> GetBasicUnit()
        {
            IList<BasicUnit> lstBasicUnit;
            dbBasicUnit db = new dbBasicUnit();
            using (var helper = new NHibernateHelper(dbConnectPath))
            {
                lstBasicUnit = db.GetAllList(helper.GetCurrentSession());
            }
            return lstBasicUnit;
        }

        public IList<BasicUnitDefault> GetBasicUnitDefault()
        {
            IList<BasicUnitDefault> lstBasicUnitDefault;
            dbBasicUnitDefault db = new dbBasicUnitDefault();
            using (var helper = new NHibernateHelper(dbConnectPath))
            {
                lstBasicUnitDefault = db.GetAllList(helper.GetCurrentSession());
            }
            return lstBasicUnitDefault;
        }

        public IList<SystemUnit> GetSystemUnit()
        {
            IList<SystemUnit> lstSystemUnit;
            dbSystemUnit db = new dbSystemUnit();
            using (var helper = new NHibernateHelper(dbConnectPath))
            {
                lstSystemUnit = db.GetAllList(helper.GetCurrentSession());
            }
            return lstSystemUnit;
        }
        public IList<UnitType> GetUnitType()
        {
            IList<UnitType> lstUnitType;
            dbUnitType db = new dbUnitType();
            using (var helper = new NHibernateHelper(dbConnectPath))
            {
                lstUnitType = db.GetAllList(helper.GetCurrentSession());
            }
            return lstUnitType;
        }
        public int BasicUnitAdd(BasicUnit model)
        {
            int tmpID = 0;
            dbBasicUnit db = new dbBasicUnit();
            using (var helper = new NHibernateHelper(dbConnectPath))
            {
                object o = db.Add(model, helper.GetCurrentSession());
                int.TryParse(o.ToString(), out tmpID);
            }
            return tmpID;
        }
        public int BasicUnitSetDefault(int id)
        {
            dbBasicUnit db = new dbBasicUnit();
            using (var helper = new NHibernateHelper(dbConnectPath))
            {
                var Session = helper.GetCurrentSession();
                string sql = "update tbBasicUnit a set IsDefault=0 where a.ID<>:ID";
                string sql2 = "update tbBasicUnit a set IsDefault=1 where a.ID=:ID";
                var query = Session.CreateSQLQuery(sql);
                var query2 = Session.CreateSQLQuery(sql2);
                query.SetInt32("ID", id);
                query2.SetInt32("ID", id);
                try
                {
                    var rows = query.ExecuteUpdate();
                    rows = query2.ExecuteUpdate();
                    return rows;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public void Save(IList<BasicUnitDefault> lst)
        {
            using (var helper = new NHibernateHelper(dbConnectPath))
            {
                var Session = helper.GetCurrentSession();
                using (ITransaction tx = Session.BeginTransaction())
                {
                    try
                    {
                        foreach (var basicUnitDefault in lst)
                        {
                            Session.SaveOrUpdate(basicUnitDefault);
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
