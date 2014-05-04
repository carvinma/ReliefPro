using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class dbTowerFire : IBaseDAL<TowerFire>
    {

        public TowerFire GetModel(ISession session, int ScenarioID)
        {
            TowerFire model = null;
            IList<TowerFire> list = null;
            try
            {
                list = session.CreateCriteria<TowerFire>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<TowerFire>();
                if (list.Count > 0)
                {
                    model = list[0];
                }
                else
                    model = null;
            }
            catch (Exception ex)
            {
                model = null;
                throw ex;

            }

            return model;
        }
    }
}
