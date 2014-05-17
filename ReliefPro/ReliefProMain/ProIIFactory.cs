using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ReliefProCommon.CommonLib;
using ProII91;
using ProII;
using ReliefProModel;

namespace ReliefProMain
{
    public  class ProIIFactory
    {
        public static string GetProIIVerison(string przFile, string rootDir)
        {
            PRIIFileOperator.DecompressProIIFile(przFile, rootDir);
            string[] files = Directory.GetFiles(rootDir, "*.inp");
            string inpFile = files[0];
            string version = PRIIFileOperator.CheckProIIVersion(inpFile);
            return version;
        }
        public static IProIIReader CreateReader(string version)
        {
            IProIIReader reader = null;
            if (version == "9.1")
            {
                reader= new ProII91.ProIIReader();
            }
            else if (version == "9.2")
            {
                reader = new ProII92.ProIIReader();
            }
            return reader;
        }
        
        public static IFlashCalculate CreateFlashCalculate(string version)
        {
            IFlashCalculate calc = null;            
            if (version == "9.1")
            {
                calc = new ProII91.FlashCalculate();
            }
            else if (version == "9.2")
            {
                calc = new ProII92.FlashCalculate();
            }
            return calc;
        }
        public static IFlashCalculateW CreateFlashCalculateW(string version)
        {
            IFlashCalculateW calc = null;            
            if (version == "9.1")
            {
                calc = new ProII91.FlashCalculateW();
            }
            else if (version == "9.2")
            {
                calc = new ProII92.FlashCalculateW();
            }
            return calc;
        }
        public static IPHASECalculate CreatePHASECalculate(string version)
        {
            IPHASECalculate calc = null;
            if (version == "9.1")
            {
                calc = new ProII91.PHASECalculate();
            }
            else if (version == "9.2")
            {
                calc = new ProII92.PHASECalculate();
            }
            return calc;
        }

    }
}
