using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class InletValveOpenDAL : IBaseDAL<InletValveOpen>
    {
        public IList<InletValveOpen> GetAllList(ISession session)
        {
            IList<InletValveOpen> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<InletValveOpen>().List<InletValveOpen>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public InletValveOpen GetModel(ISession session,int ScenarioID)
        {
            InletValveOpen model = null;
            IList<InletValveOpen> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<InletValveOpen>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<InletValveOpen>();
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
