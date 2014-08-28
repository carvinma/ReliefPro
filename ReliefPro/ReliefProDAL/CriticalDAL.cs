using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class CriticalDAL : IBaseDAL<Critical>
    {

        public Critical GetModel(ISession session)
        {
            Critical model = null;
            IList<Critical> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<Critical>().List<Critical>();
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
