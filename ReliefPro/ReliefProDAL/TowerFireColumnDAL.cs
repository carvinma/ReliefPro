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
    public class TowerFireColumnDAL : IBaseDAL<TowerFireColumn>
    {
        public TowerFireColumn GetModel(ISession session, int EqID)
        {
            TowerFireColumn model = null;
            IList<TowerFireColumn> list = null;
            try
            {
                list = session.CreateCriteria<TowerFireColumn>().Add(Expression.Eq("EqID", EqID)).List<TowerFireColumn>();
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
