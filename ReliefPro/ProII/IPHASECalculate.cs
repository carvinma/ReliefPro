using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
namespace ProII
{
    public interface IPHASECalculate
    {
        string Calculate(string fileContent, int iFirst, string firstValue, int iSecond, string secondValue, tbStream stream, string PH, string dir, ref int ImportResult, ref int RunResult);
    }
}
