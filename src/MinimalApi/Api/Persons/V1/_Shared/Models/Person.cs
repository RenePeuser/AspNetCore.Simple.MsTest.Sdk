using System.Collections.Immutable;

namespace MinimalApi.Api.Persons.V1
{
    public sealed record Person(long Id,
                                string Name,
                                string FirstName,
                                int Age,
                                ImmutableList<Email> Emails);

    public sealed record Email(string EmailAddress,
                               string Type);
}