using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using System.IO;
using System.Web.Caching;

namespace ReliefProCommon
{
   public class SecurityWebSrv
    {
        private static byte[] Key64 = { 42, 16, 93, 156, 78, 4, 218, 32 };
        private static byte[] IV64 = { 55, 103, 246, 79, 36, 99, 167, 3 };
        public static String Encrypt(String valueString) //DES加密  
        {
            string rlt = null;
            try
            {
                if (valueString.Trim() != "")
                { //定义DES的Provider  
                    //定义DES的Provider  
                    DESCryptoServiceProvider desprovider = new DESCryptoServiceProvider();

                    //定义内存流  
                    MemoryStream memoryStream = new MemoryStream();
                    //定义加密流  
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, desprovider.CreateEncryptor(Key64, IV64), CryptoStreamMode.Write);
                    //定义写IO流  
                    StreamWriter writerStream = new StreamWriter(cryptoStream);
                    //写入加密后的字符流  
                    writerStream.Write(valueString);
                    writerStream.Flush();
                    cryptoStream.FlushFinalBlock();

                    memoryStream.Flush();
                    //返回加密后的字符串  
                    rlt = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                    //return (Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length));
                }
            }
            catch (Exception err)
            {

                return err.Message;
            }
            return rlt;
        }

        public static String Decrypt(String valueString)//DES解密  
        {
            string rlt = null;
            try
            {
                if (valueString.Trim() != "")
                {
                    //定义DES的Provider  
                    DESCryptoServiceProvider desprovider = new DESCryptoServiceProvider();
                    //转换解密的字符串为二进制  
                    byte[] buffer = Convert.FromBase64String(valueString);
                    //定义内存流  
                    MemoryStream memoryStream = new MemoryStream(buffer);
                    //定义加密流  
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, desprovider.CreateDecryptor(Key64, IV64), CryptoStreamMode.Read);
                    //定义读IO流  
                    StreamReader readerStream = new StreamReader(cryptoStream);
                    //返回解密后的字符串  

                    rlt = readerStream.ReadToEnd();
                    memoryStream.Flush();
                    //cryptoStream.FlushFinalBlock();
                    readerStream.Close();
                    //return (readerStream.ReadToEnd());
                }
            }
            catch (Exception err)
            {
                return "非法解密" + err.Message;
            }
            return rlt;
        }
    }
}
