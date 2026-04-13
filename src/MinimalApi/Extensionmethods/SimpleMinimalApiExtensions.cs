using Asp.Versioning;
using Extensions.Pack;
using MinimalApi.Api.Health;
using MinimalApi.Swagger;

namespace MinimalApi.Extensionmethods
{
    public static class SimpleMinimalApiExtensions
    {
        public static void AddSimpleMinimalApiEnvironment(this IServiceCollection services)
        {
            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.DocumentFilter<OpenApiVersionFilter>();
            });
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

            services.ConfigureOptions<FixApiVersionConfigureNameOption>();
        }

        public static void UseSimpleMinimalApiEnvironment(this WebApplication webApplication,
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
            webApplication.MapHealth();


            // Configure the HTTP request pipeline.
            if (webApplication.Environment.IsDevelopment())
            {
                webApplication.UseSwagger();
                webApplication.UseSwaggerUI(options =>
                {
                    var apiVersionDescriptions = webApplication.DescribeApiVersions();
                    foreach (var apiVersionDescription in apiVersionDescriptions)
                    {
                        var url = $"/swagger/{apiVersionDescription.GroupName}/swagger.json";
                        var name = $"V{apiVersionDescription.ApiVersion.MajorVersion}";

                        options.SwaggerEndpoint(url, name);
                    }
                });
            }
        }
    }


    public static class EndpointConventionBuilderExtensions
    {
        public static TBuilder WithSummaryFromFile<TBuilder>(this TBuilder builder, string embeddedFile)
            where TBuilder : IEndpointConventionBuilder
        {
            var fileContent = EmbeddedFile.GetFileContentFrom(embeddedFile);
            return builder.WithSummary(fileContent);
        }

        public static TBuilder WithDescriptionFromFile<TBuilder>(this TBuilder builder, string embeddedFile)
            where TBuilder : IEndpointConventionBuilder
        {
            var fileContent = EmbeddedFile.GetFileContentFrom(embeddedFile);
            return builder.WithDescription(fileContent);
        }
    }
}
