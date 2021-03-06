﻿using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class HeatSourceDAL : IBaseDAL<HeatSource>
    {

        public IList<HeatSource> GetAllList(ISession session, int SourceID)
        {
            IList<HeatSource> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<HeatSource>().Add(Expression.Eq("SourceID", SourceID)).List<HeatSource>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public IList<HeatSource> GetAllList(ISession session)
        {
            IList<HeatSource> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<HeatSource>().List<HeatSource>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
