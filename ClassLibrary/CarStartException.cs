using System;

namespace ClassLibrary
{
    public class CarStartException : Exception
    {
        public CarStartException()
        {
        }

        public CarStartException(string message)
            : base(message)
        {
        }

        public CarStartException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}