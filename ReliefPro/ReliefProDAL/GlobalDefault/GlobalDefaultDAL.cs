using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProModel.GlobalDefault;

namespace ReliefProDAL.GlobalDefault
{
    public class GlobalDefaultDAL
    {
        public void DelFlareSystemByID(int id, ISession session)
        {
            var t = session.Get<FlareSystem>(id);
            session.Delete(t);
        }
        public void SaveGlobalDefault(ISession session, List<FlareSystem> lstFlarem, ConditionsSettings conditionsSettings)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {
                    session.SaveOrUpdate(conditionsSettings);
                    foreach (var detail in lstFlarem)
                    {
                        session.SaveOrUpdate(detail);
                    }
                    session.Flush();
                    tx.Commit();
                }
                catch (HibernateException hx)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
}
