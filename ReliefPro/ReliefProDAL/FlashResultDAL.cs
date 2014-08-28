using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class FlashResultDAL : IBaseDAL<FlashResult>
    {
        public IList<FlashResult> GetAllList(ISession session)
        {
            IList<FlashResult> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<FlashResult>().List<FlashResult>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public FlashResult GetModel(ISession session, string streamName)
        {
            FlashResult model = null;
            IList<FlashResult> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<FlashResult>().Add(Expression.Eq("StreamName", streamName)).List<FlashResult>();
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
