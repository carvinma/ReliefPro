﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using P2Wrap91;
using System.IO;
using System.Runtime.InteropServices;
using System.Data;

namespace ProII91
{
    public class PHASECalculation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="iFirst">1:Pressure 2:Temperature</param>
        /// <param name="firstValue">表示压力或温度值</param>
        /// <param name="iSecond">1：温度 2：压力 3：泡点 4：露点 5 duty</param>
        /// <param name="secondValue">表示iSecond 的对应的值</param>
        /// <param name="stream"></param>
        /// <param name="vapor"></param>
        /// <param name="liquid"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public string Calculate(string fileContent, int iFirst, string firstValue, int iSecond, string secondValue, CustomStream stream, string dir)
        {
            CP2ServerClass cp2Srv = new CP2ServerClass();
            cp2Srv.Initialize();

            string streamData = getStreamData(iFirst, firstValue,iSecond,secondValue, stream);
            string flashData = getPHASEData(iFirst, firstValue, iSecond, secondValue, stream);
            StringBuilder sb = new StringBuilder();
            sb.Append(fileContent).Append(streamData).Append(flashData);
            string onlyFileName = dir + @"\" + Guid.NewGuid().ToString().Substring(0, 5);
            string inpFile = onlyFileName + ".inp";
            File.WriteAllText(inpFile, sb.ToString());
            int resultImport = cp2Srv.Import(inpFile);
            string przFile = onlyFileName + ".prz";
            CP2File cp2File = (CP2File)cp2Srv.OpenDatabase(przFile);
            int runResult = cp2Srv.RunCalcs(przFile);
            runResult = runResult + cp2Srv.GenerateReport(przFile);
            Marshal.FinalReleaseComObject(cp2Srv);
            GC.ReRegisterForFinalize(cp2Srv);

            return przFile;
        }
        private string getStreamData(int iFirst, string firstValue, int iSecond, string secondValue, CustomStream stream)
        {
            StringBuilder data1 = new StringBuilder();
            string streamName = stream.StreamName;
            data1.Append("\tPROP STRM=").Append(streamName.ToUpper()).Append(",&\n");
            data1.Append("\t PRESSURE(MPAG)=").Append(stream.Pressure).Append(",&\n");
            data1.Append("\t TEMPERATURE(C)=").Append(stream.Temperature).Append(",&\n");
            string rate = stream.TotalMolarRate;
            if (rate == "")
                rate = "1";
            data1.Append("\t RATE(KGM/S)=").Append(rate).Append(",&\n");
            string com = stream.TotalComposition;
            string Componentid =stream.Componentid;
            string CompIn = stream.CompIn;
            Dictionary<string, string> compdict = new Dictionary<string, string>();
            data1.Append("\t COMP=&\n");
            string[] coms = com.Split(',');
            string[] Componentids = Componentid.Split(',');
            string[] CompIns = CompIn.Split(',');
            StringBuilder sbCom = new StringBuilder();
            for (int i = 0; i < coms.Length; i++)
            {
                compdict.Add(Componentids[i], coms[i]);
            }
            foreach (string s in CompIns)
            {
                sbCom.Append("/&\n").Append(compdict[s]);
            }
            data1.Append("\t").Append(sbCom.Remove(0, 2)).Append("\n");
            return data1.ToString();
        }
        private string getPHASEData(int iFirst, string firstValue, int iSecond, string secondValue, CustomStream stream)
        {
            StringBuilder data2 = new StringBuilder("UNIT OPERATIONS\n");
            string streamName = stream.StreamName;
            string FlashName = "PH1" ;

            data2.Append("\tPHASE UID=").Append(FlashName).Append("\n");
            data2.Append("\t EVAL  STREAM=1 ").Append(streamName.ToUpper()).Append(",IPLOT=ON\n");
                        
            data2.Append("END");
            return data2.ToString();
        }
       
    }
}
