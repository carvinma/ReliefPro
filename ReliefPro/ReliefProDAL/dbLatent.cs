using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class dbLatent : IBaseDAL<Latent>
    {
       
        public Latent GetModel(ISession session)
        {
            Latent model = null;
            IList<Latent> list = null;
            try
            {
                list = session.CreateCriteria<Latent>().List<Latent>();
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
