using System.Collections.Immutable;

namespace Controllers.Api.Persons
{
    public sealed record Person(long Id,
                                string Name,
                                string FirstName,
                                int Age,
                                ImmutableList<Email> Emails);

    public sealed record Email(string EmailAddress,
                               string Type);

    /// <summary>Response of the echo-header action — carries back the correlation id it received.</summary>
    public sealed record EchoHeaderResponse(string CorrelationId);
}