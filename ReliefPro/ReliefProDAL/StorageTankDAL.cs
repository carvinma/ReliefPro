using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class StorageTankDAL : IBaseDAL<StorageTank>
    {

        public StorageTank GetModel(ISession session)
        {
            StorageTank model = null;
            IList<StorageTank> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<StorageTank>().List<StorageTank>();
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
