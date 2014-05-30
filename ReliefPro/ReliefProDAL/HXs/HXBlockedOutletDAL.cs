using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProDAL.Common;
using ReliefProModel.HXs;

namespace ReliefProDAL.HXs
{
    public class HXBlockedOutletDAL : IBaseDAL<HXBlockedOutlet>
    {
        public IList<HXBlockedOutlet> GetAllList(ISession session)
        {
            IList<HXBlockedOutlet> list = null;
            try
            {
                list = session.CreateCriteria<HXBlockedOutlet>().List<HXBlockedOutlet>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public HXBlockedOutlet GetModelByScenarioID(ISession session, int ScenarioID)
        {
            var list = session.CreateCriteria<HXBlockedOutlet>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<HXBlockedOutlet>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, HXBlockedOutlet model)
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
