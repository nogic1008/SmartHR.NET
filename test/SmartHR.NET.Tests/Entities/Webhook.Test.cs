using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="Webhook"/>の単体テスト</summary>
public class WebhookTest
{
    /// <summary><inheritdoc cref="Webhook" path="/summary/text()"/>APIのサンプルレスポンスJSON</summary>
    internal const string Json = "{"
    + "\"id\": \"e8d6e845\","
    + "\"url\": \"https://example.com\","
    + "\"description\": \"従業員更新\","
    + "\"secret_token\": \"sample_token\","
    + "\"crew_created\": true,"
    + "\"crew_updated\": true,"
    + "\"crew_deleted\": true,"
    + "\"crew_imported\": true,"
    + "\"dependent_created\": false,"
    + "\"dependent_updated\": false,"
    + "\"dependent_deleted\": false,"
    + "\"dependent_imported\": false,"
    + "\"disabled_at\": null,"
    + "\"updated_at\": \"2021-10-26T11:54:47.644Z\","
    + "\"created_at\": \"2021-10-26T11:54:47.644Z\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(Webhook)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<Webhook>(Json, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 10, 26, 11, 54, 47, 644, TimeSpan.Zero);
        entity.Should().Be(new Webhook(
            Id: "e8d6e845",
            Url: "https://example.com",
            Description: "従業員更新",
            SecretToken: "sample_token",
            CrewCreated: true,
            CrewUpdated: true,
            CrewDeleted: true,
            CrewImported: true,
            DependentCreated: false,
            DependentUpdated: false,
            DependentDeleted: false,
            DependentImported: false,
            DisabledAt: null,
            CreatedAt: date,
            UpdatedAt: date
        ));
    }
}
