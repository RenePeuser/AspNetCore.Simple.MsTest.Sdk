using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinimalApi.Swagger
{
    internal sealed class FixApiVersionConfigureNameOption(IApiVersionDescriptionProvider apiVersionDescriptionProvider) : IConfigureNamedOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var apiVersionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                var openApiInfo = new OpenApiInfo
                                  {
                                      Title = "Minimal API - Swagger Title",
                                      Version = apiVersionDescription.ApiVersion.ToString()
                                  };

                options.SwaggerDoc(apiVersionDescription.GroupName, openApiInfo);
            }
        }

        public void Configure(string? name,
                              SwaggerGenOptions options)
        {
            Configure(options);
        }
    }
}
