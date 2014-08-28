using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProModel.Compressors;

namespace ReliefProDAL.Compressors
{
    public class CentrifugalBlockedOutletDAL
    {
        public IList<CentrifugalBlockedOutlet> GetAllList(ISession session)
        {
            IList<CentrifugalBlockedOutlet> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<CentrifugalBlockedOutlet>().List<CentrifugalBlockedOutlet>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public CentrifugalBlockedOutlet GetModelByScenarioID(ISession session, int ScenarioID)
        {
            session.Clear();
            var list = session.CreateCriteria<CentrifugalBlockedOutlet>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<CentrifugalBlockedOutlet>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, CentrifugalBlockedOutlet model)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {
                    session.SaveOrUpdate(model);
                    session.Flush();
                    tx.Commit();
                }
                catch (HibernateException hx)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
}
