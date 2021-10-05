using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHR.NET;

/// <summary>
/// JSONの値に<see cref="EnumMemberAttribute.Value"/>を用いる<typeparamref name="TEnum"/>のコンバーター。
/// </summary>
/// <typeparam name="TEnum">シリアライズ対象となる列挙型</typeparam>
[ExcludeFromCodeCoverage]
internal class JsonStringEnumConverterEx<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    internal static readonly Dictionary<TEnum, string> EnumToString = new();
    internal static readonly Dictionary<string, TEnum> StringToEnum = new();

    static JsonStringEnumConverterEx()
    {
        var type = typeof(TEnum);
#if NET5_0_OR_GREATER
        foreach (var value in Enum.GetValues<TEnum>())
#else
        foreach (TEnum value in Enum.GetValues(type))
#endif
        {
            var enumMember = type.GetMember(value.ToString())[0];
            var attr = enumMember.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                .Cast<EnumMemberAttribute>()
                .FirstOrDefault();

            StringToEnum.Add(value.ToString(), value);

            if (attr?.Value is not null)
            {
                EnumToString.Add(value, attr.Value);
                StringToEnum.Add(attr.Value, value);
            }
            else
            {
                EnumToString.Add(value, value.ToString());
            }
        }
    }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetString() is string value && StringToEnum.TryGetValue(value, out var enumValue)
            ? enumValue : default;

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        => writer.WriteStringValue(EnumToString[value]);
}
