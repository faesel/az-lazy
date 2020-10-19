using System;

namespace az_lazy.Exceptions
{
    public class ConnectionException : Exception
    {
        public ConnectionException() : base()
        {
        }

        public ConnectionException(Exception ex) : base($"Failed to connect {ex.Message}")
        {
        }

        public ConnectionException(string message) : base(message)
        {
        }

        public ConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}