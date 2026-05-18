namespace AspNetCore.Simple.MsTest.Sdk
{
    public class DeserializeException(string message) : Exception(message)
    {
        public DeserializeException()
        {
        }
    }
}