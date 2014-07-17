using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class TreePSDAL : IBaseDAL<TreePS>
    {

        public TreePS GetModel(ISession session)
        {
            TreePS model = null;
            IList<TreePS> list = null;
            try
            {
                list = session.CreateCriteria<TreePS>().List<TreePS>();
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
