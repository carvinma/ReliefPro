
using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using ReliefProDAL.Common;
using ReliefProModel;
namespace ReliefProDAL
{
    public class BasicUnitDAL : IBaseDAL<BasicUnit>
    {
        public IList<BasicUnit> GetAllList(ISession session)
        {
            IList<BasicUnit> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<BasicUnit>().List<BasicUnit>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public BasicUnit GetModel(ISession session, string UnitName)
        {
            var list = session.CreateCriteria<BasicUnit>().Add(Expression.Eq("UnitName", UnitName)).List<BasicUnit>();
            return list[0];
        }
    }
}
