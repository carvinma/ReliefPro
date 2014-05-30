using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class FeedBottomHXDAL : IBaseDAL<FeedBottomHX>
    {
        public IList<FeedBottomHX> GetAllList(ISession session,int SourceID)
        {
            IList<FeedBottomHX> list = null;
            try
            {
                list = session.CreateCriteria<FeedBottomHX>().Add(Expression.Eq("SourceID", SourceID)).List<FeedBottomHX>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public FeedBottomHX GetModel(ISession session, int HeatSourceID)
        {
            FeedBottomHX model = null;
            IList<FeedBottomHX> list = null;
            try
            {
                list = session.CreateCriteria<FeedBottomHX>().Add(Expression.Eq("HeatSourceID", HeatSourceID)).List<FeedBottomHX>();
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
