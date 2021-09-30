using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Tests;

public static class JsonConfig
{
    internal static readonly JsonSerializerOptions Default;
    static JsonConfig()
    => Default = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
}
