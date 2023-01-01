using System;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal sealed class UnexpectedResultException : Exception
    {
        internal UnexpectedResultException(string message) : base(message)
        {
        }
    }
}
