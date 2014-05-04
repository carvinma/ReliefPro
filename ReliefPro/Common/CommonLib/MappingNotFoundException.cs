using System;
using System.Runtime.Serialization;

namespace ReliefProCommon
{
    /// <summary>
    /// 反序列化excel时映射不存在
    /// </summary>
    public class MappingNotFoundException : Exception
    {
        public MappingNotFoundException()
            : base()
        {
        }

        public MappingNotFoundException(string message)
            : base(message)
        {
        }


        protected MappingNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public MappingNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
