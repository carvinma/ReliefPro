using ReliefProModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProII
{
    public class ProIIReader
    {
        private string macroFilePath = AppDomain.CurrentDomain.BaseDirectory + @"template\macro.xls";
        private string version;
        private string przFilePath;
        private string przFileName;
        public ProIIReader(string version, string przFilePath)
        {
            this.version = version;
            this.przFilePath = przFilePath;
            FileInfo fi = new FileInfo(przFilePath);
            przFileName= fi.Name;
        }
        public int[] GetAllCount()
        {
            string action = "GetAllCount";
            string przFile = string.Empty;
            object objRtn = new object();
            ExcelMacroHelper.RunExcelMacro(macroFilePath, action,
                                                        new Object[] { version, przFilePath },
                                                         out objRtn,
                                                         false
                                                    );
            int[] result=new int[2];
            if (objRtn is Array)
            {
                object[] arr = (object[])objRtn;
                result[0] =int.Parse( arr[0].ToString());
                result[1] = int.Parse(arr[1].ToString());
            }
            return result;
        }

        public List<ProIIEqData> GetAllEqInfo()
        {
            List<ProIIEqData> lst = new List<ProIIEqData>();
            string action = "GetAllEqInfo2";
            object objRtn = new object();
            ExcelMacroHelper.RunExcelMacro(macroFilePath,
                                                         action,
                                                        new Object[] { version, przFilePath },
                                                         out objRtn,
                                                         false);
            if (objRtn is Array)
            {
                object[] arrEq = (object[])objRtn;
                foreach (object obj in arrEq)
                {
                    if (obj is Array)
                    {                      
                        object[] eq = (object[])obj;
                        ProIIEqData data = ConvertToProIIEqData(eq);
                        lst.Add(data);
                    }
                }
            }
            return lst;

        }

        public List<ProIIStreamData> GetAllStreamInfo(int start,int end)
        {
            List<ProIIStreamData> lst = new List<ProIIStreamData>();
            string action = "GetStreamInfo2";
            object objRtn = new object();
            ExcelMacroHelper.RunExcelMacro(macroFilePath,
                                                         action,
                                                        new Object[] { version, przFilePath,start,end },
                                                         out objRtn,
                                                         false);
            if (objRtn is Array)
            {
                object[] arrStream = (object[])objRtn;
                foreach (object obj in arrStream)
                {
                    if (obj != null && obj is Array)
                    {
                        object[] ps = (object[])obj;
                        ProIIStreamData data = ConvertToProIIStreamData(ps);
                        lst.Add(data);
                    }
                }
            }
            return lst;

        }

        public List<ProIIStreamData> GetStreamInfo(string[] streamNames)
        {
            List<ProIIStreamData> lst = new List<ProIIStreamData>();
            string action = "GetStreamInfo";
            object objRtn = new object();
            ExcelMacroHelper.RunExcelMacro(macroFilePath,
                                                         action,
                                                        new Object[] { version, przFilePath, streamNames },
                                                         out objRtn,
                                                         false);
            if (objRtn is Array)
            {
                object[] arrStream = (object[])objRtn;
                foreach (object obj in arrStream)
                {
                    if (obj != null && obj is Array)
                    {
                        object[] ps = (object[])obj;
                        ProIIStreamData data = ConvertToProIIStreamData(ps);
                        lst.Add(data);
                    }
                }
            }
            return lst;

        }

        public ProIIStreamData GetStreamInfo(string streamName)
        {
            ProIIStreamData data = null;
            string action = "GetSingleStreamInfo";
            object objRtn = new object();
            ExcelMacroHelper.RunExcelMacro(macroFilePath,
                                                         action,
                                                        new Object[] { version, przFilePath, streamName },
                                                         out objRtn,
                                                         false);
            if (objRtn is Array)
            {
                object[] ps = (object[])objRtn;
                data = ConvertToProIIStreamData(ps);
            }
            return data;

        }


        public ProIIEqData GetEqInfo(string eqType,string eqName)
        {
            ProIIEqData data = null;
            string action = "GetEqInfo";
            object objRtn = new object();
            ExcelMacroHelper.RunExcelMacro(macroFilePath,
                                                         action,
                                                        new Object[] { version, przFilePath,eqType,eqName },
                                                         out objRtn,
                                                         false);
            if (objRtn is Array)
            {               
                object[] eq = (object[])objRtn;
                data = ConvertToProIIEqData(eq);
            }
            return data;

        }

        public double[] GetPHInfo( string eqName)
        {
            double[] data= new double[4];
            string action = "GetEqInfo";
            object objRtn = new object();
            ExcelMacroHelper.RunExcelMacro(macroFilePath,
                                                         action,
                                                        new Object[] { version, przFilePath, "PhaseEnvel", eqName },
                                                         out objRtn,
                                                         false);
            if (objRtn is Array)
            {
                object[] arr = (object[])objRtn;
                data[0] = double.Parse(arr[0].ToString());
                data[1] = double.Parse(arr[1].ToString());
                data[2] = double.Parse(arr[2].ToString());
                data[3] = double.Parse(arr[3].ToString());
            }

            return data;
        }

        public double[] GetCompInInfo(string eqName)
        {
            double[] data = new double[2];
            string action = "GetEqInfo";
            object objRtn = new object();
            ExcelMacroHelper.RunExcelMacro(macroFilePath,
                                                         action,
                                                        new Object[] { version, przFilePath, "CompIn", eqName },
                                                         out objRtn,
                                                         false);
            if (objRtn is Array)
            {
                object[] arr = (object[])objRtn;
                data[0] = double.Parse(arr[0].ToString());
                data[1] = double.Parse(arr[1].ToString());
               
            }

            return data;
        }


        public ProIIStreamData CopyStreamInfo(string columnName, int tray, int phase, int trayFlow)
        {
            ProIIStreamData data = null;
            string action = "CopyStreamInfo";
            object objRtn = new object();
            ExcelMacroHelper.RunExcelMacro(macroFilePath,
                                                         action,
                                                        new Object[] { version, przFilePath,columnName,tray,phase,trayFlow },
                                                         out objRtn,
                                                         false);
            if (objRtn is Array)
            {               
                object[] ps = (object[])objRtn;
                data = ConvertToProIIStreamData(ps);
            }
            return data;

        }

        private  string ObjectToString(object obj)
        {
            string rs = string.Empty;
            if (obj is Array)
            {
                object[] objdata = (System.Object[])obj;
                foreach (object s in objdata)
                {
                    if (s != null)
                    {
                        string v = s.ToString();
                        if (v != "")
                        {
                            rs = rs + "," + v;
                        }
                    }
                }
                rs = rs.Substring(1);
            }
            else if (obj == null)
            {
                rs = "";
            }
            else
                rs = obj.ToString();

            return rs;
        }

        private ProIIStreamData ConvertToProIIStreamData(object[] ps)
        {
            ProIIStreamData data = new ProIIStreamData();
            data.SourceFile = przFileName;
            data.StreamName = ObjectToString(ps[0]);
            data.Pressure = ObjectToString(ps[1]);
            data.Temperature = ObjectToString(ps[2]);
            data.VaporFraction = ObjectToString(ps[3]);
            data.VaporZFmKVal = ObjectToString(ps[4]);
            data.TotalComposition = ObjectToString(ps[5]);
            data.TotalMolarEnthalpy = ObjectToString(ps[6]);
            data.TotalMolarRate = ObjectToString(ps[7]);
            data.InertWeightEnthalpy = ObjectToString(ps[8]);
            data.InertWeightRate = ObjectToString(ps[9]);
            data.BulkMwOfPhase = ObjectToString(ps[10]);
            data.BulkDensityAct = ObjectToString(ps[11]);
            data.BulkViscosity = ObjectToString(ps[12]);
            data.BulkCPCVRatio = ObjectToString(ps[13]);
            data.BulkCP = ObjectToString(ps[14]);
            data.BulkThermalCond = ObjectToString(ps[15]);
            data.BulkSurfTension = ObjectToString(ps[16]);
            data.Componentid = ObjectToString(ps[17]);
            data.PrintNumber = ObjectToString(ps[18]);
            return data;
        }

        private ProIIEqData ConvertToProIIEqData(object[] eq)
        {
            ProIIEqData data = new ProIIEqData();
            data.SourceFile = przFileName;
            if (eq[0].ToString() == "Column")
            {                
                data.EqType = ObjectToString(eq[0]);
                data.EqName = ObjectToString(eq[1]);
                data.NumberOfTrays = ObjectToString(eq[2]);
                data.HeaterNames = ObjectToString(eq[3]);
                data.HeaterDuties = ObjectToString(eq[4]);
                data.HeaterNumber = ObjectToString(eq[5]);
                data.HeaterPANumberfo = ObjectToString(eq[6]);
                data.HeaterRegOrPAFlag = ObjectToString(eq[7]);
                data.HeaterTrayLoc = ObjectToString(eq[8]);

                data.HeaterTrayNumber = ObjectToString(eq[9]);
                data.ProdType = ObjectToString(eq[10]);
                data.FeedTrays = ObjectToString(eq[11]);
                data.ProdTrays = ObjectToString(eq[12]);
                data.FeedData = ObjectToString(eq[13]);
                data.ProductData = ObjectToString(eq[14]);

            }
            else if (eq[0].ToString() == "SideColumn")
            {
                data.EqType = ObjectToString(eq[0]);
                data.EqName = ObjectToString(eq[1]);
                data.NumberOfTrays = ObjectToString(eq[2]);
                data.HeaterNames = ObjectToString(eq[3]);
                data.HeaterDuties = ObjectToString(eq[4]);
                data.HeaterNumber = ObjectToString(eq[5]);
                data.HeaterPANumberfo = ObjectToString(eq[6]);
                data.HeaterRegOrPAFlag = ObjectToString(eq[7]);
                data.HeaterTrayLoc = ObjectToString(eq[8]);

                data.HeaterTrayNumber = ObjectToString(eq[9]);
                data.ProdType = ObjectToString(eq[10]);
                data.FeedTrays = ObjectToString(eq[11]);
                data.ProdTrays = ObjectToString(eq[12]);
                data.FeedData = ObjectToString(eq[13]);
                data.ProductData = ObjectToString(eq[14]);

            }
            else if (eq[0].ToString() == "Flash")
            {
                data.EqType = "Flash";
                data.EqName = ObjectToString(eq[1]);
                data.FeedData = ObjectToString(eq[2]);
                data.ProductData = ObjectToString(eq[3]);
                data.PressCalc = ObjectToString(eq[4]);
                data.TempCalc = ObjectToString(eq[5]);
                data.DutyCalc = ObjectToString(eq[6]);
                data.Type = ObjectToString(eq[7]);
                data.ProductStoreData = ObjectToString(eq[8].ToString());

            }
            else if (eq[0].ToString() == "Hx")
            {
                data.EqType = "Hx";
                data.EqName = ObjectToString(eq[1]);
                data.FeedData = ObjectToString(eq[2]);
                data.DutyCalc = ObjectToString(eq[3]);
                data.ProductStoreData = ObjectToString(eq[4]);
                data.LmtdCalc = ObjectToString(eq[5]);
                data.LmtdFactorCalc = ObjectToString(eq[6]);
                data.FirstFeed = ObjectToString(eq[7]);
                data.FirstProduct = ObjectToString(eq[8]);
                data.LastFeed = ObjectToString(eq[9]);
                data.LastProduct = ObjectToString(eq[10]);

            }
            else if (eq[0].ToString() == "Compressor")
            {
                data.EqType = "Compressor";
                data.EqName = ObjectToString(eq[1]);
                data.FeedData = ObjectToString(eq[2]);
                data.ProductData = ObjectToString(eq[3]);
                data.ProductStoreData = ObjectToString(eq[4]);

            }
            else if (eq[0] == "Splitter")
            {
                data.EqType = "Splitter";
                data.EqName = ObjectToString(eq[1]);
                data.FeedData = ObjectToString(eq[2]);
                data.ProductData = ObjectToString(eq[3]);

            }
            else if (eq[0] == "Mixer")
            {
                data.EqType = "Mixer";
                data.EqName = ObjectToString(eq[1]);
                data.FeedData = ObjectToString(eq[2]);
                data.ProductData = ObjectToString(eq[3]);

            }
            return data;
        }
    }
}
