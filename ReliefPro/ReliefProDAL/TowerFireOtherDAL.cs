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
    public class TowerFireOtherDAL : IBaseDAL<TowerFireOther>
    {
        public TowerFireOther GetModel(ISession session, int EqID)
        {
            TowerFireOther model = null;
            IList<TowerFireOther> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<TowerFireOther>().Add(Expression.Eq("EqID", EqID)).List<TowerFireOther>();
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
