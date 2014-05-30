
using System;
using System.Collections.Generic;
using NHibernate;
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
                list = session.CreateCriteria<BasicUnit>().List<BasicUnit>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
