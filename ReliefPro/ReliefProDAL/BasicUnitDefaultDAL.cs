
using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
namespace ReliefProDAL
{
    public class BasicUnitDefaultDAL : IBaseDAL<BasicUnitDefault>
    {
        public IList<BasicUnitDefault> GetAllList(ISession session)
        {
            IList<BasicUnitDefault> list = null;
            try
            {
                list = session.CreateCriteria<BasicUnitDefault>().List<BasicUnitDefault>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
