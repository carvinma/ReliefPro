using System;

namespace Pcitc.Model.Session
{
	/// <summary>
    /// �洢��ǰ��¼�û���Ϣ
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
        /// �ɹ���
        /// </summary>
        public string PurchaseGroupID
        {
            set { _PurchaseGroupID = value; }
            get { return _PurchaseGroupID; }
        }

        /// <summary>
        /// ����ID
        /// </summary>
        public string FactoryId
        {
            set { _FactoryId = value; }
            get { return _FactoryId; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public string FactoryName
        {
            set { _FactoryName = value; }
            get { return _FactoryName; }
        }

        /// <summary>
        /// ��˾ID����SAPһ��
        /// </summary>
        public string SAPCompanyID
        {
            set { _SAPCompanyID = value; }
            get { return _SAPCompanyID; }
        }

        /// <summary>
        /// ��˾���ƣ���SAPһ��
        /// </summary>
        public string SAPCompnayName
        {
            set { _SAPCompnayName = value; }
            get { return _SAPCompnayName; }
        }

        /// <summary>
        /// �û�ID
        /// </summary>
        public string UserID
        {
            set { _UserID = value; }
            get { return _UserID; }
        }
        /// <summary>
        /// �û�����
        /// </summary>
        public string UserName
        {
            set { _UserName = value; }
            get { return _UserName; }
        }
        
        /// <summary>
        /// ��¼����
        /// </summary>
        public string LoginName
        {
            set { _LoginName = value; }
            get { return _LoginName; }
        }
        /// <summary>
        /// ��¼����
        /// </summary>
        public string LoginPass
        {
            set { _LoginPass = value; }
            get { return _LoginPass; }
        }

        /// <summary>
        /// ��֤����
        /// </summary>
        public string Logintype
        {
            set { _Logintype = value; }
            get { return _Logintype; }
        }
        /// <summary>
        /// ��ҵ/��Ӧ�̱��
        /// </summary>
        public string EnterpriseId
        {
            set { _EnterpriseId = value; }
            get { return _EnterpriseId; }
        }


        /// <summary>
        /// ��ҵ/��Ӧ������
        /// </summary>
        public string EnterpriseName
        {
            set { _EnterpriseName = value; }
            get { return _EnterpriseName; }
        }
        /// <summary>
        /// ����ID
        /// </summary>
        public string CompanyId
        {
            set { _CompanyId = value; }
            get { return _CompanyId; }
        }
        /// <summary>
        /// ��������
        /// </summary>
        public string CompanyName
        {
            set { _CompanyName = value; }
            get { return _CompanyName; }
        }
        /// <summary>
        /// ����ID
        /// </summary>
        public string DepartmentId
        {
            set { _DepartmentId = value; }
            get { return _DepartmentId; }
        }
        /// <summary>
        /// ��������
        /// </summary>
        public string DepartmentName
        {
            set { _DepartmentName = value; }
            get { return _DepartmentName; }
        }

        /// <summary>
        /// ��ϵ��ַ
        /// </summary>
        public string UserAddr
        {
            set { _UserAddr = value; }
            get { return _UserAddr; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public string UserPostCode
        {
            set { _UserPostCode = value; }
            get { return _UserPostCode; }
        }

        /// <summary>
        /// �̶��绰
        /// </summary>
        public string FixPhone
        {
            set { _FixPhone = value; }
            get { return _FixPhone; }
        }

        /// <summary>
        /// �ֻ�
        /// </summary>
        public string CellPhone
        {
            set { _CellPhone = value; }
            get { return _CellPhone; }
        }

        /// <summary>
        /// ����
        /// </summary>
        public string Fax
        {
            set { _Fax = value; }
            get { return _Fax; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public string Email
        {
            set { _Email = value; }
            get { return _Email; }
        }

        /// <summary>
        /// ��λ���
        /// </summary>
        public string PostId
        {
            set { _PostId = value; }
            get { return _PostId; }
        }

        /// <summary>
        /// ��λ����
        /// </summary>
        public string PostName
        {
            set { _PostName = value; }
            get { return _PostName; }
        }

        /// <summary>
        /// �ɹ���--ҵ���û�����
        /// </summary>
        public string UserType
        {
            set { _UserType = value; }
            get { return _UserType; }
        }

        /// <summary>
        /// ���ñ�־
        /// </summary>
        public string IsUse
        {
            set { _IsUse = value; }
            get { return _IsUse; }
        }

        /// <summary>
        /// �û���Դ  0���ɹ����û���1����Ӧ���û�
        /// </summary>
        public string UserFrom
        {
            set { _UserFrom = value; }
            get { return _UserFrom; }
        }

        /// <summary>
        /// ���Ա��
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

