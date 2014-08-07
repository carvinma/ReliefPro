using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    
    public class SourceFile
    {
        public virtual int ID { set; get; }
        public virtual string FileName { set; get; }
        public virtual string FileNameNoExt { set; get; }
        public virtual int FileType { set; get; }
        public virtual string FileVersion { set; get; }
    }
}
