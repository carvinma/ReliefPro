using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using P2Wrap93;
using System.IO;
using System.Runtime.InteropServices;
using System.Data;
using ProII;
namespace ProII93
{
    public class MixCalculate : IMixCalculate
    {
       
        private string getMixData(List<CustomStream> streams,string product)
        {
            StringBuilder data2 = new StringBuilder("UNIT OPERATIONS\n");
            StringBuilder sb = new StringBuilder();
            foreach (CustomStream cs in streams)
            {
                sb.Append(cs.StreamName).Append(",");
            }
            string feeds = sb.ToString().Substring(0, sb.Length - 1);
            Guid guid = Guid.NewGuid();
            string MixName = "Mix_1";

            data2.Append("\tMIXER UID=").Append(MixName).Append("\n");
            data2.Append("\t FEED ").Append(feeds.ToUpper()).Append("\n");
            data2.Append("\t PRODUCT M=").Append(product).Append("\n");
   
            data2.Append("END");
            return data2.ToString();
        }

        public string Calculate(string fileContent,  List<CustomStream> streams,string product,  string dir, ref int ImportResult, ref int RunResult)
        {
            StringBuilder sb = new StringBuilder();
            string[] arrfileContent = fileContent.Split(new string[] { "STREAM DATA" }, StringSplitOptions.None);

            string flashData = getMixData(streams, product);
            
            sb.Append(arrfileContent[0]).Append("\nSTREAM DATA\n");
            foreach(CustomStream stream in streams)
            {
                string streamData = getStreamData(stream);
                sb.Append(streamData).Append("\n");;
            }

            sb.Append(arrfileContent[1]).Append(flashData);
            string onlyFileName = dir + @"\a" ;
            string inpFile = onlyFileName + ".inp";
            File.WriteAllText(inpFile, sb.ToString());
            CP2ServerClass cp2Srv = new CP2ServerClass();
            cp2Srv.Initialize();
            ImportResult = cp2Srv.Import(inpFile);
            string przFile = string.Empty;
            if (ImportResult == 1 || ImportResult == 2)
            {
                przFile = onlyFileName + ".prz";
                CP2File cp2File = (CP2File)cp2Srv.OpenDatabase(przFile);
                RunResult = cp2Srv.RunCalcs(przFile);
            }
            Marshal.FinalReleaseComObject(cp2Srv);
            GC.ReRegisterForFinalize(cp2Srv);

            return przFile;
        }

        private string getStreamData(CustomStream stream)
        {
            StringBuilder data1 = new StringBuilder();
            string streamName = stream.StreamName;
            data1.Append("\tPROPERTY STREAM=").Append(streamName.ToUpper()).Append(",&\n");
            data1.Append("\t PRESSURE(MPAG)=").Append(stream.Pressure).Append(",&\n");
            data1.Append("\t TEMPERATURE(C)=").Append(stream.Temperature).Append(",&\n");
            double rate = stream.TotalMolarRate;
            if (rate == 0)
                rate = 1e-8;
            data1.Append("\t RATE(KGM/S)=").Append(rate).Append(",&\n");
            string com = stream.TotalComposition;
            string Componentid = stream.Componentid;
            string CompIn = stream.CompIn;
            string PrintNumber = stream.PrintNumber;
            Dictionary<string, string> compdict = new Dictionary<string, string>();
            data1.Append("\t COMP=&\n");
            string[] coms = com.Split(',');
            string[] PrintNumbers = PrintNumber.Split(',');
            StringBuilder sbCom = new StringBuilder();
            string[] Componentids = Componentid.Split(',');
            string[] CompIns = CompIn.Split(',');
            int comCount = coms.Length;
            for (int i = 0; i < comCount; i++)
            {
                compdict.Add(Componentids[i], coms[i]);
            }
            for (int i = 0; i < comCount; i++)
            {
                //string s = CompIns[i];
                sbCom.Append("/&\n").Append(PrintNumbers[i]).Append(",").Append(coms[i]);
            }
            data1.Append("\t").Append(sbCom.Remove(0, 2)).Append("\n");
            return data1.ToString();
        }
    }
}
