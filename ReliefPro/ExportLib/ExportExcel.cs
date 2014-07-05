using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using ReliefProModel.Reports;

namespace ExportLib
{
    public class ExportExcel
    {
        private List<string> listTitle = new List<string> { "Controlling Single Scenario", "General Electric Power Failure",
            "General Cooling Water Failure", "General Instument Air Failure", "Steam Failure", "Fire","Notes" };
        private List<string> listColumn = new List<string> { "Relief Rate, Kg/hr", "Phase", "MW or SpGr", "T, C", "Z" };
        private List<string> listScenario = new List<string> { "PowerDS", "WaterDS", "AirDS", "SteamDS", "FireDS" };
        private List<string> listProperty = new List<string> { "ReliefLoad", "ReliefMW", "ReliefTemperature", "ReliefZ" };

        private void ExportExcel(List<PUsummaryGridDS> ReportDs, string fileName)
        {
            GC.Collect();//强制回收垃圾
            string saveFileName = string.Empty;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            // saveDialog.Filter = "Excel文件|*.xls|*.xlsx";
            saveDialog.Filter = "Excel(*.xls)|*.xls|Excel(*.xlsx)|*.xlsx";
            saveDialog.FileName = fileName;
            saveDialog.DefaultExt = "xlsx";
            saveDialog.ShowDialog();
            saveFileName = saveDialog.FileName;
            if (saveFileName.IndexOf(":") < 0) return;
            Microsoft.Office.Interop.Excel.Application excel = new ApplicationClass();
            if (excel == null)
            {
                return;
            }

            _Workbook xBk;
            _Worksheet xSt;

            excel.Application.DisplayAlerts = false;//使合并操作不提示警告信息   
            xBk = excel.Workbooks.Add(true);
            var query = ReportDs.GroupBy(p => p.psv.DischargeTo, p => p.psv.DischargeTo);
            foreach (var o in query)
            {
                xBk.Worksheets.Add();
                _Worksheet worksheet = (_Worksheet)xBk.ActiveSheet;
                worksheet.Name = o.Key;
                List<PUsummaryGridDS> sheetReportDs = ReportDs.Where(p => p.psv.DischargeTo == o.Key).ToList();
                worksheet = CreateSheet(worksheet, sheetReportDs);
            }
            xBk.Worksheets.Add();
            xSt = (_Worksheet)xBk.ActiveSheet;
            xBk.Worksheets.Add();
            xSt.Name = "ALL";
            xSt = CreateSheet(xSt, ReportDs);
            // xSt.Columns.ColumnWidth = 20;

            #region  清理垃圾，回收资源
            xBk.SaveCopyAs(saveFileName);
            xBk.Close(false, null, null);
            excel.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xBk);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(xSt);
            xBk = null;
            excel = null;
            // xSt = null;
            GC.Collect();
            #endregion
        }
        private _Worksheet CreateSheet(_Worksheet xSt, List<PUsummaryGridDS> ReportDs)
        {
            #region 杂乱的标题
            Range rangePlant = xSt.get_Range("B3", "C3");
            rangePlant.Value2 = "Plant Name";
            rangePlant.Merge(false);
            Range rangeProcess = xSt.get_Range("B4", "C4");
            rangeProcess.Value2 = "Process Unit Name";
            rangeProcess.Merge(false);

            Range rangePlantValue = xSt.get_Range("D3", "G3");
            rangePlantValue.Value2 = "";
            rangePlantValue.Merge(false);
            Range rangeProcessValue = xSt.get_Range("D4", "G4");
            rangeProcessValue.Value2 = "";
            rangeProcessValue.Merge(false);

            xSt.get_Range("I3").Value2 = "Description";

            Range rangeDescriptionValue = xSt.get_Range("K3", "N4");
            rangeDescriptionValue.Value2 = "";
            rangeDescriptionValue.Merge(false);
            rangeDescriptionValue.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            rangeDescriptionValue.VerticalAlignment = XlVAlign.xlVAlignCenter;

            xSt.get_Range("B24").Value2 = "#";
            Range rangeNote = xSt.get_Range("C24", "T24");
            rangeNote.Value2 = "Notes";

            #endregion
            #region Grid 设置标题头
            int titleIndex = 7;
            int endIndex;
            listTitle.ForEach(p =>
            {
                endIndex = titleIndex + 5;
                if (p.Equals("Notes")) { endIndex = titleIndex; }
                if (p.Equals("Controlling Single Scenario")) { endIndex = titleIndex + 6; }

                Range range = xSt.get_Range(xSt.Cells[7, titleIndex], xSt.Cells[7, endIndex]);
                range.Value2 = p;
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//垂直对齐
                range.VerticalAlignment = XlVAlign.xlVAlignCenter;//水平对齐
                range.Merge(false);//参数为True则为每一行合并为一个单元格 
                titleIndex += 6;
            });
            #endregion
            #region Grid 设置各列标题
            xSt.Cells[8, 2] = "Device#";
            xSt.Cells[8, 3] = "Protected System";
            xSt.Cells[8, 4] = "Device Type";
            xSt.Cells[8, 5] = "Set Pressure, MPag";
            xSt.Cells[8, 6] = "Discharge To";
            int index = 7;
            listColumn.ForEach(p =>
            {
                xSt.Cells[8, index] = p;
                index++;
            });
            xSt.Cells[7, index] = "Scenario";
            index++;

            for (int i = 1; i <= 5; i++)
            {
                listColumn.ForEach(p =>
                {
                    xSt.Cells[8, index] = p;
                    index++;
                });
            }

            #endregion

            #region 读取数据

            int rowIndex = 9;
            int colIndex = 2;
            ReportDs.ForEach(p =>
            {
                xSt.Cells[rowIndex, colIndex++] = p.psv.PSVName;
                xSt.Cells[rowIndex, colIndex++] = p.psv.PSVName;
                xSt.Cells[rowIndex, colIndex++] = p.psv.ValveType;
                xSt.Cells[rowIndex, colIndex++] = p.psv.Pressure;
                xSt.Cells[rowIndex, colIndex++] = p.psv.DischargeTo;

                xSt.Cells[rowIndex, colIndex++] = p.SingleDS.ReliefLoad;
                xSt.Cells[rowIndex, colIndex++] = p.SingleDS.Phase;
                xSt.Cells[rowIndex, colIndex++] = p.SingleDS.ReliefMW;
                xSt.Cells[rowIndex, colIndex++] = p.SingleDS.ReliefTemperature;
                xSt.Cells[rowIndex, colIndex++] = p.SingleDS.ReliefZ;
                xSt.Cells[rowIndex, colIndex++] = p.SingleDS.ScenarioName;

                listScenario.ForEach(s =>
                {
                    listProperty.ForEach(y =>
                    {
                        xSt.Cells[rowIndex, colIndex++] = GetResult(p, s, y);
                    });

                });
                rowIndex++;
            });
            var excelRange = xSt.get_Range(xSt.Cells[9, 2], xSt.Cells[rowIndex - 1, colIndex - 1]);
            //单元格边框线类型(线型,虚线型)
            excelRange.Borders.LineStyle = 1;
            //设置字体大小
            excelRange.Font.Size = 10;
            excelRange.EntireColumn.AutoFit();
            excelRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;//水平对齐
            excelRange.VerticalAlignment = XlVAlign.xlVAlignCenter;//垂直对齐
            #endregion
            return xSt;
        }
        private string GetResult(PUsummaryGridDS p, string ScenarioType, string ScenarioProperty)
        {
            Scenario scenario = p.GetType().GetProperty(ScenarioType).GetValue(p, null) as Scenario;
            object obj = scenario.GetType().GetProperty(ScenarioProperty).GetValue(scenario, null);
            return obj == null ? "" : obj.ToString();
        }
    }
}