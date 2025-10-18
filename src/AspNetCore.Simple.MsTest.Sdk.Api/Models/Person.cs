using System.Collections.Immutable;

namespace AspNetCore.Simple.MsTest.Sdk.Api.Models
{
    public sealed record Person(long Id,
                                string Name,
                                string FirstName,
                                int Age,
                                ImmutableList<Email> Emails);

    public sealed record Email(string EmailAddress,
                               string Type);
}
