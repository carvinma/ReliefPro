/*
 * 此文件是把所有ProII常用的方法集
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * */


using ProII;
using ReliefProCommon.CommonLib;
using ReliefProMain.Common;
using ReliefProModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UOMLib;

namespace ReliefProMain
{
    public class ProIIMethod
    {
        /// <summary>
        /// 合并多条物料信息
        /// </summary>
        /// <param name="proIIVersion"></param>
        /// <param name="strFeeds"></param>
        /// <param name="mixFeeds"></param>
        /// <param name="dirSource"></param>
        /// <param name="dirMix"></param>
        /// <param name="errorType"></param>
        /// <returns></returns>
        public static CustomStream  MixStream(string proIIVersion,   List<string> strFeeds,  List<CustomStream> mixFeeds,string dirSource,string dirMix,ref int errorType)
        {
            try
            {
                string[] sourceFiles = Directory.GetFiles(dirSource, "*.inp");
                string sourceFile = sourceFiles[0];
                string[] lines = System.IO.File.ReadAllLines(sourceFile);
                string sbcontent = PROIIFileOperator.getUsableContent(strFeeds, lines);
                IMixCalculate mixcalc = ProIIFactory.CreateMixCalculate(proIIVersion);
                string mixProductName = Guid.NewGuid().ToString().Substring(0, 6);

                if (Directory.Exists(dirMix))
                    Directory.Delete(dirMix, true);
                Directory.CreateDirectory(dirMix);

                int mixImportResult = 1;
                int mixRunResult = 1;
                string mixPrzFile = mixcalc.Calculate(sbcontent, mixFeeds, mixProductName, dirMix, ref mixImportResult, ref mixRunResult);

                if (mixImportResult == 1 || mixImportResult == 2)
                {
                    if (mixRunResult == 1 || mixRunResult == 2)
                    {
                        IProIIReader reader = ProIIFactory.CreateReader(proIIVersion);
                        reader.InitProIIReader(mixPrzFile);
                        ProIIStreamData proIIvapor = reader.GetSteamInfo(mixProductName);
                        reader.ReleaseProIIReader();
                        CustomStream mixCSProduct = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIvapor);
                        return mixCSProduct;
                    }
                    else
                    {
                        errorType = 1;
                        return null;
                    }
                }
                else
                {
                    errorType = 2;
                    return null;
                }
            }
            catch(Exception ex)
            {
                errorType = 3;
                return null;
            }

        }


        ///// <summary>
        ///// 计算并获取临界信息
        ///// </summary>
        ///// <param name="content"></param>
        ///// <param name="ReliefPressure"></param>
        ///// <param name="stream"></param>
        ///// <param name="dirPhase"></param>
        ///// <returns></returns>
        //public static CriticalInfo CalcCriticalInfo(string proIIVersion, List<CustomStream> lstFeed, string dirSource, string dirPhase, ref int errorType)
        //{

        //    CustomStream stream = new CustomStream();
        //    stream = lstFeed[0];
        //    string[] streamComps = stream.TotalComposition.Split(',');
        //    int len = streamComps.Length;
        //    double[] streamCompValues = new double[len];
        //    double sumTotalMolarRate = 0;
        //    foreach (CustomStream cs in lstFeed)
        //    {
        //        sumTotalMolarRate = sumTotalMolarRate + cs.TotalMolarRate;
        //    }
        //    foreach (CustomStream cs in lstFeed)
        //    {
        //        string[] comps = cs.TotalComposition.Split(',');
        //        for (int i = 0; i < len; i++)
        //        {
        //            streamCompValues[i] = streamCompValues[i] + double.Parse(comps[i]) * cs.TotalMolarRate / sumTotalMolarRate;
        //        }
        //    }
        //    StringBuilder sumComposition = new StringBuilder();
        //    foreach (double comp in streamCompValues)
        //    {
        //        sumComposition.Append(",").Append(comp.ToString());
        //    }
        //    stream.TotalComposition = sumComposition.ToString().Substring(1);
        //    string phasecontent = PROIIFileOperator.getUsablePhaseContent(stream.StreamName, dirSource);

        //    CriticalInfo ci = new CriticalInfo();
        //    int ImportResult = 0;
        //    int RunResult = 0;

        //    IPHASECalculate PhaseCalc = ProIIFactory.CreatePHASECalculate(proIIVersion);
        //    string PH = "PH" + Guid.NewGuid().ToString().Substring(0, 4);
        //    string criticalPress = string.Empty;
        //    string criticalTemp = string.Empty;
        //    string cricondenbarPress = string.Empty;
        //    string cricondenbarTemp = string.Empty;
        //    string phasef = PhaseCalc.Calculate(phasecontent, stream, PH, dirPhase, ref ImportResult, ref RunResult);
        //    if (ImportResult == 1 || ImportResult == 2)
        //    {
        //        if (RunResult == 1 || RunResult == 2)
        //        {
        //            IProIIReader reader = ProIIFactory.CreateReader(proIIVersion);
        //            reader.InitProIIReader(phasef);
        //            criticalPress = reader.GetCriticalPressure(PH);
        //            criticalTemp = reader.GetCriticalTemperature(PH);
        //            cricondenbarPress = reader.GetCricondenbarPress(PH);
        //            cricondenbarTemp = reader.GetCricondenbarTemp(PH);
        //            reader.ReleaseProIIReader();

        //            ci.CriticalPressure = UnitConvert.Convert("KPA", UOMEnum.Pressure, double.Parse(criticalPress));
        //            ci.CriticalTemperature = UnitConvert.Convert("K", UOMEnum.Temperature, double.Parse(criticalTemp));
        //            ci.CricondenbarPressure = UnitConvert.Convert("K", UOMEnum.Pressure, double.Parse(cricondenbarPress));
        //            ci.CricondenbarTemperature = UnitConvert.Convert("KPA", UOMEnum.Temperature, double.Parse(cricondenbarTemp));
        //            return ci;
        //        }
        //        else
        //        {
        //            errorType = 1;
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        errorType = 2;
        //        return null;
        //    }

        //}


        public static bool IsPureComposition(CustomStream cs)
        {
            bool b = false;
            string[] comps = cs.TotalComposition.Split(',');
            foreach (string comp in comps)
            {
                if (comp == "1")
                {
                    b = true;
                    break;
                }
            }
            return b;
        }

        public static string GetHeatMethod(string[] lines, string eqName)
        {
            string result = string.Empty;
            int i = 1;
            int length = lines.Length;

            for (i = 1; i < length; i++)
            {
                string line = lines[i];
                if (line.Contains("UNIT OPERATIONS"))
                {
                    break;
                }
            }
            int start = i + 1;
            for (i = start; i < length; i++)
            {
                string line = lines[i].Trim();
                if ((line + ",").Contains("UID="+eqName+","))
                {
                    break;
                }
            }
            start = i + 1;
            for (i = start; i < length; i++)
            {
                string line = lines[i].Trim();
                if (line.Length>7&&line.Substring(0,6)=="METHOD")
                {
                    result = line ;
                    break;
                }
                if (line.Contains("UID=") || line=="END") //新的Unit或Unit结束符，退出。 说明没有Method
                {                    
                    break;
                }
            }

            return result;
        }



    }
}
