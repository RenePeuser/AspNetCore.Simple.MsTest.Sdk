using MinimalApi.Api.Errors;
using MinimalApi.Api.NativeTypes;
using MinimalApi.Api.Persons;
using MinimalApi.ErrorHandling;
using MinimalApi.Extensionmethods;
using StrategyPattern.Evolution;

var builder = WebApplication.CreateBuilder(args);

// Brand new cool stuff -> Ready to run registration
builder.Services.AddSimpleMinimalApiEnvironment();

builder.Services.AddErrorHandlingMiddleware();

builder.Services.AddSingleton<RegisterEndpoints>();

builder.Services.AddPersons();
builder.Services.AddErrors();
builder.Services.AddNativeTypes();

var app = builder.Build();

app.UseErrorHandling();

app.UseSimpleMinimalApiEnvironment(endpoint =>
                                   {
                                       // Register all IEndpoint implementations
                                       var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
                                       registerEndpoints.MapEndpoints(endpoint);
                                   });

app.Run();

// important for api tests !
namespace MinimalApi
{
    public class Program
    {
    }
}
