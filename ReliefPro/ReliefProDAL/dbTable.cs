
using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
namespace ReliefProDAL
{
    public class dbTable : IBaseDAL<DemoModel>
    {
        public IList<DemoModel> GetAllList(ISession session)
        {
            IList<DemoModel> list = null;
            try
            {
                list = session.CreateCriteria<DemoModel>().List<DemoModel>();
            }
            catch (Exception ee)
            { throw ee; }
            return list;
        }
    }
}
