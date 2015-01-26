using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProDAL.Common;
using ReliefProModel.ReactorLoops;

namespace ReliefProDAL.ReactorLoops
{
    public class ReactorLoopCommonDAL : IBaseDAL<ReactorLoopCommon>
    {
        public IList<ReactorLoopCommon> GetAllList(ISession session)
        {
            IList<ReactorLoopCommon> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<ReactorLoopCommon>().List<ReactorLoopCommon>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public ReactorLoopCommon GetModelByScenarioID(ISession session, int ScenarioID, int ReactorType)
        {
            session.Clear();
            var list = session.CreateCriteria<ReactorLoopCommon>().Add(Expression.Eq("ScenarioID", ScenarioID)).Add(Expression.Eq("ReactorType", ReactorType)).List<ReactorLoopCommon>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, ReactorLoopCommon model)
        {
            session.Clear();
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
