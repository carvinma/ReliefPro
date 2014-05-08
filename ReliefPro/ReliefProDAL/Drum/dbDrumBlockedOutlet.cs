using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using ReliefProModel.Drum;

namespace ReliefProDAL.Drum
{
    public class dbDrumBlockedOutlet
    {
        public IList<DrumBlockedOutlet> GetAllList(ISession session)
        {
            IList<DrumBlockedOutlet> list = null;
            try
            {
                list = session.CreateCriteria<DrumBlockedOutlet>().List<DrumBlockedOutlet>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public DrumBlockedOutlet GetModelByDrumID(ISession session, int drumID)
        {
            var list = session.CreateCriteria<DrumBlockedOutlet>().Add(Expression.Eq("DrumID", drumID)).List<DrumBlockedOutlet>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void SaveDrumBlockedOutlet(ISession session, DrumBlockedOutlet model)
        {
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
