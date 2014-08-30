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
            session.Flush();
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

        public IList<FlareSystem> GetFlareSystem(ISession session)
        {
            session.Clear();
            return session.CreateCriteria<FlareSystem>().List<FlareSystem>();
        }
        public ConditionsSettings GetConditionsSettings(ISession session)
        {
            session.Clear();
            var lstSettings = session.CreateCriteria<ConditionsSettings>().List<ConditionsSettings>();
            if (lstSettings.Count > 0)
                return lstSettings[0];
            return null;
        }
    }
}
