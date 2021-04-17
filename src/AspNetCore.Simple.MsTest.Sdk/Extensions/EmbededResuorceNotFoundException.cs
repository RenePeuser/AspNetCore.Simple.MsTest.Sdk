using System;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class EmbededResuorceNotFoundException : Exception
    {
        public EmbededResuorceNotFoundException(string message) : base(message)
        {
        }
    }
}