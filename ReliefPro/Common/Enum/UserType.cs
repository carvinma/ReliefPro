using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Enum
{
    public enum UserType
    {
        /// <summary>
        /// 采购商
        /// </summary>
        Purchase = 0,

        /// <summary>
        /// 供应商
        /// </summary>
        Supply = 1,
        /// <summary>
        /// 首页
        /// </summary>
        SY = -1,
        /// <summary>
        /// 供应商用户
        /// </summary>
        GYSYH = 2,
        /// <summary>
        /// 企业内部
        /// </summary>
        QYNB = 3,
    }

    /// <summary>
    /// 采购单位的用户信息
    /// 对应 EC_DEPARTMENT_VIEW 的 codetype
    /// </summary>
    public enum UserRank
    {
        /// <summary>
        /// 总部
        /// </summary>
        HQ = 1,
        /// <summary>
        /// 大区
        /// </summary>
        Region = 2,
        /// <summary>
        /// 省市公司
        /// </summary>
        City = 3
    }

    public enum NoticeUserType
    {
        SY = -1,
        GYSYH
    }

}
