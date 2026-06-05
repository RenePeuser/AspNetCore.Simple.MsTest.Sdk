using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AspNetCore.Simple.MsTest.Sdk;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Persons.V1.Shared
{
    /// <summary>
    /// Shared helper functions for Person endpoint tests.
    /// These can be reused across Native, Fluent Neutral, and Fluent Endpoint style tests.
    /// </summary>
    internal static class TestHelpers
    {
        /// <summary>
        /// Orders persons by ID for consistent comparison.
        /// </summary>
        public static IEnumerable<Person>? OrderByIdFilter(IEnumerable<Person>? persons)
        {
            return persons?.OrderBy(x => x.Id).ToImmutableList();
        }

        /// <summary>
        /// Filters out differences for the Id property.
        /// Useful when IDs are auto-generated and shouldn't be compared.
        /// </summary>
        public static IEnumerable<Difference> IgnoreIdDifferences(ImmutableList<Difference> differences)
        {
            foreach (var difference in differences)
            {
                if (difference.MemberPath == nameof(Person.Id))
                {
                    continue;
                }

                yield return difference;
            }
        }

        /// <summary>
        /// Creates a valid test person object.
        /// </summary>
        public static Person CreateValidPerson()
        {
            return new Person(Id: 1,
                              Name: "Son",
                              FirstName: "Goku",
                              Age: 42,
                              Emails: ImmutableList<Email>.Empty);
        }

        /// <summary>
        /// Creates a person with email addresses.
        /// </summary>
        public static Person CreatePersonWithEmails()
        {
            return new Person(Id: 1,
                              Name: "Son",
                              FirstName: "Goku",
                              Age: 99,
                              Emails: ImmutableList.Create(new Email("alf@gmx.de", "GMX"),
                                                           new Email("abc@hotmail.de", "Microsoft")));
        }
    }
}