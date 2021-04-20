using System;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class UnexpectedResultException : Exception
    {
        public UnexpectedResultException(string message) : base(message)
        {
        }
    }
}