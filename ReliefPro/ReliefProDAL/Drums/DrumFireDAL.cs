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
    public class DrumFireDAL
    {
        public IList<DrumFireCalc> GetAllList(ISession session,int ScenarioID)
        {
            IList<DrumFireCalc> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<DrumFireCalc>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<DrumFireCalc>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public DrumFireCalc GetModelByDrumID(ISession session, int drumFireCalcID)
        {
            session.Clear();
            var list = session.CreateCriteria<DrumFireCalc>().Add(Expression.Eq("ID", drumFireCalcID)).List<DrumFireCalc>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void SaveDrumFireCalc(ISession session, DrumFireCalc model)
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
