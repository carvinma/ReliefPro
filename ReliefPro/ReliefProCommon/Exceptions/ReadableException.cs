using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ReliefProCommon.Exceptions
{
    public class ReadableException: Exception
    {
        public ReadableException(): base()
        {
        }

        public ReadableException(string message)
            : base(message)
        {
        }

        
        protected ReadableException(SerializationInfo info, StreamingContext context):base(info, context)
        {
        }

        public ReadableException(string message, Exception innerException): base(message, innerException)
        {
        }
    }
}
