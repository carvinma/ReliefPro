using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class SourceDAL : IBaseDAL<Source>
    {
        public IList<Source> GetAllList(ISession session)
        {
            IList<Source> list = null;
            try
            {
                list = session.CreateCriteria<Source>().List<Source>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public Source GetModel(ISession session,string name)
        {
            Source model = null;
            IList<Source> list = null;
            try
            {
                list = session.CreateCriteria<Source>().Add(Expression.Eq("SourceName",name)).List<Source>();
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
        public Source GetModel(string StreamName,ISession session)
        {
            Source model = null;
            IList<Source> list = null;
            try
            {
                list = session.CreateCriteria<Source>().Add(Expression.Eq("StreamName", StreamName)).List<Source>();
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
