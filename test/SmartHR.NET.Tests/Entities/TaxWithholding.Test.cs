using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="TaxWithholding"/>の単体テスト</summary>
public class TaxWithholdingTest
{
    /// <summary>源泉徴収APIのサンプルレスポンスJSON</summary>
    internal const string Json = "{"
    + "\"id\":\"id\","
    + "\"name\":\"name\","
    + "\"status\":\"wip\","
    + "\"year\":\"H23\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(TaxWithholding)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<TaxWithholding>(Json, JsonConfig.Default);

        // Assert
        entity.Should().Be(new TaxWithholding("id", "name", TaxWithholding.FormStatus.Wip, "H23"));
    }
}
