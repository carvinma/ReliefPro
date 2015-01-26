using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using ReliefProModel.Drums;

namespace ReliefProDAL.Drums
{
    public class DrumBlockedOutletDAL
    {
        public IList<DrumBlockedOutlet> GetAllList(ISession session)
        {
            IList<DrumBlockedOutlet> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<DrumBlockedOutlet>().List<DrumBlockedOutlet>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public DrumBlockedOutlet GetModelByScenarioID(ISession session, int ScenarioID)
        {
            session.Clear();
            var list = session.CreateCriteria<DrumBlockedOutlet>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<DrumBlockedOutlet>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void SaveDrumBlockedOutlet(ISession session, DrumBlockedOutlet model)
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
                catch (HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
}
