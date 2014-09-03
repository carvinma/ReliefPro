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
    public class TubeRuptureDAL : IBaseDAL<TubeRupture>
    {
        public IList<TubeRupture> GetAllList(ISession session)
        {
            IList<TubeRupture> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<TubeRupture>().List<TubeRupture>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public TubeRupture GetModelByScenarioID(ISession session, int ScenarioID)
        {
            session.Clear();
            var list = session.CreateCriteria<TubeRupture>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<TubeRupture>();
            if (list.Count() > 0)
            {
                TubeRupture model = list[0];
                ScenarioDAL db = new ScenarioDAL();
                var sModel = db.GetModel(model.ScenarioID, session);
                model.ReliefLoad = sModel.ReliefLoad;
                model.ReliefPressure = sModel.ReliefPressure;
                model.ReliefTemperature = sModel.ReliefTemperature;
                model.ReliefMW = sModel.ReliefMW;
                model.ReliefCpCv = sModel.ReliefCpCv;
                model.ReliefZ = sModel.ReliefZ;
                return model;
            }
            return null;
        }
        public void Save(ISession session, TubeRupture model)
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
