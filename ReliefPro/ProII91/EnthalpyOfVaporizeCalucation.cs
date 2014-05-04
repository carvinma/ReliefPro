using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using P2Wrap91;
using System.IO;
using System.Runtime.InteropServices;
using System.Data;
using ReliefProCommon.CommonLib;

namespace ProII91
{
    public class EnthalpyOfVaporizeCalucation
    {

        public void CopyStream(string przFile,string columnName, int tray, ref CustomStream  stream)
        {
            CP2ServerClass cp2Srv = new CP2ServerClass();
            cp2Srv.Initialize();
            CP2File cp2File = (CP2File)cp2Srv.OpenDatabase(przFile);
            string streamName = "temp" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            CP2Object tempStream = (CP2Object)cp2File.CreateObject("Stream", streamName);

            bool b = cp2File.CopyTrayToStream(columnName, (short)tray, (p2Phase)2, 0, (p2TrayFlow)1, streamName);;
            bool bCalulate = cp2File.CalculateStreamProps(streamName);

            CP2Object compCalc = (CP2Object)cp2File.ActivateObject("CompCalc", "CompCalc");
            object ComponentId = compCalc.GetAttribute("ComponentId");
            stream.Componentid = ConvertExt.ObjectToString(ComponentId);
            object CompIn = cp2File.GetObjectNames("CompIn");
            stream.CompIn = ConvertExt.ObjectToString(CompIn);
            stream.StreamName = streamName;
            //dr["sourcefile"] = przFileName;
            //dr["tray"] = 1;
            //dr["prodtype"] = 2;
            //CP2Object curStream = (CP2Object)cp2File.ActivateObject("Stream", streamName);
            //foreach (string s in arrStreamAttributes)
            //{
            //    object v = curStream.GetAttribute(s);
            //    dr[s] = ConvertExt.ObjectToString(v);
            //}
            //if (bCalulate)
            //{
            //    CP2Object bulkDrop = (CP2Object)cp2File.ActivateObject("SrBulkProp", streamName);
            //    foreach (string s in arrBulkPropAttributes)
            //    {
            //        object v = bulkDrop.GetAttribute(s);
            //        dr[s] = ConvertExt.ObjectToString(v);
            //    }
            //}

            cp2File.DeleteObject("Stream", streamName);
            Marshal.FinalReleaseComObject(cp2Srv);
            GC.ReRegisterForFinalize(cp2Srv);
        }

    }

}
