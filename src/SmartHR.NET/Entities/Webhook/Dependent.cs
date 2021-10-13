using System;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities.Webhook;

public record DependentWebhook(
    [property: JsonPropertyName("event")] WebhookEvent Event,
    [property: JsonPropertyName("triggered_at")] DateTimeOffset TriggeredAt,
    [property: JsonPropertyName("sender")] Crew? Sender,
    [property: JsonPropertyName("dependent")] Dependent Body
) : IWebhookObject;

/// <summary>家族データ</summary>
/// <param name="Id">家族ID</param>
/// <param name="CrewId">紐づく従業員のID</param>
/// <param name="Relation">続柄</param>
/// <param name="IsSpouse">配偶者かどうか</param>
/// <param name="LastName">姓</param>
/// <param name="FirstName">名</param>
/// <param name="LastNameYomi">姓 (カタカナ)</param>
/// <param name="FirstNameYomi">名 (カタカナ)</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record Dependent(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("crew_id")] string CrewId,
    [property: JsonPropertyName("relation")] DependentRelation? Relation = null,
    [property: JsonPropertyName("is_spouse")] bool? IsSpouse = default,
    [property: JsonPropertyName("last_name")] string? LastName = null,
    [property: JsonPropertyName("first_name")] string? FirstName = null,
    [property: JsonPropertyName("last_name_yomi")] string? LastNameYomi = null,
    [property: JsonPropertyName("first_name_yomi")] string? FirstNameYomi = null,
    [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
    [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
);
