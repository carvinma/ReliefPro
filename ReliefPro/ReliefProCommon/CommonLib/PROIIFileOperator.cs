using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpCompress;
using SharpCompress.Reader;
using SharpCompress.Common;

namespace ReliefProCommon.CommonLib
{
    public static class PROIIFileOperator
    {        
        public static bool DecompressProIIFile(string przFile, string decompressDir)
        {
            bool result = false;
            StringBuilder sb = new StringBuilder();
            using (Stream stream = File.OpenRead(przFile))
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    reader.WriteEntryToDirectory(decompressDir, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                }
                result = true;
            }
            return result;
        }

        public static string CheckProIIVersion(string inpFile)
        {
            string version = string.Empty;
            using (var file = System.IO.File.OpenText(inpFile))
            {
                string firstLineText = file.ReadLine();
                if (firstLineText.Contains("9.1"))
                {
                    version = "9.1";
                }
                else if (firstLineText.Contains("9.2"))
                {
                    version = "9.2";
                }
                else if (firstLineText.Contains("9.3"))
                {
                    version = "9.3";
                }
            }
            return version;
        }

        public static string CheckProIIhs2Version(string hs2File)
        {
            string version = string.Empty;
            using (var file = System.IO.File.OpenText(hs2File))
            {
                string firstLineText = file.ReadLine();
                string secondLineText = file.ReadLine();
                string thirdLineText = file.ReadLine();
                if (thirdLineText.Contains("VERSION 9.1"))
                {
                    version = "9.1";
                }
                else if (thirdLineText.Contains("VERSION 9.2"))
                {
                    version = "9.2";
                }
                else if (thirdLineText.Contains("VERSION 9.3"))
                {
                    version = "9.3";
                }
            }
            return version;
        }

        public static string getUsableContent(string streamName, string rootDir)
        {
            string PropStream = "PROPERTY STREAM=" + streamName.ToUpper();
            string[] keys = { "srk", "srkh", "srkm", "srkp", "srks", "pr", "prh", "prm", "prp" };
            StringBuilder sb = new StringBuilder();
            string[] files = Directory.GetFiles(rootDir, "*.inp");
            string sourceFile = files[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            List<int> list = new List<int>();
            int i = 0;
            while (i < lines.Length)
            {
                string s = lines[i];
                if (s.Trim().IndexOf("SEQUENCE") > -1)
                {
                    while (lines[i].Contains("&"))
                    {
                        i = i + 1;
                    }
                    if (!lines[i].Contains("&")) //表示该SEQUENCE最后一行
                    {
                        i = i + 1;
                    }
                }
                else if (s.Trim().IndexOf("OUTPUT") > -1)
                {
                    while (lines[i].Contains("&"))
                    {
                        i = i + 1;
                    }
                    if (!lines[i].Contains("&")) //表示该output最后一行
                    {
                        i = i + 1;
                    }
                }
                else if (s.Trim().ToUpper().IndexOf(PropStream) > -1)
                {
                    while (lines[i].Contains("&"))
                    {
                        i = i + 1;
                    }
                    if (!lines[i].Contains("&"))
                    {
                        i = i + 1;
                    }

                }
                else if (s.Trim().IndexOf("NAME") == 0 || s.Trim().IndexOf("UNIT") == 0)
                {
                    break;
                }

                else if (s.IndexOf("REFSTREAM") > -1)
                {
                    int idx = s.IndexOf("REFSTREAM");

                    string subS = s.Substring(idx);
                    int spitIdx = subS.IndexOf(",");
                    if (spitIdx > -1)
                    {
                        string old = subS.Substring(0, spitIdx);
                        s = s.Replace(old, "REFSTREAM=" + streamName);
                    }
                    else
                    {
                        s = s.Replace(subS, "REFSTREAM=" + streamName);
                    }
                    sb.Append(s).Append("\n");

                    i++;
                }
                else
                {
                    sb.Append(s).Append("\n");
                    i++;
                }


            }


            return sb.ToString();
        }

        public static string getUsableContent(List<string>streamNames, string rootDir)
        {
            
            string[] keys = { "srk", "srkh", "srkm", "srkp", "srks", "pr", "prh", "prm", "prp" };
            StringBuilder sb = new StringBuilder();
            string[] files = Directory.GetFiles(rootDir, "*.inp");
            string sourceFile = files[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            List<int> list = new List<int>();
            int i = 0;
            while (i < lines.Length)
            {
                string s = lines[i];
                if (s.Trim().IndexOf("SEQUENCE") > -1)
                {
                    while (lines[i].Contains("&"))
                    {
                        i = i + 1;
                    }
                    if (!lines[i].Contains("&")) //表示该SEQUENCE最后一行
                    {
                        i = i + 1;
                    }
                }
                else if (s.Trim().IndexOf("OUTPUT") > -1)
                {
                    while (lines[i].Contains("&"))
                    {
                        i = i + 1;
                    }
                    if (!lines[i].Contains("&")) //表示该output最后一行
                    {
                        i = i + 1;
                    }
                }
                else if (checkUsableStreamName(s,streamNames))
                {
                    while (lines[i].Contains("&"))
                    {
                        i = i + 1;
                    }
                    if (!lines[i].Contains("&"))
                    {
                        i = i + 1;
                    }

                }
                else if (s.Trim().IndexOf("NAME") == 0 || s.Trim().IndexOf("UNIT") == 0)
                {
                    break;
                }

                else if (s.IndexOf("REFSTREAM") > -1)
                {
                    int idx = s.IndexOf("REFSTREAM");

                    string subS = s.Substring(idx);
                    int spitIdx = subS.IndexOf(",");
                    if (spitIdx > -1)
                    {
                        string old = subS.Substring(0, spitIdx);
                        s = s.Replace(old, "REFSTREAM=" + streamNames[0]);
                    }
                    else
                    {
                        s = s.Replace(subS, "REFSTREAM=" + streamNames[0]);
                    }
                    sb.Append(s).Append("\n");

                    i++;
                }
                else
                {
                    sb.Append(s).Append("\n");
                    i++;
                }


            }


            return sb.ToString();
        }

        public static bool checkUsableStreamName(string line, List<string> streamNames)
        {
            bool b = false;
            foreach (string streamName in streamNames)
            {
                string PropStream = "PROPERTY STREAM=" + streamName.ToUpper();
                if (line.Trim().ToUpper().IndexOf(PropStream) > -1)
                {
                    b = true;
                    break;
                }
            }
            return b;
        }
        public static string getUsablePhaseContent(string streamName, string rootDir)
        {
            string PropStream = "PROPERTY STREAM=" + streamName.ToUpper();
            string[] keys = { "srk", "srkh", "srkm", "srkp", "srks", "pr", "prh", "prm", "prp" };
            StringBuilder sb = new StringBuilder();
            string[] files = Directory.GetFiles(rootDir, "*.inp");
            string sourceFile = files[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            List<int> list = new List<int>();
            int i = 0;
            while (i < lines.Length)
            {
                string s = lines[i];
                if (s.Trim().IndexOf("METHOD SYSTEM=") > -1)
                {
                    int start = s.IndexOf("=");
                    int end = s.IndexOf(",");
                    string key = s.Substring(start + 1, end - start - 1);
                    if (!keys.Contains(key.ToLower()))
                    {
                        string result = s.Replace("METHOD SYSTEM=" + key, "METHOD SYSTEM=SRK");
                        sb.Append(result).Append("\r\n");

                        if (s.Contains("&"))
                        {
                            i++;
                            s = lines[i];
                            while (s.Contains("&"))
                            {
                                sb.Append(s).Append("\r\n");
                                i++;
                                s = lines[i];
                                
                            }
                            sb.Append(s).Append("\r\n");
                        }
                        
                        ////////更改方法和名字
                        //////int st = s.IndexOf("SET=");
                        //////while (st == -1)
                        //////{
                        //////    sb.Append(s).Append("\n");
                        //////    i++;
                        //////    s = lines[i];
                        //////    st = s.IndexOf("SET=");
                        //////}
                        //////string methodName = s.Substring(st);
                        //////int ed = methodName.IndexOf(",");

                        //////string mName = s.Substring(st);
                        //////if (ed != -1)
                        //////{
                        //////    mName = methodName.Substring(0, ed);
                        //////}

                        //////string result = "METHOD SYSTEM=SRK," + mName;
                        //////sb.Append(result).Append("\n");
                    }
                    else
                        sb.Append(s).Append("\r\n");
                    i++;

                    s = lines[i];

                    while (s.Trim().IndexOf("METHOD SYSTEM=") == -1 && s.Trim().IndexOf("STREAM DATA") == -1)
                    {
                        i++;
                        s = lines[i];
                    }
                }
                else if (s.Trim().IndexOf("SEQUENCE") > -1)
                {
                    while (lines[i].Contains("&"))
                    {
                        i = i + 1;
                    }
                    if (!lines[i].Contains("&")) //表示该SEQUENCE最后一行
                    {
                        i = i + 1;
                    }
                }
                else if (s.Trim().IndexOf("OUTPUT") > -1)
                {
                    while (lines[i].Contains("&"))
                    {
                        i = i + 1;
                    }
                    if (!lines[i].Contains("&")) //表示该output最后一行
                    {
                        i = i + 1;
                    }
                }
                else if (s.Trim().ToUpper().IndexOf(PropStream) > -1)
                {
                    while (lines[i].Contains("&"))
                    {
                        i = i + 1;
                    }
                    if (!lines[i].Contains("&"))
                    {
                        i = i + 1;
                    }

                }
                else if (s.Trim().IndexOf("NAME") == 0 || s.Trim().IndexOf("UNIT") == 0)
                {
                    break;
                }

                else if (s.IndexOf("REFSTREAM") > -1)
                {
                    int idx = s.IndexOf("REFSTREAM");

                    string subS = s.Substring(idx);
                    int spitIdx = subS.IndexOf(",");
                    if (spitIdx > -1)
                    {
                        string old = subS.Substring(0, spitIdx);
                        s = s.Replace(old, "REFSTREAM=" + streamName);
                    }
                    else
                    {
                        s = s.Replace(subS, "REFSTREAM=" + streamName);
                    }
                    sb.Append(s).Append("\r\n");

                    i++;
                }
                else
                {
                    sb.Append(s).Append("\r\n");
                    i++;
                }


            }


            return sb.ToString();
        }

        public static InpPosInfo GetHxPosInfo(string[] lines, string hxName, string rewriteAttr, string rewriteValue)
        {
            if (string.IsNullOrEmpty(hxName))
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            List<string> list = new List<string>();
            InpPosInfo spi = new InpPosInfo();
            for (int i = 2; i < lines.Length; i++)
            {
                string line = lines[i];
                string key = "HX   UID=" + hxName.ToUpper();
                if ((line + ",").Contains(key + ","))
                {
                    spi.start = i;
                    break;
                }
            }

            bool b = false;
            string attrvalue = string.Empty;
            for (int i = spi.start; i < lines.Length; i++)
            {
                string line = lines[i];
                string key1 = "UID=";
                string key2 = "END";
                string key = "UID=" + hxName.ToUpper();
                if ((line.Contains(key1) || line.Contains(key2)) && !line.Contains(key))
                {
                    spi.end = i - 1;
                    if (!b)
                    {
                        attrvalue = "\tOPER " + rewriteAttr + rewriteValue;
                        list.Add(attrvalue);
                    }
                    break;
                }
                else if (line.Contains("OPER "))
                {
                    b = true;
                    attrvalue = "\tOPER " + rewriteAttr + rewriteValue;
                    list.Add(attrvalue);
                    spi.end = i;
                    break;
                }
                else if (line.Contains("CONFIGURE COUNTER"))
                {
                    list.Add("\tCONFIGURE COUNTER");
                }
                else
                {
                    list.Add(line);
                }
            }


            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(list[i]).Append("\r\n");
            }

            spi.Name = hxName;
            spi.NewInfo = sb.ToString();
            return spi;
        }

        public static InpPosInfo GetStreamPosInfo(string[] lines, string streamName, string attr, string rewriteAttr, string rewriteValue)
        {
            if (double.Parse(rewriteValue) == 0)
            {
                rewriteValue = "1e-8";
            }
            string attr1 = attr.ToUpper();
            string attr2 = "";
            if (attr.Contains("RATE("))
            {
                attr1 = "RATE(WT)=";
                attr2 = "RATE(KGM/S)=";
            }

            if (string.IsNullOrEmpty(streamName))
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            List<string> list = new List<string>();
            InpPosInfo spi = new InpPosInfo();
            for (int i = 2; i < lines.Length; i++)
            {
                string line = lines[i];
                string key = "PROPERTY STREAM=" + streamName.ToUpper();
                if (line.Contains(key + ","))
                {
                    spi.start = i;
                    break;
                }
            }

            bool b = false;
            string attrvalue = string.Empty;
            for (int i = spi.start; i < lines.Length; i++)
            {
                string line = lines[i];
                string key1 = "PROPERTY STREAM=";
                string key2 = "UNIT OPERATIONS";
                string key = "PROPERTY STREAM=" + streamName.ToUpper();
                if ((line.Contains(key1) || line.Contains(key2)) && !line.Contains(key + ","))
                {
                    spi.end = i - 1;
                    if (!b)
                    {
                        attrvalue = rewriteAttr + rewriteValue;
                    }
                    break;
                }
                else if (line.Contains(attr1.ToUpper()) || (!string.IsNullOrEmpty(attr2) && line.Contains(attr2)))
                {
                    b = true;
                    string oldValue = string.Empty;
                    string newValue = rewriteAttr + rewriteValue;

                    int s = line.IndexOf(attr1.ToUpper());
                    if (s == -1)
                        s = line.IndexOf(attr2.ToUpper());
                    string sub = line.Substring(s);
                    s = sub.IndexOf(",");
                    oldValue = sub.Substring(0, s);
                    string newLine = line.Replace(oldValue, newValue);
                    list.Add(newLine);
                }
                else
                {
                    list.Add(line);

                }
            }
            if (b)
            {
                sb.Append(list[0]).Append("\r\n");
            }
            else
            {
                sb.Append(list[0]).Append(@",");
                sb.Append(attrvalue).Append("\r\n");
            }
            for (int i = 1; i < list.Count; i++)
            {
                sb.Append(list[i]).Append("\r\n");
            }
            spi.Name = streamName;
            spi.NewInfo = sb.ToString();
            return spi;
        }

        public static InpPosInfo GetFlashPosInfo(string[] lines, string flashName, string rewriteAttr, string rewriteValue)
        {
            if (string.IsNullOrEmpty(flashName))
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            List<string> list = new List<string>();
            InpPosInfo spi = new InpPosInfo();
            for (int i = 2; i < lines.Length; i++)
            {
                string line = lines[i];
                string key = "UID=" + flashName.ToUpper();
                if ((line + ",").Contains(key + ","))
                {
                    spi.start = i;
                    break;
                }
            }
            string attrvalue = string.Empty;
            for (int i = spi.start; i < lines.Length; i++)
            {
                string line = lines[i];
                string key1 = "UID=";
                string key2 = "END";
                string key = "UID=" + flashName.ToUpper();
                if ((line.Contains(key1) || line.Contains(key2)) && !line.Contains(key))
                {
                    spi.end = i - 1;
                    break;
                }
                else
                {
                    list.Add(line);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0 && list[i].Contains("PRODUCT  "))
                {
                    sb.Append(list[i]).Append("\r\n");
                    sb.Append("\t ADIABATIC Duty=0,").Append(rewriteAttr).Append(rewriteValue).Append("\r\n");
                    break;
                }
                else
                {
                    sb.Append(list[i]).Append("\r\n");
                }
            }

            spi.Name = flashName;
            spi.NewInfo = sb.ToString();
            return spi;
        }

    }

    public class InpPosInfo
    {
        public int start;
        public int end;
        public string Name;
        public string NewInfo;
    }
}
