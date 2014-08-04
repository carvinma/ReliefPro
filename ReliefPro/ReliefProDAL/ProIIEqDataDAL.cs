using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;

namespace ReliefProDAL
{
    public class ProIIEqDataDAL : IBaseDAL<ProIIEqData>
    {
        public ProIIEqData GetModel(ISession session, string SourceFile,string EqName,string EqType)
        {
            ProIIEqData model = null;
            IList<ProIIEqData> list = null;
            try
            {
                list = session.CreateCriteria<ProIIEqData>().Add(Expression.Eq("EqName", EqName)).Add(Expression.Eq("EqType", EqType)).Add(Expression.Eq("SourceFile", SourceFile)).List<ProIIEqData>();
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

        public ProIIEqData GetModel(ISession session, string SourceFile, string EqName)
        {
            ProIIEqData model = null;
            IList<ProIIEqData> list = null;
            try
            {
                list = session.CreateCriteria<ProIIEqData>().Add(Expression.Eq("EqName", EqName)).Add(Expression.Eq("SourceFile", SourceFile)).List<ProIIEqData>();
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

        public IList<ProIIEqData> GetAllList(ISession session,string SourceFile,string EqType)
        {
            IList<ProIIEqData> list = null;
            try
            {
                list = session.CreateCriteria<ProIIEqData>().Add(Expression.Eq("EqType", EqType)).Add(Expression.Eq("SourceFile", SourceFile)).List<ProIIEqData>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public IList<ProIIEqData> GetAllList(ISession session)
        {
            IList<ProIIEqData> list = null;
            try
            {
                list = session.CreateCriteria<ProIIEqData>().List<ProIIEqData>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

       
    }
}
