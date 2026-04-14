using Asp.Versioning;
using Asp.Versioning.Builder;
using MinimalApi.Api.Errors;
using MinimalApi.Api.NativeTypes;
using MinimalApi.Api.Persons;
using MinimalApi.ErrorHandling;
using StrategyPattern.Evolution;

var builder = WebApplication.CreateBuilder(args);

// Add minimal required services
builder.Services.AddProblemDetails();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

// Add error handling
builder.Services.AddErrorHandlingMiddleware();

// Register endpoint orchestrator
builder.Services.AddSingleton<RegisterEndpoints>();

// Register feature endpoints
builder.Services.AddPersons();
builder.Services.AddErrors();
builder.Services.AddNativeTypes();

var app = builder.Build();

// Use error handling middleware
app.UseErrorHandling();

// Use routing (required for endpoint mapping)
app.UseRouting();

// Map endpoints using UseEndpoints
var endpointRegistrations = app.Services.GetRequiredService<RegisterEndpoints>();

var apiVersionSet = app.NewApiVersionSet();
var apiVersions = apiVersionSet.HasApiVersion(new ApiVersion(1)).Build();

var apiV1 = app.MapGroup("api/v1").WithApiVersionSet(apiVersions);
endpointRegistrations.MapEndpoints(apiV1);

app.Run();

// Important for API tests!
namespace MinimalApi
{
    public class Program
    {
    }
}
