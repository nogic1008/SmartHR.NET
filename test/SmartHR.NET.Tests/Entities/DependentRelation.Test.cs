using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary>
/// <see cref="DependentRelation"/>の単体テスト
/// </summary>
public class DependentRelationTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    [Fact(DisplayName = $"{nameof(DependentRelation)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange
        const string jsonString = "{"
        + "\"id\":\"9ba6128d-9a8a-4b6a-9464-03907dc37e71\","
        + "\"name\":\"妻\","
        + "\"preset_type\":\"wife\","
        + "\"is_child\":false,"
        + "\"position\":1,"
        + "\"created_at\":\"2021-09-24T17:22:12.426Z\","
        + "\"updated_at\":\"2021-09-24T17:22:12.426Z\""
        + "}";

        // Act
        var entity = JsonSerializer.Deserialize<DependentRelation>(jsonString, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 9, 24, 17, 22, 12, 426, TimeSpan.Zero);
        entity.Should().Be(new DependentRelation(
            "9ba6128d-9a8a-4b6a-9464-03907dc37e71",
            "妻",
            "wife",
            false,
            1,
            date,
            date
        ));
    }
}
