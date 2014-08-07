using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class StorageTank
    {
        public virtual int ID { get; set; }
        public virtual string StorageTankName { get; set; }
        public virtual string SourceFile { get; set; }

        public virtual string StorageTankName_Color { get; set; }
        public virtual string SourceFile_Color { get; set; }
    }
}
