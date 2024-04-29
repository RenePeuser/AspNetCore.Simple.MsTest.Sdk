
using System.Collections.Immutable;

namespace AspNetCore.Simple.MsTest.Sdk.Api.Models
{
    public record Person(long Id, string Name, string FirstName, int Age, IImmutableList<Email> Emails);

    public record Email(string EmailAddress, string Type);
}
