﻿using System;
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
            session.Clear();
            var ListModel = session.CreateCriteria<PUsummary>().Add(Expression.Eq("UnitID", UnitID)).List<PUsummary>();
            if (ListModel.Count > 0)
            {
                return ListModel[0];
            }
            return null;
        }

        public void Save(ISession session, PUsummary model)
        {
            session.Clear();
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {
                    session.SaveOrUpdate(model);
                    session.Flush();
                    tx.Commit();
                }
                catch (HibernateException hx)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
}
