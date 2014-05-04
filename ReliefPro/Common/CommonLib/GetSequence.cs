using System;
using System.Collections.Generic;
using System.Text;
//using ReliefProCommon.DBUtility;
using System.Web.UI;
using NHibernate;

namespace ReliefProCommon
{
    #region 获取序列
    /// <summary>
    /// 获取序列ID
    /// </summary>
    public class GetSequence
    {
        /// <summary>
        /// 获取序列ID
        /// </summary>
        /// <param name="preTag">前缀</param>
        /// <param name="seqName">序列名称</param>
        /// <param name="page">当前托管Page</param>
        /// <returns></returns>
        public static string getID(string preTag, string seqName, Page page)
        {
            Pcitc.Model.Session.SessionModel sessionModel = ReliefProCommon.CommonFunc.GetCurrentUser(page);
            return getSeqID(preTag, seqName, sessionModel.EnterpriseId);
        }
        /// <summary>
        /// 获取序列ID
        /// </summary>
        /// <param name="preTag">前缀</param>
        /// <param name="seqName">序列名称</param>
        /// <param name="enterpriseid">企业ID</param>
        /// <returns></returns>
        public static string getID(string preTag, string seqName, string enterpriseid)
        {
            return getSeqID(preTag, seqName, enterpriseid);
        }
        /// <summary>
        /// 从数据库获取指定序列ID
        /// </summary>
        /// <param name="preTag">前缀</param>
        /// <param name="seqName">序列名称</param>
        /// <param name="enterpriseid">企业ID</param>
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