using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;

namespace ProII
{
    public interface IFlashCalculate
    {
        /// <summary> 
        /// <0 – security missing or improperly installed
        /// 1 – simulation solved with no errors or warnings
        /// 2 – simulation solved with warnings
        /// 3 – simulation solved with errors
        /// 4 – simulation not solved
        /// 5 – specified keyword file was not found
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="iFirst"></param>
        /// <param name="firstValue"></param>
        /// <param name="iSecond"></param>
        /// <param name="secondValue"></param>
        /// <param name="stream"></param>
        /// <param name="vapor"></param>
        /// <param name="liquid"></param>
        /// <param name="dir"></param>
        /// <param name="ImportResult"></param>
        /// <param name="RunResult"></param>
        /// <returns></returns>
        string Calculate(string fileContent, int iFirst, string firstValue, int iSecond, string secondValue,string heatMethod, tbStream stream, string vapor, string liquid, string dir, ref int ImportResult, ref int RunResult);
        string Calculate(string fileContent, int iFirst, string firstValue, int iSecond, string secondValue, string heatMethod, List<tbStream> streams, string vapor, string liquid, string dir, ref int ImportResult, ref int RunResult);
    }
}
