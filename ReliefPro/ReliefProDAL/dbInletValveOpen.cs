using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class dbInletValveOpen : IBaseDAL<InletValveOpen>
    {
        public IList<InletValveOpen> GetAllList(ISession session)
        {
            IList<InletValveOpen> list = null;
            try
            {
                list = session.CreateCriteria<InletValveOpen>().List<InletValveOpen>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public InletValveOpen GetModel(ISession session)
        {
            InletValveOpen model = null;
            IList<InletValveOpen> list = null;
            try
            {
                list = session.CreateCriteria<InletValveOpen>().List<InletValveOpen>();
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
