using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using ReliefProModel.ReactorLoops;

namespace ReliefProDAL.ReactorLoops
{
    public class GeneralFailureCommonDAL
    {
        public GeneralFailureCommon GetModelByScenarioID(ISession session, int ScenarioID, int GeneralType)
        {
            session.Clear();
            var list = session.CreateCriteria<GeneralFailureCommon>().Add(Expression.Eq("ScenarioID", ScenarioID))
                .Add(Expression.Eq("GeneralType", GeneralType)).List<GeneralFailureCommon>();
            if (list.Count() > 0)
            {
                return list[0];
            }
            return null;
        }
        public IList<GeneralFailureCommonDetail> GetGeneralFailureCommonDetail(ISession session, int GeneralFailureCommonID, int ReactorType)
        {
            session.Clear();
            var list = session.CreateCriteria<GeneralFailureCommonDetail>().Add(Expression.Eq("GeneralFailureCommonID", GeneralFailureCommonID)).Add(Expression.Eq("ReactorType", ReactorType))
               .List<GeneralFailureCommonDetail>();
            return list;
        }
        public void Save(ISession session, GeneralFailureCommon model, IList<GeneralFailureCommonDetail> lstDetailModel)
        {
            session.Clear();
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {
                    session.SaveOrUpdate(model);
                    var sql = "from ReliefProModel.ReactorLoops.GeneralFailureCommonDetail Where GeneralFailureCommonID=" + model.ID;
                    session.Delete(sql);
                    foreach (var detail in lstDetailModel)
                    {
                        detail.GeneralFailureCommonID = model.ID;
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
