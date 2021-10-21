using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="Department"/>の単体テスト</summary>
public class DepartmentTest
{
    /// <summary><inheritdoc cref="Department" path="/summary/text()"/>APIのサンプルレスポンスJSON</summary>
    internal const string Json = "{"
    + "\"id\": \"c16d1f5f\","
    + "\"name\": \"test_child\","
    + "\"position\": 1,"
    + "\"code\": \"10000\","
    + "\"parent\": {"
    + "  \"id\": \"d189be1b\","
    + "  \"name\": \"test\","
    + "  \"position\": 0,"
    + "  \"code\": null,"
    + "  \"parent\": null"
    + "},"
    + "\"children\": ["
    + "  {"
    + "    \"id\": \"92516e77\","
    + "    \"name\": \"test_child_child\","
    + "    \"position\": 2,"
    + "    \"code\": null"
    + "  }"
    + "],"
    + "\"updated_at\": \"2021-10-21T09:57:34.395Z\","
    + "\"created_at\": \"2021-10-21T09:57:34.395Z\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(Department)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<Department>(Json, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 10, 21, 9, 57, 34, 395, TimeSpan.Zero);
        entity.Should().NotBeNull();
        entity!.Id.Should().Be("c16d1f5f");
        entity.Position.Should().Be(1);
        entity.Code.Should().Be("10000");
        entity.Parent.Should().Be(new Department("d189be1b", "test", 0));
        entity.Children.Should().Equal(new Department("92516e77", "test_child_child", 2));
        entity.CreatedAt.Should().Be(date);
        entity.UpdatedAt.Should().Be(date);
    }
}
