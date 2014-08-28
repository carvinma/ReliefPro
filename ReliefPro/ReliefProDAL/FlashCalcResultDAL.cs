using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class FlashCalcResultDAL : IBaseDAL<FlashCalcResult>
    {
        public IList<FlashCalcResult> GetAllList(ISession session)
        {
            IList<FlashCalcResult> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<FlashCalcResult>().List<FlashCalcResult>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public FlashCalcResult GetModel(ISession session, int ScenarioID)
        {
            FlashCalcResult model = null;
            IList<FlashCalcResult> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<FlashCalcResult>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<FlashCalcResult>();
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
