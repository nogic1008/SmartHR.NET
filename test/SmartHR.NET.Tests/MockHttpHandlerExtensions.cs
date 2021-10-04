using System.Net.Http.Json;
using System.Text.Json;
using Moq.Language.Flow;

namespace SmartHR.NET.Tests;

public static class MockHttpHandlerExtensions
{
    private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);
    public static IReturnsResult<HttpMessageHandler> ReturnsJson<T>(
        this ISetup<HttpMessageHandler, Task<HttpResponseMessage>> setup,
        T content,
        HttpStatusCode statusCode = HttpStatusCode.OK)
        => setup.ReturnsResponse(statusCode, JsonContent.Create(content, options: _options));
}

