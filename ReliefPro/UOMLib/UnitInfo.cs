using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProDAL;
using ReliefProModel;
using System.ComponentModel;

namespace UOMLib
{
    public class UnitInfo
    {
        private readonly string dbConnectPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\plant.mdb";

        public IList<BasicUnit> GetBasicUnit(ISession SessionPlan)
        {
            IList<BasicUnit> lstBasicUnit;
            BasicUnitDAL db = new BasicUnitDAL();
            using (var helper = new UOMLNHibernateHelper(dbConnectPath))
            {
                lstBasicUnit = db.GetAllList(helper.GetCurrentSession());
            }
            return lstBasicUnit;
        }
        public BasicUnit GetBasicUnitUOM(ISession SessionPlan)
        {
            BasicUnitDAL db = new BasicUnitDAL();
            var lstBasicUnit = db.GetAllList(SessionPlan);
            return lstBasicUnit.Where(p => p.IsDefault == 1).First();
        }
        public IList<BasicUnitDefault> GetBasicUnitDefault(ISession SessionPlan)
        {
            BasicUnitDefaultDAL db = new BasicUnitDefaultDAL();
            var lstBasicUnitDefault = db.GetAllList(SessionPlan);
            return lstBasicUnitDefault;
        }

        public IList<BasicUnitCurrent> GetBasicUnitCurrent(ISession SessionPlan)
        {
            BasicUnitCurrentDAL db = new BasicUnitCurrentDAL();
            var lstBasicUnitCurrent = db.GetAllList(SessionPlan);
            return lstBasicUnitCurrent;
        }

        public IList<SystemUnit> GetSystemUnit(ISession SessionPlan)
        {
            IList<SystemUnit> lstSystemUnit;
            SystemUnitDAL db = new SystemUnitDAL();

            lstSystemUnit = db.GetAllList(SessionPlan);
            return lstSystemUnit;
        }
        public IList<UnitType> GetUnitType(ISession SessionPlan)
        {
            UnitTypeDAL db = new UnitTypeDAL();

            var lstUnitType = db.GetAllList(SessionPlan);
            return lstUnitType;
        }
        public int BasicUnitAdd(BasicUnit model, ISession SessionPlan)
        {
            int tmpID = 0;
            BasicUnitDAL db = new BasicUnitDAL();
            object o = db.Add(model, SessionPlan);
            int.TryParse(o.ToString(), out tmpID);
            return tmpID;
        }
        public int BasicUnitSetDefault(int id)
        {
            BasicUnitDAL db = new BasicUnitDAL();

            string sql = "update tbBasicUnit a set IsDefault=0 where a.ID<>:ID";
            string sql2 = "update tbBasicUnit a set IsDefault=1 where a.ID=:ID";
            var query = UOMSingle.Session.CreateSQLQuery(sql);
            var query2 = UOMSingle.Session.CreateSQLQuery(sql2);
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
        public void Save(IList<BasicUnitDefault> lst, ISession SessionPlan)
        {
            using (ITransaction tx = SessionPlan.BeginTransaction())
            {
                try
                {
                    foreach (var basicUnitDefault in lst)
                    {
                        SessionPlan.SaveOrUpdate(basicUnitDefault);
                    }
                    SessionPlan.Flush();
                    tx.Commit();
                }
                catch (HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
        public void SaveCurrent(IList<BasicUnitCurrent> lst, ISession SessionPlan)
        {
            using (ITransaction tx = SessionPlan.BeginTransaction())
            {
                try
                {
                    string sql = "from ReliefProModel.BasicUnitCurrent";
                    SessionPlan.Delete(sql);
                    foreach (var basicCurrent in lst)
                    {
                        SessionPlan.Save(basicCurrent);
                    }
                    SessionPlan.Flush();
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
