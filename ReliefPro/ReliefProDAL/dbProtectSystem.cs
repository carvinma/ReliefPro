using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class dbProtectSystem : IBaseDAL<ProtectedSystem>
    {
        public IList<ProtectedSystem> GetAllList(ISession session)
        {
            IList<ProtectedSystem> list = null;
            try
            {
                list = session.CreateCriteria<ProtectedSystem>().List<ProtectedSystem>();
            }
            catch (Exception ee)
            { throw ee; }
            return list;
        }
    }
}