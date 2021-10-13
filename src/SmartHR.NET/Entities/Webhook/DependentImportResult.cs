using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities.Webhook;

public record DependentImportedWebhook(
    [property: JsonPropertyName("event")] WebhookEvent Event,
    [property: JsonPropertyName("triggered_at")] DateTimeOffset TriggeredAt,
    [property: JsonPropertyName("sender")] Crew? Sender,
    [property: JsonPropertyName("dependent_import_result")] DependentImportResult Body
) : IWebhookObject;

/// <summary>
/// ファイルの取り込みによって一括で登録・更新された家族情報
/// </summary>
/// <param name="Created">登録された家族</param>
/// <param name="Updated">更新された家族</param>
public record DependentImportResult(
    [property: JsonPropertyName("created_dependents")] IReadOnlyList<Dependent>? Created,
    [property: JsonPropertyName("updated_dependents")] IReadOnlyList<Dependent>? Updated
);
