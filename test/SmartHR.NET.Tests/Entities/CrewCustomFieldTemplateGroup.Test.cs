using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="CrewCustomFieldTemplateGroup"/>の単体テスト</summary>
public class CrewCustomFieldTemplateGroupTest
{
    /// <summary>
    /// <inheritdoc cref="CrewCustomFieldTemplateGroup" path="/summary/text()"/>APIのサンプルレスポンスJSON
    /// </summary>
    internal const string Json = "{"
    + "\"id\": \"group_1\","
    + "\"name\": \"group_1_name\","
    + "\"position\": 1,"
    + "\"access_type\": \"read_and_update\","
    + "\"templates\": ["
    + "  {"
    + "    \"id\": \"string\","
    + "    \"name\": \"string\","
    + "    \"type\": \"date\","
    + "    \"elements\": ["
    + "        {"
    + "        \"id\": \"string\","
    + "        \"name\": \"string\","
    + "        \"physical_name\": \"string\","
    + "        \"position\": 0"
    + "        }"
    + "      ],"
    + "    \"group_id\": \"group_1\","
    + "    \"hint\": \"string\","
    + "    \"scale\": 0,"
    + "    \"separated_by_commas\": true,"
    + "    \"position\": 0,"
    + "    \"updated_at\": \"2021-09-24T17:22:14.403Z\","
    + "    \"created_at\": \"2021-09-24T17:22:14.403Z\""
    + "  }"
    + "],"
    + "\"updated_at\": \"2021-09-24T17:22:14.403Z\","
    + "\"created_at\": \"2021-09-24T17:22:14.403Z\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(CrewCustomFieldTemplateGroup)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<CrewCustomFieldTemplateGroup>(Json, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 9, 24, 17, 22, 14, 403, TimeSpan.Zero);
        entity.Should().NotBeNull();
        entity!.Id.Should().Be("group_1");
        entity.Name.Should().Be("group_1_name");
        entity.Position.Should().Be(1);
        entity.AccessType.Should().Be(CrewCustomFieldTemplateGroup.Accessibility.ReadAndUpdate);
        entity.Templates.Should().HaveCount(1);
        entity.CreatedAt.Should().Be(date);
        entity.UpdatedAt.Should().Be(date);
    }
}
