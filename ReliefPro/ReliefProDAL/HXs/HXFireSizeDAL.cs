using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProDAL.Common;
using ReliefProModel.HXs;
using ReliefProModel.HXs;

namespace ReliefProDAL.HXs
{
    public class HXFireSizeDAL : IBaseDAL<HXFireSize>
    {
        public IList<HXFireSize> GetAllList(ISession session)
        {
            IList<HXFireSize> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<HXFireSize>().List<HXFireSize>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public HXFireSize GetModelByScenarioID(ISession session, int ScenarioID)
        {
            session.Clear();
            var list = session.CreateCriteria<HXFireSize>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<HXFireSize>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, HXFireSize model)
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
