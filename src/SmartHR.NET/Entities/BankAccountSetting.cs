using System;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>口座情報</summary>
/// <param name="Id">口座情報ID</param>
/// <param name="Name">名称</param>
/// <param name="Enabled">有効かどうか</param>
/// <param name="Number">順序</param>
/// <param name="PresetType">プリセットタイプ</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record BankAccountSetting(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("enabled")] bool Enabled,
    [property: JsonPropertyName("number")] int Number,
    [property: JsonPropertyName("preset_type")] string? PresetType,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTimeOffset UpdatedAt
);
