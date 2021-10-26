using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>従業員カスタム項目グループ情報</summary>
/// <param name="Id">従業員カスタム項目グループID</param>
/// <param name="Name">カスタム項目グループ名</param>
/// <param name="Position">ポジション</param>
/// <param name="AccessType">アクセス種別</param>
/// <param name="Templates">所属している従業員カスタム項目のリスト</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record CrewCustomFieldTemplateGroup(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("position")] int? Position,
    [property: JsonPropertyName("access_type")] CrewCustomFieldTemplateGroup.Accessibility AccessType,
    // TODO: CrewCustomFieldTemplateオブジェクトの定義
    [property: JsonPropertyName("templates")] IReadOnlyList<JsonElement>? Templates,
    [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
    [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
)
{
    /// <summary>
    /// <inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='AccessType']/text()"/>
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Accessibility>))]
    public enum Accessibility
    {
        [EnumMember(Value = "read_and_update")] ReadAndUpdate,
        [EnumMember(Value = "hidden")] Hidden,
        [EnumMember(Value = "read_and_update_values")] ReadAndUpdateValues,
    }
}
