using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class TowerScenarioStreamDAL : IBaseDAL<TowerScenarioStream>
    {
        public IList<TowerScenarioStream> GetAllList(ISession session)
        {
            IList<TowerScenarioStream> list = null;
            try
            {
                list = session.CreateCriteria<TowerScenarioStream>().List<TowerScenarioStream>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public IList<TowerScenarioStream> GetAllList(ISession session, int ScenarioID)
        {
            IList<TowerScenarioStream> list = null;
            try
            {
                list = session.CreateCriteria<TowerScenarioStream>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<TowerScenarioStream>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public IList<TowerScenarioStream> GetAllList(ISession session, int ScenarioID,bool isProduct)
        {
            IList<TowerScenarioStream> list = null;
            try
            {
                list = session.CreateCriteria<TowerScenarioStream>().Add(Expression.Eq("ScenarioID", ScenarioID)).Add(Expression.Eq("IsProduct", isProduct)).List<TowerScenarioStream>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }


        public TowerScenarioStream GetModel(ISession session, string StreamName, int ScenarioID)
        {
            TowerScenarioStream model = null;
            IList<TowerScenarioStream> list = null;
            try
            {
                list = session.CreateCriteria<TowerScenarioStream>().Add(Expression.Eq("StreamName", StreamName)).Add(Expression.Eq("ScenarioID", ScenarioID)).List<TowerScenarioStream>();
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
