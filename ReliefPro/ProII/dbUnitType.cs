
using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
namespace ReliefProDAL
{
    public class dbUnitType : IBaseDAL<UnitType>
    {
        public IList<UnitType> GetAllList(ISession session)
        {
            IList<UnitType> list = null;
            try
            {
                list = session.CreateCriteria<UnitType>().List<UnitType>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
