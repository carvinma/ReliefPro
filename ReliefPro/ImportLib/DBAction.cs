using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace ImportLib
{
    public  class DBAction
    {
        public void InsertDataByRow(DataRow dr, OleDbConnection Connection)
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

        public void UpdateDataByRow(DataRow dr, OleDbConnection Connection)
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

        public void DeleteDataByRow(DataRow dr, OleDbConnection Connection)
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

    }
}
