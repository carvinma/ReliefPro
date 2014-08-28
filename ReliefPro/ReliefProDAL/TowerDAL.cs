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
    public class TowerDAL : IBaseDAL<Tower>
    {
        public IList<Tower> GetAllList(ISession session)
        {
            IList<Tower> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<Tower>().List<Tower>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public Tower GetModel(ISession session)
        {
            Tower model = null;
            IList<Tower> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<Tower>().List<Tower>();
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
