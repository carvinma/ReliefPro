using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace ReliefProMain.Common
{
    public static class CustomZipFile
    {
        public delegate string MyDelegate(int ms);
        public static void Zip(string inputFolder, string outputZip,string targetRef)
        {
            try
            {
                //Create an empty zip file
                byte[] emptyzip = new byte[] { 80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                string outputRef = outputZip;
                outputZip = outputZip.Substring(0, outputZip.Length - 3) + "zip";
                FileStream fs = File.Create(outputZip);
                fs.Write(emptyzip, 0, emptyzip.Length);
                fs.Flush();
                fs.Close();
                fs = null;

                //Copy a folder and its contents into the newly created zip file
                Shell32.ShellClass sc = new Shell32.ShellClass();
                Shell32.Folder SrcFlder = sc.NameSpace(inputFolder);
                Shell32.Folder DestFlder = sc.NameSpace(outputZip);
                Shell32.FolderItems items = SrcFlder.Items();
                DestFlder.CopyHere(items, 20);
                while (File.Exists(outputZip))
                {

                    //MyDelegate dl = DelegateMethod;
                    AsyncProcessor asyncIO = new AsyncProcessor(outputZip);
                    asyncIO.StartProcess();
                    while (true)
                    {
                        if (asyncIO.IsReadEnd)
                        {
                            File.Copy(outputZip, outputRef, true);
                            AsyncProcessor asyncIO2 = new AsyncProcessor(outputRef);
                            asyncIO2.StartProcess();
                            while (true)
                            {
                                if (asyncIO2.IsReadEnd)
                                {
                                    File.Copy(outputRef, targetRef, true);
                                    AsyncProcessor asyncIO3 = new AsyncProcessor(targetRef);
                                    asyncIO3.StartProcess();
                                    while (true)
                                    {
                                        if (asyncIO3.IsReadEnd)
                                        {
                                            string winTempDir = System.IO.Path.GetDirectoryName(outputZip);
                                            if (Directory.Exists(winTempDir))
                                            {
                                                Directory.Delete(winTempDir, true);
                                            }
                                            break;
                                        }
                                    }
                                    break;

                                }
                            }
                            break;
                        }

                    }
                }
                
            }
            catch (Exception ex)
            {
                string test = ex.ToString();
            }
        }
        public static void UnZip(string inputZip, string outputFolder)
        {
            string inputZip2 = inputZip.Substring(0, inputZip.Length - 3) + ".zip";
            File.Copy(inputZip, inputZip2,true);
            Shell32.ShellClass sc = new Shell32.ShellClass();
            //Create directory in which you will unzip your items.
            Directory.CreateDirectory(outputFolder);

            //Declare the folder where the items will be extracted.
            Shell32.Folder DestFlder = sc.NameSpace(outputFolder);

            //Declare the input zip file.
            Shell32.Folder SrcFlder = sc.NameSpace(inputZip2);
            Shell32.FolderItems items = SrcFlder.Items();
            //Extract the items from the zip file.
            DestFlder.CopyHere((items), 4);
            File.Delete(inputZip2);
        }
    }


    public class AsyncProcessor
    {
        private Stream inputStream;

        // 每次读取块的大小
        private int bufferSize = 2048;

        public int BufferSize
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }

        public bool IsReadEnd;

        // 容纳接收数据的缓存
        private byte[] buffer;

        public AsyncProcessor(string fileName)
        {
            buffer = new byte[bufferSize];

            // 打开文件，指定参数为 true 以提供对异步操作的支持
            inputStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, true);
        }

        public void StartProcess()
        {
            // 开始异步读取文件，填充缓存区
            inputStream.BeginRead(buffer, 0, buffer.Length, OnCompletedRead, null);
        }

        public void OnCompletedRead(IAsyncResult asyncResult)
        {
            // 已经异步读取一个 块 ，接收数据
            int bytesRead = inputStream.EndRead(asyncResult);

            // 如果没有读取任何字节，则流已达文件结尾
            if (bytesRead > 0)
            {
                // 暂停以模拟对数据块的处理
                //Debug.WriteLine("   异步线程：已读取一块");
                Thread.Sleep(TimeSpan.FromMilliseconds(20));

                // 开始读取下一块
                inputStream.BeginRead(buffer, 0, buffer.Length, OnCompletedRead, null);
            }
            else
            {
                // 结束操作
                //Debug.WriteLine("   异步线程：读取文件结束");
                inputStream.Close();
                IsReadEnd = true;
            }
        }
    }

}
