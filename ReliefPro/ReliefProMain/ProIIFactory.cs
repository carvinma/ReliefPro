using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ReliefProCommon.CommonLib;
using ProII;
using ReliefProModel;

namespace ReliefProMain
{
    public  class ProIIFactory
    {
        public static string GetProIIVerison(string przFile, string rootDir)
        {
            PROIIFileOperator.DecompressProIIFile(przFile, rootDir);
            //string inpFile = przFile.Substring(0, przFile.Length - 4) + "_backup.inp";
            string[] files = Directory.GetFiles(rootDir,"*.inp");
            string inpFile = string.Empty;
            if (files.Length == 1)
                inpFile = files[0];
            else
            {
                foreach (string f in files)
                {
                    if (f.Length > 11 || f.Contains("_backup.inp"))
                    {
                        inpFile = files[0];
                    }
                }
            }
            string version = PROIIFileOperator.CheckProIIVersion(inpFile);
            return version;
        }
        public static string GetProIIhs2Verison(string przFile, string rootDir)
        {
            PROIIFileOperator.DecompressProIIFile(przFile, rootDir);
            //string inpFile = przFile.Substring(0, przFile.Length - 4) + "_backup.inp";
            string[] files = Directory.GetFiles(rootDir, "*.hs2");
            string hs2file = files[0];
            string version = PROIIFileOperator.CheckProIIhs2Version(hs2file);
            return version;
        }
        public static IProIIRunCalcSave CreateRunCalcSave(string version)
        {
            IProIIRunCalcSave r = null;
            if (version == "9.1")
            {
                //r = new ProII91.();
            }
            else if (version == "9.2")
            {
                //r = new ProII92.ProIIRunCalcSave();
            }
            return r;
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
            else if (version == "9.3")
            {
                reader = new ProII93.ProIIReader();
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
            else if (version == "9.3")
            {
                calc = new ProII93.FlashCalculate();
            }
            return calc;
        }
        ////public static IFlashCalculateW CreateFlashCalculateW(string version)
        ////{
        ////    IFlashCalculateW calc = null;            
        ////    if (version == "9.1")
        ////    {
        ////        calc = new ProII91.FlashCalculateW();
        ////    }
        ////    else if (version == "9.2")
        ////    {
        ////        calc = new ProII92.FlashCalculateW();
        ////    }
        ////    return calc;
        ////}
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
            else if (version == "9.3")
            {
                calc = new ProII93.PHASECalculate();
            }
            return calc;
        }

        public static IProIIImport CreateProIIImport(string version)
        {
            IProIIImport impt = null;
            if (version == "9.1")
            {
                impt = new ProII92.ProIIImport();
            }
            else if (version == "9.2")
            {
                impt = new ProII92.ProIIImport();
            }
            if (version == "9.1")
            {
                impt = new ProII92.ProIIImport();
            }
            else if (version == "9.3")
            {
                impt = new ProII93.ProIIImport();
            }
            return impt;
        }

    }
}
