using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using ReliefProCommon;

namespace ReliefProCommon
{
    /// <summary>
    /// 把excel数据形成类实例列表
    /// 
    /// </summary>
    public class ExcelDeserializer<C> where C : class, new()
    {
        public ExcelDeserializer()
        {
        }

        /// <summary>
        /// 反序列化，
        /// 抛出MappingNotFoundException异常
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="stream"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public IList<C> Deserialize(Stream stream, ExcelMappingCollection mappings)
        {
            SpreadsheetDocument workbook = null;

            try
            {
                workbook = SpreadsheetDocument.Open(stream, stream.CanWrite);
            }
            catch (OpenXmlPackageException ex)
            {
                throw new Exception("文件不是有效的电子表格", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("无法打开文件", ex);
            }

            using (workbook)
            {
                try
                {
                    var sharedStrings = workbook.WorkbookPart.SharedStringTablePart.SharedStringTable;
                    var worksheet = this.GetWorksheet(workbook, mappings.SheetName);
                    var rows = worksheet.Descendants<Row>();
                    
                    
                    //默认第一行是列定义
                    var heading = rows.First();
                    Dictionary<string, int> columnToMapping = this.MakeColumnToMapping(heading, sharedStrings, mappings);

                    var data = rows.Skip(1);

                    List<C> list = new List<C>();
                    foreach (Row row in data)
                    {
                        C o = this.Deserialize<C>(row, sharedStrings, mappings, columnToMapping);

                        if(o!=null)
                            list.Add(o);
                    }

                    return list;
                }
                finally
                {
                    workbook.Close();
                }
            }
        }

        /// <summary>
        /// 生成列和要映射的类的属性的映射
        /// </summary>
        /// <param name="row"></param>
        /// <param name="sharedStrings"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        protected Dictionary<string, int> MakeColumnToMapping(Row row, SharedStringTable sharedStrings, ExcelMappingCollection mappings)
        {
            Dictionary<string, int> columnToMapping = new Dictionary<string, int>();
            var heading = row;

            Regex regex = new Regex("[A-Za-z]+");

            for (int i = 0; i < mappings.Count; ++i)
            {
                //string right = mapping.ExcelColumnNames[i];
                string right = mappings[i].ColumnName;
                var cells = heading.Descendants<Cell>();

                string columnIndex = null;
                foreach(Cell cell in cells)
                {
                    if (this.GetCellValue(cell, sharedStrings) == right)
                    {
                        columnIndex = this.GetColumnIndex(cell, regex);
                        
                        break;
                    }
                }

                if (columnIndex == null)
                {
                    throw new MappingNotFoundException("mapping not found");
                }

                columnToMapping[columnIndex] = i;
            }

            return columnToMapping;
        }

        /// <summary>
        /// 生成类的实例
        /// </summary>
        /// <param name="row"></param>
        /// <param name="stringTable"></param>
        /// <param name="mapping"></param>
        /// <param name="columnToMapping"></param>
        /// <returns></returns>
        protected C Deserialize<C>(Row row, SharedStringTable stringTable, ExcelMappingCollection mappings, Dictionary<string, int> columnToMapping) where C : class, new()
        {
            C o = null;
            Type type = typeof(C);

            Regex regex = new Regex("[A-Za-z]+");
            foreach(Cell cell in row.Descendants<Cell>())
            {
                string columnIndex = this.GetColumnIndex(cell, regex);

                if (!columnToMapping.ContainsKey(columnIndex))
                {
                    continue;
                }

                int mappingIndex = columnToMapping[columnIndex];
                string cellValue = this.GetCellValue(cell, stringTable);

                //if (cellValue != null)
                //if(!string.IsNullOrEmpty(cellValue))
                {
                    if (o == null) o = new C();
                    string propertyName = mappings[mappingIndex].PropertyName; //mapping.ClassPropertyNames[mappingIndex];

                    var property = type.GetProperty(propertyName);

                    //if (mappings[mappingIndex].OnValidation != null)
                    {
                        mappings[mappingIndex].Validate(cellValue);
                    }

                    property.SetValue(o, Convert.ChangeType(cellValue, property.PropertyType), null);
                }
            }

            return o;
        }

        /// <summary>
        /// 取一个cell的列编号
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        protected string GetColumnIndex(Cell cell, Regex regex)
        {            
            Match match = regex.Match(cell.CellReference);
            string index = match.Value;
            return index;
        }

        /// <summary>
        /// 取一个cell的值
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="stringTable"></param>
        /// <returns></returns>
        protected string GetCellValue(Cell cell, SharedStringTable stringTable)
        {
            string v = null;
            //if (cell.DataType != null)
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
            //if (cell.CellValue==null || string.IsNullOrEmpty(cell.CellValue.InnerText))
            //{
            //    //v = cell.CellValue.InnerText;
                
            //}
            //else
            //{
            //    var strings = stringTable.Descendants<SharedStringItem>();

            //    int index = int.Parse(cell.CellValue.InnerText);
            //    var item = strings.ElementAt(index);
            //    v = item.Text.InnerText;
            //}

            return v;
        }

        /// <summary>
        /// 取一个Worksheet
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        protected Worksheet GetWorksheet(SpreadsheetDocument workbook, string sheetName)
        {
            WorkbookPart workbookPart = workbook.WorkbookPart;



            Sheet sheet = null;
            var sheets = workbookPart.Workbook.Descendants<Sheet>();
            if(string.IsNullOrEmpty(sheetName))
            {
                sheet = sheets.First();
            }
            else
            {
                sheet = sheets.Where(x => x.Name == sheetName).First();
            }

            var sheetpart = workbookPart.GetPartById(sheet.Id) as WorksheetPart;

            return sheetpart.Worksheet;
        }

        //public IList<A> GetList<A>()
        //{
        //    IList<object> o = this.deserialized[typeof(A)] as IList<object>;

        //    return o.Cast<A>().ToList();
        //}
    }
}
