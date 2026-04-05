# Pipeline Pattern

## Objective

Use the Pipeline Pattern when processing requires multiple sequential steps that must execute in a specific order.

Key characteristics:
- All steps execute sequentially (unless one fails)
- Steps are executed in registration order
- Fail-fast: pipeline stops at first failing step
- No `CanHandle` logic needed - simpler than Strategy Pattern
- Each step focuses on a single responsibility

## Apply These Rules

- Consider the Pipeline Pattern when you need to execute multiple sequential operations on the same data
- Steps execute in the order they are registered in DI
- Each step should have a single, clear responsibility
- Steps should fail fast if their validation or processing fails
- Steps are validators or processors - they don't decide IF they should run (no `CanHandle`)
- The pipeline orchestrator iterates through all steps and calls `Execute` on each
- If a step throws an exception (e.g., `Assert.Fail`), the pipeline stops immediately
- Steps should not modify shared state - they validate or process the input context
- Register steps via DI in the order they should execute

## When to Use Pipeline vs Strategy Pattern

### Use Pipeline Pattern when:
- You need to execute multiple operations sequentially
- All steps should run (unless one fails)
- Steps don't need to decide whether they apply (no conditional logic)
- The order of execution matters
- You want fail-fast behavior
- **Extending is simpler**: just add a new step, no `CanHandle` logic needed

### Use Strategy Pattern when:
- You need to select exactly ONE implementation based on conditions
- Each strategy has `CanHandle` logic to determine if it applies
- Only one strategy should execute
- Zero or multiple matches should fail explicitly
- **More complex extension**: new strategies need `CanHandle` logic

## Pattern Structure

### 1. Step Interface

```csharp
public interface IHttpAssertionStep
{
    void Execute<TResult>(HttpResponseContext<TResult> context);
}
```

- Simple contract: just `Execute`
- No `CanHandle` method needed
- Takes a context object with all required data

### 2. Pipeline Orchestrator

```csharp
internal sealed class HttpAssertionPipeline(IEnumerable<IHttpAssertionStep> steps) 
    : IHttpAssertionPipeline
{
    public TResult Execute<TResult>(HttpResponseContext<TResult> context)
    {
        foreach (var step in steps)
        {
            step.Execute(context);
        }
        
        return context.CurrentResult!;
    }
}
```

- Injects all steps via `IEnumerable<IXxxStep>`
- Iterates through steps in registration order
- No selection logic - just executes all steps
- Stops automatically if a step throws

### 3. Individual Steps

```csharp
internal sealed class StatusCodeValidationStep(IOutputFormatter outputFormatter) 
    : IHttpAssertionStep
{
    public void Execute<TResult>(HttpResponseContext<TResult> context)
    {
        if (context.IsExpectedStatusCode)
        {
            return; // Step passes - continue to next
        }
        
        // Build error message and fail
        var errorOutput = BuildErrorMessage(context);
        Assert.Fail(errorOutput); // Throws - pipeline stops here
    }
}
```

- Focused on single responsibility
- Early return if validation passes
- Throws exception to fail fast
- No `CanHandle` logic needed

### 4. Service Registration

```csharp
public static void AddHttpAssertionPipeline(this IServiceCollection services)
{
    // Register steps in execution order
    services.AddStatusCodeValidationStep();         // 1. Status code
    services.AddContentTypeHeaderValidationStep();  // 2. Content-Type
    services.AddContentFormatValidationStep();      // 3. JSON format
    services.AddJsonComparisonStep();               // 4. JSON comparison
    
    // Register the pipeline itself
    services.AddSingletonIfNotExists<IHttpAssertionPipeline, HttpAssertionPipeline>();
}
```

- Steps are registered in execution order
- Each step registers its own dependencies via its own `AddXxxStep()` extension
- Pipeline is registered last
- Order matters - document the sequence in comments

## Benefits Over Strategy Pattern

1. **Simpler Extension**: Add a new step without `CanHandle` logic
2. **Clear Execution Flow**: Steps run in registration order
3. **Single Responsibility**: Each step validates/processes one aspect
4. **Less Branching**: No conditional logic to determine which step runs
5. **Fail Fast**: First failing step stops the entire pipeline
6. **Easier Testing**: Test each step independently

## When Steps Need Dependencies

Steps can inject their own dependencies:

```csharp
internal sealed class JsonComparisonStep(
    IPrimitiveTypeConverter primitiveTypeConverter,
    IJsonDiffer jsonDiffer,
    IOutputFormatter outputFormatter) : IHttpAssertionStep
{
    public void Execute<TResult>(HttpResponseContext<TResult> context)
    {
        // Use injected dependencies
    }
}
```

Register dependencies in the step's registration extension:

```csharp
public static void AddJsonComparisonStep(this IServiceCollection services)
{
    // 1. Register dependencies
    services.AddPrimitiveTypeConverter();
    services.AddJsonDiffer();
    services.AddOutputFormatter();
    
    // 2. Register the step itself
    services.AddSingletonIfNotExists<IHttpAssertionStep, JsonComparisonStep>();
}
```

## Exception Guidance

- Steps should throw meaningful exceptions when validation fails
- Include context in error messages (what was expected vs. what was found)
- Use repository-specific exception types when available
- Format error output for clear test failure messages

## Avoid

- Adding `CanHandle` logic to steps (use Strategy Pattern instead)
- Steps that modify the context in ways that affect other steps
- Silent failures - steps should fail explicitly
- Steps with multiple responsibilities - keep them focused
- Registering steps in the wrong order
- Steps that depend on execution order implicitly - make dependencies explicit via DI

## Notes

The Pipeline Pattern in this repository:
- Sequential execution of all steps
- Fail-fast behavior on first failure
- No conditional step selection
- Simple extension by adding new steps
- Order controlled by registration sequence
- Each step independently testable
- Feature-based DI registration per step
