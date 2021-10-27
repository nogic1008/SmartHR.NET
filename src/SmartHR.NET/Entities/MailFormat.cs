using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>メールフォーマット情報</summary>
/// <param name="Id">メールフォーマットID</param>
/// <param name="MailType">メール種別</param>
/// <param name="Name">名称</param>
/// <param name="CrewInputForms">所属している従業員情報収集フォームのリスト</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record MailFormat(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("mail_type")] MailFormat.Kind MailType,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("crew_input_forms")] IReadOnlyList<CrewInputForm>? CrewInputForms = null,
    [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
    [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
)
{
    /// <summary>
    /// <inheritdoc cref="MailFormat" path="/param[@name='MailType']/text()"/>
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Kind>))]
    public enum Kind
    {
        [EnumMember(Value = "invitation")] Invitation
    }
}
