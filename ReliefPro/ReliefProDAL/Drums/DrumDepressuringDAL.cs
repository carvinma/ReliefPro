using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProModel.Drums;

namespace ReliefProDAL.Drums
{
    public class DrumDepressuringDAL
    {
        public IList<DrumDepressuring> GetAllList(ISession session)
        {
            IList<DrumDepressuring> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<DrumDepressuring>().List<DrumDepressuring>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public DrumDepressuring GetModelByDrumID(ISession session, int drumFireCalcID)
        {
            session.Clear();
            var list = session.CreateCriteria<DrumDepressuring>().Add(Expression.Eq("ID", drumFireCalcID)).List<DrumDepressuring>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void SaveDrumPressuring(ISession session, DrumDepressuring model)
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
