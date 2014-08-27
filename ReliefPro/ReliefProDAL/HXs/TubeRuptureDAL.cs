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
    public class TubeRuptureDAL : IBaseDAL<TubeRupture>
    {
        public IList<TubeRupture> GetAllList(ISession session)
        {
            IList<TubeRupture> list = null;
            try
            {
                list = session.CreateCriteria<TubeRupture>().List<TubeRupture>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public TubeRupture GetModelByScenarioID(ISession session, int ScenarioID)
        {
            var list = session.CreateCriteria<TubeRupture>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<TubeRupture>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, TubeRupture model)
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
