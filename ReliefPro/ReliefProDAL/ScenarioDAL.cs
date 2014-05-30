using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class ScenarioDAL : IBaseDAL<Scenario>
    {
        public IList<Scenario> GetAllList(ISession session)
        {
            IList<Scenario> list = null;
            try
            {
                list = session.CreateCriteria<Scenario>().List<Scenario>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        
    }
}
