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
using ReliefProModel.HXs;

namespace ReliefProDAL.HXs
{
    public class HeatExchangerDAL : IBaseDAL<HeatExchanger>
    {

        public IList<HeatExchanger> GetAllList(ISession session)
        {
            IList<HeatExchanger> list = null;
            try
            {
                list = session.CreateCriteria<HeatExchanger>().List<HeatExchanger>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public HeatExchanger GetModel(ISession session)
        {
            HeatExchanger m = null;
            IList<HeatExchanger> list = null;
            try
            {
                list = session.CreateCriteria<HeatExchanger>().List<HeatExchanger>();
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
