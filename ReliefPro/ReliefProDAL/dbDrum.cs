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
using ReliefProModel.Drum;

namespace ReliefProDAL
{
    public class dbDrum : IBaseDAL<ReliefProModel.Drum.Drum>
    {

        public IList<ReliefProModel.Drum.Drum> GetAllList(ISession session)
        {
            IList<ReliefProModel.Drum.Drum> list = null;
            try
            {
                list = session.CreateCriteria<ReliefProModel.Drum.Drum>().List<ReliefProModel.Drum.Drum>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public ReliefProModel.Drum.Drum GetModel(ISession session)
        {
            ReliefProModel.Drum.Drum m = null;
            IList<ReliefProModel.Drum.Drum> list = null;
            try
            {
                list = session.CreateCriteria<ReliefProModel.Drum.Drum>().List<ReliefProModel.Drum.Drum>();
                if (list.Count > 0)
                {
                    m = list[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return m;
        }
    }
}
