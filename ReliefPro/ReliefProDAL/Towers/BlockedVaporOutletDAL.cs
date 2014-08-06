using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProDAL.Common;
using ReliefProModel.Towers;

namespace ReliefProDAL.Towers
{
    public class BlockedVaporOutletDAL : IBaseDAL<BlockedVaporOutlet>
    {
        public IList<BlockedVaporOutlet> GetBlockedVaporOutlet(ISession session, int OutletType)
        {
            var list = session.CreateCriteria<BlockedVaporOutlet>()
                .Add(Expression.Eq("OutletType", OutletType)).List<BlockedVaporOutlet>();
            return list;
        }

        public void Save(ISession session, BlockedVaporOutlet model)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {
                    session.SaveOrUpdate(model);
                    session.Flush();
                    tx.Commit();
                }
                catch (HibernateException hx)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
}
