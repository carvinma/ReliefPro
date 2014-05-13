using System.Data;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.IO;
using ProII;
using P2Wrap91;
using ReliefProCommon.CommonLib;


namespace ProII91
{
    public class ProIIReader : IProIIReader
    {
        string[] arrStreamAttributes = { "Pressure", "Temperature", "VaporFraction", "VaporZFmKVal", "TotalComposition", "TotalMolarEnthalpy", "TotalMolarRate", "InertWeightEnthalpy", "InertWeightRate" };
        string[] arrBulkPropAttributes = { "BulkMwOfPhase", "BulkDensityAct", "BulkViscosity", "BulkCPCVRatio", "BulkCP", "BulkThermalCond", "BulkSurfTension" };

        string[] arrColumnAttributes = { "NumberOfTrays", "HeaterNames", "HeaterDuties", "HeaterNumber", "HeaterPANumberfo", "HeaterRegOrPAFlag", "HeaterTrayLoc", "HeaterTrayNumber" };
        string[] arrColumnInAttributes = { "ProdType", "FeedTrays", "ProdTrays", "FeedData", "ProductData" };
        string[] arrFlashAttributes = { "FeedData", "ProductData", "PressCalc", "TempCalc", "DutyCalc", "Type", "ProductStoreData" };

        Dictionary<string, FeedInfo> dicFeedInfo = new Dictionary<string, FeedInfo>();
        Dictionary<string, ProductInfo> dicProductInfo = new Dictionary<string, ProductInfo>();

        string przFileName;
        //string przFileFullName;
        CP2File cp2File;
        CP2Object objCompCalc;
        CP2ServerClass cp2Srv;

        string ComponentIds = string.Empty;
        string CompIns = string.Empty;

        public string Read()
        {
            return "";
        }

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
        public int getAllEqAndStreamTotal(DataTable dtEqType, ref ArrayList eqList, ref ArrayList streamList)
        {
            int total = 0;
            int eqCount = getEqTotal(dtEqType, ref eqList);
            int streamCount = getStreamTotal(ref streamList);
            total = eqCount + streamCount;
            return total;
        }

        private int getStreamTotal(ref ArrayList streamList)
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

        private int getEqTotal(DataTable dtEqType, ref ArrayList eqList)
        {
            int eqCount = 0;
            foreach (DataRow dr in dtEqType.Rows)
            {
                string otype = dr["eqtypename"].ToString();
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
                            if (otype == "Column")
                            {
                                eq.isColumn = true;
                            }
                            eqList.Add(eq);
                        }
                    }
                    else
                    {
                        EqInfo eq = new EqInfo();
                        eq.eqName = objectnames.ToString();
                        eq.eqType = otype;
                        if (otype == "Column")
                        {
                            eq.isColumn = true;
                        }
                        eqList.Add(eq);
                    }
                    eqCount = eqCount + objCount;
                }
                
            }
            return eqCount;
        }

        public void getEqInfo(string otype, string name, ref DataTable dtEqList)
        {
            DataRow drEq = dtEqList.NewRow();
            CP2Object eq = (CP2Object)cp2File.ActivateObject(otype, name);
            drEq["eqtype"] = otype;
            drEq["eqname"] = name;
            drEq["sourcefile"] = przFileName;
            if (otype == "Column" || otype == "SideColumn")
            {
                foreach (string col in arrColumnAttributes)
                {
                    object value = eq.GetAttribute(col);
                    string strValue = ConvertExt.ObjectToString(value);
                    drEq[col] = strValue;
                }
                P2Wrap91.CP2Object objColumnIn = (P2Wrap91.CP2Object)cp2File.ActivateObject("ColumnIn", name);
                foreach (string col in arrColumnInAttributes)
                {
                    object value = objColumnIn.GetAttribute(col);
                    string strValue = ConvertExt.ObjectToString(value);
                    drEq[col] = strValue;
                }
                string[] feeds = drEq["feeddata"].ToString().Split(',');
                string[] feedtrays = drEq["feedtrays"].ToString().Split(',');
                string[] prods = drEq["ProductData"].ToString().Split(',');
                string[] prodtypes = drEq["prodtype"].ToString().Split(',');
                string[] prodtrays = drEq["prodtrays"].ToString().Split(',');
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
            else if (otype == "Flash")
            {
                foreach (string col in arrFlashAttributes)
                {
                    object value = eq.GetAttribute(col);
                    string strValue = ConvertExt.ObjectToString(value);
                    drEq[col] = strValue;
                }
            }
            dtEqList.Rows.Add(drEq);

        }

        public void getSteamInfo(string name, ref DataTable dtStream)
        {
            try
            {
                DataRow r = dtStream.NewRow();
                r["streamname"] = name;
                r["sourcefile"] = przFileName;
                r["prodtype"] = "";
                r["tray"] = "";
                if (dicFeedInfo.Keys.Contains(name))
                {
                    r["tray"] = dicFeedInfo[name].FeedTray;
                }
                if (dicProductInfo.Keys.Contains(name))
                {
                    r["tray"] = dicProductInfo[name].ProductTray;
                    r["prodtype"] = dicProductInfo[name].ProductType;
                }
                r["CompIn"] = CompIns;
                r["ComponentId"] = ComponentIds;
                try
                {
                    CP2Object objStream = (CP2Object)cp2File.ActivateObject("Stream", name);
                    foreach (string col in arrStreamAttributes)
                    {
                        object value = objStream.GetAttribute(col);
                        r[col] = ConvertExt.ObjectToString(value);
                    }
                    if (r["TotalMolarRate"].ToString() != "0")
                    {
                        Marshal.FinalReleaseComObject(objStream);
                        GC.ReRegisterForFinalize(objStream);

                        bool bCalulate = cp2File.CalculateStreamProps(name);
                        if (bCalulate)
                        {
                            CP2Object objBulkDrop = (CP2Object)cp2File.ActivateObject("SrBulkProp", name);
                            foreach (string col in arrBulkPropAttributes)
                            {
                                object value = objBulkDrop.GetAttribute(col);
                                r[col] = ConvertExt.ObjectToString(value);
                            }
                            //Marshal.FinalReleaseComObject(objBulkDrop);
                            // GC.ReRegisterForFinalize(objBulkDrop);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                dtStream.Rows.Add(r);
            }
            catch (Exception ex2)
            {
            }
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
        public void CopyStream(string columnName, int tray,int phase,int trayFlow ,ref DataTable dtStream)
        {
            string streamName = "temp" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            CP2Object tempStream = (CP2Object)cp2File.CreateObject("Stream", streamName);

            bool b = cp2File.CopyTrayToStream(columnName, (short)tray, (p2Phase)phase, 0, (p2TrayFlow)trayFlow, streamName);

            string bb = b.ToString();
            DataRow dr = dtStream.NewRow();
            bool bCalulate = cp2File.CalculateStreamProps(streamName);

            CP2Object compCalc = (CP2Object)cp2File.ActivateObject("CompCalc", "CompCalc");
            object ComponentId = compCalc.GetAttribute("ComponentId");
            dr["ComponentId"] = ConvertExt.ObjectToString(ComponentId);
            object CompIn = cp2File.GetObjectNames("CompIn");
            dr["CompIn"] = ConvertExt.ObjectToString(CompIn);
            dr["streamname"] = streamName;
            
            dr["tray"] = 1;
            dr["prodtype"] = 2;
            CP2Object curStream = (CP2Object)cp2File.ActivateObject("Stream", streamName);
            foreach (string s in arrStreamAttributes)
            {
                object v = curStream.GetAttribute(s);
                dr[s] = ConvertExt.ObjectToString(v);
            }
            if (bCalulate)
            {
                CP2Object bulkDrop = (CP2Object)cp2File.ActivateObject("SrBulkProp", streamName);
                foreach (string s in arrBulkPropAttributes)
                {
                    object v = bulkDrop.GetAttribute(s);
                    dr[s] = ConvertExt.ObjectToString(v);
                }
            }

            cp2File.DeleteObject("Stream", streamName);
            dtStream.Rows.Add(dr);

        }


        public void getDataFromFile(ref DataTable dtEqType, ref DataTable dtEqlist, ref DataTable dtStream)
        {
            try
            {
                List<string> streamList = new List<string>();
                objCompCalc = (CP2Object)cp2File.ActivateObject("CompCalc", "CompCalc");
                foreach (DataRow dr in dtEqType.Rows)
                {
                    string otype = dr["eqtypename"].ToString();
                    object objectnames = cp2File.GetObjectNames(otype);
                    if (objectnames.ToString() != "")
                    {
                        if (objectnames is Array)
                        {
                            string[] oNames = (string[])objectnames;
                            foreach (string name in oNames)
                            {
                                getEqDataFromFile(otype, name, ref streamList, ref dtEqlist, ref dtStream);
                            }
                        }
                        else
                        {
                            string name = (string)objectnames;
                            getEqDataFromFile(otype, name, ref streamList, ref dtEqlist, ref dtStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                //CloseReader();
            }

        }



        private void getEqDataFromFile(string otype, string name, ref List<string> streamList, ref DataTable dtEqlist, ref DataTable dtStream)
        {
            DataRow r = dtEqlist.NewRow();
            CP2Object eq = (CP2Object)cp2File.ActivateObject(otype, name);
            object feeddata = eq.GetAttribute("FeedData");
            object productdata = eq.GetAttribute("ProductData");
            string strfeeddata = ConvertExt.ObjectToString(feeddata);
            string strproductdata = ConvertExt.ObjectToString(productdata);
            string strprodtype = string.Empty;
            r["eqtype"] = otype;
            r["eqname"] = name;
            r["sourcefile"] = przFileName;
            r["FeedData"] = strfeeddata;
            r["ProductData"] = strproductdata;
            if (otype == "Column")
            {
                foreach (string s in arrColumnAttributes)
                {
                    object v = eq.GetAttribute(s);
                    string strV = ConvertExt.ObjectToString(v);
                    r[s] = strV;
                }
                P2Wrap91.CP2Object obj = (P2Wrap91.CP2Object)cp2File.ActivateObject("ColumnIn", name);
                foreach (string s in arrColumnInAttributes)
                {
                    object v = obj.GetAttribute(s);
                    string strV = ConvertExt.ObjectToString(v);
                    r[s] = strV;
                }


                Marshal.FinalReleaseComObject(obj);
                GC.ReRegisterForFinalize(obj);

            }
            dtEqlist.Rows.Add(r);
            Marshal.FinalReleaseComObject(eq);
            GC.ReRegisterForFinalize(eq);


            string[] feeds = r["feeddata"].ToString().Split(',');
            string[] feedtrays = r["feedtrays"].ToString().Split(',');
            for (int i = 0; i < feeds.Length; i++)
            {
                string s = feeds[i];
                if (!streamList.Contains(s))
                {
                    streamList.Add(s);
                    if (otype == "Column")
                        getStreamDataFromFile(s, "", feedtrays[i], ref dtStream);
                    else
                        getStreamDataFromFile(s, "", "", ref dtStream);

                }
            }
            string[] prods = r["ProductData"].ToString().Split(',');
            string[] prodtypes = r["prodtype"].ToString().Split(',');
            string[] prodtrays = r["prodtrays"].ToString().Split(',');
            for (int i = 0; i < prods.Length; i++)
            {
                string s = prods[i];
                if (!streamList.Contains(s))
                {
                    streamList.Add(s);
                    if (otype == "Column")
                        getStreamDataFromFile(s, prodtypes[i], prodtrays[i], ref dtStream);
                    else
                        getStreamDataFromFile(s, "", "", ref dtStream);

                }
            }
        }
        private void getStreamDataFromFile(string name, string prodtype, string tray, ref DataTable dtStream)
        {
            try
            {
                DataRow r = dtStream.NewRow();

                r["streamname"] = name;
                r["sourcefile"] = przFileName;
                r["prodtype"] = prodtype;
                r["tray"] = tray;
                object ComponentId = objCompCalc.GetAttribute("ComponentId");
                r["ComponentId"] = ConvertExt.ObjectToString(ComponentId);
                object CompIn = cp2File.GetObjectNames("CompIn");
                r["CompIn"] = ConvertExt.ObjectToString(CompIn);
                try
                {
                    CP2Object objStream = (CP2Object)cp2File.ActivateObject("Stream", name);

                    foreach (string s in arrStreamAttributes)
                    {
                        object v = objStream.GetAttribute(s);
                        r[s] = ConvertExt.ObjectToString(v);
                    }
                    Marshal.FinalReleaseComObject(objStream);
                    GC.ReRegisterForFinalize(objStream);

                    bool bCalulate = cp2File.CalculateStreamProps(name);
                    if (bCalulate)
                    {
                        CP2Object objBulkDrop = (CP2Object)cp2File.ActivateObject("SrBulkProp", name);
                        foreach (string s in arrBulkPropAttributes)
                        {
                            object v = objBulkDrop.GetAttribute(s);
                            r[s] = ConvertExt.ObjectToString(v);
                        }
                        //Marshal.FinalReleaseComObject(objBulkDrop);
                        //GC.ReRegisterForFinalize(objBulkDrop);

                    }
                }
                catch (Exception ex)
                {
                }

                dtStream.Rows.Add(r);
            }
            catch (Exception ex2)
            {
            }
        }

        public string GetCriticalPressure(string phaseName)
        {
            string otype = "PhaseEnvel";
            string attr = "CriticalPress";
            CP2Object eq = (CP2Object)cp2File.ActivateObject(otype, phaseName);
            object value = eq.GetAttribute(attr);
            return value.ToString();
        }

    }

    public class ProductInfo
    {
        public string ProductName;
        public string ProductTray;
        public string ProductType;
    }
    public class FeedInfo
    {
        public string FeedName;
        public string FeedTray;
    }
    public class EqInfo
    {
        public string eqName;
        public string eqType;
        public bool isColumn;
    }
}
