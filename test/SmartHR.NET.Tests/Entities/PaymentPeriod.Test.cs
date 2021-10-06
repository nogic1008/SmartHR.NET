using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary>
/// <see cref="PaymentPeriod"/>の単体テスト
/// </summary>
public class PaymentPeriodTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    [Fact(DisplayName = $"{nameof(PaymentPeriod)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange
        const string jsonString = "{"
        + "\"id\":\"id\","
        + "\"name\":\"name\","
        + "\"period_type\":\"monthly\","
        + "\"updated_at\":\"2021-10-06T00:24:48.897Z\","
        + "\"created_at\":\"2021-10-06T00:24:48.897Z\""
        + "}";

        // Act
        var entity = JsonSerializer.Deserialize<PaymentPeriod>(jsonString, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 10, 6, 0, 24, 48, 897, TimeSpan.Zero);
        entity.Should().Be(new PaymentPeriod("id", "name", PaymentPeriod.Period.Monthly, date, date));
    }
}
