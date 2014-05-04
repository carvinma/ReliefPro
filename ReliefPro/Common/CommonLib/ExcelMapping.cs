using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProCommon
{
    public class ExcelMappingCollection: List<ExcelMapping>
    {
        //public ExcelMappingCollection(string sheetName, string[] properties, string[] columns)
        //{
        //    this.SheetName = sheetName;
        //    this.ClassPropertyNames = properties;
        //    this.ExcelColumnNames = columns;
        //}        

        /// <summary>
        /// excel sheet名称
        /// </summary>
        public string SheetName
        {
            get;
            set;
        }

        ///// <summary>
        ///// 类的属性名称，
        ///// </summary>
        //public string[] ClassPropertyNames
        //{
        //    get;
        //    private set;
        //}

        ///// <summary>
        ///// excel的列名
        ///// </summary>
        //public string[] ExcelColumnNames
        //{
        //    get;
        //    private set;
        //}
    }

    public class ExcelMapping
    {
        public string ColumnName
        {
            get;set;
        }

        public string PropertyName
        {
            get;set;
        }

        
        public delegate void ValidationHandler(string value);
        
        public event ValidationHandler OnValidation;
        
        public virtual void Validate(string value)
        {
            if (this.OnValidation != null)
            {
                this.OnValidation(value);
            }

        }

    }
}
