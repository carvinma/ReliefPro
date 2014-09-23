﻿using System;
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

    }
}
