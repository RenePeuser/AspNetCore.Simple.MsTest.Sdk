using System.Collections.Immutable;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal sealed record ResponseInfoUltra(ImmutableList<ResponseType> ResponseType,
                                             string Body,
                                             int StatusCode,
                                             HttpResponse HttpResponse);
}
