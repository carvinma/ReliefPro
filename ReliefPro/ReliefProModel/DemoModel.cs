
using System;
namespace ReliefProModel
{
    [Serializable]
    public class DemoModel
    {
        public virtual int ID { get; set; }
        public virtual double col1 { get; set; }
        public virtual DateTime colDate { get; set; }
        public virtual string colString { get; set; }
    }
}
