using System.Collections.Generic;
using System.Collections.Immutable;

namespace Controllers.Api.Persons
{
    public sealed record Person(long Id,
                                string Name,
                                string FirstName,
                                int Age,
                                ImmutableList<Email> Emails);

    /// <summary>
    /// A person MINUS <c>emails</c>. Declared as the response type of the lean endpoint, which writes a
    /// full <see cref="Person"/> anyway - so the body carries a field this type does not model.
    ///
    /// That mismatch is the point. It mirrors what a real api does when it declares an abstract base and
    /// writes the concrete shape, and it is the only way to notice when the snapshot diff stops comparing
    /// the raw response body: a type that models everything loses nothing on a round trip and shows no
    /// difference either way.
    /// </summary>
    public sealed record PersonWithoutEmails(long Id,
                                             string Name,
                                             string FirstName,
                                             int Age);

    public sealed record Email(string EmailAddress,
                               string Type);

    /// <summary>Response of the echo-header action — carries back the correlation id it received.</summary>
    public sealed record EchoHeaderResponse(string CorrelationId);

    /// <summary>
    /// Response of the echo-query action: the query string as it reached the server (<see cref="RawQuery"/>, still
    /// encoded) and as model binding sees it (<see cref="Values"/>, key → values in arrival order).
    /// </summary>
    public sealed record EchoQueryResponse(string RawQuery,
                                           IReadOnlyDictionary<string, string[]> Values);

    /// <summary>
    /// Response of the echo-authorization action: <c>"(absent)"</c> when the request carried no
    /// <c>Authorization</c> header, otherwise its value.
    /// </summary>
    public sealed record EchoAuthorizationResponse(string Authorization);

    /// <summary>The request body of <c>POST persons/echo-request</c>.</summary>
    public sealed record EchoRequestBody(string Name);

    /// <summary>Response of <c>POST persons/echo-request</c> — the correlation id header and the body name.</summary>
    public sealed record EchoRequestResponse(string CorrelationId,
                                             string Name);
}