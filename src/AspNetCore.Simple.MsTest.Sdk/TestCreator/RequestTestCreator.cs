using System.Collections.Generic;
using System.Linq;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddRequestTestCreatorExtension
    {
        public static void AddRequestTestCreator(this IServiceCollection services)
        {
            services.AddSingleton<IRequestTestCreator, RequestTestCreator>();
        }
    }

    internal interface IRequestTestCreator
    {
        string CreateTestFor(RequestInfo requestInfo,
                             ResponseInfoUltra responseInfoUltra);
    }

    internal sealed class RequestTestCreator(IEnumerable<ISpecificTestCreator> testCreators) : IRequestTestCreator
    {
        public string CreateTestFor(RequestInfo requestInfo,
                                    ResponseInfoUltra responseInfoUltra)
        {
            var testCreator = testCreators.Where(creator => creator.CanCreateTestFor(requestInfo, responseInfoUltra)).ToList();

            if (testCreator.Count.EqualsTo(1))
            {
                return testCreator[0].CreateTestFor(requestInfo, responseInfoUltra);
            }

            return string.Empty;
        }
    }
}
