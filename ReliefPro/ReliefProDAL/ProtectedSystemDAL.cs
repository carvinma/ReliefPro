using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class ProtectedSystemDAL : IBaseDAL<ProtectedSystem>
    {

        public ProtectedSystem GetModel(ISession session)
        {
            ProtectedSystem model = null;
            IList<ProtectedSystem> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<ProtectedSystem>().List<ProtectedSystem>();
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
