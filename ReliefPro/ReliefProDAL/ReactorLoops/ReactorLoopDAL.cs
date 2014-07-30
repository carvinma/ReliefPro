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
    public class ReactorLoopDAL : IBaseDAL<ReactorLoop>
    {
        public ReactorLoop GetModel(ISession session)
        {
            var list = session.CreateCriteria<ReactorLoop>().List<ReactorLoop>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public IList<ReactorLoopDetail> GetReactorLoopDetail(ISession session, int ReactorLoopID, int ReactorType)
        {
            var list = session.CreateCriteria<ReactorLoopDetail>().Add(Expression.Eq("ReactorLoopID", ReactorLoopID))
                .Add(Expression.Eq("ReactorType", ReactorType)).List<ReactorLoopDetail>();
            return list;
        }
        public void Save(ISession session, ReactorLoop model, IList<ReactorLoopDetail> lstDetailModel)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {
                    session.SaveOrUpdate(model);
                    var sql = "from tbReactorLoopDetail Where ReactorType in (0,1,2) and ReactorLoopID=" + model.ID;
                    session.Delete(sql);
                    foreach (var detail in lstDetailModel)
                    {
                        detail.ReactorLoopID = model.ID;
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
