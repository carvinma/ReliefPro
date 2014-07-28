using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class TreePSDAL : IBaseDAL<TreePS>
    {
        public IList<TreePS> GetAllList(int UnitID, ISession session)
        {
            IList<TreePS> list = null;
            try
            {
                list = session.CreateCriteria<TreePS>().Add(Expression.Eq("UnitID", UnitID)).List<TreePS>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public TreePS GetModel(ISession session)
        {
            TreePS model = null;
            IList<TreePS> list = null;
            try
            {
                list = session.CreateCriteria<TreePS>().List<TreePS>();
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
