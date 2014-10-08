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
    public class TowerFireColumnDetailDAL : IBaseDAL<TowerFireColumnDetail>
    {
        public IList<TowerFireColumnDetail> GetAllList(ISession session, int ColumnID)
        {
            IList<TowerFireColumnDetail> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<TowerFireColumnDetail>().Add(Expression.Eq("ColumnID", ColumnID)).List<TowerFireColumnDetail>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public IList<TowerFireColumnDetail> GetAllList(ISession session)
        {
            IList<TowerFireColumnDetail> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<TowerFireColumnDetail>().List<TowerFireColumnDetail>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
