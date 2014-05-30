using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class ScenarioHeatSourceDAL : IBaseDAL<ScenarioHeatSource>
    {

        public IList<ScenarioHeatSource> GetScenarioStreamList(ISession session, int ScenarioStreamID)
        {
            IList<ScenarioHeatSource> list = null;
            try
            {
                list = session.CreateCriteria<ScenarioHeatSource>().Add(Expression.Eq("ScenarioStreamID", ScenarioStreamID)).List<ScenarioHeatSource>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public IList<ScenarioHeatSource> GetAllList(ISession session)
        {
            IList<ScenarioHeatSource> list = null;
            try
            {
                list = session.CreateCriteria<ScenarioHeatSource>().List<ScenarioHeatSource>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public IList<ScenarioHeatSource> GetScenarioHeatSourceList(ISession session, int ScenarioID)
        {
            IList<ScenarioHeatSource> list = null;
            try
            {
                list = session.CreateCriteria<ScenarioHeatSource>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<ScenarioHeatSource>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
