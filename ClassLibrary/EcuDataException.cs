using System;

namespace ClassLibrary
{
    public class EcuDataException : Exception
    {
        public EcuDataException()
        {
        }

        public EcuDataException(string message)
            : base(message)
        {
        }

        public EcuDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}