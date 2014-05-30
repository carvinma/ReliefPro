using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NPOI.HSSF.UserModel;
using System.IO;
using Common.Exceptions;


namespace ReliefProCommon.CommonLib
{
    public class ExcelUtils
    {

        public static DataTable GetExcelDatas(FileStream file)
        {
            try
            {
                return GetData(GetHSSFWorkbook(file), 0);
            }
            catch (Exception e)
            {
                WriterExceptions.WriterToLog(e);
                throw e;
            }

        }

        public static DataTable GetExcelDatas(Stream file)
        {
            try
            {
                return GetData(GetHSSFWorkbook(file), 0);
            }
            catch (Exception e)
            {
                WriterExceptions.WriterToLog(e);
                throw e;
            }

        }

        public static DataTable GetExcelDatas(Stream file, int SheetNo)
        {
            try
            {
                return GetData(GetHSSFWorkbook(file), SheetNo);
            }
            catch (Exception e)
            {
                WriterExceptions.WriterToLog(e);
                throw e;
            }

        }

        public static DataTable GetExcelDatas(FileStream file, int SheetNo)
        {
            try
            {
                return GetData(GetHSSFWorkbook(file), SheetNo);
            }
            catch (Exception e)
            {
                WriterExceptions.WriterToLog(e);
                throw e;
            }

        }

        private static HSSFWorkbook GetHSSFWorkbook(Stream file)
        {

            return new HSSFWorkbook(file);
        }

        private static HSSFWorkbook GetHSSFWorkbook(FileStream file)
        {

            return new HSSFWorkbook(file);
        }

        private static DataTable GetData(HSSFWorkbook hssfworkbook, int SheetNo)
        {
            NPOI.SS.UserModel.ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            DataTable dt = new DataTable();
            int hh = sheet.GetRow(0).LastCellNum;
            for (int j = 0; j < hh; j++)
            {
                dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
            }

            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                DataRow dr = dt.NewRow();
                for (int i = 0; i < row.LastCellNum; i++)
                {
                    NPOI.SS.UserModel.ICell cell = row.GetCell(i);
                    if (cell == null)
                    {
                        dr[i] = null;
                    }
                    else
                    {
                        dr[i] = cell.ToString();
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

    }
}
