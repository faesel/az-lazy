using System;
namespace az_lazy.Exceptions
{
    public class QueueException : Exception
    {
        public QueueException() : base()
        {
        }

        public QueueException(Exception ex) : base($"Failed queue operation {ex.Message}")
        {
        }

        protected QueueException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public QueueException(string message) : base(message)
        {
        }

        public QueueException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}