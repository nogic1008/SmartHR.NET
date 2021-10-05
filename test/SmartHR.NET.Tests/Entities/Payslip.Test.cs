using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary>
/// <see cref="Payslip"/>の単体テスト
/// </summary>
public class PayslipTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    [Fact(DisplayName = $"{nameof(Payslip)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange
        const string jsonString = "{"
        + "  \"id\": \"id\","
        + "  \"crew_id\": \"crew_id\","
        + "  \"payroll_id\": \"payroll_id\","
        + "  \"memo\": \"memo\","
        + "  \"mismatch\": true,"
        + "  \"last_notified_at\": \"2021-10-01T00:00:00Z\","
        + "  \"allowances\": ["
        + "    {"
        + "      \"name\": \"allowances\","
        + "      \"amount\": 200000"
        + "    }"
        + "  ],"
        + "  \"deductions\": ["
        + "    {"
        + "      \"name\": \"deductions\","
        + "      \"amount\": 50000"
        + "    }"
        + "  ],"
        + "  \"attendances\": ["
        + "    {"
        + "      \"name\": \"attendances\","
        + "      \"amount\": 160,"
        + "      \"unit_type\": \"hours\","
        + "      \"numeral_system_type\": \"decimal\","
        + "      \"delimiter_type\": \"period\""
        + "    }"
        + "  ],"
        + "  \"payroll_aggregates\": ["
        + "    {"
        + "      \"name\": \"payroll_aggregates\","
        + "      \"aggregate_type\": \"net_payment\","
        + "      \"amount\": 100,"
        + "      \"value\": \"string\""
        + "    }"
        + "  ]"
        + "}";

        // Act
        var entity = JsonSerializer.Deserialize<Payslip>(jsonString, JsonConfig.Default);

        // Assert
        entity.Should().NotBeNull();
        entity!.Id.Should().Be("id");
        entity.CrewId.Should().Be("crew_id");
        entity.PayrollId.Should().Be("payroll_id");
        entity.Memo.Should().Be("memo");
        entity.Mismatch.Should().Be(true);
        entity.LastNotifiedAt.Should().Be(new(2021, 10, 1, 0, 0, 0, TimeSpan.Zero));
        entity.Allowances.Should().Equal(new PayslipDetail("allowances", 200000));
        entity.Deductions.Should().Equal(new PayslipDetail("deductions", 50000));
        entity.Attendances.Should().Equal(new Attendance("attendances", 160, Attendance.Unit.Hours, Attendance.NumeralSystem.Decimal, Attendance.Delimiter.Period));
        entity.PayrollAggregates.Should().Equal(new PayrollAggregate("payroll_aggregates", PayrollAggregate.Aggregate.NetPayment, 100, "string"));
    }
}
