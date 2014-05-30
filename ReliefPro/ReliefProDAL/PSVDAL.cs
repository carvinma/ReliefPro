using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class PSVDAL : IBaseDAL<PSV>
    {
        public IList<PSV> GetAllList(ISession session)
        {
            IList<PSV> list = null;
            try
            {
                list = session.CreateCriteria<PSV>().List<PSV>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public PSV GetModel(ISession session)
        {
            PSV model = null;
            IList<PSV> list = null;
            try
            {
                list = session.CreateCriteria<PSV>().List<PSV>();
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
