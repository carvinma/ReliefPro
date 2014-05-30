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
    public class DrumFireFluidDALs
    {
        public IList<DrumFireFluid> GetAllList(ISession session)
        {
            IList<DrumFireFluid> list = null;
            try
            {
                list = session.CreateCriteria<DrumFireFluid>().List<DrumFireFluid>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public DrumFireFluid GetModelByDrumID(ISession session, int drumFireCalcID)
        {
            var list = session.CreateCriteria<DrumFireFluid>().Add(Expression.Eq("DrumFireCalcID", drumFireCalcID)).List<DrumFireFluid>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void SaveDrumFireFluid(ISession session, DrumFireFluid model)
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
