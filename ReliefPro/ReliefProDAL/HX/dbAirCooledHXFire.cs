using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProDAL.Common;
using ReliefProModel.HX;

namespace ReliefProDAL.HX
{
    public class dbAirCooledHXFire : IBaseDAL<AirCooledHXFire>
    {
        public IList<AirCooledHXFire> GetAllList(ISession session)
        {
            IList<AirCooledHXFire> list = null;
            try
            {
                list = session.CreateCriteria<AirCooledHXFire>().List<AirCooledHXFire>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public AirCooledHXFire GetModelByScenarioID(ISession session, int ScenarioID)
        {
            var list = session.CreateCriteria<AirCooledHXFire>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<AirCooledHXFire>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, AirCooledHXFire model)
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
