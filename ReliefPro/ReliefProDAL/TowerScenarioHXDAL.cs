using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class TowerScenarioHXDAL : IBaseDAL<TowerScenarioHX>
    {
        public IList<TowerScenarioHX> GetAllList(ISession session)
        {
            IList<TowerScenarioHX> list = null;
            try
            {
                list = session.CreateCriteria<TowerScenarioHX>().List<TowerScenarioHX>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public IList<TowerScenarioHX> GetAllList(ISession session, int ScenarioID)
        {
            IList<TowerScenarioHX> list = null;
            try
            {
                list = session.CreateCriteria<TowerScenarioHX>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<TowerScenarioHX>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public IList<TowerScenarioHX> GetAllList(ISession session,int ScenarioID,int HeaterType )
        {
            IList<TowerScenarioHX> list = null;
            try
            {
                list = session.CreateCriteria<TowerScenarioHX>().Add(Expression.Eq("ScenarioID", ScenarioID)).Add(Expression.Eq("HeaterType", HeaterType)).List<TowerScenarioHX>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public TowerScenarioHX GetModel(ISession session, int DetailID, int ScenarioID)
        {
            TowerScenarioHX model = null;
            IList<TowerScenarioHX> list = null;
            try
            {
                list = session.CreateCriteria<TowerScenarioHX>().Add(Expression.Eq("DetailID", DetailID)).Add(Expression.Eq("ScenarioID", ScenarioID)).List<TowerScenarioHX>();
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
