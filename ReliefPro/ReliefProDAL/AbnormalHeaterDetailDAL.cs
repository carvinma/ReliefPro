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
                list = session.CreateCriteria<AbnormalHeaterDetail>().Add(Expression.Eq("ScenarioID", ScenarioID)).List<AbnormalHeaterDetail>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
       
    }
}
