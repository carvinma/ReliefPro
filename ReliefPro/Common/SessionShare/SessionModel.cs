using System;

namespace Pcitc.Model.Session
{
	/// <summary>
    /// 存储当前登录用户信息
	/// </summary>
	[Serializable]
	public class SessionModel
	{
        public SessionModel()
		{}
		#region Model

        private string  _UserID;
        private string  _UserName;
        private string  _LoginName;
        private string  _LoginPass;
        private string  _Logintype;
        private string  _EnterpriseId;
        private string  _EnterpriseName;
        private string  _CompanyId;
        private string  _CompanyName;
        private string  _DepartmentId;
        private string  _DepartmentName;
        private string _UserAddr;
        private string _UserPostCode;
        private string _FixPhone;
        private string _CellPhone;
        private string _Fax;
        private string _Email;
        private string _PostId;
        private string _PostName;
        private string  _UserType;
        private string  _IsUse;
        private string  _UserFrom;
        private string _LanguageId;
        private string _PurchaseGroupID;
        private string _FactoryId;
        private string _FactoryName;
        private string _SAPCompanyID;
        private string _SAPCompnayName;


        /// <summary>
        /// 采购组
        /// </summary>
        public string PurchaseGroupID
        {
            set { _PurchaseGroupID = value; }
            get { return _PurchaseGroupID; }
        }

        /// <summary>
        /// 工厂ID
        /// </summary>
        public string FactoryId
        {
            set { _FactoryId = value; }
            get { return _FactoryId; }
        }

        /// <summary>
        /// 工厂名称
        /// </summary>
        public string FactoryName
        {
            set { _FactoryName = value; }
            get { return _FactoryName; }
        }

        /// <summary>
        /// 公司ID，与SAP一致
        /// </summary>
        public string SAPCompanyID
        {
            set { _SAPCompanyID = value; }
            get { return _SAPCompanyID; }
        }

        /// <summary>
        /// 公司名称，与SAP一致
        /// </summary>
        public string SAPCompnayName
        {
            set { _SAPCompnayName = value; }
            get { return _SAPCompnayName; }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID
        {
            set { _UserID = value; }
            get { return _UserID; }
        }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName
        {
            set { _UserName = value; }
            get { return _UserName; }
        }
        
        /// <summary>
        /// 登录名称
        /// </summary>
        public string LoginName
        {
            set { _LoginName = value; }
            get { return _LoginName; }
        }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPass
        {
            set { _LoginPass = value; }
            get { return _LoginPass; }
        }

        /// <summary>
        /// 验证类型
        /// </summary>
        public string Logintype
        {
            set { _Logintype = value; }
            get { return _Logintype; }
        }
        /// <summary>
        /// 企业/供应商编号
        /// </summary>
        public string EnterpriseId
        {
            set { _EnterpriseId = value; }
            get { return _EnterpriseId; }
        }


        /// <summary>
        /// 企业/供应商名称
        /// </summary>
        public string EnterpriseName
        {
            set { _EnterpriseName = value; }
            get { return _EnterpriseName; }
        }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string CompanyId
        {
            set { _CompanyId = value; }
            get { return _CompanyId; }
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string CompanyName
        {
            set { _CompanyName = value; }
            get { return _CompanyName; }
        }
        /// <summary>
        /// 科室ID
        /// </summary>
        public string DepartmentId
        {
            set { _DepartmentId = value; }
            get { return _DepartmentId; }
        }
        /// <summary>
        /// 科室名称
        /// </summary>
        public string DepartmentName
        {
            set { _DepartmentName = value; }
            get { return _DepartmentName; }
        }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string UserAddr
        {
            set { _UserAddr = value; }
            get { return _UserAddr; }
        }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string UserPostCode
        {
            set { _UserPostCode = value; }
            get { return _UserPostCode; }
        }

        /// <summary>
        /// 固定电话
        /// </summary>
        public string FixPhone
        {
            set { _FixPhone = value; }
            get { return _FixPhone; }
        }

        /// <summary>
        /// 手机
        /// </summary>
        public string CellPhone
        {
            set { _CellPhone = value; }
            get { return _CellPhone; }
        }

        /// <summary>
        /// 传真
        /// </summary>
        public string Fax
        {
            set { _Fax = value; }
            get { return _Fax; }
        }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email
        {
            set { _Email = value; }
            get { return _Email; }
        }

        /// <summary>
        /// 岗位编号
        /// </summary>
        public string PostId
        {
            set { _PostId = value; }
            get { return _PostId; }
        }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string PostName
        {
            set { _PostName = value; }
            get { return _PostName; }
        }

        /// <summary>
        /// 采购商--业务用户类型
        /// </summary>
        public string UserType
        {
            set { _UserType = value; }
            get { return _UserType; }
        }

        /// <summary>
        /// 启用标志
        /// </summary>
        public string IsUse
        {
            set { _IsUse = value; }
            get { return _IsUse; }
        }

        /// <summary>
        /// 用户来源  0：采购商用户；1：供应商用户
        /// </summary>
        public string UserFrom
        {
            set { _UserFrom = value; }
            get { return _UserFrom; }
        }

        /// <summary>
        /// 语言编号
        /// </summary>
        public string LanguageId
        {
            set { _LanguageId = value; }
            get { return _LanguageId; }
        }
		
		#endregion Model

        public override string ToString()
        {
            string str = string.Format("UserId: {0}, DepartmentId: {1}, CompanyId: {2}", this.UserID, this.DepartmentId, this.CompanyId);
            return str;
        }
	}
}

