using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class dbTowerScenario : IBaseDAL<TowerScenario>
    {
        public IList<TowerScenario> GetAllList(ISession session)
        {
            IList<TowerScenario> list = null;
            try
            {
                list = session.CreateCriteria<TowerScenario>().List<TowerScenario>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        
    }
}
