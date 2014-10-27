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
    public class HXBlockedOutletDAL : IBaseDAL<HXBlockedInlet>
    {
        public IList<HXBlockedInlet> GetAllList(ISession session)
        {
            IList<HXBlockedInlet> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<HXBlockedInlet>().List<HXBlockedInlet>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public HXBlockedInlet GetModelByScenarioID(ISession session, int ScenarioID)
        {
            session.Clear();
            var list = session.CreateCriteria<HXBlockedInlet>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<HXBlockedInlet>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, HXBlockedInlet model)
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
