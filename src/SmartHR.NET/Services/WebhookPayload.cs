using System;
using System.Text.Json.Serialization;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Services;

/// <summary><inheritdoc cref="Webhook" path="/summary/text()"/>APIのリクエストボディ</summary>
/// <inheritdoc cref="Webhook" path="/param"/>
public record WebhookPayload(
    [property: JsonPropertyName("url")] string? Url = null,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("secret_token")] string? SecretToken = null,
    [property: JsonPropertyName("crew_created")] bool? CrewCreated = default,
    [property: JsonPropertyName("crew_updated")] bool? CrewUpdated = default,
    [property: JsonPropertyName("crew_deleted")] bool? CrewDeleted = default,
    [property: JsonPropertyName("crew_imported")] bool? CrewImported = default,
    [property: JsonPropertyName("dependent_created")] bool? DependentCreated = default,
    [property: JsonPropertyName("dependent_updated")] bool? DependentUpdated = default,
    [property: JsonPropertyName("dependent_deleted")] bool? DependentDeleted = default,
    [property: JsonPropertyName("dependent_imported")] bool? DependentImported = default,
    [property: JsonPropertyName("disabled_at")] DateTimeOffset? DisabledAt = default
);
