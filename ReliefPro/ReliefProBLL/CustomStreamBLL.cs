using NHibernate;
using ReliefProDAL;
using ReliefProModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ReliefProBLL
{
    public class CustomStreamBLL
    {
        private ISession SessionPF;
        private ISession SessionPS;
        public CustomStreamBLL(ISession SessionPF, ISession SessionPS)
        {
            this.SessionPF = SessionPF;
            this.SessionPS = SessionPS;
        }
        public ObservableCollection<CustomStream> GetStreams(ISession Session, bool IsProduct)
        {
            ObservableCollection<CustomStream> list = new ObservableCollection<CustomStream>();
            CustomStreamDAL db = new CustomStreamDAL();
            IList<CustomStream> lt = db.GetAllList(Session, IsProduct);
            foreach (CustomStream c in lt)
            {
                list.Add(c);
            }

            return list;
        }
    }
}
