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
        public static void Run(string przVersion, string proiiFilePath)
        {
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
                            Process pro = Process.Start(psInfo);
                        }
                    }
                }
            }
        }

    }
}
