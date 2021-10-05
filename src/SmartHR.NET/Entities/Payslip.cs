using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>給与明細</summary>
public record Payslip
{
    /// <summary>
    /// Payslipの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="id">給与明細ID</param>
    /// <param name="crewId">従業員ID</param>
    /// <param name="payrollId">給与ID</param>
    /// <param name="memo">備考</param>
    /// <param name="mismatch">項目合計金額の不整合フラグ</param>
    /// <param name="lastNotifiedAt">通知日時</param>
    /// <param name="allowances">支給</param>
    /// <param name="deductions">控除</param>
    /// <param name="attendances">勤怠</param>
    /// <param name="payrollAggregates">合計</param>
    public Payslip(
        string id,
        string crewId,
        string payrollId,
        string? memo = null,
        bool? mismatch = default,
        DateTimeOffset? lastNotifiedAt = default,
        IReadOnlyList<PayslipDetail>? allowances = null,
        IReadOnlyList<PayslipDetail>? deductions = null,
        IReadOnlyList<Attendance>? attendances = null,
        IReadOnlyList<PayrollAggregate>? payrollAggregates = null)
        => (Id, CrewId, PayrollId, Memo, Mismatch, LastNotifiedAt, Allowances, Deductions, Attendances, PayrollAggregates)
            = (id, crewId, payrollId, memo, mismatch, lastNotifiedAt, allowances, deductions, attendances, payrollAggregates);

    /// <summary>給与明細ID</summary>
    [JsonPropertyName("id")]
    public string Id { get; init; }

    /// <summary>従業員ID</summary>
    [JsonPropertyName("crew_id")]
    public string CrewId { get; init; }

    /// <summary>給与ID</summary>
    [JsonPropertyName("payroll_id")]
    public string PayrollId { get; init; }

    /// <summary>備考</summary>
    [JsonPropertyName("memo")]
    public string? Memo { get; init; }

    /// <summary>項目合計金額の不整合フラグ</summary>
    [JsonPropertyName("mismatch")]
    public bool? Mismatch { get; init; }

    /// <summary>通知日時</summary>
    [JsonPropertyName("last_notified_at")]
    public DateTimeOffset? LastNotifiedAt { get; init; }

    /// <summary>支給</summary>
    [JsonPropertyName("allowances")]
    public IReadOnlyList<PayslipDetail>? Allowances { get; init; }

    /// <summary>控除</summary>
    [JsonPropertyName("deductions")]
    public IReadOnlyList<PayslipDetail>? Deductions { get; init; }

    /// <summary>勤怠</summary>
    [JsonPropertyName("attendances")]
    public IReadOnlyList<Attendance>? Attendances { get; init; }

    /// <summary>合計</summary>
    [JsonPropertyName("payroll_aggregates")]
    public IReadOnlyList<PayrollAggregate>? PayrollAggregates { get; init; }
}

/// <summary>支給/控除細目</summary>
/// <param name="Name">名前</param>
/// <param name="Amount">支給/控除金額</param>
public record PayslipDetail(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("amount")] int Amount
);

/// <summary>勤怠</summary>
public record Attendance
{
    /// <summary>
    /// Attendanceの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="name"><see cref="Name"/></param>
    /// <param name="amount"><see cref="Amount"/></param>
    /// <param name="unitType"><see cref="UnitType"/></param>
    /// <param name="numeralSystemType"><see cref="NumeralSystemType"/></param>
    /// <param name="delimiterType"><see cref="DelimiterType"/></param>
    public Attendance(string name, int amount, Unit unitType, NumeralSystem? numeralSystemType = default, Delimiter? delimiterType = default)
        => (Name, Amount, UnitType, NumeralSystemType, DelimiterType) = (name, amount, unitType, numeralSystemType, delimiterType);

    /// <summary>名前</summary>
    [JsonPropertyName("name")]
    public string Name { get; init; }

    /// <summary>勤怠の日数・時間</summary>
    [JsonPropertyName("amount")]
    public int Amount { get; init; }

    /// <summary>単位</summary>
    [JsonPropertyName("unit_type")]
    public Unit UnitType { get; init; }

    /// <summary>記数法種別</summary>
    [JsonPropertyName("numeral_system_type")]
    public NumeralSystem? NumeralSystemType { get; init; }

    /// <summary>区切り種別</summary>
    [JsonPropertyName("delimiter_type")]
    public Delimiter? DelimiterType { get; init; }

    #region Enum
    /// <summary>単位</summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Unit>))]
    public enum Unit
    {
        [EnumMember(Value = "hours")] Hours,
        [EnumMember(Value = "days")] Days,
    }

    /// <summary>記数法</summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<NumeralSystem>))]
    public enum NumeralSystem
    {
        [EnumMember(Value = "decimal")] Decimal,
        [EnumMember(Value = "sexagesimal")] Sexagesimal,
    }

    /// <summary>区切り種別</summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Delimiter>))]
    public enum Delimiter
    {
        [EnumMember(Value = "period")] Period,
        [EnumMember(Value = "colon")] Colon
    }
    #endregion
}

/// <summary>合計</summary>
public record PayrollAggregate
{
    /// <summary>
    /// PayrollAggregateの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="name"><see cref="Name"/></param>
    /// <param name="aggregateType"><see cref="AggregateType"/></param>
    /// <param name="amount"><see cref="Amount"/></param>
    /// <param name="value"><see cref="Value"/></param>
    public PayrollAggregate(string name, Aggregate aggregateType, int? amount = default, string? value = null)
        => (Name, AggregateType, Amount, Value) = (name, aggregateType, amount, value);

    /// <summary>名前</summary>
    [JsonPropertyName("name")]
    public string Name { get; init; }

    /// <summary>集計種別</summary>
    [JsonPropertyName("aggregate_type")]
    public Aggregate AggregateType { get; init; }

    /// <summary>その他の金額</summary>
    [JsonPropertyName("amount")]
    public int? Amount { get; init; }

    /// <summary>その他の値</summary>
    [JsonPropertyName("value")]
    public string? Value { get; init; }

    #region Enum
    /// <summary>集計種別</summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Aggregate>))]
    public enum Aggregate
    {
        [EnumMember(Value = "net_payment")] NetPayment,
        [EnumMember(Value = "total_payment")] TotalPayment,
        [EnumMember(Value = "deduction")] Deduction,
        [EnumMember(Value = "others")] Others,
    }
    #endregion
}
