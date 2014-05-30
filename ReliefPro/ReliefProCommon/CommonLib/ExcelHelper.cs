using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ReliefProCommon.CommonLib
{
    public class ExcelHelper
    {
        Regex columnIndexRegex = new Regex("[A-Za-z]+");

        /// <summary>
        /// 取一个cell的列编号
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public string GetColumnIndex(Cell cell)
        {
            Match match = columnIndexRegex.Match(cell.CellReference);
            string index = match.Value;
            return index;
        }

        public string GetCellValue(Cell cell, SharedStringTable stringTable)
        {
            string v = null;
            if (cell != null)
            {
                v = cell.InnerText;

                if (cell.DataType != null)
                {
                    switch (cell.DataType.Value)
                    {
                        case CellValues.SharedString:
                            var strings = stringTable.Descendants<SharedStringItem>();

                            int index = int.Parse(cell.CellValue.InnerText);
                            var item = strings.ElementAt(index);
                            v = item.Descendants<Text>().First<Text>().InnerText;
                            break;

                        case CellValues.String:
                            v = cell.CellValue.InnerText;
                            break;
                    }
                }
            }
            return v;
        }
    }
}
