using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ReliefProDAL;
using ReliefProModel;
using UOMLib;

namespace ReliefProBLL
{
    public class SourceFileBLL
    {
        private ISession SessionPF;
        public SourceFileBLL(ISession SessionPF)
        {
            this.SessionPF = SessionPF;
        }
        public SourceFile GetSourceFileInfo(string fileName)
        {
            SourceFileDAL dal = new SourceFileDAL();
            SourceFile sf = dal.GetModel(fileName, SessionPF);
            return sf;
        }
    }
}
