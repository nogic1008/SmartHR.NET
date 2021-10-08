using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary>
/// <see cref="TaxWithholding"/>の単体テスト
/// </summary>
public class TaxWithholdingTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    [Fact(DisplayName = $"{nameof(TaxWithholding)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange
        const string jsonString = "{"
        + "\"id\":\"id\","
        + "\"name\":\"name\","
        + "\"status\":\"wip\","
        + "\"year\":\"H23\""
        + "}";

        // Act
        var entity = JsonSerializer.Deserialize<TaxWithholding>(jsonString, JsonConfig.Default);

        // Assert
        entity.Should().Be(new TaxWithholding("id", "name", TaxWithholding.FormStatus.Wip, "H23"));
    }
}
