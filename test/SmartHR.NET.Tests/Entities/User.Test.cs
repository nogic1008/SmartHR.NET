using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="User"/>の単体テスト</summary>
public class UserTest
{
    /// <summary>
    /// <inheritdoc cref="User" path="/summary/text()"/>APIのサンプルレスポンスJSON
    /// </summary>
    internal const string Json = "{"
    + "\"id\": \"70eb1bbb-3a04-437b-ab59-22bc6da2c67d\","
    + "\"email\": \"admin@example.com\","
    + "\"admin\": true,"
    + "\"role\": {"
    + "  \"id\": \"b2041eb8-f982-4b5c-a483-af68e578788a\","
    + "  \"name\": \"管理者\","
    + "  \"description\": null,"
    + "  \"crews_scope\": \"crews_scope_all\","
    + "  \"crews_scope_query\": null,"
    + "  \"session_timeout_in\": null,"
    + "  \"preset_type\": \"admin\""
    + "},"
    + "\"agreement_for_electronic_delivery\": false,"
    + "\"crew_id\": \"0723a1df-7b50-4c95-ae2f-183d60ec57c0\","
    + "\"invitation_created_at\": \"2021-09-24T17:00:00.000+09:00\","
    + "\"invitation_opened_at\": \"2021-09-24T17:00:00.000+09:00\","
    + "\"invitation_accepted_at\": \"2021-09-24T17:00:00.000+09:00\","
    + "\"invitation_answered_at\": \"2021-09-24T17:00:00.000+09:00\","
    + "\"suppressed_email_logs\": ["
    + "   {"
    + "     \"id\": \"string\","
    + "     \"suppression_type\": 0,"
    + "     \"reason\": \"string\","
    + "     \"suppressed_at\": \"2021-10-22\","
    + "     \"updated_at\": \"2021-09-24T17:00:00.000+09:00\","
    + "     \"created_at\": \"2021-09-24T17:00:00.000+09:00\""
    + "   }"
    + "],"
    + "\"has_password\": true,"
    + "\"updated_at\": \"2021-09-24T17:00:00.000+09:00\","
    + "\"created_at\": \"2021-09-24T17:00:00.000+09:00\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(User)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<User>(Json, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 9, 24, 17, 0, 0, 0, new TimeSpan(9, 0, 0));
        entity.Should().BeAssignableTo<User>();
        entity!.Id.Should().Be("70eb1bbb-3a04-437b-ab59-22bc6da2c67d");
        entity.Email.Should().Be("admin@example.com");
        entity.IsAdmin.Should().BeTrue();
        entity.Role.Should().Be(new User.UserRole(
            "b2041eb8-f982-4b5c-a483-af68e578788a",
            "管理者",
            null,
            User.CrewsScope.All,
            null,
            null,
            "admin"));
        entity.AgreementForElectronicDelivery.Should().BeFalse();
        entity.Crew.Should().BeNull();
        entity.CrewId.Should().Be("0723a1df-7b50-4c95-ae2f-183d60ec57c0");
        entity.InvitationCreatedAt.Should().Be(date);
        entity.InvitationOpenedAt.Should().Be(date);
        entity.InvitationAcceptedAt.Should().Be(date);
        entity.InvitationAnsweredAt.Should().Be(date);
        entity.SuppressedEmailLogs.Should().HaveCount(1);
        entity.SuppressedEmailLogs![0].Id.Should().Be("string");
        entity.SuppressedEmailLogs[0].SuppressionType.Should().Be(0);
        entity.SuppressedEmailLogs![0].Reason.Should().Be("string");
        entity.SuppressedEmailLogs[0].SuppressedAt.Should().Be(new DateTimeOffset(new(2021, 10, 22)));
        entity.SuppressedEmailLogs[0].CreatedAt.Should().Be(date);
        entity.SuppressedEmailLogs[0].UpdatedAt.Should().Be(date);
        entity.HasPassword.Should().BeTrue();
        entity.CreatedAt.Should().Be(date);
        entity.UpdatedAt.Should().Be(date);
    }
}
