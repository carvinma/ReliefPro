﻿using System;
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

        //public IList<ScenarioHeatSource> GetScenarioStreamHeatSourceList(ISession session, int ScenarioStreamID, string HeatSourceType)
        //{
        //    IList<ScenarioHeatSource> list = null;
        //    try
        //    {
        //        session.Clear();
        //        list = session.CreateCriteria<ScenarioHeatSource>().Add(Expression.Eq("ScenarioStreamID", ScenarioStreamID)).Add(Expression.Eq("HeatSourceType", HeatSourceType)).List<ScenarioHeatSource>();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return list;
        //}
        public IList<ScenarioHeatSource> GetScenarioStreamHeatSourceList(ISession session, int ScenarioStreamID,bool IsFired)
        {
            IList<ScenarioHeatSource> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<ScenarioHeatSource>().Add(Expression.Eq("IsFired", IsFired)).Add(Expression.Eq("ScenarioStreamID", ScenarioStreamID)).List<ScenarioHeatSource>();
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
                session.Clear();
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
                session.Clear();
                list = session.CreateCriteria<ScenarioHeatSource>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<ScenarioHeatSource>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public ScenarioHeatSource GetModel(ISession session, int HeatSourceID,bool IsFired, int ScenarioID)
        {
            ScenarioHeatSource model = null;
            IList<ScenarioHeatSource> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<ScenarioHeatSource>().Add(Expression.Eq("IsFired", IsFired)).Add(Expression.Eq("HeatSourceID", HeatSourceID)).Add(Expression.Eq("ScenarioID", ScenarioID)).List<ScenarioHeatSource>();
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
