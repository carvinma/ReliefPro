using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProDAL.Common;
using ReliefProModel.ReactorLoops;
using ReliefProModel.Reports;

namespace ReliefProDAL.Reports
{
    public class PUsummaryDAL : IBaseDAL<PUsummary>
    {
        public PUsummary GetModel(int UnitID, ISession session)
        {
            var ListModel = session.CreateCriteria<PUsummary>().Add(Expression.Eq("UnitID", UnitID)).List<PUsummary>();
            if (ListModel.Count > 0)
            {
                return ListModel[0];
            }
            return null;
        }
    }
}
