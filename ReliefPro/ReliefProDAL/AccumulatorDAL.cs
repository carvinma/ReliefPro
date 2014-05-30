using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class AccumulatorDAL : IBaseDAL<Accumulator>
    {
        public IList<Accumulator> GetAllList(ISession session)
        {
            IList<Accumulator> list = null;
            try
            {
                list = session.CreateCriteria<Accumulator>().List<Accumulator>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public Accumulator GetModel(ISession session)
        {
            Accumulator model = null;
            IList<Accumulator> list = null;
            try
            {
                list = session.CreateCriteria<Accumulator>().List<Accumulator>();
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
