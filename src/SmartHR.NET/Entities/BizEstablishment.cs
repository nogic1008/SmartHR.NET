using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>事業所情報</summary>
/// <param name="Id">事業所ID</param>
/// <param name="HelInsType">健康保険組合の種類</param>
/// <param name="HelInsName">健康保険組合の名称</param>
/// <param name="Name">管理名</param>
/// <param name="SocInsName">社会保険登録名</param>
/// <param name="SocInsOwnerId">社会保険代表者ID</param>
/// <param name="SocInsOwner">社会保険代表者</param>
/// <param name="SocInsAddress">社会保険登録住所</param>
/// <param name="SocInsTelNumber">電話番号</param>
/// <param name="LabInsName">労働保険登録名</param>
/// <param name="LabInsOwnerId">労働保険代表者ID</param>
/// <param name="LabInsOwner">労働保険代表者</param>
/// <param name="LabInsAddress">労働保険登録住所</param>
/// <param name="LabInsTelNumber">電話番号</param>
/// <param name="JurisdictionTax">管轄の税務署</param>
/// <param name="SalaryPayerAddress">給与支払者住所</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record BizEstablishment(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("hel_ins_type")] BizEstablishment.HealthInsurance HelInsType,
    [property: JsonPropertyName("hel_ins_name")] string? HelInsName,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("soc_ins_name")] string SocInsName,
    [property: JsonPropertyName("soc_ins_owner_id")] string? SocInsOwnerId,
    // TODO: Crewオブジェクトの定義
    [property: JsonPropertyName("soc_ins_owner")] JsonElement? SocInsOwner,
    [property: JsonPropertyName("soc_ins_address")] Address? SocInsAddress,
    [property: JsonPropertyName("soc_ins_tel_number")] string? SocInsTelNumber,
    [property: JsonPropertyName("lab_ins_name")] string LabInsName,
    [property: JsonPropertyName("lab_ins_owner_id")] string? LabInsOwnerId = null,
    // TODO: Crewオブジェクトの定義
    [property: JsonPropertyName("lab_ins_owner")] JsonElement? LabInsOwner = default,
    [property: JsonPropertyName("lab_ins_address")] Address? LabInsAddress = null,
    [property: JsonPropertyName("lab_ins_tel_number")] string? LabInsTelNumber = null,
    [property: JsonPropertyName("jurisdiction_tax")] string? JurisdictionTax = null,
    [property: JsonPropertyName("salary_payer_address")] Address? SalaryPayerAddress = null,
    [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
    [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
)
{
    /// <summary>
    /// <inheritdoc cref="BizEstablishment" path="/param[@name='HelInsType']/text()"/>
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<HealthInsurance>))]
    public enum HealthInsurance
    {
        [EnumMember(Value = "kyokai_kenpo")] KyokaiKenpo,
        [EnumMember(Value = "its_kenpo")] ItsKenpo,
        [EnumMember(Value = "tj_kenpo")] TjKenpo,
        [EnumMember(Value = "general_kenpo")] GeneralKenpo,
    }
}
