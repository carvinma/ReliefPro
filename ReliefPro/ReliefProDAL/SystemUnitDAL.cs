
using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
namespace ReliefProDAL
{
    public class SystemUnitDAL : IBaseDAL<SystemUnit>
    {
        public IList<SystemUnit> GetAllList(ISession session)
        {
            IList<SystemUnit> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<SystemUnit>().List<SystemUnit>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
