using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProCommon
{
    public class RegularExpressionPatterns
    {
        /// <summary>
        /// 数字格式，xxx.xxx或者xxx
        /// </summary>
        public static string NumberOnly
        {
            get
            {
                string pattern = @"(\d+)(\.\d+)?";
                return pattern;
            }
        }

        /// <summary>
        /// 32位的guid pattern
        /// 无分隔符和括号
        /// </summary>
        public static string Guid32Digits
        {
            get
            {
                string pattern = @"^(\{){0,1}[0-9a-fA-F]{8}[0-9a-fA-F]{4}[0-9a-fA-F]{4}[0-9a-fA-F]{4}[0-9a-fA-F]{12}(\}){0,1}$";
                return pattern;
            }
        }
    }
}
