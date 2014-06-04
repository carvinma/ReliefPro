using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class ReboilerPinchDAL : IBaseDAL<ReboilerPinch>
    {
        
        public ReboilerPinch GetModel(ISession session, int TowerScenarioHXID)
        {
            ReboilerPinch model = null;
            IList<ReboilerPinch> list = null;
            try
            {
                list = session.CreateCriteria<ReboilerPinch>().Add(Expression.Eq("TowerScenarioHXID", TowerScenarioHXID)).List<ReboilerPinch>();
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
