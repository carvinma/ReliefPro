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

        public IList<BasicUnit> GetBasicUnit()
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
        public IList<BasicUnitCurrent> GetBasicUnitCurrent()
        {
            IList<BasicUnitCurrent> lstBasicUnitCurrent;
            BasicUnitCurrentDAL db = new BasicUnitCurrentDAL();

            lstBasicUnitCurrent = db.GetAllList(TempleSession.Session);
            return lstBasicUnitCurrent;
        }
        public IList<BasicUnitDefault> GetBasicUnitDefault()
        {
            IList<BasicUnitDefault> lstBasicUnitDefault;
            BasicUnitDefaultDAL db = new BasicUnitDefaultDAL();

            lstBasicUnitDefault = db.GetAllList(TempleSession.Session);
            return lstBasicUnitDefault;
        }
        public IList<BasicUnitDefault> GetBasicUnitDefaultUserSet(ISession SessionPlan)
        {
            BasicUnitDefaultDAL db = new BasicUnitDefaultDAL();
            var lstBasicUnitDefault = db.GetAllList(SessionPlan);
            return lstBasicUnitDefault;
        }
        public IList<BasicUnitCurrent> GetBasicUnitCurrentUserSet(ISession SessionPlan)
        {
            BasicUnitCurrentDAL db = new BasicUnitCurrentDAL();
            var lstBasicUnitCurrent = db.GetAllList(SessionPlan);
            return lstBasicUnitCurrent;
        }
        public IList<SystemUnit> GetSystemUnit()
        {
            IList<SystemUnit> lstSystemUnit;
            SystemUnitDAL db = new SystemUnitDAL();

            lstSystemUnit = db.GetAllList(TempleSession.Session);
            return lstSystemUnit;
        }
        public IList<SystemUnit> GetSystemUnit(ISession SessionPlan)
        {
            IList<SystemUnit> lstSystemUnit;
            SystemUnitDAL db = new SystemUnitDAL();

            lstSystemUnit = db.GetAllList(SessionPlan);
            return lstSystemUnit;
        }
        public IList<UnitType> GetUnitType()
        {
            IList<UnitType> lstUnitType;
            UnitTypeDAL db = new UnitTypeDAL();

            lstUnitType = db.GetAllList(TempleSession.Session);
            return lstUnitType;
        }
        public int BasicUnitAdd(BasicUnit model)
        {
            int tmpID = 0;
            BasicUnitDAL db = new BasicUnitDAL();
            using (var helper = new UOMLNHibernateHelper(dbConnectPath))
            {
                object o = db.Add(model, helper.GetCurrentSession());
                int.TryParse(o.ToString(), out tmpID);
            }
            return tmpID;
        }
        public int BasicUnitSetDefault(int id)
        {
            BasicUnitDAL db = new BasicUnitDAL();
            using (var helper = new UOMLNHibernateHelper(dbConnectPath))
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
            using (var helper = new UOMLNHibernateHelper(dbConnectPath))
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
        public void SaveCurrent(IList<BasicUnitCurrent> lst)
        {
            using (var helper = new UOMLNHibernateHelper(dbConnectPath))
            {
                var Session = helper.GetCurrentSession();
                using (ITransaction tx = Session.BeginTransaction())
                {
                    try
                    {
                        string sql = "from ReliefProModel.BasicUnitCurrent";
                        Session.Delete(sql);
                        foreach (var basicCurrent in lst)
                        {
                            Session.Save(basicCurrent);
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
