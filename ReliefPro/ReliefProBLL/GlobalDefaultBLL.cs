using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL.GlobalDefault;
using ReliefProModel.GlobalDefault;

namespace ReliefProLL
{
    public class GlobalDefaultBLL
    {
        private GlobalDefaultDAL globalDefaultDAL = new GlobalDefaultDAL();
        public void DelFlareSystemByID(int id, ISession SessionPS)
        {
            globalDefaultDAL.DelFlareSystemByID(id, SessionPS);
        }
        public void Save(ISession SessionPS, List<FlareSystem> lstFlarem, ConditionsSettings conditionsSettings)
        {
            globalDefaultDAL.SaveGlobalDefault(SessionPS, lstFlarem, conditionsSettings);
        }
        public IList<FlareSystem> GetFlareSystem(ISession SessionPS)
        {
            return globalDefaultDAL.GetFlareSystem(SessionPS);
        }
        public ConditionsSettings GetConditionsSettings(ISession SessionPS)
        {
            return globalDefaultDAL.GetConditionsSettings(SessionPS);
        }
    }
}
