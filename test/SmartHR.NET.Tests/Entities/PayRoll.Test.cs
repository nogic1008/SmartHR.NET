using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

public class PayRollTest
{
    [Fact]
    public void CanDeserializeJSON()
    {
        const string jsonString = "{"
        + "\"id\": \"string\","
        + "\"payment_type\": \"salary\","
        + "\"paid_at\": \"2021-09-26\","
        + "\"period_start_at\": \"2021-09-26\","
        + "\"period_end_at\": \"2021-09-26\","
        + "\"source_type\": \"api\","
        + "\"status\": \"wip\","
        + "\"published_at\": null,"
        + "\"notify_with_publish\": true,"
        + "\"numeral_system_handle_type\": \"as_is\","
        + "\"name_for_admin\": \"string\","
        + "\"name_for_crew\": \"string\""
        + "}";

        var entity = JsonSerializer.Deserialize<PayRoll>(jsonString, JsonConfig.Default);

        entity.Should().NotBeNull();
        entity.Should().Be(new PayRoll(
            "string",
            PaymentType.Salary,
            new(2021, 9, 26),
            new(2021, 9, 26),
            new(2021, 9, 26),
            "api",
            PaymentStatus.Wip,
            default,
            true,
            NumeralSystemType.AsIs,
            "string",
            "string"
        ));
    }
}
