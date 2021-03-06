﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ProII;
using P2Wrap91;

namespace ProII91
{
    public class ProIIImport : IProIIImport
    {
        public string ImportProIIINP(string inpFile, ref int ImportResult, ref int RunResult)
        {
            ImportResult = -1;
            RunResult = -1;
            string przFile = string.Empty;
            CP2ServerClass cp2Srv = new CP2ServerClass();
            cp2Srv.Initialize();
            ImportResult = cp2Srv.Import(inpFile);
            if (ImportResult == 1 || ImportResult == 2)
            {
                string path = inpFile.Substring(0, inpFile.Length - 4);
                przFile = path + ".prz";
                CP2File cp2File = (CP2File)cp2Srv.OpenDatabase(przFile);
                RunResult = cp2Srv.RunCalcs(przFile);
            }
            Marshal.FinalReleaseComObject(cp2Srv);
            GC.ReRegisterForFinalize(cp2Srv);

            return przFile;
        }
        public int CheckProIISolved(string przFile)
        {
            int RunResult = -1;
            CP2ServerClass cp2Srv = new CP2ServerClass();
            cp2Srv.Initialize();
            CP2File cp2File = (CP2File)cp2Srv.OpenDatabase(przFile);
            RunResult = cp2Srv.RunCalcs(przFile);
            Marshal.FinalReleaseComObject(cp2Srv);
            GC.ReRegisterForFinalize(cp2Srv);
            return RunResult;
        }
    }
}
