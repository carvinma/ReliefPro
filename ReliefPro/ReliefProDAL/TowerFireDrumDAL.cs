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
    public class TowerFireDrumDAL : IBaseDAL<TowerFireDrum>
    {
        public TowerFireDrum GetModel(ISession session, int EqID)
        {
            TowerFireDrum model = null;
            IList<TowerFireDrum> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<TowerFireDrum>().Add(Expression.Eq("EqID", EqID)).List<TowerFireDrum>();
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
