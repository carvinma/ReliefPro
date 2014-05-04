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
    public static class PRIIFileOperator
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

            }
            return version;
        }

        public static string getUsableContent(string streamName, string rootDir)
        {
            StringBuilder sb = new StringBuilder();
            string[] files = Directory.GetFiles(rootDir, "*.inp");
            string sourceFile = files[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            List<int> list = new List<int>();
            int i = 0;
            while (i < lines.Length)
            {
                string s = lines[i];
                if (s.Trim().IndexOf("NAME") == 0 || s.Trim().IndexOf("UNIT") == 0)
                {
                    break;
                }
                else
                {
                    int idx = s.IndexOf("REFSTREAM");
                    if (idx == -1)
                    {
                        sb.Append(s).Append("\n");
                    }
                    else
                    {
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
                    }
                    i++;
                }

            }


            return sb.ToString();
        }
    }
}
