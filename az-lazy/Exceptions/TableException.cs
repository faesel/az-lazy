using System;

namespace az_lazy.Exceptions
{
    public class TableException: Exception
    {
        public TableException() : base()
        {
        }

        public TableException(Exception ex) : base($"Failed table operation {ex.Message}")
        {
        }

        protected TableException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public TableException(string message) : base(message)
        {
        }

        public TableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}