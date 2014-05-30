using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProModel.CompressorBlocked;

namespace ReliefProDAL.Compressors
{
    public class PistonDAL
    {
        public IList<Piston> GetAllList(ISession session)
        {
            IList<Piston> list = null;
            try
            {
                list = session.CreateCriteria<Piston>().List<Piston>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public Piston GetModelByScenarioID(ISession session, int ScenarioID)
        {
            var list = session.CreateCriteria<Piston>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<Piston>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, Piston model)
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
