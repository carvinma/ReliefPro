using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;
namespace ReliefProDAL
{
    public class CondenserCalcDAL : IBaseDAL<CondenserCalc>
    {
        public CondenserCalc GetModel(ISession session, int ScenarioID)
        {
            CondenserCalc model = null;
            IList<CondenserCalc> list = null;
            try
            {
                session.Clear();
                list = session.CreateCriteria<CondenserCalc>().List<CondenserCalc>();
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
