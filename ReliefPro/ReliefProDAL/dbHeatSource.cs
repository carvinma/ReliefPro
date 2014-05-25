using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class dbHeatSource : IBaseDAL<HeatSource>
    {
       
        public IList<HeatSource> GetAllList(ISession session, int SourceID)
        {
            IList<HeatSource> list = null;
            try
            {
                list = session.CreateCriteria<HeatSource>().Add(Expression.Eq("SourceID", SourceID)).List<HeatSource>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
      
    }
}
