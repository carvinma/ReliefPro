using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    [Serializable]
    public class ProtectedSystem
    {
        public virtual int ID { set; get; }
        public virtual string DeviceName { set; get; }
        public virtual string ProtectedSystemName { set; get; }
    }
}
