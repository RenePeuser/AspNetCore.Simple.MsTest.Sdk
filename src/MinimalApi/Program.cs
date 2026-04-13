using System.Collections.Immutable;
using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Api.ToDos;
using MinimalApi.Api.ToDos.V1;
using MinimalApi.Database.DbContext;
using MinimalApi.ErrorHandling;
using MinimalApi.Extensionmethods;
using StrategyPattern.Evolution;


var builder = WebApplication.CreateBuilder(args);

// Brand new cool stuff -> Ready to run registration
builder.Services.AddSimpleMinimalApiEnvironment();

builder.Services.AddErrorHandlingMiddleware();

builder.Services.AddToDos();

var app = builder.Build();

app.UseErrorHandling();

app.UseSimpleMinimalApiEnvironment(endpoint =>
                                   {
                                       // API endpoint registrations
                                       // API -> Domain -> V1 -> GetAll
                                       endpoint.UseToDos();
                                   });

// Register endpoints
var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
registerEndpoints.MapEndpoints(app);

app.Run();


// important for api tests !
namespace MinimalApi
{
    public class Program
    {
    }
}
