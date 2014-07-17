using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class TreeUnitDAL : IBaseDAL<TreeUnit>
    {

        public TreeUnit GetModel(ISession session,string UnitName)
        {
            TreeUnit model = null;
            IList<TreeUnit> list = null;
            try
            {
                list = session.CreateCriteria<TreeUnit>().Add(Expression.Eq("UnitName", UnitName)).List<TreeUnit>();
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
