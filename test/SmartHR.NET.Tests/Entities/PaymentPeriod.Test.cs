using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="PaymentPeriod"/>の単体テスト</summary>
public class PaymentPeriodTest
{
    /// <summary>給与支給形態APIのサンプルレスポンスJSON</summary>
    internal const string Json = "{"
    + "\"id\":\"id\","
    + "\"name\":\"name\","
    + "\"period_type\":\"monthly\","
    + "\"updated_at\":\"2021-10-06T00:24:48.897Z\","
    + "\"created_at\":\"2021-10-06T00:24:48.897Z\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(PaymentPeriod)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<PaymentPeriod>(Json, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 10, 6, 0, 24, 48, 897, TimeSpan.Zero);
        entity.Should().Be(new PaymentPeriod("id", "name", PaymentPeriod.Period.Monthly, date, date));
    }
}
