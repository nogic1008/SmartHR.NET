using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>従業員カスタム項目情報</summary>
/// <param name="Id">従業員カスタム項目ID</param>
/// <param name="Name">項目名</param>
/// <param name="Type">入力タイプ</param>
/// <param name="Elements">ドロップダウン項目</param>
/// <param name="GroupId">
/// <see cref="CrewCustomFieldTemplateGroup.Id"/>
/// <paramref name="GroupId"/>か<paramref name="Group"/>のいずれかが出力されます。
/// </param>
/// <param name="Group">
/// 所属する<inheritdoc cref="CrewCustomFieldTemplateGroup" path="/summary/text()" />
/// <paramref name="GroupId"/>か<paramref name="Group"/>のいずれかが出力されます。
/// </param>
/// <param name="Hint">入力ヒント</param>
/// <param name="Scale"><see cref="Input.Decimal"/>の場合の有効桁数</param>
/// <param name="SeparatedByCommas">入力された数値を3桁ごとにカンマで区切って表示</param>
/// <param name="Position">ポジション</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record CrewCustomFieldTemplate(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] CrewCustomFieldTemplate.Input Type,
    [property: JsonPropertyName("elements")] IReadOnlyList<CrewCustomFieldTemplate.Element>? Elements,
    [property: JsonPropertyName("group_id")] string? GroupId,
    [property: JsonPropertyName("group")] CrewCustomFieldTemplateGroup? Group,
    [property: JsonPropertyName("hint")] string? Hint,
    [property: JsonPropertyName("scale")] int? Scale,
    [property: JsonPropertyName("separated_by_commas")] bool? SeparatedByCommas,
    [property: JsonPropertyName("position")] int Position,
    [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
    [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
)
{
    /// <summary>
    /// <inheritdoc cref="CrewCustomFieldTemplate" path="/param[@name='Type']/text()" />
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Input>))]
    public enum Input
    {
        [EnumMember(Value = "date")] Date,
        [EnumMember(Value = "decimal")] Decimal,
        [EnumMember(Value = "enum")] Enum,
        [EnumMember(Value = "file")] File,
        [EnumMember(Value = "string")] String,
        [EnumMember(Value = "text")] Text,
    }

    /// <summary>ドロップダウンリスト項目</summary>
    /// <param name="Id">ドロップダウンリスト項目ID</param>
    /// <param name="Name">名称</param>
    /// <param name="PhysicalName">物理名称</param>
    /// <param name="Position">ポジション</param>
    public record Element(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("physical_name")] string? PhysicalName,
        [property: JsonPropertyName("position")] int Position
    );
}

