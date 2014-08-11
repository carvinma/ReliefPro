using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProII
{
    public interface IProIIImport
    {
      string ImportProIIINP(string inpFile, out int ImportResult, out int RunResult);
    }
}
