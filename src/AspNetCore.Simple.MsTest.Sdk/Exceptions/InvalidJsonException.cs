using System;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class InvalidJsonException : Exception
    {
        internal InvalidJsonException(string message) : base(message)
        {
        }
    }
}
