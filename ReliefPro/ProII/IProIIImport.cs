using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProII
{
    public interface IProIIImport
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inpFile"></param>
        /// <param name="ImportResult">1,2 import success,3error 5 no inp</param>
        /// <param name="RunResult">1,2 solved,3 solved error,4not solved 5 no inp</param>
        /// <returns></returns>
      string ImportProIIINP(string inpFile, ref int ImportResult, ref int RunResult);
    }
}
