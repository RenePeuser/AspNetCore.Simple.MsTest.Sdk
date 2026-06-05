using System;
using Asp.Versioning;
using Extensions.Pack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApi.Extensionmethods
{
    internal static class SimpleMinimalApiExtensions
    {
        internal static void AddSimpleMinimalApiEnvironment(this IServiceCollection services)
        {
            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            services.AddProblemDetails();

            // Add API versioning
            services.AddApiVersioning(apiVersion =>
            {
                apiVersion.DefaultApiVersion = new ApiVersion(1, 0);
                apiVersion.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddApiExplorer(apiExplorer =>
            {
                apiExplorer.GroupNameFormat = "'v'V";
                apiExplorer.SubstituteApiVersionInUrl = true;
            });
        }

        internal static void UseSimpleMinimalApiEnvironment(this WebApplication webApplication,
                                                            Action<IEndpointRouteBuilder> endpointRegistration)
        {
            webApplication.UseHttpsRedirection();

            // Setup API versioning
            var apiVersionSet = webApplication.NewApiVersionSet()
                                              .HasApiVersion(new ApiVersion(1))
                                              .ReportApiVersions()
                                              .Build();

            // Setup base path for API versioning
            var basePath = webApplication.MapGroup("api/v{apiVersion:apiVersion}")
                                         .WithApiVersionSet(apiVersionSet);

            endpointRegistration(basePath);
        }
    }

    public static class EndpointConventionBuilderExtensions
    {
        public static TBuilder WithSummaryFromFile<TBuilder>(this TBuilder builder,
                                                             string embeddedFile)
            where TBuilder : IEndpointConventionBuilder
        {
            var fileContent = EmbeddedFile.GetFileContentFrom(embeddedFile);

            return builder.WithSummary(fileContent);
        }

        public static TBuilder WithDescriptionFromFile<TBuilder>(this TBuilder builder,
                                                                 string embeddedFile)
            where TBuilder : IEndpointConventionBuilder
        {
            var fileContent = EmbeddedFile.GetFileContentFrom(embeddedFile);

            return builder.WithDescription(fileContent);
        }
    }
}