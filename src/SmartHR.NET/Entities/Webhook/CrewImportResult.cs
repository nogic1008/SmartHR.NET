using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities.Webhook;

public record CrewImportedWebhook(
    [property: JsonPropertyName("event")] WebhookEvent Event,
    [property: JsonPropertyName("triggered_at")] DateTimeOffset TriggeredAt,
    [property: JsonPropertyName("sender")] Crew? Sender,
    [property: JsonPropertyName("crew_import_result")] CrewImportResult Body
) : IWebhookObject;

/// <summary>
/// ファイルの取り込みによって一括で登録・更新された従業員情報
/// </summary>
/// <param name="Created">従業員された家族</param>
/// <param name="Updated">従業員された家族</param>
public record CrewImportResult(
    [property: JsonPropertyName("created_crews")] IReadOnlyList<Crew>? Created,
    [property: JsonPropertyName("updated_crews")] IReadOnlyList<Crew>? Updated
);
