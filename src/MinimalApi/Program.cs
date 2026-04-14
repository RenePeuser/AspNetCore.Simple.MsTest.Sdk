using Asp.Versioning;
using MinimalApi.Api.Errors;
using MinimalApi.Api.NativeTypes;
using MinimalApi.Api.Persons;
using MinimalApi.ErrorHandling;
using MinimalApi.Startup;
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

// Add IStartupFilter for endpoint mapping (WebApplicationFactory compatibility)
builder.Services.AddEndpointMappingStartupFilter();

var app = builder.Build();

// Map endpoints for production (IStartupFilter handles testing scenario)
app.UseErrorHandling();
var apiV1 = app.MapGroup("api/v1");
var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
registerEndpoints.MapEndpoints(apiV1);

app.Run();

// Important for API tests!
namespace MinimalApi
{
    public class Program
    {
    }
}
