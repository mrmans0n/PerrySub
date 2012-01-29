using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace scriptASS
{
    public class PerrySubException : Exception
    {
        public PerrySubException() : base()
        {
        }

        public PerrySubException(string message)
            : base(message)
        {
        }

        public PerrySubException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public PerrySubException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
