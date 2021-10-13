using System;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities.Webhook;

public record CrewWebhook(
    [property: JsonPropertyName("event")] WebhookEvent Event,
    [property: JsonPropertyName("triggered_at")] DateTimeOffset TriggeredAt,
    [property: JsonPropertyName("sender")] Crew? Sender,
    [property: JsonPropertyName("crew")] Crew Body
) : IWebhookObject;

public record Crew(
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTimeOffset UpdatedAt
);
