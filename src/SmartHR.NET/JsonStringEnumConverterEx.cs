using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHR.NET;

/// <summary>
/// JSONの値に<see cref="EnumMemberAttribute.Value"/>を用いる<typeparamref name="TEnum"/>のコンバーター。
/// </summary>
/// <typeparam name="TEnum">シリアライズ対象となる列挙型</typeparam>
internal class JsonStringEnumConverterEx<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    private readonly Dictionary<TEnum, string> _enumToString = new();
    private readonly Dictionary<string, TEnum> _stringToEnum = new();

    public JsonStringEnumConverterEx()
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

            _stringToEnum.Add(value.ToString(), value);

            if (attr?.Value is not null)
            {
                _enumToString.Add(value, attr.Value);
                _stringToEnum.Add(attr.Value, value);
            }
            else
            {
                _enumToString.Add(value, value.ToString());
            }
        }
    }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetString() is string value && _stringToEnum.TryGetValue(value, out var enumValue)
            ? enumValue : default;

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        => writer.WriteStringValue(_enumToString[value]);
}
