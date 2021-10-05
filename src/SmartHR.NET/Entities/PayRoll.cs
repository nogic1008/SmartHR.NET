using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>給与情報</summary>
public record PayRoll
{
    /// <summary>
    /// PayRollの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="id"><see cref="Id"/></param>
    /// <param name="paymentType"><see cref="PaymentType"/></param>
    /// <param name="paidAt"><see cref="PaidAt"/></param>
    /// <param name="periodStartAt"><see cref="PeriodStartAt"/></param>
    /// <param name="periodEndAt"><see cref="PeriodEndAt"/></param>
    /// <param name="sourceType"><see cref="SourceType"/></param>
    /// <param name="status"><see cref="Status"/></param>
    /// <param name="publishedAt"><see cref="PublishedAt"/></param>
    /// <param name="notifyWithPublish"><see cref="NotifyWithPublish"/></param>
    /// <param name="numeralSystemHandleType"><see cref="NumeralSystemHandleType"/></param>
    /// <param name="nameForAdmin"><see cref="NameForAdmin"/></param>
    /// <param name="nameForCrew"><see cref="NameForCrew"/></param>
    public PayRoll(string id, PaymentType paymentType, DateTime paidAt, DateTime periodStartAt, DateTime periodEndAt, string sourceType, PaymentStatus status, DateTimeOffset? publishedAt, bool notifyWithPublish, NumeralSystemType numeralSystemHandleType, string nameForAdmin, string nameForCrew)
        => (Id, PaymentType, PaidAt, PeriodStartAt, PeriodEndAt, SourceType, Status, PublishedAt, NotifyWithPublish, NumeralSystemHandleType, NameForAdmin, NameForCrew)
        = (id, paymentType, paidAt, periodStartAt, periodEndAt, sourceType, status, publishedAt, notifyWithPublish, numeralSystemHandleType, nameForAdmin, nameForCrew);

    /// <summary>給与ID</summary>
    [JsonPropertyName("id")]
    public string Id { get; init; }

    /// <summary>支給タイプ</summary>
    [JsonPropertyName("payment_type")]
    public PaymentType PaymentType { get; init; }

    /// <summary>支給日</summary>
    [JsonPropertyName("paid_at")]
    public DateTime PaidAt { get; init; }

    /// <summary>対象期間 (From)</summary>
    [JsonPropertyName("period_start_at")]
    public DateTime PeriodStartAt { get; init; }

    /// <summary>対象期間 (To)</summary>
    [JsonPropertyName("period_end_at")]
    public DateTime PeriodEndAt { get; init; }

    /// <summary>データの取り込み方法</summary>
    [JsonPropertyName("source_type")]
    public string SourceType { get; init; }

    /// <summary>ステータス</summary>
    [JsonPropertyName("status")]
    public PaymentStatus Status { get; init; }

    /// <summary>公開時刻</summary>
    [JsonPropertyName("published_at")]
    public DateTimeOffset? PublishedAt { get; init; }

    /// <summary>公開と同時に通知を行う</summary>
    [JsonPropertyName("notify_with_publish")]
    public bool NotifyWithPublish { get; init; }

    /// <summary>記数法</summary>
    [JsonPropertyName("numeral_system_handle_type")]
    public NumeralSystemType NumeralSystemHandleType { get; init; }

    /// <summary>給与明細の名前 (管理者向け)</summary>
    [JsonPropertyName("name_for_admin")]
    public string NameForAdmin { get; init; }

    /// <summary>給与明細の名前 (従業員向け)</summary>
    [JsonPropertyName("name_for_crew")]
    public string NameForCrew { get; init; }
}

#region Enum
/// <summary>支給タイプ</summary>
[JsonConverter(typeof(JsonStringEnumConverterEx<PaymentType>))]
public enum PaymentType
{
    [EnumMember(Value = "salary")] Salary,
    [EnumMember(Value = "bonus")] Bonus,
}

/// <summary>給与ステータス</summary>
[JsonConverter(typeof(JsonStringEnumConverterEx<PaymentStatus>))]
public enum PaymentStatus
{
    [EnumMember(Value = "wip")] Wip,
    [EnumMember(Value = "fixed")] Fixed,
    [EnumMember(Value = "failed")] Failed,
    [EnumMember(Value = "importing")] Importing,
}

/// <summary>記数法</summary>
[JsonConverter(typeof(JsonStringEnumConverterEx<NumeralSystemType>))]
public enum NumeralSystemType
{
    [EnumMember(Value = "as_is")] AsIs,
    [EnumMember(Value = "force_sexagesimal")] ForceSexagesimal,
    [EnumMember(Value = "force_decimal")] ForceDecimal,
}
#endregion
