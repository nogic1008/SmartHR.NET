using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>給与支給形態</summary>
public record PaymentPeriod
{
    /// <summary>
    /// PaymentPeriodの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="id"><see cref="Id"/></param>
    /// <param name="name"><see cref="Name"/></param>
    /// <param name="periodType"><see cref="PeriodType"/></param>
    /// <param name="createdAt"><see cref="CreatedAt"/></param>
    /// <param name="updatedAt"><see cref="UpdatedAt"/></param>
    public PaymentPeriod(string id, string name, Period periodType, DateTimeOffset createdAt, DateTimeOffset updatedAt)
        => (Id, Name, PeriodType, CreatedAt, UpdatedAt) = (id, name, periodType, createdAt, updatedAt);

    /// <summary>給与支給形態ID</summary>
    [JsonPropertyName("id")]
    public string Id { get; init; }

    /// <summary>名称</summary>
    [JsonPropertyName("name")]
    public string Name { get; init; }

    /// <summary>支払い期間種別</summary>
    [JsonPropertyName("period_type")]
    public Period PeriodType { get; init; }

    /// <summary>作成日</summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>最終更新日</summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; init; }

    /// <summary>支払い期間種別</summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Period>))]
    public enum Period
    {
        [EnumMember(Value = "monthly")] Monthly,
        [EnumMember(Value = "weekly")] Weekly,
        [EnumMember(Value = "daily")] Daily,
        [EnumMember(Value = "hourly")] Hourly,
        [EnumMember(Value = "etc")] Etc,
    }
}
