namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Provides assertion methods with cleaner output compared to standard Assert.Fail().
    /// Throws AssertFailedException directly without the "Assert.Fail failed." prefix.
    /// </summary>
    public static class AssertThat
    {
        /// <summary>
        /// Fails the test with the specified message.
        /// Unlike Assert.Fail(), this throws AssertFailedException directly without additional prefix.
        /// Output: "SNAPSHOT TEST FAILED..." instead of "Assert.Fail failed. SNAPSHOT TEST FAILED..."
        /// </summary>
        /// <param name="_">just placeholder to use it as extension</param>
        /// <param name="message">The error message to display</param>
        public static void Fail(this Assert _,
                                string message)
        {
            throw new AssertFailedException(message);
        }
    }
}