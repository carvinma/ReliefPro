using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class SinkDAL : IBaseDAL<Sink>
    {
        public IList<Sink> GetAllList(ISession session)
        {
            IList<Sink> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<Sink>().List<Sink>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public Sink GetModel(ISession session, string name)
        {
            Sink model = null;
            IList<Sink> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<Sink>().Add(Expression.Eq("SinkName", name)).List<Sink>();
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
