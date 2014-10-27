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
    public class HXBlockedInDAL : IBaseDAL<HXBlockedIn>
    {
        public IList<HXBlockedIn> GetAllList(ISession session)
        {
            IList<HXBlockedIn> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<HXBlockedIn>().List<HXBlockedIn>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public HXBlockedIn GetModelByScenarioID(ISession session, int ScenarioID)
        {
            session.Clear();
            var list = session.CreateCriteria<HXBlockedIn>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<HXBlockedIn>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, HXBlockedIn model)
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
