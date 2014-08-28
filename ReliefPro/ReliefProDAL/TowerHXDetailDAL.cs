using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class TowerHXDetailDAL : IBaseDAL<TowerHXDetail>
    {
        public IList<TowerHXDetail> GetAllList(ISession session)
        {
            IList<TowerHXDetail> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<TowerHXDetail>().List<TowerHXDetail>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public IList<TowerHXDetail> GetAllList(ISession session, int HXID)
        {
            IList<TowerHXDetail> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<TowerHXDetail>().Add(Expression.Eq("HXID", HXID)).List<TowerHXDetail>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public TowerHXDetail GetModel(ISession session, int ID)
        {
            TowerHXDetail model = null;
            IList<TowerHXDetail> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<TowerHXDetail>().Add(Expression.Eq("ID", ID)).List<TowerHXDetail>();
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
