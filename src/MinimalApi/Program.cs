using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Api.Errors;
using MinimalApi.Api.NativeTypes;
using MinimalApi.Api.Persons;
using MinimalApi.Api.Versioning;
using MinimalApi.Endpoints;
using MinimalApi.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

// Add minimal required services
builder.Services.AddProblemDetails();

// Add error handling
builder.Services.AddErrorHandlingMiddleware();

// Register endpoint orchestrator
builder.Services.AddSingleton<RegisterEndpoints>();

// Register feature endpoints
builder.Services.AddPersons();
builder.Services.AddErrors();
builder.Services.AddNativeTypes();

builder.Services.AddErrorHandling();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();

// Add API versioning
builder.Services.AddApiVersioning(apiVersion =>
{
    // apiVersion.DefaultApiVersion = new ApiVersion(1, 0);
    // Url segment for the classic routes; query string, header and media type for the versioning samples.
    // "ver", "x-custom-version" and ";v=" are no common names - the sdk has to detect them from this reader
    apiVersion.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                           new QueryStringApiVersionReader("api-version", "ver"),
                                                           new HeaderApiVersionReader("x-api-version", "x-custom-version"),
                                                           new MediaTypeApiVersionReader("v"));
}).AddApiExplorer(apiExplorer =>
{
    apiExplorer.GroupNameFormat = "'v'V";
    apiExplorer.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

app.UseHttpsRedirection();

// No error middleware
app.UseErrorHandling();

// Setup API versioning
var apiVersionSet = app.NewApiVersionSet()
                       .HasApiVersion(new ApiVersion(1))
                       .ReportApiVersions()
                       .Build();

// Setup base path for API versioning
var basePath = app.MapGroup("api/v{version:apiVersion}")
                  .WithApiVersionSet(apiVersionSet);

// Register endpoints
var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
registerEndpoints.MapEndpoints(basePath);

// V1 and V2 side by side (own version sets) - via url segment, query string and header
app.MapVersioningEndpoints();

app.Run();

// Important for API tests!
namespace MinimalApi
{
    public partial class Program
    {
    }
}