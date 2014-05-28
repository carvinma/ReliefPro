using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class dbScenarioHeatSource : IBaseDAL<ScenarioHeatSource>
    {

        public IList<ScenarioHeatSource> GetAllList(ISession session, int ScenarioStreamID)
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
      
    }
}
