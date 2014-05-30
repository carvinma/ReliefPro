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
using ReliefProModel.Drums;

namespace ReliefProDAL.Drums
{
    public class DrumDAL : IBaseDAL<Drum>
    {

        public IList<Drum> GetAllList(ISession session)
        {
            IList<Drum> list = null;
            try
            {
                list = session.CreateCriteria<Drum>().List<Drum>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public Drum GetModel(ISession session)
        {
            Drum m = null;
            IList<Drum> list = null;
            try
            {
                list = session.CreateCriteria<Drum>().List<Drum>();
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
