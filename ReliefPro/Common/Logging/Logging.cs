using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProCommon.Logging
{
    public class Logging
    {
        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="ex"></param>
        public static void Error(Exception ex)
        {
            WriterExceptions.WriterToLog(ex);
        }

        /// <summary>
        /// 记录debug信息
        /// </summary>
        /// <param name="info"></param>
        public static void Debug(String info)
        {
            WriterExceptions.WriterToDebug(info);
        }

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="flagName"></param>
        public static void Error(Exception ex, String flagName)
        {
            WriterExceptions.WriterToLog(ex, flagName);
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="text"></param>
        public static void Info(string text)
        {
            WriterExceptions.Logger.Info(text);
        }
    }
}
