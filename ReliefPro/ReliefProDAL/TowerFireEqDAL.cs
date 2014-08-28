using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class TowerFireEqDAL : IBaseDAL<TowerFireEq>
    {
        public IList<TowerFireEq> GetAllList(ISession session, int FireID)
        {
            IList<TowerFireEq> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<TowerFireEq>().Add(Expression.Eq("FireID", FireID)).List<TowerFireEq>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

    }
}
