using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProModel.Drum;

namespace ReliefProDAL.Drum
{
    public class dbDrumPressuring
    {
        public IList<DrumPressuring> GetAllList(ISession session)
        {
            IList<DrumPressuring> list = null;
            try
            {
                list = session.CreateCriteria<DrumPressuring>().List<DrumPressuring>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public DrumPressuring GetModelByDrumID(ISession session, int drumFireCalcID)
        {
            var list = session.CreateCriteria<DrumPressuring>().Add(Expression.Eq("ID", drumFireCalcID)).List<DrumPressuring>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void SaveDrumPressuring(ISession session, DrumPressuring model)
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
