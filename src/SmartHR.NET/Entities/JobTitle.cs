using System;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>役職</summary>
/// <param name="Id">役職ID</param>
/// <param name="Name">役職の名前</param>
/// <param name="Rank">役職のランク (1 ~ 99999)</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record JobTitle(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("rank")] int? Rank,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTimeOffset UpdatedAt
);
