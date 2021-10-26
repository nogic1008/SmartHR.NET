using System;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>Webhook情報</summary>
/// <param name="Id">Webhook ID</param>
/// <param name="Url">通知先URL</param>
/// <param name="Description">説明</param>
/// <param name="SecretToken">通知に付与されるSecret Token</param>
/// <param name="CrewCreated">従業員情報が作成された時に通知するかどうか</param>
/// <param name="CrewUpdated">従業員情報が更新された時に通知するかどうか</param>
/// <param name="CrewDeleted">従業員情報が削除された時に通知するかどうか</param>
/// <param name="CrewImported">従業員情報がインポートされた時に通知するかどうか</param>
/// <param name="DependentCreated">家族情報が作成された時に通知するかどうか</param>
/// <param name="DependentUpdated">家族情報が更新された時に通知するかどうか</param>
/// <param name="DependentDeleted">家族情報が削除された時に通知するかどうか</param>
/// <param name="DependentImported">家族情報がインポートされた時に通知するかどうか</param>
/// <param name="DisabledAt">無効化された日時</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record Webhook(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("url")] string? Url,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("secret_token")] string? SecretToken,
    [property: JsonPropertyName("crew_created")] bool CrewCreated,
    [property: JsonPropertyName("crew_updated")] bool CrewUpdated,
    [property: JsonPropertyName("crew_deleted")] bool CrewDeleted,
    [property: JsonPropertyName("crew_imported")] bool CrewImported,
    [property: JsonPropertyName("dependent_created")] bool DependentCreated,
    [property: JsonPropertyName("dependent_updated")] bool DependentUpdated,
    [property: JsonPropertyName("dependent_deleted")] bool DependentDeleted,
    [property: JsonPropertyName("dependent_imported")] bool DependentImported,
    [property: JsonPropertyName("disabled_at")] DateTimeOffset? DisabledAt = default,
    [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
    [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
);
