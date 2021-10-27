using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="CrewCustomFieldTemplate"/>の単体テスト</summary>
public class CrewCustomFieldTemplateTest
{
    /// <summary>
    /// <inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/>APIのサンプルレスポンスJSON
    /// </summary>
    internal const string Json = "{"
    + "\"id\": \"bc1e5e8f\","
    + "\"name\": \"性別\","
    + "\"type\": \"enum\","
    + "\"elements\": ["
    + "  {"
    + "    \"id\": \"49bae7cb\","
    + "    \"name\": \"男\","
    + "    \"physical_name\": \"男性\","
    + "    \"position\": 1"
    + "  },"
    + "  {"
    + "    \"id\": \"a1ec77df\","
    + "    \"name\": \"女\","
    + "    \"physical_name\": \"女性\","
    + "    \"position\": 2"
    + "  }"
    + "],"
    + "\"group_id\": \"1367708c\","
    + "\"hint\": \"ヒント\","
    + "\"scale\": null,"
    + "\"separated_by_commas\": false,"
    + "\"position\": 1,"
    + "\"updated_at\": \"2021-09-24T17:22:14.403Z\","
    + "\"created_at\": \"2021-09-24T17:22:14.403Z\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(CrewCustomFieldTemplate)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<CrewCustomFieldTemplate>(Json, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 9, 24, 17, 22, 14, 403, TimeSpan.Zero);
        entity.Should().NotBeNull();
        entity!.Id.Should().Be("bc1e5e8f");
        entity.Name.Should().Be("性別");
        entity.Type.Should().Be(CrewCustomFieldTemplate.Input.Enum);
        entity.Elements.Should().HaveCount(2);
        entity.GroupId.Should().Be("1367708c");
        entity.Group.Should().BeNull();
        entity.Hint.Should().Be("ヒント");
        entity.Scale.Should().BeNull();
        entity.SeparatedByCommas.Should().BeFalse();
        entity.Position.Should().Be(1);
        entity.CreatedAt.Should().Be(date);
        entity.UpdatedAt.Should().Be(date);
    }
}
