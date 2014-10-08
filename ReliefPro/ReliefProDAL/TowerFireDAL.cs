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
    public class TowerFireDAL : IBaseDAL<TowerFire>
    {

        public TowerFire GetModel(ISession session, int ScenarioID)
        {
            TowerFire model = null;
            IList<TowerFire> list = null;
            try
            {
                session.Clear();
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

        public IList<TowerFire> GetAllList(ISession session)
        {
            IList<TowerFire> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<TowerFire>().List<TowerFire>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public void DeleteAll(ISession session)
        {

        }
    }
}
