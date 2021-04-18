using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Simple.MsTest.Sdk.TestCreator
{
    public class RequestTestCreator : IRequestTestCreator
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
