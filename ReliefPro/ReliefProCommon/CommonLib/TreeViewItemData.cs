using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProCommon.CommonLib
{
    public class TreeViewItemData
    {
        public string Text { get; set; }
        public string Pic { get; set; }
        public string FullName { get; set; }
        //1 Plant 2 Unit 3 ProtectSystem 4 visio File
        public int Type { get; set; }
        public string dbPlantFile{ get; set; }
        public string dbProtectedSystemFile { get; set; }

        List<TreeViewItemData> _children = new List<TreeViewItemData>();
        public IList<TreeViewItemData> Children
        {
            get { return _children; }
        }
    }

    //用于Icon的list item数据类
    public class ListViewItemData
    {
        public string Name { get; set; }
        public string Pic { get; set; }
    }
}
