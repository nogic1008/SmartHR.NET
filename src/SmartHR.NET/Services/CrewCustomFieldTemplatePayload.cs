using System.Collections.Generic;
using System.Text.Json.Serialization;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Services;

/// <summary><inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/>APIのリクエストボディ</summary>
/// <inheritdoc cref="CrewCustomFieldTemplate" path="/param"/>
public record CrewCustomFieldTemplatePayload(
    [property: JsonPropertyName("name")] string? Name = null,
    [property: JsonPropertyName("type")] CrewCustomFieldTemplate.Input? Type = default,
    [property: JsonPropertyName("elements")] IReadOnlyList<CrewCustomFieldTemplatePayload.Element>? Elements = null,
    [property: JsonPropertyName("group_id")] string? GroupId = null,
    [property: JsonPropertyName("hint")] string? Hint = null,
    [property: JsonPropertyName("scale")] int? Scale = default,
    [property: JsonPropertyName("separated_by_commas")] bool? SeparatedByCommas = default,
    [property: JsonPropertyName("position")] int? Position = default
)
{
    /// <inheritdoc cref="CrewCustomFieldTemplate.Element"/>
    public record Element(
        [property: JsonPropertyName("id")] string? Id = null,
        [property: JsonPropertyName("name")] string? Name = null,
        [property: JsonPropertyName("physical_name")] string? PhysicalName = null,
        [property: JsonPropertyName("position")] int? Position = default
    );
}

