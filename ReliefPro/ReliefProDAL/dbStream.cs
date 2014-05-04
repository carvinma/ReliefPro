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
    public class dbStream
    {
        public IList<CustomStream> GetAllList(ISession session)
        {
            IList<CustomStream> list = null;
            try
            {
                list = session.CreateCriteria<CustomStream>().List<CustomStream>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public CustomStream GetModel(ISession session, string name)
        {
            CustomStream model = null;
            IList<CustomStream> list = null;
            try
            {
                list = session.CreateCriteria<CustomStream>().Add(Expression.Eq("StreamName", name)).List<CustomStream>();
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
