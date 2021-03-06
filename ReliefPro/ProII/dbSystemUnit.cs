﻿
using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
namespace ReliefProDAL
{
    public class dbSystemUnit : IBaseDAL<SystemUnit>
    {
        public IList<SystemUnit> GetAllList(ISession session)
        {
            IList<SystemUnit> list = null;
            try
            {
                list = session.CreateCriteria<SystemUnit>().List<SystemUnit>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
