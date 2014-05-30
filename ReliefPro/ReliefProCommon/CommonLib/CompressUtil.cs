using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProCommon.CommonLib
{
    public class CompressUtil
    {

        ///<summary>压缩</summary>
        public static string Compress(string strSource)
        {
            //if (strSource == null || strSource.Length > 8 * 1024)
            //    throw new System.ArgumentException("字符串为空或长度太大！");
            if (strSource == null)
                throw new System.ArgumentException("字符串为空或长度太大！");

            System.Text.Encoding encoding = System.Text.Encoding.Unicode;
            byte[] buffer = encoding.GetBytes(strSource);
            //byte[] buffer = Convert.FromBase64String(strSource); //传入的字符串不一定是Base64String类型，这样写不行

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.DeflateStream stream = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Compress, true);
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();

            buffer = ms.ToArray();
            ms.Close();

            return Convert.ToBase64String(buffer); //将压缩后的byte[]转换为Base64String
        }
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string Decompress(string strSource)
        {
            //System.Text.Encoding encoding = System.Text.Encoding.Unicode;
            //byte[] buffer = encoding.GetBytes(strSource);
            byte[] buffer = Convert.FromBase64String(strSource);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            ms.Position = 0;
            System.IO.Compression.DeflateStream stream = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Decompress);
            stream.Flush();

            int nSize = 1024 * 8; //字符串不会超过16K
            byte[] decompressBuffer = new byte[nSize];
            var sb = new StringBuilder();
            int nSizeIncept = 0;
            do
            {
                nSizeIncept = stream.Read(decompressBuffer, 0, nSize);
                sb.Append(System.Text.Encoding.Unicode.GetString(decompressBuffer, 0, nSizeIncept));
            } while (nSizeIncept > 0);

            stream.Close();

            return sb.ToString();//转换为普通的字符串
        }
    }
}
