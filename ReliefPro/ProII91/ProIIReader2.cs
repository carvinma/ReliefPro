using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using P2Wrap91;
using ReliefProCommon.CommonLib;
using ProII;
using ProII.Common;
using ReliefProModel;

namespace ProII911
{
    class ProIIReader2
    {
        string[] arrStreamAttributes = { "Pressure", "Temperature", "VaporFraction", "VaporZFmKVal", "TotalComposition", "TotalMolarEnthalpy", "TotalMolarRate", "InertWeightEnthalpy", "InertWeightRate" };
        string[] arrBulkPropAttributes = { "BulkMwOfPhase", "BulkDensityAct", "BulkViscosity", "BulkCPCVRatio", "BulkCP", "BulkThermalCond", "BulkSurfTension" };

        string[] arrColumnAttributes = { "PressureDrop", "Duty","NumberOfTrays", "HeaterNames", "HeaterDuties", "HeaterNumber", "HeaterPANumberfo", "HeaterRegOrPAFlag", "HeaterTrayLoc", "HeaterTrayNumber" };
        string[] arrColumnInAttributes = { "ProdType", "FeedTrays", "ProdTrays", "FeedData", "ProductData" };

        string przFileName;
        CP2File cp2File;
        CP2Object objCompCalc;
        CP2ServerClass cp2Srv;

        string ComponentIds = string.Empty;
        string CompIns = string.Empty;
        Dictionary<string, FeedInfo> dicFeedInfo = new Dictionary<string, FeedInfo>();
        Dictionary<string, ProductInfo> dicProductInfo = new Dictionary<string, ProductInfo>();

        public void InitProIIPicker(string przFileFullName)
        {
            przFileName = System.IO.Path.GetFileName(przFileFullName);
            cp2Srv = new CP2ServerClass();
            cp2Srv.Initialize();
            cp2File = (CP2File)cp2Srv.OpenDatabase(przFileFullName);

            objCompCalc = (CP2Object)cp2File.ActivateObject("CompCalc", "CompCalc");
            object ComponentId = objCompCalc.GetAttribute("ComponentId");

            ComponentIds = ConvertExt.ObjectToString(ComponentId);
            object CompIn = cp2File.GetObjectNames("CompIn");
            CompIns = ConvertExt.ObjectToString(CompIn);
        }



        //获得设备和物流线的个数和名字信息
        public int getAllEqAndStreamTotal(IList<string> eqTypeList, ref IList<EqInfo> eqList, ref IList<string> streamList)
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

        private int getEqTotal(IList<string> eqTypeList, ref IList<EqInfo> eqList)
        {
            int eqCount = 0;
            foreach (string  s in eqTypeList)
            {
                string otype = s;
                int objCount = cp2File.GetObjectCount(otype);
                if (objCount > 0)
                {
                    object objectnames = cp2File.GetObjectNames(otype);
                    if (objectnames is Array)
                    {
                        string[] oNames = (string[])objectnames;
                        foreach (string name in oNames)
                        {
                            EqInfo eq = new EqInfo();
                            eq.eqName = name;
                            eq.eqType = otype;                           
                            eqList.Add(eq);
                        }
                    }
                    else
                    {
                        EqInfo eq = new EqInfo();
                        eq.eqName = objectnames.ToString();
                        eq.eqType = otype;                        
                        eqList.Add(eq);
                    }
                }
                eqCount = eqCount + objCount;
            }
            return eqCount;
        }

        public void getEqInfo(string otype, string name, ref IList<ProIIEqData> eqListData)
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
                        case "FeedData":
                            data.FeedData = value;
                            break;
                        case "ProductData":
                            data.ProductData = value;
                            break;
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
                P2Wrap91.CP2Object objColumnIn = (P2Wrap91.CP2Object)cp2File.ActivateObject("ColumnIn", name);

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
                string[] feeds = data.FeedData.ToString().Split(',');
                string[] feedtrays = data.FeedTrays.ToString().Split(',');
                string[] prods = data.ProductData.ToString().Split(',');
                string[] prodtypes = data.ProdType.ToString().Split(',');
                string[] prodtrays = data.ProdTrays.ToString().Split(',');
                for (int i = 0; i < feeds.Length; i++)
                {
                    FeedInfo info = new FeedInfo();
                    info.FeedName = feeds[i];
                    info.FeedTray = feedtrays[i];
                    dicFeedInfo.Add(info.FeedName, info);
                }
                for (int i = 0; i < prods.Length; i++)
                {
                    ProductInfo info = new ProductInfo();
                    info.ProductName = prods[i];
                    info.ProductTray = prodtrays[i];
                    info.ProductType = prodtypes[i];
                    dicProductInfo.Add(info.ProductName, info);

                }
            }
            eqListData.Add(data);
        }

        public void getSteamInfo(string name, ref IList<ProIIStreamData> streamListData)
        {
            ProIIStreamData data = new ProIIStreamData();
            bool bCalulate = cp2File.CalculateStreamProps(name);
            data.SourceFile = przFileName;
            data.StreamName = name;
            data.ProdType = "";
            data.Tray = "";
            
            if (dicFeedInfo.Keys.Contains(name))
            {
                data.Tray = dicFeedInfo[name].FeedTray;
            }
            if (dicProductInfo.Keys.Contains(name))
            {
                data.Tray = dicProductInfo[name].ProductTray;
                data.ProdType = dicProductInfo[name].ProductType;
            }
            data.CompIn = CompIns;
            data.Componentid = ComponentIds;
            CP2Object objStream = (CP2Object)cp2File.ActivateObject("Stream", name);
            foreach (string s in arrStreamAttributes)
            {
                object v = objStream.GetAttribute(s);
                string value = v.ToString();
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
                    string value = v.ToString();
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
                //Marshal.FinalReleaseComObject(objBulkDrop);
                // GC.ReRegisterForFinalize(objBulkDrop);
            }

            streamListData.Add(data);
        }

        public void ReleaseProIIPicker()
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
        public void CopyStream(string columnName, int tray, int phase, int trayFlow, ref CustomStream cstream)
        {
            cstream = new CustomStream();
            string streamName = "temp" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            CP2Object tempStream = (CP2Object)cp2File.CreateObject("Stream", streamName);

            bool b = cp2File.CopyTrayToStream(columnName, (short)tray, (p2Phase)phase, 0, (p2TrayFlow)trayFlow, streamName);

            string bb = b.ToString();
            bool bCalulate = cp2File.CalculateStreamProps(streamName);

            CP2Object compCalc = (CP2Object)cp2File.ActivateObject("CompCalc", "CompCalc");
            object ComponentId = compCalc.GetAttribute("ComponentId");           
            object CompIn = cp2File.GetObjectNames("CompIn");
            
            cstream.Componentid=ConvertExt.ObjectToString(ComponentId);
            cstream.CompIn=ConvertExt.ObjectToString(CompIn);
            cstream.StreamName=streamName;
            cstream.Tray = "1";
            cstream.ProdType = "2";
            CP2Object curStream = (CP2Object)cp2File.ActivateObject("Stream", streamName);
            foreach (string s in arrStreamAttributes)
            {
                object v = curStream.GetAttribute(s);
                string value = ConvertExt.ObjectToString(v);
                switch (s)
                {
                    case "Pressure":
                        cstream.Pressure = value;
                        break;
                    case "Temperature":
                        cstream.Temperature = value;
                        break;
                    case "VaporFraction":
                        cstream.VaporFraction = value;
                        break;
                    case "VaporZFmKVal":
                        cstream.VaporZFmKVal = value;
                        break;
                    case "TotalComposition":
                        cstream.TotalComposition = value;
                        break;
                    case "TotalMolarEnthalpy":
                        cstream.TotalMolarEnthalpy = value;
                        break;
                    case "TotalMolarRate":
                        cstream.TotalMolarRate = value;
                        break;
                    case "InertWeightEnthalpy":
                        cstream.InertWeightEnthalpy = value;
                        break;
                    case "InertWeightRate":
                        cstream.InertWeightRate = value;
                        break;

                }
            }
            if (bCalulate)
            {                
                CP2Object bulkDrop = (CP2Object)cp2File.ActivateObject("SrBulkProp", streamName);
                foreach (string s in arrBulkPropAttributes)
                {
                    object v = bulkDrop.GetAttribute(s);
                    string value = ConvertExt.ObjectToString(v);
                    switch (s)
                    {
                        case "BulkMwOfPhase":
                            cstream.BulkMwOfPhase = value;
                            break;
                        case "BulkDensityAct":
                            cstream.BulkDensityAct = value;
                            break;
                        case "VaporFraction":
                            cstream.Pressure = value;
                            break;
                        case "BulkViscosity":
                            cstream.BulkViscosity = value;
                            break;
                        case "BulkCPCVRatio":
                            cstream.BulkCPCVRatio = value;
                            break;
                        case "BulkCP":
                            cstream.BulkCP = value;
                            break;
                        case "BulkThermalCond":
                            cstream.BulkThermalCond = value;
                            break;                       
                        case "BulkSurfTension":
                            cstream.BulkSurfTension = value;
                            break;
                    }
                }
            }

            cp2File.DeleteObject("Stream", streamName);
            

        }

        
    }
}
