using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>源泉徴収情報</summary>
public record TaxWithholding
{
    /// <summary>
    /// TaxWithholdingの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="id"><see cref="Id"/></param>
    /// <param name="name"><see cref="Name"/></param>
    /// <param name="status"><see cref="Status"/></param>
    /// <param name="year"><see cref="Year"/></param>
    public TaxWithholding(string id, string name, FormStatus status, string year)
        => (Id, Name, Status, Year) = (id, name, status, year);

    /// <summary>源泉徴収票ID</summary>
    [JsonPropertyName("id")]
    public string Id { get; init; }

    /// <summary>名称</summary>
    [JsonPropertyName("name")]
    public string Name { get; init; }

    /// <summary>ステータス</summary>
    [JsonPropertyName("status")]
    public FormStatus Status { get; init; }

    /// <summary>源泉徴収票に印字される年</summary>
    [JsonPropertyName("year")]
    public string Year { get; init; }

    /// <summary>ステータス</summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<FormStatus>))]
    public enum FormStatus
    {
        [EnumMember(Value = "wip")] Wip,
        [EnumMember(Value = "fixed")] Fixed,
    }
}
