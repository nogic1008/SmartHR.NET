using System;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>続柄</summary>
/// <param name="Id">続柄ID</param>
/// <param name="Name">続柄名</param>
/// <param name="PresetType">プリセット続柄</param>
/// <param name="IsChild">子であるかどうか</param>
/// <param name="Position">ポジション</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record DependentRelation(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("preset_type")] string? PresetType = null,
    [property: JsonPropertyName("is_child")] bool? IsChild = default,
    [property: JsonPropertyName("position")] int? Position = default,
    [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
    [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
);
