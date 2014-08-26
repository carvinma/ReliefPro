
using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
namespace ReliefProDAL
{
    public class BasicUnitCurrentDAL : IBaseDAL<BasicUnitCurrent>
    {
        public IList<BasicUnitCurrent> GetAllList(ISession session)
        {
            IList<BasicUnitCurrent> list = null;
            try
            {
                list = session.CreateCriteria<BasicUnitCurrent>().List<BasicUnitCurrent>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
