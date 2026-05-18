using System.Collections.Immutable;

#pragma warning disable CA1032 // Implement standard exception constructors
namespace MinimalApi.ErrorHandling.Exceptions
{
    public class SecurityProblemException : ProblemDetailsException
    {
        public SecurityProblemException(string title,
                                        string details,
                                        params (string key, object value)[] extensions) : base(StatusCodes.Status400BadRequest, title, details,
                                                                                               extensions)
        {
        }

        public SecurityProblemException(string title,
                                        string details,
                                        IImmutableDictionary<string, object> extensions) : base(StatusCodes.Status400BadRequest, title, details,
                                                                                                extensions)
        {
        }
    }
}
#pragma warning restore CA1032 // Implement standard exception constructors