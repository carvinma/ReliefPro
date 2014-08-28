using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class AbnormalHeaterDetailDAL : IBaseDAL<AbnormalHeaterDetail>
    {

        public IList<AbnormalHeaterDetail> GetAllList(ISession session, int ScenarioID)
        {
            IList<AbnormalHeaterDetail> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<AbnormalHeaterDetail>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<AbnormalHeaterDetail>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public IList<AbnormalHeaterDetail> GetAllList(ISession session, int ScenarioID, int AbnormalType)
        {
            IList<AbnormalHeaterDetail> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<AbnormalHeaterDetail>().Add(Expression.Eq("ScenarioID", ScenarioID)).Add(Expression.Eq("AbnormalType", AbnormalType)).List<AbnormalHeaterDetail>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public AbnormalHeaterDetail GetModel(ISession session, int ScenarioID, int HeaterID, int AbnormalType)
        {
            AbnormalHeaterDetail model = null;
            try
            {
                session.Clear();
                IList<AbnormalHeaterDetail> list = session.CreateCriteria<AbnormalHeaterDetail>().Add(Expression.Eq("ScenarioID", ScenarioID)).Add(Expression.Eq("HeaterID", HeaterID)).Add(Expression.Eq("AbnormalType", AbnormalType)).List<AbnormalHeaterDetail>();
                if (list.Count > 0)
                    model = list[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }

    }
}
