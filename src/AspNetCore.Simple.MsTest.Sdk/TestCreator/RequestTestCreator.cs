using System.Collections.Generic;
using System.Linq;
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
        string CreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfoUltra);
    }

    internal class RequestTestCreator : IRequestTestCreator
    {
        private readonly IEnumerable<ISpecificTestCreator> _testCreators;

        public RequestTestCreator(IEnumerable<ISpecificTestCreator> testCreators)
        {
            _testCreators = testCreators;
        }

        public string CreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfoUltra)
        {
            var testCreator = _testCreators.Where(creator => creator.CanCreateTestFor(requestInfo, responseInfoUltra)).ToList();
            if (testCreator.Count == 1)
            {
                return testCreator[0].CreateTestFor(requestInfo, responseInfoUltra);
            }

            return string.Empty;
        }
    }
}
