using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProDAL.Common;
using ReliefProModel.HX;

namespace ReliefProDAL.HX
{
    public class dbHXFire : IBaseDAL<HXFire>
    {
        public IList<HXFire> GetAllList(ISession session)
        {
            IList<HXFire> list = null;
            try
            {
                list = session.CreateCriteria<HXFire>().List<HXFire>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public HXFire GetModelByScenarioID(ISession session, int ScenarioID)
        {
            var list = session.CreateCriteria<HXFire>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<HXFire>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, HXFire model)
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
