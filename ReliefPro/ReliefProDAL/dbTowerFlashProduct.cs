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
    public class dbTowerFlashProduct : IBaseDAL<TowerFlashProduct>
    {
        public IList<TowerFlashProduct> GetAllList(ISession session)
        {
            IList<TowerFlashProduct> list = null;
            try
            {
                list = session.CreateCriteria<TowerFlashProduct>().List<TowerFlashProduct>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
       
        public TowerFlashProduct GetModel(ISession session, string name)
        {
            TowerFlashProduct model = null;
            IList<TowerFlashProduct> list = null;
            try
            {
                list = session.CreateCriteria<TowerFlashProduct>().Add(Expression.Eq("StreamName", name)).List<TowerFlashProduct>();
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
