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
    public class dbSystemScenarioFactor : IBaseDAL<SystemScenarioFactor>
    {
        public IList<SystemScenarioFactor> GetAllList(ISession session)
        {
            IList<SystemScenarioFactor> list = null;
            try
            {
                list = session.CreateCriteria<SystemScenarioFactor>().List<SystemScenarioFactor>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public SystemScenarioFactor GetSystemScenarioFactor(ISession session, string category, string categoryvalue)
        {
            SystemScenarioFactor model = null;
            IList<SystemScenarioFactor> list = null;
            try
            {
                list = session.CreateCriteria<SystemScenarioFactor>().Add(Expression.Eq("Category", category)).Add(Expression.Eq("CategoryValue", categoryvalue)).List<SystemScenarioFactor>();
                if (list.Count > 0)
                {
                    model = list[0];
                }
                else
                    model = null;
            }
            catch (Exception ex)
            {
                model = null;
                throw ex;

            }

            return model;
        }
        
    }
}
