using System;
using System.Collections.Generic;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class SourceFileDAL : IBaseDAL<SourceFile>
    {
        public IList<SourceFile> GetAllList(ISession session)
        {
            IList<SourceFile> list = null;
            try
            {
                list = session.CreateCriteria<SourceFile>().List<SourceFile>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public SourceFile GetModel(string FileName, ISession session)
        {
            SourceFile model = null;
            IList<SourceFile> list = null;
            try
            {
                list = session.CreateCriteria<SourceFile>().Add(Expression.Eq("FileName", FileName)).List<SourceFile>();
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
