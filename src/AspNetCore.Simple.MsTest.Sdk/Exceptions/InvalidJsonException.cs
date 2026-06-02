using System;
#pragma warning disable CA1032 // Implement standard exception constructors
namespace AspNetCore.Simple.MsTest.Sdk
{
    public class InvalidJsonException : Exception
    {
        internal InvalidJsonException(string message) : base(message)
        {
        }

        public InvalidJsonException(string message,
                                    Exception innerException) : base(message, innerException)
        {
        }
    }
}
#pragma warning restore CA1032 // Implement standard exception constructors