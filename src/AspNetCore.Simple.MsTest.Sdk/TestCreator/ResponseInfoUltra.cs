using System;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Simple.MsTest.Sdk.TestCreator
{
    public record ResponseInfoUltra(Type ResponseType, string Body, int StatusCode,
                                    HttpResponse httpResponse);
}
