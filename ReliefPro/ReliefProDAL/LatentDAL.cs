using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class LatentDAL : IBaseDAL<Latent>
    {

        public Latent GetModel(ISession session,int latentType)
        {
            Latent model = null;
            IList<Latent> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<Latent>().Add(Expression.Eq("LatentType", latentType)).List<Latent>();
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

        public void RemoveALL(ISession session)
        {
            IList<Latent> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<Latent>().List<Latent>();
                foreach (Latent m in list)
                {
                    session.Delete(m);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
