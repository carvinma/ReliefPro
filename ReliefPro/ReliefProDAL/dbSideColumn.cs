using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class dbSideColumn : IBaseDAL<SideColumn>
    {
        public IList<SideColumn> GetAllList(ISession session)
        {
            IList<SideColumn> list = null;
            try
            {
                list = session.CreateCriteria<SideColumn>().List<SideColumn>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
       
    }
}
