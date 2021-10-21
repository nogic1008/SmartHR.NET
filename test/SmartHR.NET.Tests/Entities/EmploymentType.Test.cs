using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="EmploymentType"/>の単体テスト</summary>
public class EmploymentTypeTest
{
    /// <summary>
    /// <inheritdoc cref="EmploymentType" path="/summary/text()"/>APIのサンプルレスポンスJSON
    /// </summary>
    internal const string Json = "{"
    + "\"id\": \"3a54c46e\","
    + "\"name\": \"正社員\","
    + "\"preset_type\": \"full_timer\","
    + "\"updated_at\": \"2021-09-24T17:22:12.288+09:00\","
    + "\"created_at\": \"2021-09-24T17:22:12.288+09:00\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(EmploymentType)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<EmploymentType>(Json, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 9, 24, 17, 22, 12, 288, new TimeSpan(9, 0, 0));
        entity.Should().Be(new EmploymentType(
            "3a54c46e",
            "正社員",
            EmploymentType.Preset.FullTimer,
            date,
            date));
    }
}
