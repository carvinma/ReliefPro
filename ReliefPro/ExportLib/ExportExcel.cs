using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using ReliefProLL;
using ReliefProModel.Reports;

namespace ExportLib
{
    public class ExportExcel
    {
        private List<string> listTitle = new List<string> { "Controlling Single Scenario", "General Electric Power Failure",
            "General Cooling Water Failure", "General Instument Air Failure", "Steam Failure", "Fire","Notes" };
        private List<string> listColumn = new List<string> { "Relief Rate,\r\nKg/hr", "Phase", "MW \r\nor SpGr", "T, C", "Z" };
        private List<string> listScenario = new List<string> { "PowerDS", "WaterDS", "AirDS", "SteamDS", "FireDS" };
        private List<string> listProperty = new List<string> { "ReliefLoad", "Phase", "ReliefMW", "ReliefTemperature", "ReliefZ" };

        public void ExportToExcelPUsummary(List<PUsummaryGridDS> ReportDs, string fileName)
        {
            GC.Collect();//强制回收垃圾
            string saveFileName = string.Empty;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            saveDialog.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
            saveDialog.FileName = fileName;
            saveDialog.DefaultExt = "xlsx";
            saveDialog.RestoreDirectory = true;
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
            //xBk.Windows[1].DisplayGridlines = false;
            int worksheetNum = 0;
            var query = ReportDs.Where(p => !string.IsNullOrEmpty(p.psv.DischargeTo)).GroupBy(p => p.psv.DischargeTo, p => p.psv.DischargeTo);
            foreach (var o in query)
            {
                if (worksheetNum > 0) xBk.Worksheets.Add();
                excel.ActiveWindow.DisplayGridlines = false;//不显示网格线 
                _Worksheet worksheet = (_Worksheet)xBk.ActiveSheet;
                worksheet.Name = o.Key;
                List<PUsummaryGridDS> sheetReportDs = ReportDs.Where(p => p.psv.DischargeTo == o.Key).ToList();
                ReportBLL reportBLL = new ReportBLL();
                sheetReportDs = reportBLL.CalcMaxSum(sheetReportDs);
                worksheet = CreateSheet(worksheet, sheetReportDs);
                worksheetNum++;
            }
            xBk.Worksheets.Add();
            xSt = (_Worksheet)xBk.ActiveSheet;
            xSt.Name = "ALL";
            excel.ActiveWindow.DisplayGridlines = false;//不显示网格线 
            xSt = CreateSheet(xSt, ReportDs);

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
            Range rangePlant = xSt.get_Range("B3", "D3");
            rangePlant.Value2 = "Plant Name";
            rangePlant.Merge(false);
            Range rangeProcess = xSt.get_Range("B4", "D4");
            rangeProcess.Value2 = "Process Unit Name";
            rangeProcess.Merge(false);

            Range rangePlantValue = xSt.get_Range("E3", "H3");
            rangePlantValue.Value2 = "";
            rangePlantValue.Borders.LineStyle = 1;
            rangePlantValue.Merge(false);
            Range rangeProcessValue = xSt.get_Range("E4", "H4");
            rangeProcessValue.Value2 = "";
            rangeProcessValue.Borders.LineStyle = 1;
            rangeProcessValue.Merge(false);

            xSt.get_Range("J3").Value2 = "Description";

            Range rangeDescriptionValue = xSt.get_Range("K3", "N4");
            rangeDescriptionValue.Value2 = "";
            rangeDescriptionValue.Borders.LineStyle = 1;
            rangeDescriptionValue.Merge(false);
            rangeDescriptionValue.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            rangeDescriptionValue.VerticalAlignment = XlVAlign.xlVAlignCenter;

            Range rangePlantProcess = xSt.get_Range("B2", "O5");
            rangePlantProcess.Borders[XlBordersIndex.xlEdgeTop].LineStyle = 1;
            rangePlantProcess.Borders[XlBordersIndex.xlEdgeBottom].LineStyle = 1;
            rangePlantProcess.Borders[XlBordersIndex.xlEdgeLeft].LineStyle = 1;
            rangePlantProcess.Borders[XlBordersIndex.xlEdgeRight].LineStyle = 1;

            #endregion
            #region Grid 设置标题头
            int titleIndex = 7;
            int endIndex;
            listTitle.ForEach(p =>
            {
                endIndex = titleIndex + 4;
                if (p.Equals("Notes")) { endIndex = titleIndex; }
                if (p.Equals("Controlling Single Scenario")) { endIndex = titleIndex + 5; }

                Range range = xSt.get_Range(xSt.Cells[7, titleIndex], xSt.Cells[7, endIndex]);
                range.Value2 = p;
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//垂直对齐
                range.VerticalAlignment = XlVAlign.xlVAlignCenter;//水平对齐
                range.Merge(false);//参数为True则为每一行合并为一个单元格 
                titleIndex = endIndex + 1;
            });
            #endregion
            #region Grid 设置各列标题
            xSt.Cells[8, 2] = "Device\r\n#";
            xSt.Cells[8, 3] = "Protected\r\nSystem";
            xSt.Cells[8, 4] = "Device\r\nType";
            xSt.Cells[8, 5] = "Set\r\nPressure,\r\nMPag";
            xSt.Cells[8, 6] = "Discharge\r\nTo";

            int index = 7;
            listColumn.ForEach(p =>
            {
                xSt.Cells[8, index] = p;
                index++;
            });
            xSt.Cells[8, index] = "Scenario";
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
                colIndex = 2;
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
                    int tmp = colIndex;
                });
                rowIndex++;
            });
            #endregion

            #region  Notes
            xSt.Cells[rowIndex + 1, 2] = "#";
            // xSt.get_Range("B24").Value2 = "#";
            //Range rangeNote = xSt.get_Range("C24", "T24");
            Range rangeNote = xSt.get_Range(xSt.Cells[rowIndex + 1, 3], xSt.Cells[rowIndex + 1, 20]);
            rangeNote.Value2 = "Notes";
            rangeNote.Merge(false);

            Range rangeRemark = xSt.get_Range(xSt.Cells[rowIndex + 1, 2], xSt.Cells[rowIndex + 1, 20]);
            rangeRemark.Borders.LineStyle = 1;
            rangeRemark.HorizontalAlignment = XlHAlign.xlHAlignCenter;//水平对齐
            rangeRemark.VerticalAlignment = XlVAlign.xlVAlignCenter;//垂直对齐
            #endregion

            #region 调整样式
            var excelRange = xSt.get_Range(xSt.Cells[7, 2], xSt.Cells[rowIndex - 1, colIndex]);
            //单元格边框线类型(线型,虚线型)
            excelRange.Borders.LineStyle = 1;
            //设置字体大小
            excelRange.Font.Size = 10;
            excelRange.EntireColumn.AutoFit();
            excelRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;//水平对齐
            excelRange.VerticalAlignment = XlVAlign.xlVAlignCenter;//垂直对齐

            for (int i = 2; i <= 6; i++)
            {
                var bRange = xSt.get_Range(xSt.Cells[7, i], xSt.Cells[8, i]);
                bRange.Merge();
            }

            //max Summation
            if (rowIndex >= 12)
            {
                for (int i = 1; i <= 2; i++)
                {
                    var range = xSt.get_Range(xSt.Cells[rowIndex - i, 2], xSt.Cells[rowIndex - i, 6]);
                    range.Merge();
                }
            }
            #endregion
            return xSt;
        }
        private string GetResult(PUsummaryGridDS p, string ScenarioType, string ScenarioProperty)
        {
            ReliefProModel.Scenario scenario = p.GetType().GetProperty(ScenarioType).GetValue(p, null) as ReliefProModel.Scenario;
            if (scenario == null) return "";
            object obj = scenario.GetType().GetProperty(ScenarioProperty).GetValue(scenario, null);
            return obj == null ? "" : obj.ToString();
        }
    }
}