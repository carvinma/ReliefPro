using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class dbReboilerPinch : IBaseDAL<ReboilerPinch>
    {
        public IList<ReboilerPinch> GetAllList(ISession session)
        {
            IList<ReboilerPinch> list = null;
            try
            {
                list = session.CreateCriteria<ReboilerPinch>().List<ReboilerPinch>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public ReboilerPinch GetModel(ISession session)
        {
            ReboilerPinch model = null;
            IList<ReboilerPinch> list = null;
            try
            {
                list = session.CreateCriteria<ReboilerPinch>().List<ReboilerPinch>();
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
