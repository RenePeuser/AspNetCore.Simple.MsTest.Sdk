# Pipeline Pattern Examples

## Example: HTTP Assertion Pipeline

This example shows the actual implementation from the codebase.

### Step Interface

```csharp
namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    /// <summary>
    /// Represents a single step in the HTTP assertion pipeline.
    /// Steps are executed sequentially and can fail fast by throwing an assertion exception.
    /// </summary>
    public interface IHttpAssertionStep
    {
        /// <summary>
        /// Executes this assertion step on the given HTTP response context.
        /// If the assertion fails, this method should call Assert.Fail() to stop the pipeline.
        /// If the assertion succeeds, the method returns normally and the pipeline continues.
        /// Steps are validators only - they never modify the result.
        /// </summary>
        void Execute<TResult>(HttpResponseContext<TResult> context);
    }
}
```

### Pipeline Orchestrator

```csharp
namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    /// <summary>
    /// Pipeline that executes HTTP assertion steps sequentially.
    /// Each step validates a specific aspect of the HTTP response.
    /// The pipeline stops at the first failing step (Assert.Fail throws an exception).
    /// </summary>
    internal sealed class HttpAssertionPipeline(IEnumerable<IHttpAssertionStep> steps) 
        : IHttpAssertionPipeline
    {
        public TResult Execute<TResult>(HttpResponseContext<TResult> context)
        {
            // Execute each step in sequence
            // If a step calls Assert.Fail(), execution stops immediately
            foreach (var step in steps)
            {
                step.Execute(context);
            }

            // All assertions passed - return the result
            return context.CurrentResult!;
        }
    }
}
```

### Example Step: Status Code Validation

```csharp
namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    /// <summary>
    /// Validates that the HTTP status code matches expectations.
    /// Fast-fail step: if the status code is unexpected, the test fails immediately.
    /// This step replaces the UnexpectedStatusCodeStrategy.
    /// </summary>
    internal sealed class StatusCodeValidationStep(IOutputFormatter outputFormatter) 
        : IHttpAssertionStep
    {
        public void Execute<TResult>(HttpResponseContext<TResult> context)
        {
            // If status code matches expectations, continue to next step
            if (context.IsExpectedStatusCode)
            {
                return;
            }

            // Status code mismatch - build error message and fail fast
            var errorInfo = context.Request.IsSuccessStatusCode
                ? $"Expected OK but got {context.HttpStatusCode}"
                : $"Expected ERROR but got {context.HttpStatusCode}";

            var errorOutput = outputFormatter.GetOutputString(
                string.Empty,
                errorInfo,
                expectedResult,
                context.ContentAsString,
                string.Empty,
                string.Empty);

            // Fail immediately - pipeline stops here
            Assert.Fail(errorOutput);
        }
    }
}
```

### Example Step: Content Type Validation

```csharp
namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    /// <summary>
    /// Validates that the Content-Type header is application/json.
    /// This step assumes the status code was already validated by a previous step.
    /// </summary>
    internal sealed class ContentTypeHeaderValidationStep : IHttpAssertionStep
    {
        public void Execute<TResult>(HttpResponseContext<TResult> context)
        {
            var contentType = context.HttpResponseMessage.Content.Headers.ContentType?.MediaType;
            
            // If Content-Type is correct, continue
            if (contentType == "application/json")
            {
                return;
            }

            // Content-Type mismatch - fail immediately
            Assert.Fail($"Expected Content-Type: application/json, but got: {contentType}");
        }
    }
}
```

### Service Registration

```csharp
namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    public static class AddHttpAssertionPipelineExtension
    {
        /// <summary>
        /// Registers the HTTP assertion pipeline and its steps.
        /// Steps are executed in the order they are registered.
        /// </summary>
        public static void AddHttpAssertionPipeline(this IServiceCollection services)
        {
            // Register steps in execution order
            services.AddStatusCodeValidationStep();         // 1. Status code must match
            services.AddContentTypeHeaderValidationStep();  // 2. Content-Type must be JSON
            services.AddContentFormatValidationStep();      // 3. Body must be valid JSON
            services.AddJsonComparisonStep();               // 4. JSON comparison
            
            // Register the pipeline itself
            services.AddSingletonIfNotExists<IHttpAssertionPipeline, HttpAssertionPipeline>();
        }
    }
}
```

### Individual Step Registration

```csharp
namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    public static class AddStatusCodeValidationStepExtension
    {
        /// <summary>
        /// Registers the status code validation step and its dependencies.
        /// </summary>
        public static void AddStatusCodeValidationStep(this IServiceCollection services)
        {
            // 1. Register dependencies
            services.AddOutputFormatter();
            
            // 2. Register the step itself
            services.AddSingletonIfNotExists<IHttpAssertionStep, StatusCodeValidationStep>();
        }
    }
}
```

## Usage Example

```csharp
// In test code
var result = await client.AssertPostAsync<MyResponse>(
    "/api/endpoint",
    request,
    expectedResult: "expected-response.json");

// Internally, the pipeline executes:
// 1. StatusCodeValidationStep - validates 200 OK
// 2. ContentTypeHeaderValidationStep - validates application/json
// 3. ContentFormatValidationStep - validates JSON structure
// 4. JsonComparisonStep - compares expected vs actual JSON
// If any step fails, pipeline stops immediately with Assert.Fail()
```

## Adding a New Step

To add a new validation step:

1. **Create the step**:

```csharp
internal sealed class CustomHeaderValidationStep : IHttpAssertionStep
{
    public void Execute<TResult>(HttpResponseContext<TResult> context)
    {
        // Your validation logic
        if (!context.HttpResponseMessage.Headers.Contains("X-Custom-Header"))
        {
            Assert.Fail("Missing X-Custom-Header");
        }
    }
}
```

2. **Create registration extension**:

```csharp
public static class AddCustomHeaderValidationStepExtension
{
    public static void AddCustomHeaderValidationStep(this IServiceCollection services)
    {
        services.AddSingletonIfNotExists<IHttpAssertionStep, CustomHeaderValidationStep>();
    }
}
```

3. **Add to pipeline in desired order**:

```csharp
public static void AddHttpAssertionPipeline(this IServiceCollection services)
{
    services.AddStatusCodeValidationStep();
    services.AddContentTypeHeaderValidationStep();
    services.AddCustomHeaderValidationStep();  // <-- Insert here
    services.AddContentFormatValidationStep();
    services.AddJsonComparisonStep();
    
    services.AddSingletonIfNotExists<IHttpAssertionPipeline, HttpAssertionPipeline>();
}
```

**That's it!** No `CanHandle` logic needed, no orchestrator changes, just insert at the right position.

## Comparison: Before (Strategy Pattern) vs After (Pipeline Pattern)

### Before: Strategy Pattern with CanHandle

```csharp
// Each strategy needs CanHandle logic
public interface IResponseStrategy
{
    bool CanHandle(HttpResponseContext context);
    void Execute(HttpResponseContext context);
}

// Strategy orchestrator needs to find the right one
var strategy = strategies.Single(s => s.CanHandle(context));
strategy.Execute(context);

// Adding new strategy requires CanHandle logic
public class NewStrategy : IResponseStrategy
{
    public bool CanHandle(HttpResponseContext context) 
        => /* complex logic to determine if this applies */;
    
    public void Execute(HttpResponseContext context) { /* ... */ }
}
```

### After: Pipeline Pattern - No CanHandle

```csharp
// Steps just execute - no CanHandle needed
public interface IHttpAssertionStep
{
    void Execute<TResult>(HttpResponseContext<TResult> context);
}

// Pipeline just runs all steps in order
foreach (var step in steps)
{
    step.Execute(context);
}

// Adding new step is simpler - just implement Execute
public class NewStep : IHttpAssertionStep
{
    public void Execute<TResult>(HttpResponseContext<TResult> context)
    {
        // Just do your validation - no CanHandle logic!
    }
}
```

The Pipeline Pattern is simpler when you need sequential execution without conditional branching.
