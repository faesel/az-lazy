using System;
namespace az_lazy.Exceptions
{
    public class ContainerException : Exception
    {
        public ContainerException() : base()
        {
        }

        public ContainerException(Exception ex) : base($"Failed queue operation {ex.Message}")
        {
        }

        protected ContainerException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public ContainerException(string message) : base(message)
        {
        }

        public ContainerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}