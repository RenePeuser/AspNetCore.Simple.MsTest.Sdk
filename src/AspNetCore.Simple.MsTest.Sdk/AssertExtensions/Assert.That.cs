namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Modern, AI-friendly assertion methods with rich context and fix guidance.
    /// All assertions require 'because' (why) and 'fix' (how to resolve) parameters.
    ///
    /// Usage:
    ///   Assert.That.IsNotNull(user,
    ///       because: "User must exist after registration",
    ///       fix: "Check database seeding in TestBase.InitializeAsync()");
    /// </summary>
    public static partial class AssertThat
    {
        // This is the main partial class definition.
        // All assertion methods are implemented in category-specific files:
        // - Core/Assert.That.Null.cs
        // - Core/Assert.That.Boolean.cs
        // - Numeric/Assert.That.Comparison.cs
        // - String/Assert.That.String.Content.cs
        // - Collection/Assert.That.Collection.Count.cs
        // ... and many more

        // Shared helpers can be added here if needed across multiple categories
    }
}