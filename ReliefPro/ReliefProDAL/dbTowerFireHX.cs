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
    public class dbTowerFireHX : IBaseDAL<TowerFireHX>
    {
        public TowerFireHX GetModel(ISession session, int EqID)
        {
            TowerFireHX model = null;
            IList<TowerFireHX> list = null;
            try
            {
                list = session.CreateCriteria<TowerFireHX>().Add(Expression.Eq("EqID", EqID)).List<TowerFireHX>();
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
