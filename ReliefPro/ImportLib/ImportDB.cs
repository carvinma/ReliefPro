using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.OleDb;
//using UOMLib;
namespace ImportLib
{
    /// <summary>
    /// 该类是从读取
    /// </summary>
    public class ImportDB
    {
        public string ConnectionString;
        public OleDbConnection Connection = new OleDbConnection();

        public ImportDB(string dbFile)
        {
            ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbFile + ";Persist Security Info=False;";
            Connection = new OleDbConnection(ConnectionString);
        }
        public ImportDB()
        {
            string dbFile = AppDomain.CurrentDomain.BaseDirectory.ToString() + "template.accdb";
            ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbFile + ";Persist Security Info=False;";
            Connection = new OleDbConnection(ConnectionString);
        }       

        //获取ProII设备类型
        public DataTable GetEqTypeData()
        {
            try
            {
                DataTable dt = new DataTable();
                string sql = "select * from proIIeqtype order by id";
                OleDbCommand cmd = new OleDbCommand(sql, Connection);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
               
        public DataSet GetStreamListByEq(string eqName, string FeedOrProduct)
        {
            try
            {
                Dictionary<string, string> dictFeeds = new Dictionary<string, string>();
                Dictionary<string, string> dicProducts = new Dictionary<string, string>();
                GetMaincolumnRealFeedProduct(eqName, ref dictFeeds, ref dicProducts);
                StringBuilder realstreams = new StringBuilder();
                if (FeedOrProduct == "ProductData")
                {
                    foreach (string s in dicProducts.Keys)
                    {
                        realstreams.Append(",'").Append(s).Append("'");
                    }
                }
                else
                {
                    foreach (string s in dictFeeds.Keys)
                    {
                        realstreams.Append(",'").Append(s).Append("'");
                    }
                }
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                string sql = "select " + FeedOrProduct + " from proIIeqdata  where eqname='" + eqName + "'";

                OleDbCommand cmd = new OleDbCommand(sql, Connection);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    StringBuilder sb = new StringBuilder();

                    string[] streams = dr[FeedOrProduct].ToString().Split(',');
                    for (int i = 0; i < streams.Length; i++)
                    {
                        string s = streams[i].Trim();
                        if (s != string.Empty)
                        {
                            sb.Append(",'").Append(s).Append("'");
                        }
                    }
                    sb.Remove(0, 1);
                    sql = "select * from proiistreamdata  where streamname in (" + sb.ToString() + ")";
                    if (streams.ToString() != string.Empty)
                    {
                        sql = "select * from proiistreamdata  where   streamname in (" + realstreams.ToString().Substring(1) + ")";
                    }
                    cmd = new OleDbCommand(sql, Connection);
                    da = new OleDbDataAdapter(cmd);
                    da.Fill(ds2);


                    if (ds2 != null && ds2.Tables.Count > 0)
                    {
                        DataTable dt = ds2.Tables[0];
                        DataColumn dcflowrate = new DataColumn("flowrate");
                        DataColumn dcSpH = new DataColumn("specificenthalpy");
                        DataColumn dcH = new DataColumn("enthalpy");

                        dt.Columns.Add(dcflowrate);
                        dt.Columns.Add(dcSpH);
                        dt.Columns.Add(dcH);



                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            DataRow dr2 = dt.Rows[i];
                            string temperature = dr2["Temperature"].ToString();
                            if (temperature != "")
                            {
                                dr2["Temperature"] = temperature;// UnitConverter.unitConv(temperature, "K", "C", "{0:0.0000}");
                            }
                            string pressure = dr2["Pressure"].ToString();
                            if (pressure != "")
                            {
                                dr2["pressure"] = pressure;// UnitConverter.unitConv(pressure, "KPA", "MPAG", "{0:0.0000}");
                            }


                            double TotalMolarRate = 0;
                            if (dr2["TotalMolarRate"].ToString() != "")
                                TotalMolarRate = double.Parse(dr2["TotalMolarRate"].ToString());
                            string bulkmwofphase = dr2["BulkMwOfPhase"].ToString();
                            if (bulkmwofphase != "")
                            {
                                double wf = TotalMolarRate * double.Parse(bulkmwofphase);
                                dr2["FlowRate"] = string.Format("{0:0.0000}", wf * 3600);
                            }

                            //enthalpy=TotalMolarEnthalpy*TotalMolarRate+InertWeightEnthalpy*InertWeightRate;
                            double TotalMolarEnthalpy = 0;
                            if (dr2["TotalMolarEnthalpy"].ToString() != "")
                            {
                                TotalMolarEnthalpy = double.Parse(dr2["TotalMolarEnthalpy"].ToString());
                            }


                            double InertWeightEnthalpy = 0;
                            string strInertWeightEnthalpy = dr2["InertWeightEnthalpy"].ToString();
                            if (strInertWeightEnthalpy != "")
                            {
                                InertWeightEnthalpy = double.Parse(strInertWeightEnthalpy);
                            }
                            double InertWeightRate = 0;
                            string strInertWeightRate = dr2["InertWeightRate"].ToString();
                            if (strInertWeightRate != "")
                            {
                                InertWeightRate = double.Parse(strInertWeightRate);
                            }
                            double Enthalpy = TotalMolarEnthalpy * TotalMolarRate + InertWeightEnthalpy * InertWeightRate;
                            dr2["Enthalpy"] = string.Format("{0:0.0000}", Enthalpy * 3600 / 1000000);

                            //TotalMassRate=IF(TotalMolarRate>0,TotalMolarRate*BulkMwOfPhase,RMISS)
                            double TotalMassRate = 0;
                            if (TotalMolarRate > 0 && bulkmwofphase != "")
                            {
                                TotalMassRate = TotalMolarRate * double.Parse(bulkmwofphase);
                            }

                            //SpEnthalpy=IF(TotalMolarRate+InertWeightRate>0,Enthalpy/(TotalMassRate+InertWeightRate),RMISS)
                            double SpEnthalpy = 0;
                            if (TotalMolarRate + InertWeightRate > 0)
                            {
                                SpEnthalpy = Enthalpy / (TotalMassRate + InertWeightRate);
                            }
                            dr2["specificenthalpy"] = string.Format("{0:0.0000}", SpEnthalpy);




                        }
                    }


                }

                return ds2;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataSet GetHeaterListByEq(string eqName)
        {
            try
            {
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();

                DataTable dtHeatIn = GetTableStructureByTableName("frmcase_reboiler");
                DataTable dtHeatOut = GetTableStructureByTableName("frmcase_condenser");
                ds2.Tables.Add(dtHeatIn);
                ds2.Tables.Add(dtHeatOut);
                string sql = "select HeaterNames,HeaterDuties  from eqlist  where eqname='" + eqName + "'";
                OleDbCommand cmd = new OleDbCommand(sql, Connection);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    string heaterNames = dr["HeaterNames"].ToString();
                    string heaterDuties = dr["HeaterDuties"].ToString();
                    string[] arrHeaterNames = heaterNames.Split(',');
                    string[] arrHeaterDuties = heaterDuties.Split(',');

                    for (int i = 0; i < arrHeaterNames.Length; i++)
                    {

                        if (double.Parse(arrHeaterDuties[i]) >= 0)
                        {
                            DataRow r = dtHeatIn.NewRow();
                            r["heatername"] = arrHeaterNames[i];
                            r["heaterduty"] = arrHeaterDuties[i];
                            r["dutylost"] = false;
                            r["dutycalcfactor"] = 1;
                            dtHeatIn.Rows.Add(r);

                        }
                        else
                        {
                            DataRow r = dtHeatOut.NewRow();
                            r["heatername"] = arrHeaterNames[i];
                            r["heaterduty"] = arrHeaterDuties[i];
                            r["dutylost"] = false;
                            r["dutycalcfactor"] = 1;
                            dtHeatOut.Rows.Add(r);

                        }
                    }

                }
                return ds2;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public void AddDataByDataTable(DataTable dt)
        {
            try
            {
                Connection.Open();
                foreach (DataRow dr in dt.Rows)
                {
                    StringBuilder sb = new StringBuilder();
                    StringBuilder sb2 = new StringBuilder();
                    StringBuilder sb3 = new StringBuilder();

                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName.ToUpper() != "ID")
                        {
                            sb.Append(",").Append(dc.ColumnName);
                            sb2.Append(",'").Append(dr[dc.ColumnName].ToString()).Append("'");
                            sb3.Append(",").Append(dc.ColumnName).Append("='").Append(dr[dc.ColumnName].ToString()).Append("'");
                        }
                    }
                    string fileds = sb.Remove(0, 1).ToString();
                    string values = sb2.Remove(0, 1).ToString();
                    string sql = "insert into " + dt.TableName + "(" + fileds + ")values(" + values + ")";
                    if (dt.TableName == "frmtower")
                    {
                        
                        string towername = dr["towername"].ToString();
                        string strWhere = "towername='" + towername ;
                        if (IsExist(dt.TableName, strWhere))
                        {
                            sql = "update frmtower set " + sb3.Remove(0, 1).ToString() + " where " + strWhere;
                        }
                    }
                    OleDbCommand cmd = new OleDbCommand(sql, Connection);
                    cmd.ExecuteNonQuery();
                }
                Connection.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        public void SaveDataByTableName(DataTable dt)
        {
            Connection.Open();
            try
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = Connection;
                cmd.CommandText = "delete from " + dt.TableName ;
                cmd.ExecuteNonQuery();
                foreach (DataRow dr in dt.Rows)
                {
                    StringBuilder sb = new StringBuilder();
                    StringBuilder sb2 = new StringBuilder();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName.ToUpper() == "ID")
                        {

                        }
                        else
                        {
                            sb.Append(",").Append(dc.ColumnName);
                            if (dc.DataType == typeof(bool))
                            {
                                sb2.Append(",").Append(dr[dc.ColumnName].ToString()).Append("");
                            }
                            else
                            {
                                string value = dr[dc.ColumnName].ToString();
                                if (dc.ColumnName.ToLower().Contains("_color"))
                                {
                                    if (value == "")
                                        value = "green";
                                    sb2.Append(",'").Append(value).Append("'");
                                }
                                else
                                {
                                    sb2.Append(",'").Append(value).Append("'");
                                }
                            }

                        }
                    }
                    string fileds = sb.Remove(0, 1).ToString();
                    string values = sb2.Remove(0, 1).ToString();
                    string sql = "insert into " + dt.TableName + "(" + fileds + ")values(" + values + ")";

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }

                Connection.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
        }

        public void InsertDataByRow(DataRow dr)
        {
            Connection.Open();
            try
            {
                DataTable dt = dr.Table;
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = Connection;

                StringBuilder sb = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                StringBuilder sb4 = new StringBuilder();
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName.ToUpper() == "ID")
                    {
                        sb4.Append("ID=").Append(dr["ID"].ToString());

                    }
                    else
                    {
                        sb.Append(",[").Append(dc.ColumnName).Append("]");
                        if (dc.DataType == typeof(bool))
                        {
                            sb2.Append(",").Append(dr[dc.ColumnName].ToString()).Append("");
                        }
                        else if (dc.DataType == typeof(int))
                        {
                            if (string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                            {
                                sb2.Append(",null");
                            }
                            else
                            {
                                sb2.Append(",").Append(dr[dc.ColumnName].ToString()).Append("");
                            }
                        }
                        else
                        {
                            string value = dr[dc.ColumnName].ToString();
                            if (dc.ColumnName.ToLower().Contains("color"))
                            {
                                if (value == "")
                                    value = "green";
                                sb2.Append(",'").Append(value).Append("'");
                            }
                            else
                            {
                                sb2.Append(",'").Append(value).Append("'");
                            }

                        }

                    }
                }
                string fileds = sb.Remove(0, 1).ToString();
                string values = sb2.Remove(0, 1).ToString();
                string sql = "insert into " + dt.TableName + "(" + fileds + ")values(" + values + ")";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                Connection.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
        }

        public void UpdateDataByRow(DataRow dr)
        {
            Connection.Open();
            try
            {
                DataTable dt = dr.Table;
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = Connection;


                StringBuilder sb3 = new StringBuilder();
                StringBuilder sb4 = new StringBuilder();
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName.ToUpper() == "ID")
                    {
                        sb4.Append("ID=").Append(dr["ID"].ToString());

                    }
                    else
                    {
                        if (dc.DataType == typeof(bool))
                        {
                            sb3.Append(",[").Append(dc.ColumnName).Append("]=").Append(dr[dc.ColumnName].ToString()).Append("");
                        }
                        else if (dc.DataType == typeof(int))
                        {
                            if (string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                            {
                                sb3.Append(",[").Append(dc.ColumnName).Append("]=0");
                            }
                            else
                            {
                                sb3.Append(",[").Append(dc.ColumnName).Append("]=").Append(dr[dc.ColumnName].ToString()).Append("");
                            }
                        }
                        else
                        {
                            string value = dr[dc.ColumnName].ToString();
                            if (dc.ColumnName.ToLower().Contains("color"))
                            {
                                if (value == "")
                                    value = "green";
                                sb3.Append(",[").Append(dc.ColumnName).Append("]='").Append(value).Append("'");
                            }
                            else
                            {
                                sb3.Append(",[").Append(dc.ColumnName).Append("]='").Append(dr[dc.ColumnName].ToString()).Append("'");
                            }

                        }

                    }
                }
                string sql = "update " + dt.TableName + " set " + sb3.Remove(0, 1).ToString() + " where " + sb4.ToString();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                Connection.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
        }

        public void DeleteDataByRow(DataRow dr)
        {
            Connection.Open();
            try
            {
                DataTable dt = dr.Table;
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = Connection;
                string sql = "delete " + dt.TableName + "  where ID=" + dr["ID"].ToString();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();


                Connection.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }

        }


        public DataTable GetTableStructureByTableName(string tableName)
        {
            try
            {
                DataTable dt = new DataTable();
                string sql = "select * from " + tableName + "  where 1=0";
                OleDbCommand cmd = new OleDbCommand(sql, Connection);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);
                dt.TableName = tableName;
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void ImportDataByDataSet(DataSet ds)
        {
            try
            {
                Connection.Open();
                foreach (DataTable dt in ds.Tables)
                {

                    foreach (DataRow dr in dt.Rows)
                    {
                        StringBuilder sb = new StringBuilder();
                        StringBuilder sb2 = new StringBuilder();
                        StringBuilder sb3 = new StringBuilder();

                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (dc.ColumnName.ToUpper() != "ID")
                            {
                                sb.Append(",[").Append(dc.ColumnName).Append("]");
                                sb2.Append(",'").Append(dr[dc.ColumnName].ToString()).Append("'");
                                sb3.Append(",").Append(dc.ColumnName).Append("='").Append(dr[dc.ColumnName].ToString()).Append("'");
                            }
                        }
                        string fileds = sb.Remove(0, 1).ToString();
                        string values = sb2.Remove(0, 1).ToString();
                        string sql = "insert into " + dt.TableName + "(" + fileds + ")values(" + values + ")";


                        OleDbCommand cmd = new OleDbCommand(sql, Connection);
                        cmd.ExecuteNonQuery();
                    }
                }
                Connection.Close();
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
            finally
            {
                Connection.Close();
            }
        }

        public bool IsExist(string tableName, string strWhere)
        {
            try
            {
                DataTable dt = new DataTable();
                string sql = "select 1 from " + tableName + "  where " + strWhere;
                OleDbCommand cmd = new OleDbCommand(sql, Connection);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string[] computeH(DataTable dtstream, string streamName)
        {
            string[] h = new string[7];

            double H = 0;
            foreach (DataRow dr in dtstream.Rows)
            {
                string name = dr["streamname"].ToString();
                if (name == streamName)
                {
                    double TotalMolarRate = 0;
                    double TotalMolarEnthalpy = 0;
                    double InertWeightEnthalpy = 0;
                    double InertWeightRate = 0;
                    object oo = dr["InertWeightEnthalpy"];

                    string strInertWeightEnthalpy = dr["InertWeightEnthalpy"].ToString();

                    if (strInertWeightEnthalpy != "")
                        InertWeightEnthalpy = double.Parse(strInertWeightEnthalpy);
                    string strInertWeightRate = dr["InertWeightRate"].ToString();
                    if (strInertWeightRate != "")
                        InertWeightRate = double.Parse(strInertWeightRate);
                    string strTotalMolarEnthalpy = dr["TotalMolarEnthalpy"].ToString();
                    if (strTotalMolarEnthalpy != "")
                        TotalMolarEnthalpy = double.Parse(strTotalMolarEnthalpy);
                    //enthalpy=TotalMolarEnthalpy*TotalMolarRate+InertWeightEnthalpy*InertWeightRate;
                    string strTotalMolarRate = dr["TotalMolarRate"].ToString();
                    if (strTotalMolarRate != "")
                        TotalMolarRate = double.Parse(strTotalMolarRate);
                    double Enthalpy = TotalMolarEnthalpy * TotalMolarRate + InertWeightEnthalpy * InertWeightRate;

                    H = Enthalpy * 3600 ;

                    double TotalMassRate = 0;
                    double BulkMwOfPhase = 0;

                    string strBulkMwOfPhase = dr["BulkMwOfPhase"].ToString();
                    if (TotalMolarRate > 0 && strBulkMwOfPhase != "")
                    {
                        BulkMwOfPhase = double.Parse(strBulkMwOfPhase);
                        TotalMassRate = TotalMolarRate * BulkMwOfPhase;
                    }

                    //SpEnthalpy=IF(TotalMolarRate+InertWeightRate>0,Enthalpy/(TotalMassRate+InertWeightRate),RMISS)
                    double SpEnthalpy = 0;
                    if (TotalMolarRate + InertWeightRate > 0)
                    {
                        SpEnthalpy = Enthalpy / (TotalMassRate + InertWeightRate);
                    }

                    double flowrate = 0;
                    string bulkmwofphase = dr["BulkMwOfPhase"].ToString();
                    if (bulkmwofphase != "")
                    {
                        double wf = TotalMolarRate * double.Parse(bulkmwofphase);
                        flowrate = wf * 3600;
                    }



                    h[0] = string.Format("{0:0.0000}", H);
                    h[1] = string.Format("{0:0.0000}", SpEnthalpy);
                    h[2] = string.Format("{0:0.0000}", flowrate);

                    string temperature = dr["Temperature"].ToString();
                    if (temperature != "")
                    {
                        h[3] = UnitConverter.unitConv(temperature, "K", "C", "{0:0.0000}");
                    }

                    string pressure = dr["Pressure"].ToString();
                    if (pressure != "")
                    {
                        h[4] = UnitConverter.unitConv(pressure, "KPA", "MPAG", "{0:0.0000}");
                    }
                    h[5] = string.Format("{0:0.0000}", strTotalMolarRate);
                    h[6] = string.Format("{0:0.0000}", strBulkMwOfPhase);
                }
            }
            return h;
        }

        private string GetSideColumnTray(string[] sideColumnFeeds, Dictionary<string, string> dicProducts)
        {
            string result = string.Empty;
            foreach (string feed in sideColumnFeeds)
            {
                if (dicProducts.Keys.Contains(feed))
                {
                    result = dicProducts[feed];
                    break;
                }
            }
            return result;
        }

        public void GetEqFeedProduct(string ColumnName, ref Dictionary<string, string> dicFeeds, ref Dictionary<string, string> dicProducts)
        {
            DataTable dt = new DataTable();
            string sql = "select eqname, feeddata,productdata,feedtrays,prodtrays  from tbproiieqdata  where eqname='" + ColumnName + "'";

            OleDbCommand cmd = new OleDbCommand(sql, Connection);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string feeddata = dr["feeddata"].ToString();
                    string productdata = dr["productdata"].ToString();
                    string feedtrays = dr["feedtrays"].ToString();
                    string prodtrays = dr["prodtrays"].ToString();
                    string[] arrFeeds = feeddata.Split(',');
                    string[] arrProducts = productdata.Split(',');
                    string[] arrFeedtrays = feedtrays.Split(',');
                    string[] arrProdtrays = prodtrays.Split(',');
                    for (int i = 0; i < arrFeeds.Length; i++)
                    {
                        dicFeeds.Add(arrFeeds[i], arrFeedtrays[i]);
                    }
                    for (int i = 0; i < arrProducts.Length; i++)
                    {
                        dicProducts.Add(arrProducts[i], arrProdtrays[i]);
                    }
                }
            }
        }
        
        public bool GetAllSideColumnFeedProductData(ref Dictionary<string, string[]> dictFeed, ref Dictionary<string, string[]> dictProdcut)
        {
            bool b = false;
            DataTable dt = new DataTable();
            string sql = "select eqname, feeddata,productdata  from tbproiieqdata  where eqtype='SideColumn'";
            OleDbCommand cmd = new OleDbCommand(sql, Connection);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {

                    string key = dr["eqname"].ToString();
                    string feeddata = dr["feeddata"].ToString();
                    string productdata = dr["productdata"].ToString();
                    string[] feeds = feeddata.Split(',');
                    string[] products = productdata.Split(',');
                    dictFeed.Add(key, feeds);
                    dictProdcut.Add(key, products);
                }
                b = true;
            }
            return b;
        }

        public void GetMaincolumnRealFeedProduct(string ColumnName, ref Dictionary<string, string> dicFeeds, ref Dictionary<string, string> dicProducts)
        {
            Dictionary<string, string> tempFeeds = new Dictionary<string, string>();
            Dictionary<string, string> tempProducts = new Dictionary<string, string>();
            Dictionary<string, string[]> sideColumnFeeds = new Dictionary<string, string[]>();
            Dictionary<string, string[]> sideColumnProducts = new Dictionary<string, string[]>();
            GetEqFeedProduct(ColumnName, ref tempFeeds, ref tempProducts);
            GetAllSideColumnFeedProductData(ref sideColumnFeeds, ref sideColumnProducts);
            foreach (KeyValuePair<string, string> feed in tempFeeds)
            {
                bool isInternal = false;
                foreach (KeyValuePair<string, string[]> p in sideColumnProducts)
                {
                    if (p.Value.Contains(feed.Key))
                    {
                        isInternal = true;
                        break;
                    }
                }
                if (!isInternal)
                {
                    dicFeeds.Add(feed.Key, feed.Value);
                }
            }
            foreach (KeyValuePair<string, string> product in tempProducts)
            {
                bool isInternal = false;
                foreach (KeyValuePair<string, string[]> p in sideColumnFeeds)
                {
                    if (p.Value.Contains(product.Key))
                    {
                        isInternal = true;
                        break;
                    }
                }
                if (!isInternal)
                {
                    dicProducts.Add(product.Key, product.Value);
                }
            }

            Dictionary<string, string> sideColumnTray = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string[]> p in sideColumnFeeds)
            {
                string tray = GetSideColumnTray(p.Value, tempProducts);
                sideColumnTray.Add(p.Key, tray);
                foreach (string feed in p.Value)
                {
                    if (feed != string.Empty)
                    {
                        bool isInternal = false;
                        if (tempProducts.Keys.Contains(feed))
                        {
                            isInternal = true;
                        }
                        if (!isInternal)
                        {
                            if (dicFeeds.Keys.Contains(feed) == false)
                            {
                                dicFeeds.Add(feed, tray);
                            }
                        }
                    }
                }

            }

            foreach (KeyValuePair<string, string[]> p in sideColumnProducts)
            {
                string tray = sideColumnTray[p.Key];
                foreach (string product in p.Value)
                {
                    if (product != string.Empty)
                    {
                        bool isInternal = false;
                        if (tempFeeds.Keys.Contains(product))
                        {
                            isInternal = true;
                        }
                        if (!isInternal)
                        {
                            if (dicProducts.Keys.Contains(product) == false)
                            {
                                dicProducts.Add(product, tray);
                            }
                        }
                    }
                }

            }
        }

        public List<string> GetAllSideColumn()
        {
            List<string> list = new List<string>();
            DataTable dt = new DataTable();
            string sql = "select eqname  from tbproiieqdata  where eqtype='SideColumn'";
            OleDbCommand cmd = new OleDbCommand(sql, Connection);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr["eqname"].ToString());
            }
            return list;
        }





        /// <summary>
        /// 根据表名获取数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public DataTable GetDataByTableName(string tableName, string strWhere)
        {
            try
            {
                DataTable dt = new DataTable();
                string sql = "select * from " + tableName;
                if (strWhere != "")
                {
                    sql = "select * from " + tableName + " where " + strWhere;
                }
                OleDbCommand cmd = new OleDbCommand(sql, Connection);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);
                dt.TableName = tableName;
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据sql获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataBySQL(string sql)
        {
            try
            {
                DataTable dt = new DataTable();
                OleDbCommand cmd = new OleDbCommand(sql, Connection);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据sql保存数据
        /// </summary>
        /// <param name="sql"></param>
        public void SaveDataBySQL(string sql)
        {
            try
            {
                Connection.Open();
                OleDbCommand cmd = new OleDbCommand(sql, Connection);
                cmd.ExecuteNonQuery();
                Connection.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        public void GetAndConvertStreamInfo(string streamName, ref DataRow dr)
        {
            //UnitConvert unitConvert = new UnitConvert();
            DataTable dt = GetDataByTableName("tbproiistreamdata", " streamname='" + streamName + "' ");

            if (dt.Rows.Count > 0)
            {
                DataRow drStream = dt.Rows[0];
                dr["streamname"] = drStream["streamname"];
                //显示温度
                string temperature = drStream["Temperature"].ToString();
                if (temperature != "")
                {
                    dr["Temperature"] = UnitConverter.unitConv(temperature, "K", "C", "{0:0.0000}");
                }

                //显示并转换压力
                string pressure = drStream["Pressure"].ToString();
                if (pressure != "")
                {
                    dr["Pressure"] = UnitConverter.unitConv(pressure, "KPA", "MPAG", "{0:0.0000}");
                }

                //
                string vabfrac = drStream["VaporFraction"].ToString();
                if (vabfrac != "")
                {
                    dr["VaporFraction"] = string.Format("{0:0.0000}", double.Parse(vabfrac));
                }

                double TotalMolarRate = 0;
                if (drStream["TotalMolarRate"].ToString() != "")
                    TotalMolarRate = double.Parse(drStream["TotalMolarRate"].ToString());
                string bulkmwofphase = drStream["BulkMwOfPhase"].ToString();
                if (bulkmwofphase != "")
                {
                    double wf = TotalMolarRate * double.Parse(bulkmwofphase);
                    dr["WeightFlow"] =  string.Format("{0:0.0000}", wf * 3600);  //Kg/h
                }

                //enthalpy=TotalMolarEnthalpy*TotalMolarRate+InertWeightEnthalpy*InertWeightRate;
                double TotalMolarEnthalpy = 0;
                string strTotalMolarEnthalpy = drStream["TotalMolarEnthalpy"].ToString();
                if (strTotalMolarEnthalpy != "")
                {
                    TotalMolarEnthalpy = double.Parse(strTotalMolarEnthalpy);
                }


                double InertWeightEnthalpy = 0;
                string strInertWeightEnthalpy = drStream["InertWeightEnthalpy"].ToString();
                if (strInertWeightEnthalpy != "")
                {
                    InertWeightEnthalpy = double.Parse(strInertWeightEnthalpy);
                }

                double InertWeightRate = 0;
                string strInertWeightRate = drStream["InertWeightRate"].ToString();
                if (strInertWeightRate != "")
                {
                    InertWeightRate = double.Parse(strInertWeightRate);
                }


                double Enthalpy = TotalMolarEnthalpy * TotalMolarRate + InertWeightEnthalpy * InertWeightRate;
                dr["Enthalpy"] = string.Format("{0:0.0000}", Enthalpy * 3600 ); //KJ

               
                double TotalMassRate = 0;
                if (TotalMolarRate > 0 && bulkmwofphase != "")
                {
                    TotalMassRate = TotalMolarRate * double.Parse(bulkmwofphase);
                }

                
                double SpEnthalpy = 0;
                if (TotalMolarRate + InertWeightRate > 0)
                {
                    SpEnthalpy = Enthalpy / (TotalMassRate + InertWeightRate);
                }
                dr["SpEnthalpy"] = string.Format("{0:0.0000}", SpEnthalpy);  //KJ/Kg


                string[] columns = { "BulkMwOfPhase", "BulkDensityAct", "BulkViscosity", "BulkCPCVRatio", "VaporZFmKVal", "BulkCP", "BulkThermalCond", "BulkSurfTension", "TotalComposition", "TotalMolarEnthalpy", "TotalMolarRate", "InertWeightEnthalpy", "InertWeightRate", "CompIn", "ComponentId", "ProdType" };
                foreach (string c in columns)
                {
                    dr[c] = drStream[c].ToString();
                }
            }

        }

    }

    public class UnitConverter
    {
        public static string unitConv(string param, string sourcetype, string targetype, string format)
        {
            //string.Format("{0:000.000}", 12.2);
            string result = param;
            if (sourcetype.ToUpper() == "K" && targetype == "C")
            {
                double temp = double.Parse(param) - 273.15;
                result = string.Format(format, temp);
            }
            if (sourcetype.ToUpper() == "KPA" && targetype == "MPAG")
            {
                double temp = double.Parse(param) / 1000 - 0.10135;
                result = string.Format(format, temp);
            }
            if (sourcetype.ToUpper() == "KG/SEC" && targetype == " KG/HR")
            {
                double temp = double.Parse(param) / 3600;
                result = string.Format(format, temp);
            }
            if (sourcetype.ToUpper() == "M3/SEC" && targetype == "M3/HR")
            {
                double temp = double.Parse(param) / 3600;
                result = string.Format(format, temp);
            }

            if (sourcetype.ToUpper() == "PAS" && targetype == "CP")
            {
                //1P=0.1PaS=100CP=100mPaS
                double temp = double.Parse(param) * 1000;
                result = string.Format(format, temp);
            }
            return result;
        }

        public static string convertData(object obj)
        {
            string rs = string.Empty;
            if (obj is Array)
            {
                object[] objdata = (System.Object[])obj;
                foreach (object s in objdata)
                {
                    if (s.ToString() != string.Empty)
                    {
                        rs = rs + "," + s;
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
    }
    
}
