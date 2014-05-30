using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProDAL.Common;
using ReliefProModel.ReactorLoop;

namespace ReliefProDAL.ReactorLoops
{
    public class ReactorLoopBlockedOutletDAL : IBaseDAL<ReactorLoopBlockedOutlet>
    {
        public IList<ReactorLoopBlockedOutlet> GetAllList(ISession session)
        {
            IList<ReactorLoopBlockedOutlet> list = null;
            try
            {
                list = session.CreateCriteria<ReactorLoopBlockedOutlet>().List<ReactorLoopBlockedOutlet>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public ReactorLoopBlockedOutlet GetModelByScenarioID(ISession session, int ScenarioID)
        {
            var list = session.CreateCriteria<ReactorLoopBlockedOutlet>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<ReactorLoopBlockedOutlet>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, ReactorLoopBlockedOutlet model)
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
