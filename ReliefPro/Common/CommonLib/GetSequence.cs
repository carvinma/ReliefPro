using System;
using System.Collections.Generic;
using System.Text;
//using ReliefProCommon.DBUtility;
using System.Web.UI;
using NHibernate;

namespace ReliefProCommon
{
    #region ��ȡ����
    /// <summary>
    /// ��ȡ����ID
    /// </summary>
    public class GetSequence
    {
        /// <summary>
        /// ��ȡ����ID
        /// </summary>
        /// <param name="preTag">ǰ׺</param>
        /// <param name="seqName">��������</param>
        /// <param name="page">��ǰ�й�Page</param>
        /// <returns></returns>
        public static string getID(string preTag, string seqName, Page page)
        {
            Pcitc.Model.Session.SessionModel sessionModel = ReliefProCommon.CommonFunc.GetCurrentUser(page);
            return getSeqID(preTag, seqName, sessionModel.EnterpriseId);
        }
        /// <summary>
        /// ��ȡ����ID
        /// </summary>
        /// <param name="preTag">ǰ׺</param>
        /// <param name="seqName">��������</param>
        /// <param name="enterpriseid">��ҵID</param>
        /// <returns></returns>
        public static string getID(string preTag, string seqName, string enterpriseid)
        {
            return getSeqID(preTag, seqName, enterpriseid);
        }
        /// <summary>
        /// �����ݿ��ȡָ������ID
        /// </summary>
        /// <param name="preTag">ǰ׺</param>
        /// <param name="seqName">��������</param>
        /// <param name="enterpriseid">��ҵID</param>
        /// <returns></returns>
        private static string getSeqID(string preTag, string seqName, string enterpriseid)
        {
            //try
            //{
            //    string strsql = "SELECT get_sequence('" + preTag + "'," + enterpriseid + ",'" + seqName + "') FROM dual";
            //    //object obj = ReliefProCommon.DBUtility.DbHelperOra.GetSingle(strsql);
            //    //return obj.ToString();

            //    ISQLQuery query = NHibernateHelper.GetCurrentSession().CreateSQLQuery(strsql);
            //    return query.UniqueResult().ToString();
            //}
            //catch (Exception msg)
            //{
            //    throw new Exception(msg.Message);
            //}
            throw new  NotImplementedException();
        }
    }
    #endregion
}