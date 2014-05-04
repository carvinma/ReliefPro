using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProCommon.Enum
{
    /// <summary>
    /// 合同行数据来源
    /// </summary>
    public enum ContractSourceType
    {
        /// <summary>
        /// 正常采购
        /// </summary>
        Default = 0,
        /// <summary>
        /// 合并
        /// </summary>
        Merge = 1,
        /// <summary>
        /// 赠送
        /// </summary>
        Gift = 2,
        /// <summary>
        /// 自动合并
        /// </summary>
        AutoMerge = 3
    }
}
