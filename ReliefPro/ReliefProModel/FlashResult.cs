using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class FlashResult
    {
        public virtual int ID { get; set; }

        public virtual string PrzFile { get; set; }
        public virtual string LiquidName { get; set; }
        public virtual string VaporName { get; set; }
        public virtual string StreamName { get; set; }
        public virtual int Tray { get; set; }
        public virtual string ProdType { get; set; }

        public virtual string PrzFile_Color { get; set; }
        public virtual string LiquidName_Color { get; set; }
        public virtual string VaporName_Color { get; set; }
        public virtual string StreamName_Color { get; set; }
        public virtual string Tray_Color { get; set; }
        public virtual string ProdType_Color { get; set; }
    }
}
