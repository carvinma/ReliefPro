using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProModel.CompressorBlocked;

namespace ReliefProDAL.CompressorBlocked
{
    public class CentrifugalDAL
    {
        public IList<Centrifugal> GetAllList(ISession session)
        {
            IList<Centrifugal> list = null;
            try
            {
                list = session.CreateCriteria<Centrifugal>().List<Centrifugal>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public Centrifugal GetModelByScenarioID(ISession session, int ScenarioID)
        {
            var list = session.CreateCriteria<Centrifugal>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<Centrifugal>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, Centrifugal model)
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
