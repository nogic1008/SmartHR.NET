using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>部署情報</summary>
/// <param name="Id">部署ID</param>
/// <param name="Name">名称</param>
/// <param name="Position">ポジション</param>
/// <param name="Code">コード</param>
/// <param name="Parent">親の部署</param>
/// <param name="Children">子の部署</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record Department(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("position")] int Position,
    [property: JsonPropertyName("code")] string? Code = null,
    [property: JsonPropertyName("parent")] Department? Parent = null,
    [property: JsonPropertyName("children")] IReadOnlyList<Department>? Children = null,
    [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
    [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
);
