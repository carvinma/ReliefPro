using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using P2Wrap93;
using ReliefProCommon.CommonLib;
using ProII;
using ReliefProModel;

namespace ProII93
{
    /// <summary>
    /// 读取PRZ文件信息的类库
    /// 
    /// 对于Flash  ProductStoreData的值： 1-Vapor 2-Liquid 3-Solid 4-Mixed
    /// 
    /// 
    /// 
    /// </summary>
    public class ProIIReader : IProIIReader
    {
        string[] arrStreamAttributes = { "Pressure", "Temperature", "VaporFraction", "VaporZFmKVal", "TotalComposition", "TotalMolarEnthalpy", "TotalMolarRate", "InertWeightEnthalpy", "InertWeightRate" };
        string[] arrBulkPropAttributes = { "BulkMwOfPhase", "BulkDensityAct", "BulkViscosity", "BulkCPCVRatio", "BulkCP", "BulkThermalCond", "BulkSurfTension" };

        string[] arrColumnAttributes = { "PressureDrop", "Duty", "NumberOfTrays", "HeaterNames", "HeaterDuties", "HeaterNumber", "HeaterPANumberfo", "HeaterRegOrPAFlag", "HeaterTrayLoc", "HeaterTrayNumber" };
        string[] arrColumnInAttributes = { "ProdType", "FeedTrays", "ProdTrays", "FeedData", "ProductData" };
        string[] arrFlashAttributes = { "FeedData", "ProductData", "PressCalc", "TempCalc", "DutyCalc", "Type", "ProductStoreData" };
        string[] arrHxAttributes = { "FeedData", "ProductData", "DutyCalc", "ProductStoreData", "LmtdCalc", "LmtdFactorCalc", "FirstFeed", "FirstProduct", "LastFeed", "LastProduct", };
        string[] arrCompressorAttributes = { "FeedData", "ProductData", "ProductStoreData" };
        string[] arrMixerAttributes = { "FeedData", "ProductData"};
        string[] arrSplitterAttributes = { "FeedData", "ProductData"};

        string przFileName;
        CP2File cp2File;
        CP2Object objCompCalc;
        CP2ServerClass cp2Srv;

        string ComponentIds = string.Empty;
        string CompIns = string.Empty;
        string PrintNumbers = string.Empty;
        Dictionary<string, ProIIStreamData> dicFeedInfo = new Dictionary<string, ProIIStreamData>();
        Dictionary<string, ProIIStreamData> dicProductInfo = new Dictionary<string, ProIIStreamData>();

        public void InitProIIReader(string przFileFullName)
        {
            przFileName = System.IO.Path.GetFileName(przFileFullName);
            cp2Srv = new CP2ServerClass();
            cp2Srv.Initialize();
            cp2File = (CP2File)cp2Srv.OpenDatabase(przFileFullName);

            objCompCalc = (CP2Object)cp2File.ActivateObject("CompCalc", "CompCalc");
            object ComponentId = objCompCalc.GetAttribute("ComponentId");
            object PrintNumber = objCompCalc.GetAttribute("PrintNumber");
            PrintNumbers = ConvertExt.ObjectToString(PrintNumber);
            ComponentIds = ConvertExt.ObjectToString(ComponentId);
            object CompIn = cp2File.GetObjectNames("CompIn");
            CompIns = ConvertExt.ObjectToString(CompIn);
        }



        //获得设备和物流线的个数和名字信息
        public int GetAllEqAndStreamTotal(IList<ProIIEqType> eqTypeList, ref IList<ProIIEqData> eqList, ref IList<string> streamList)
        {
            int total = 0;
            int eqCount = getEqTotal(eqTypeList, ref eqList);
            int streamCount = getStreamTotal(ref streamList);
            total = eqCount + streamCount;
            return total;
        }

        private int getStreamTotal(ref IList<string> streamList)
        {
            int streamCount = cp2File.GetObjectCount("Stream");
            object streamnames = cp2File.GetObjectNames("Stream");

            if (streamnames is Array)
            {
                string[] oNames = (string[])streamnames;
                foreach (string name in oNames)
                {
                    streamList.Add(name);
                }
            }
            else
            {
                streamList.Add(streamnames.ToString());
            }
            return streamCount;
        }

        private int getEqTotal(IList<ProIIEqType> eqTypeList, ref IList<ProIIEqData> eqList)
        {
            int eqCount = 0;
            foreach (ProIIEqType eqType in eqTypeList)
            {
                string otype = eqType.EqTypeName;
                int objCount = cp2File.GetObjectCount(otype);
                if (objCount > 0)
                {
                    object objectnames = cp2File.GetObjectNames(otype);
                    if (objectnames is Array)
                    {
                        string[] oNames = (string[])objectnames;
                        foreach (string name in oNames)
                        {
                            ProIIEqData eq = new ProIIEqData();
                            eq.EqName = name;
                            eq.EqType = otype;
                            eqList.Add(eq);
                        }
                    }
                    else
                    {
                        ProIIEqData eq = new ProIIEqData();
                        eq.EqName = objectnames.ToString();
                        eq.EqType = otype;
                        eqList.Add(eq);
                    }
                }
                eqCount = eqCount + objCount;
            }
            return eqCount;
        }

        public void GetEqInfo(string otype, string name, ref IList<ProIIEqData> eqListData)
        {
            ProIIEqData data = GetEqInfo(otype, name);
            eqListData.Add(data);
        }

        public void GetSteamInfo(string name, ref IList<ProIIStreamData> streamListData)
        {
            ProIIStreamData data = GetSteamInfo(name);
            streamListData.Add(data);
        }

        public void ReleaseProIIReader()
        {
            Marshal.FinalReleaseComObject(cp2Srv);
            GC.ReRegisterForFinalize(cp2Srv);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="tray"></param>
        /// <param name="phase">0:liquid+vapor 1:vapor 2:liquid</param>
        /// <param name="trayFlow">1:net 2:total</param>
        public ProIIStreamData CopyStream(string columnName, int tray, int phase, int trayFlow)
        {
            string pressure1 = "0";
            
            ProIIStreamData proIIStream = new ProIIStreamData();
            string streamName = "temp" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            CP2Object tempStream = (CP2Object)cp2File.CreateObject("Stream", streamName);
            bool b = cp2File.CopyTrayToStream(columnName, (short)tray, (p2Phase)phase, 0, (p2TrayFlow)trayFlow, streamName);
            cp2File.CalculateStreamProps();
            proIIStream = GetSteamInfo(streamName);
            proIIStream.Tray = tray.ToString();
            proIIStream.ProdType = phase.ToString();
            if (proIIStream.Pressure == "0")
            {
                CP2Object objColumn = (CP2Object)cp2File.ActivateObject("Column", columnName);
                object v = objColumn.GetAttribute("TrayPressures");
                if (v is Array)
                {
                    object[] values = (object[])v;
                    pressure1 = values[1].ToString();
                }
                proIIStream.Pressure = pressure1;
            }
            cp2File.DeleteObject("Stream", streamName);
            return proIIStream;
        }

        public ProIIEqData GetEqInfo(string otype, string name)
        {
            ProIIEqData data = new ProIIEqData();
            CP2Object eq = (CP2Object)cp2File.ActivateObject(otype, name);
            data.EqType = otype;
            data.EqName = name;
            data.SourceFile = przFileName;
            if (otype == "Column" || otype == "SideColumn")
            {
                foreach (string s in arrColumnAttributes)
                {
                    object v = eq.GetAttribute(s);
                    string value = ConvertExt.ObjectToString(v);
                    switch (s)
                    {
                        case "PressureDrop":
                            data.PressureDrop = value;
                            break;
                        case "Duty":
                            data.Duty = value;
                            break;
                        case "NumberOfTrays":
                            data.NumberOfTrays = value;
                            break;
                        case "HeaterNames":
                            data.HeaterNames = value;
                            break;
                        case "HeaterDuties":
                            data.HeaterDuties = value;
                            break;
                        case "HeaterNumber":
                            data.HeaterNumber = value;
                            break;
                        case "HeaterPANumberfo":
                            data.HeaterPANumberfo = value;
                            break;
                        case "HeaterRegOrPAFlag":
                            data.HeaterRegOrPAFlag = value;
                            break;
                        case "HeaterTrayLoc":
                            data.HeaterTrayLoc = value;
                            break;
                        case "HeaterTrayNumber":
                            data.HeaterTrayNumber = value;
                            break;
                    }
                }
                CP2Object objColumnIn = (CP2Object)cp2File.ActivateObject("ColumnIn", name);

                string[] arrColumnInAttributes = { "ProdType", "FeedTrays", "ProdTrays", "FeedData", "ProductData" };

                foreach (string s in arrColumnInAttributes)
                {
                    object v = objColumnIn.GetAttribute(s);
                    string value = ConvertExt.ObjectToString(v);
                    switch (s)
                    {
                        case "FeedData":
                            data.FeedData = value;
                            break;
                        case "ProductData":
                            data.ProductData = value;
                            break;
                        case "ProdType":
                            data.ProdType = value;
                            break;
                        case "FeedTrays":
                            data.FeedTrays = value;
                            break;
                        case "ProdTrays":
                            data.ProdTrays = value;
                            break;
                    }
                }
            }
            else if (otype == "Flash")
            {
                foreach (string s in arrFlashAttributes)
                {
                    object v = eq.GetAttribute(s);
                    string value = ConvertExt.ObjectToString(v);
                    switch (s)
                    {
                        case "FeedData":
                            data.FeedData = value;
                            break;
                        case "ProductData":
                            data.ProductData = value;
                            break;
                        case "PressCalc":
                            data.PressCalc = value;
                            break;
                        case "TempCalc":
                            data.TempCalc = value;
                            break;
                        case "DutyCalc":
                            data.DutyCalc = value;
                            break;
                        case "Type":
                            data.Type = value;
                            break;
                        case "ProductStoreData":
                            data.ProductStoreData = value;
                            break;
                    }
                }
            }
            else if (otype == "Hx" )
            {
                foreach (string s in arrHxAttributes)
                {
                    object v = eq.GetAttribute(s);
                    string value = ConvertExt.ObjectToString(v);
                    switch (s)
                    {
                        case "FeedData":
                            data.FeedData = value;
                            break;
                        case "ProductData":
                            data.ProductData = value;
                            break;
                        case "DutyCalc":
                            data.DutyCalc = value;
                            break;
                        case "LmtdCalc":
                            data.LmtdCalc = value;
                            break;
                        case "LmtdFactorCalc":
                            data.LmtdFactorCalc = value;
                            break;
                        case "ProductStoreData":
                            data.ProductStoreData = value;
                            break;
                        case "FirstFeed":
                            data.FirstFeed = value;
                            break;
                        case "LastFeed":
                            data.LastFeed = value;
                            break;
                        case "FirstProduct":
                            data.FirstProduct = value;
                            break;
                        case "LastProduct":
                            data.LastProduct = value;
                            break;
                    }
                }
            }
            else if (otype == "Compressor")
            {
                foreach (string s in arrCompressorAttributes)
                {
                    object v = eq.GetAttribute(s);
                    string value = ConvertExt.ObjectToString(v);
                    switch (s)
                    {
                        case "FeedData":
                            data.FeedData = value;
                            break;
                        case "ProductData":
                            data.ProductData = value;
                            break;
                        case "ProductStoreData":
                            data.ProductStoreData = value;
                            break;
                    }
                }
            }
            else if (otype == "Mixer")
            {
                foreach (string s in arrMixerAttributes)
                {
                    object v = eq.GetAttribute(s);
                    string value = ConvertExt.ObjectToString(v);
                    switch (s)
                    {
                        case "FeedData":
                            data.FeedData = value;
                            break;
                        case "ProductData":
                            data.ProductData = value;
                            break;
                       
                    }
                }
            }
            else if (otype == "Splitter")
            {
                foreach (string s in arrSplitterAttributes)
                {
                    object v = eq.GetAttribute(s);
                    string value = ConvertExt.ObjectToString(v);
                    switch (s)
                    {
                        case "FeedData":
                            data.FeedData = value;
                            break;
                        case "ProductData":
                            data.ProductData = value;
                            break;

                    }
                }
            }
            return data;
        }

        public ProIIStreamData GetSteamInfo(string name)
        {
            ProIIStreamData data = new ProIIStreamData();
            bool bCalulate = cp2File.CalculateStreamProps(name);
            data.SourceFile = przFileName;
            data.StreamName = name;
            data.ProdType = "";
            data.Tray = "";

            data.CompIn = CompIns;
            data.Componentid = ComponentIds;
            data.PrintNumber = PrintNumbers;
            CP2Object objStream = (CP2Object)cp2File.ActivateObject("Stream", name);

            foreach (string s in arrStreamAttributes)
            {
                object v = objStream.GetAttribute(s);
                string value = ConvertExt.ObjectToString(v);
                switch (s)
                {
                    case "Pressure":
                        data.Pressure = value;
                        break;
                    case "Temperature":
                        data.Temperature = value;
                        break;
                    case "VaporFraction":
                        data.VaporFraction = value;
                        break;
                    case "VaporZFmKVal":
                        data.VaporZFmKVal = value;
                        break;
                    case "TotalComposition":
                        data.TotalComposition = value;
                        break;
                    case "TotalMolarEnthalpy":
                        data.TotalMolarEnthalpy = value;
                        break;
                    case "TotalMolarRate":
                        data.TotalMolarRate = value;
                        break;
                    case "InertWeightEnthalpy":
                        data.InertWeightEnthalpy = value;
                        break;
                    case "InertWeightRate":
                        data.InertWeightRate = value;
                        break;

                }
            }
            Marshal.FinalReleaseComObject(objStream);
            GC.ReRegisterForFinalize(objStream);
            if (bCalulate)
            {
                CP2Object objBulkDrop = (CP2Object)cp2File.ActivateObject("SrBulkProp", name);
                foreach (string s in arrBulkPropAttributes)
                {
                    object v = objBulkDrop.GetAttribute(s);
                    string value = ConvertExt.ObjectToString(v);
                    switch (s)
                    {
                        case "BulkMwOfPhase":
                            data.BulkMwOfPhase = value;
                            break;
                        case "BulkDensityAct":
                            data.BulkDensityAct = value;
                            break;
                        case "VaporFraction":
                            data.Pressure = value;
                            break;
                        case "BulkViscosity":
                            data.BulkViscosity = value;
                            break;
                        case "BulkCPCVRatio":
                            data.BulkCPCVRatio = value;
                            break;
                        case "BulkCP":
                            data.BulkCP = value;
                            break;
                        case "BulkThermalCond":
                            data.BulkThermalCond = value;
                            break;
                        case "BulkSurfTension":
                            data.BulkSurfTension = value;
                            break;
                    }
                }
            }

            return data;
        }

        public Dictionary<string, ProIIStreamData> GetTowerStreamInfoExtra(string otype, string eqname)
        {
            Dictionary<string, ProIIStreamData> dic = new Dictionary<string, ProIIStreamData>();
            CP2Object eq = (CP2Object)cp2File.ActivateObject(otype, eqname);
            object pd = eq.GetAttribute("ProductData");
            string productdata = ConvertExt.ObjectToString(pd);
            string[] productdatas = productdata.Split(',');

            object ptype = eq.GetAttribute("ProdType");
            string producttype = ConvertExt.ObjectToString(ptype);
            string[] producttypes = producttype.Split(',');

            object ptray = eq.GetAttribute("ProdTrays");
            string prodtray = ConvertExt.ObjectToString(ptray);
            string[] prodtrays = prodtray.Split(',');
            int count = productdatas.Length;
            for (int i = 0; i < count; i++)
            {
                ProIIStreamData data = new ProIIStreamData();
                data.Tray = prodtrays[i];
                data.ProdType = producttypes[i];
                dic.Add(productdatas[i], data);

            }
            return dic;
        }

        public string GetCriticalPressure(string phaseName)
        {
            string otype = "PhaseEnvel";
            string attr = "CriticalPress";
            CP2Object eq = (CP2Object)cp2File.ActivateObject(otype, phaseName);
            object value = eq.GetAttribute(attr);
            return value.ToString();
        }
        public string GetCriticalTemperature(string phaseName)
        {
            string otype = "PhaseEnvel";
            string attr = "CriticalTemp";
            CP2Object eq = (CP2Object)cp2File.ActivateObject(otype, phaseName);
            object value = eq.GetAttribute(attr);
            return value.ToString();
        }
        public string GetCricondenbarPress(string phaseName)
        {
            string otype = "PhaseEnvel";
            string attr = "CricondenbarPress";
            CP2Object eq = (CP2Object)cp2File.ActivateObject(otype, phaseName);
            object value = eq.GetAttribute(attr);
            return value.ToString();
        }

        public string GetCricondenbarTemp(string phaseName)
        {
            string otype = "PhaseEnvel";
            string attr = "CricondenbarTemp";
            CP2Object eq = (CP2Object)cp2File.ActivateObject(otype, phaseName);
            object value = eq.GetAttribute(attr);
            return value.ToString();
        }

        public double[] GetCompInInfo(string compname)
        {
            double[] data = new double[4];
            CP2Object eq = (CP2Object)cp2File.ActivateObject("CompIn", compname);
            double d1 = double.Parse(eq.GetAttribute("CritPressLibr").ToString());
            double d2 = double.Parse(eq.GetAttribute("CritTempLibr").ToString());
            data[0] = d1;
            return data;
        }
    }
}
