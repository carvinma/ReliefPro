using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ReliefProCommon.CommonLib
{
    public class ProIIHelper
    {

        /// <summary>
        ///  open prz 文件
        /// </summary>
        /// <param name="przVersion"></param>
        /// <param name="proiiFilePath"></param>
        public static Process Run(string przVersion, string proiiFilePath)
        {
            Process pro = null;
            if (!string.IsNullOrEmpty(przVersion) && !string.IsNullOrEmpty(proiiFilePath))
            {
                ProcessStartInfo psInfo = new ProcessStartInfo();
                //HKEY_LOCAL_MACHINE\SOFTWARE\SIMSCI\PRO/II
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey softWare = rk.OpenSubKey("Software");
                RegistryKey simsci = softWare.OpenSubKey("SIMSCI");
                if (simsci != null && simsci.SubKeyCount != 0)
                {
                    RegistryKey proii = simsci.OpenSubKey("PRO/II");
                    if (proii != null && proii.SubKeyCount != 0)
                    {
                        RegistryKey version = proii.OpenSubKey(przVersion);
                        if (version != null && version.SubKeyCount != 0)
                        {
                            psInfo.FileName = version.GetValue("SecDir").ToString() + @"\PROII.exe";
                            psInfo.Arguments = string.Format("/I=\"{0}\" \"{1}\"", version.GetValue("SecIni").ToString(), proiiFilePath);
                            pro = Process.Start(psInfo);
                        }
                    }
                }
            }
            return pro;
        }

        /// <summary>
        /// open inp file
        /// </summary>
        /// <param name="ProiiInpFile"></param>
        public static Process Open(string ProiiInpFile)
        {
            ProcessStartInfo procInfo = new System.Diagnostics.ProcessStartInfo();
            //设置要调用的外部程序名
            procInfo.FileName = "notepad.exe";
            //设置外部程序的启动参数（命令行参数）为1.txt
            procInfo.Arguments = ProiiInpFile;

            //设置外部程序工作目录为 C:\
            //procInfo.WorkingDirectory = "C:\\";
           return System.Diagnostics.Process.Start(procInfo);
        }
    }
}
