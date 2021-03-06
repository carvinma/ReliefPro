﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProModel.Compressors;

namespace ReliefProDAL.Compressors
{
    public class PistonBlockedOutletDAL
    {
        public IList<PistonBlockedOutlet> GetAllList(ISession session)
        {
            IList<PistonBlockedOutlet> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<PistonBlockedOutlet>().List<PistonBlockedOutlet>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public PistonBlockedOutlet GetModelByScenarioID(ISession session, int ScenarioID)
        {
            session.Clear();
            var list = session.CreateCriteria<PistonBlockedOutlet>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<PistonBlockedOutlet>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public void Save(ISession session, PistonBlockedOutlet model)
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
