using System;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class DeserializeException : Exception
    {
        public DeserializeException(string message) : base(message)
        {
        }
    }
}