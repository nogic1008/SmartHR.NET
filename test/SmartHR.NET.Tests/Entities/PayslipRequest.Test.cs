using System.Collections.Generic;
using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary>
/// <see cref="PayslipRequest"/>の単体テスト
/// </summary>
public class PayslipRequestTest
{
    /// <summary>
    /// JSONにシリアライズできる。
    /// </summary>
    [Fact(DisplayName = $"{nameof(PayslipRequest)} > JSONにシリアライズできる。")]
    public void CanSerializeJSON()
    {
        // Arrange
        var entity = new PayslipRequest("従業員ID", new Dictionary<string, string>()
        {
            { "支給項目1", "10000" },
            { "支給項目2", "100000" },
            { "支給項目3", "200000" },
            { "控除項目1", "3000" },
            { "控除項目2", "30000" },
            { "控除項目3", "4000" },
            { "勤怠項目1", "160" },
            { "勤怠項目2", "20" },
            { "勤怠項目3", "10" },
            { "合計項目1", "9000000" },
            { "合計項目2", "5000000" },
            { "合計項目3", "4000000" }
        }.ToArray(), "string");

        // Act
        string jsonString = JsonSerializer.Serialize(entity, JsonConfig.Default);

        // Assert
        jsonString.Should().Be("{"
        + "\"crew_id\":\"従業員ID\","
        + "\"values\":["
        + "{\"key\":\"支給項目1\",\"value\":\"10000\"},"
        + "{\"key\":\"支給項目2\",\"value\":\"100000\"},"
        + "{\"key\":\"支給項目3\",\"value\":\"200000\"},"
        + "{\"key\":\"控除項目1\",\"value\":\"3000\"},"
        + "{\"key\":\"控除項目2\",\"value\":\"30000\"},"
        + "{\"key\":\"控除項目3\",\"value\":\"4000\"},"
        + "{\"key\":\"勤怠項目1\",\"value\":\"160\"},"
        + "{\"key\":\"勤怠項目2\",\"value\":\"20\"},"
        + "{\"key\":\"勤怠項目3\",\"value\":\"10\"},"
        + "{\"key\":\"合計項目1\",\"value\":\"9000000\"},"
        + "{\"key\":\"合計項目2\",\"value\":\"5000000\"},"
        + "{\"key\":\"合計項目3\",\"value\":\"4000000\"}"
        + "],"
        + "\"memo\":\"string\"}");
    }
}
