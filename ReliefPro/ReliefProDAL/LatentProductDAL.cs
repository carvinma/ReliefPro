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
    public class LatentProductDAL : IBaseDAL<LatentProduct>
    {
        public IList<LatentProduct> GetAllList(ISession session)
        {
            IList<LatentProduct> list = null;
            try
            {
                list = session.CreateCriteria<LatentProduct>().List<LatentProduct>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public LatentProduct GetModel(ISession session, string ProdType)
        {
            LatentProduct model = null;
            IList<LatentProduct> list = null;
            try
            {
                list = session.CreateCriteria<LatentProduct>().Add(Expression.Eq("ProdType", ProdType)).List<LatentProduct>();
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

        public void RemoveALL(ISession session)
        {
            IList<LatentProduct> list = null;
            try
            {
                list = session.CreateCriteria<LatentProduct>().List<LatentProduct>();
                foreach(LatentProduct m in list)
                {
                    session.Delete(m);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
