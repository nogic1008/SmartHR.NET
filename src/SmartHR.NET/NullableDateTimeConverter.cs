using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHR.NET;

/// <summary>
/// "yyyy-MM-dd HH:mm:ss" &lt;-&gt; DateTime? 変換
/// </summary>
public class NullableDateTimeConverter : JsonConverter<DateTime?>
{
    /// <inheritdoc/>
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TryGetDateTime(out var dateTime) ? dateTime : null;

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.GetValueOrDefault());
    }
}
