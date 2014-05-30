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
    public class DrumSizeDAL
    {
        public IList<DrumSize> GetAllList(ISession session)
        {
            IList<DrumSize> list = null;
            try
            {
                list = session.CreateCriteria<DrumSize>().List<DrumSize>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public DrumSize GetModelByDrumID(ISession session, int drumFireCalcID)
        {
            var list = session.CreateCriteria<DrumSize>().Add(Expression.Eq("DrumFireCalcID", drumFireCalcID)).List<DrumSize>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void SaveDrumSize(ISession session, DrumSize model)
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
