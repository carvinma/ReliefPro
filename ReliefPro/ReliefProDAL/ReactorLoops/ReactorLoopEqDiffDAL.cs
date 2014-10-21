using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProDAL.Common;
using ReliefProModel.ReactorLoops;

namespace ReliefProDAL.ReactorLoops
{
    public class ReactorLoopEqDiffDAL : IBaseDAL<ReactorLoopEqDiff>
    {

        public IList<ReactorLoopEqDiff> GetList(ISession session, int ReactorLoopID)
        {
            session.Clear();
            var list = session.CreateCriteria<ReactorLoopEqDiff>().Add(Expression.Eq("ReactorLoopID", ReactorLoopID))
                .List<ReactorLoopEqDiff>();
            return list;
        }
        public void Save(ISession session,int ReactorLoopID, IList<ReactorLoopEqDiff> lst)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {
                    session.Clear();
                    var sql = " from ReliefProModel.ReactorLoops.ReactorLoopEqDiff o Where  o.ReactorLoopID=" + ReactorLoopID.ToString();
                    session.Delete(sql);

                    foreach (var detail in lst)
                    {
                        session.Save(detail);
                    }

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
