using ReliefProModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProII
{
    public interface IMixCalculate
    {
        string Calculate(string fileContent, List<tbStream> streams,string product, string dir, ref int ImportResult, ref int RunResult);
    }
}
