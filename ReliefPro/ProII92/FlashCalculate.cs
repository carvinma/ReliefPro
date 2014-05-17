﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using P2Wrap92;
using System.IO;
using System.Runtime.InteropServices;
using System.Data;
using ProII;
namespace ProII92
{
    public class FlashCalculate:IFlashCalculate
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
        public string Calculate(string fileContent, int iFirst, string firstValue, int iSecond, string secondValue, CustomStream stream, string vapor, string liquid, string dir)
        {
            CP2ServerClass cp2Srv = new CP2ServerClass();
            cp2Srv.Initialize();

            string streamData = getStreamData(iFirst, firstValue,iSecond,secondValue, stream);
            string flashData = getFlashData(iFirst, firstValue, iSecond, secondValue, stream, vapor, liquid);
            StringBuilder sb = new StringBuilder();
            string[] arrfileContent = fileContent.Split(new string[]{"STREAM DATA"},StringSplitOptions.None);
            sb.Append(arrfileContent[0]).Append("\nSTREAM DATA\n").Append(streamData).Append(arrfileContent[1]).Append(flashData);
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
        private string getFlashData(int iFirst, string firstValue, int iSecond, string secondValue, CustomStream stream, string vapor, string liquid)
        {
            StringBuilder data2 = new StringBuilder("UNIT OPERATIONS\n");
            string streamName = stream.StreamName;
            Guid guid = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            string FlashName = "F_" + guid.ToString().Substring(0, 5).ToUpper();
            
            data2.Append("\tFLASH UID=").Append(FlashName).Append("\n");
            data2.Append("\t FEED ").Append(streamName.ToUpper()).Append("\n");
            data2.Append("\t PRODUCT V=").Append(vapor).Append(",&\n");
            data2.Append("\t L=").Append(liquid).Append("\n");

            StringBuilder sbPT = new StringBuilder();
            if (iFirst == 1)
                sbPT.Append("PRESSURE(MPAG)=").Append(firstValue).Append("\n");
            else
                sbPT.Append("TEMPERATURE(C)=").Append(firstValue).Append("\n");
            switch (iSecond)
            {
                case 1:
                    data2.Append("\t ISO PRESSURE(MPAG)=").Append(secondValue).Append(",");
                    data2.Append(sbPT.ToString());
                    break;
                case 2:
                    data2.Append("\t ISO TEMPERATURE(C)=").Append(secondValue).Append(",");
                    data2.Append(sbPT.ToString());
                    break;
                case 3:                    
                    data2.Append("\t Bubble ");
                    data2.Append(sbPT.ToString());
                    data2.Append("\t DEFINE ERAT AS 1\n");
                    break;
                case 4:                  
                    data2.Append("\t Dew ");
                    data2.Append(sbPT.ToString());
                    data2.Append("\t DEFINE ERAT AS 1\n");
                    break;
                case 5:                    
                    data2.Append("\t ADIABATIC Duty=").Append(secondValue).Append(",");
                    data2.Append(sbPT.ToString());
                    break;
                case 6:
                    data2.Append("\t TPSPEC ");
                    data2.Append(sbPT.ToString());
                    data2.Append("\t SPEC STREAM=").Append(vapor).Append(",RATE(KGM/H),TOTAL,WET, DIVIDE, REFFEED,\n");
                    data2.Append("RATE(KGM/H),WET, VALUE=").Append(secondValue).Append(",&\n");
                    break;

                default:
                    break;

            }
            
            data2.Append("END");
            return data2.ToString();
        }
       
    }
}