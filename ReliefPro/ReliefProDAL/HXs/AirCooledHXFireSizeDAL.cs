using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProDAL.Common;
using ReliefProModel.HXs;

namespace ReliefProDAL.HXs
{
    public class AirCooledHXFireSizeDAL : IBaseDAL<AirCooledHXFireSize>
    {
        public IList<AirCooledHXFireSize> GetAllList(ISession session)
        {
            IList<AirCooledHXFireSize> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<AirCooledHXFireSize>().List<AirCooledHXFireSize>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public AirCooledHXFireSize GetModelByScenarioID(ISession session, int ScenarioID)
        {
            session.Clear();
            var list = session.CreateCriteria<AirCooledHXFireSize>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<AirCooledHXFireSize>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, AirCooledHXFireSize model)
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
