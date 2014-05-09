using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class dbTowerHX : IBaseDAL<TowerHX>
    {
        public IList<TowerHX> GetAllList(ISession session)
        {
            IList<TowerHX> list = null;
            try
            {
                list = session.CreateCriteria<TowerHX>().List<TowerHX>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public IList<TowerHX> GetAllList(ISession session,int HeaterType)
        {
            IList<TowerHX> list = null;
            try
            {
                list = session.CreateCriteria<TowerHX>().Add(Expression.Eq("HeaterType", HeaterType)).List<TowerHX>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public TowerHX GetModel(ISession session, string name)
        {
            TowerHX model = null;
            IList<TowerHX> list = null;
            try
            {
                list = session.CreateCriteria<TowerHX>().Add(Expression.Eq("HeaterName", name)).List<TowerHX>();
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
        public TowerHX GetModel(ISession session, int ID)
        {
            TowerHX model = null;
            IList<TowerHX> list = null;
            try
            {
                list = session.CreateCriteria<TowerHX>().Add(Expression.Eq("ID", ID)).List<TowerHX>();
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
