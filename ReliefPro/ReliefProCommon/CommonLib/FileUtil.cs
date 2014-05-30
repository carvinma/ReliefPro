using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Exceptions;
using System.IO;

namespace Common.CommonLib
{
    public class FileUtil
    {

        /// <summary>
        /// 文件名称 中文转换
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToHexString(string s)
        {
            char[] chars = s.ToCharArray();
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < chars.Length; index++)
            {
                bool needToEncode = NeedToEncode(chars[index]);
                if (needToEncode)
                {
                    string encodedString = ToHexString(chars[index]);
                    builder.Append(encodedString);
                }
                else
                {
                    builder.Append(chars[index]);
                }
            }

            return builder.ToString();
        }


        /// <summary>
        /// 判断文件是否为图片
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Boolean IsImage(string path)
        {
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                return true;
            }
            catch (Exception e)
            {
                WriterExceptions.WriterToLog(e);
            }

            return false;
        }


        /// <summary>
        /// 需要转换的字符
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        private static bool NeedToEncode(char chr)
        {
            string reservedChars = "$-_.+!*'(),@=&";

            if (chr > 127)
                return true;
            if (char.IsLetterOrDigit(chr) || reservedChars.IndexOf(chr) >= 0)
                return false;

            return true;
        }

        //字符转换
        private static string ToHexString(char chr)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encodedBytes = utf8.GetBytes(chr.ToString());
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < encodedBytes.Length; index++)
            {
                builder.AppendFormat("%{0}", Convert.ToString(encodedBytes[index], 16));
            }
            return builder.ToString();
        }

        //获取INP文件中有用的部分
        public static string getUsableContent(string streamName, string inpFilePath)
        {
            StringBuilder sb = new StringBuilder();
            string[] lines = System.IO.File.ReadAllLines(inpFilePath);
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
