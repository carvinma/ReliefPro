using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;

namespace ReliefProDAL
{
    public class dbUnit : IBaseDAL<Device>
    {
        public IList<Device> GetAllList(ISession session)
        {
            IList<Device> list = null;
            try
            {
                list = session.CreateCriteria<Device>().List<Device>();
            }
            catch (Exception ee)
            { throw ee; }
            return list;
        }
    }
}