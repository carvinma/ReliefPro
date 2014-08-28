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
    public class TowerFireCoolerDAL : IBaseDAL<TowerFireCooler>
    {
        public TowerFireCooler GetModel(ISession session, int EqID)
        {
            TowerFireCooler model = null;
            IList<TowerFireCooler> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<TowerFireCooler>().Add(Expression.Eq("EqID", EqID)).List<TowerFireCooler>();
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
