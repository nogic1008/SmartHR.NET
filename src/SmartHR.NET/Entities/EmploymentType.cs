using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>雇用形態情報</summary>
/// <param name="Id">雇用形態ID</param>
/// <param name="Name">名称</param>
/// <param name="PresetType">プリセット種別</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record EmploymentType(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("preset_type")] EmploymentType.Preset? PresetType = default,
    [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
    [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
)
{
    /// <summary>
    /// <inheritdoc cref="EmploymentType" path="/param[@name='PresetType']/text()"/>
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Preset>))]
    public enum Preset
    {
        /// <summary>役員</summary>
        [EnumMember(Value = "board_member")] BoardMember,
        /// <summary>正社員</summary>
        [EnumMember(Value = "full_timer")] FullTimer,
        /// <summary>契約社員</summary>
        [EnumMember(Value = "contract_worker")] ContractWorker,
        /// <summary>派遣社員</summary>
        [EnumMember(Value = "permatemp")] Permatemp,
        /// <summary>アルバイト・パート</summary>
        [EnumMember(Value = "part_timer")] PartTimer,
        /// <summary>業務委託</summary>
        [EnumMember(Value = "outsourcing_contractor")] OutsourcingContractor,
        /// <summary>その他</summary>
        [EnumMember(Value = "etc")] Etc
    }
}
