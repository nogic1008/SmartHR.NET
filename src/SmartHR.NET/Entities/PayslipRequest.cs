using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>
/// Payslips APIへのリクエストオブジェクト
/// </summary>
public record PayslipRequest
{
    /// <summary>
    /// PayslipRequestの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="crewId">従業員ID</param>
    /// <param name="values">給与明細情報</param>
    /// <param name="memo">備考</param>
    public PayslipRequest(string crewId, IReadOnlyList<KeyValuePair<string, string>> values, string? memo = null)
        => (CrewId, Values, Memo) = (crewId, values, memo);

    /// <summary>従業員ID</summary>
    [JsonPropertyName("crew_id")]
    public string CrewId { get; init; }

    /// <summary>給与明細情報</summary>
    [JsonPropertyName("values")]
    public IReadOnlyList<KeyValuePair<string, string>> Values { get; init; }

    /// <summary>備考</summary>
    [JsonPropertyName("memo")]
    public string? Memo { get; init; }
}
