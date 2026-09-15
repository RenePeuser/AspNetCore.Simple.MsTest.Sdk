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
}
