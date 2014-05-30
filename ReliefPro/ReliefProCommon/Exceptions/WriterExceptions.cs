using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Common.Exceptions
{
    /// <summary>
    /// add by st 2013/6/21
    /// 采用Log4net 记录日志
    /// </summary>
   public class WriterExceptions
    {

       private static ILog logger = LogManager.GetLogger("pec");

       public static ILog Logger
       {
           get { return logger; }
       }



       /// <summary>
       /// 记录错误信息
       /// </summary>
       /// <param name="ex"></param>
       public static void WriterToLog(Exception ex)
       {
           String flagName = "未处理异常";

           logger.Error(flagName, ex);
           Exception currentEx = ex;
           while (currentEx != null)
           {
               currentEx = currentEx.InnerException;
               logger.Error(flagName, currentEx);
               
           }
       }

       /// <summary>
       /// 记录debug信息
       /// </summary>
       /// <param name="info"></param>
       public static void WriterToDebug(String info) {
           logger.Debug(info);
       }

       /// <summary>
       /// 记录错误信息
       /// </summary>
       /// <param name="ex"></param>
       /// <param name="flagName"></param>
       public static void WriterToLog(Exception ex,String flagName){
           logger.Error(flagName, ex);
           Exception currentEx = ex;
           while (currentEx != null)
           {
               currentEx = currentEx.InnerException;
               logger.Error(flagName, currentEx);
           }
       }

    }
}
