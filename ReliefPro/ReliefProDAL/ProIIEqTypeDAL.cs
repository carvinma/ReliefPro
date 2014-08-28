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
    public class ProIIEqTypeDAL : IBaseDAL<ProIIEqType>
    {
        public IList<ProIIEqType> GetAllList(ISession session)
        {
            IList<ProIIEqType> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<ProIIEqType>().List<ProIIEqType>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
