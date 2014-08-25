using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProMain.Models
{
    
    public class TVFile
    {        
        public string Name { get;  set; }
        public string FullPath { get;  set; }
        public string dbPlantFile{ get; set; }
        public string dbProtectedSystemFile { get; set; }
    }
    
    public class TVPS
    {
        public string Name { get;  set; }
        public string FullPath { get;  set; }
        public string dbPlantFile { get; set; }
        public string dbProtectedSystemFile { get; set; }

        List<TVFile> _files = new List<TVFile>();
        public List<TVFile> Files
        {
            get { return _files; }
        }
    }
    public class TVUnit
    {
        public int ID { get; set; }
        public string Name { get;  set; }
        public string FullPath { get;  set; }
        public string dbPlantFile { get; set; }

        List<TVPS> _pss = new List<TVPS>();
        public List<TVPS> PSs
        {
            get { return _pss; }
        }
    }
    public class TVPlant
    {       
        public string Name { get;  set; }
        public string FullPath { get;  set; }//工作目录
        public string FullRefPath { get;  set; }
        public string dbPlantFile { get; set; }
        List<TVUnit> _units = new List<TVUnit>();
        public List<TVUnit> Units
        {
            get { return _units; }
        }
    }
}
