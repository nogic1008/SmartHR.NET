using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>従業員情報収集フォーム情報</summary>
/// <param name="Id">収集フォームID</param>
/// <param name="PresetType">プリセット種別</param>
/// <param name="FormType">フォーム種別</param>
/// <param name="Name">名称</param>
/// <param name="WithMyNumber">マイナンバーの提出を依頼するか</param>
/// <param name="CrewMyNumberRequired">従業員本人のマイナンバーを提出を依頼するか</param>
/// <param name="DependentsMyNumberRequired">被扶養者（配偶者含む）のマイナンバーの提出を依頼するか</param>
/// <param name="CrewMyNumberCardRequired">従業員本人の番号確認書類の提出を依頼するか</param>
/// <param name="CrewIdentificationRequired">従業員本人の身元確認書類の提出を依頼するか</param>
/// <param name="CrewIsForHelIns">従業員本人のマイナンバーの利用目的が「健康保険・厚生年金保険関係届出事務」に該当するか</param>
/// <param name="CrewIsForEmpIns">従業員本人のマイナンバーの利用目的が「雇用保険関係届出事務」が該当するか</param>
/// <param name="CrewIsForAccIns">従業員本人のマイナンバーの利用目的が「労働者災害補償保険法関係届出事務」に該当するか</param>
/// <param name="CrewIsForTaxDeduction">従業員本人のマイナンバーの利用目的が「利用目的：源泉徴収に係る事務」に該当するか</param>
/// <param name="CrewIsForShareholding">従業員本人のマイナンバーの利用目的が「持株会にかかる金融商品取引に関する法定書類の作成・提供事務」に該当するか</param>
/// <param name="DependentsMyNumberCardRequired">被扶養者（配偶者含む）の番号確認書類の提出を依頼するか</param>
/// <param name="DependentsIdentificationRequired">被扶養者（配偶者含む）の身元確認書類の提出を依頼するか</param>
/// <param name="DependentsIsForHelIns">被扶養者（配偶者含む）のマイナンバーの利用目的が「健康保険・厚生年金保険関係届出事務」に該当するか</param>
/// <param name="DependentsIsForEmpIns">被扶養者（配偶者含む）のマイナンバーの利用目的が「雇用保険関係届出事務」に該当するか</param>
/// <param name="DependentsIsForAccIns">被扶養者（配偶者含む）のマイナンバーの利用目的が「労働者災害補償保険法関係届出事務に該当するか</param>
/// <param name="DependentsIsForTaxDeduction">被扶養者（配偶者含む）のマイナンバーの利用目的が「源泉徴収に係る事務」に該当するか</param>
/// <param name="DependentsIsForShareholding">被扶養者（配偶者含む）のマイナンバーの利用目的が「持株会にかかる金融商品取引に関する法定書類の作成・提供事務」に該当するか</param>
/// <param name="DependentsIsForCat3Ins">被扶養者（配偶者含む）のマイナンバーの利用目的が「国民年金第3号被保険者関係届出事務（配偶者がいる場合）」に該当するか</param>
/// <param name="FieldGroups">従業員入力項目グループのリスト</param>
/// <param name="MailFormatId">
/// 紐づく<see cref="MailFormat.Id"/>。
/// <paramref name="MailFormatId"/>か<paramref name="MailFormat"/>のいずれかが出力されます。
/// </param>
/// <param name="MailFormat">
/// 紐づく<see cref="Entities.MailFormat"/>。
/// <paramref name="MailFormatId"/>か<paramref name="MailFormat"/>のいずれかが出力されます。
/// </param>
/// <param name="FamilyEnabled">扶養しない家族も登録できるかどうか</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record CrewInputForm(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("preset_type")] CrewInputForm.Preset? PresetType,
    [property: JsonPropertyName("form_type")] CrewInputForm.Kind FormType,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("with_my_number")] bool? WithMyNumber = default,
    [property: JsonPropertyName("crew_my_number_required")] bool? CrewMyNumberRequired = default,
    [property: JsonPropertyName("dependents_my_number_required")] bool? DependentsMyNumberRequired = default,
    [property: JsonPropertyName("crew_my_number_card_required")] bool? CrewMyNumberCardRequired = default,
    [property: JsonPropertyName("crew_identification_required")] bool? CrewIdentificationRequired = default,
    [property: JsonPropertyName("crew_is_for_hel_ins")] bool? CrewIsForHelIns = default,
    [property: JsonPropertyName("crew_is_for_emp_ins")] bool? CrewIsForEmpIns = default,
    [property: JsonPropertyName("crew_is_for_acc_ins")] bool? CrewIsForAccIns = default,
    [property: JsonPropertyName("crew_is_for_tax_deduction")] bool? CrewIsForTaxDeduction = default,
    [property: JsonPropertyName("crew_is_for_shareholding")] bool? CrewIsForShareholding = default,
    [property: JsonPropertyName("dependents_my_number_card_required")] bool? DependentsMyNumberCardRequired = default,
    [property: JsonPropertyName("dependents_identification_required")] bool? DependentsIdentificationRequired = default,
    [property: JsonPropertyName("dependents_is_for_hel_ins")] bool? DependentsIsForHelIns = default,
    [property: JsonPropertyName("dependents_is_for_emp_ins")] bool? DependentsIsForEmpIns = default,
    [property: JsonPropertyName("dependents_is_for_acc_ins")] bool? DependentsIsForAccIns = default,
    [property: JsonPropertyName("dependents_is_for_tax_deduction")] bool? DependentsIsForTaxDeduction = default,
    [property: JsonPropertyName("dependents_is_for_shareholding")] bool? DependentsIsForShareholding = default,
    [property: JsonPropertyName("dependents_is_for_cat3_ins")] bool? DependentsIsForCat3Ins = default,
    [property: JsonPropertyName("field_groups")] IReadOnlyList<CrewInputForm.FieldGroup>? FieldGroups = null,
    [property: JsonPropertyName("mail_format_id")] string? MailFormatId = null,
    [property: JsonPropertyName("mail_format")] MailFormat? MailFormat = null,
    [property: JsonPropertyName("family_enabled")] bool? FamilyEnabled = default,
    [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
    [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
)
{
    /// <summary>従業員入力項目グループ</summary>
    /// <param name="BasicFieldGroupType">基本項目のグループ種別</param>
    /// <param name="Hint">ヒント</param>
    /// <param name="Position">ポジション</param>
    /// <param name="Fields">従業員入力項目のリスト</param>
    /// <param name="CustomFieldTemplateGroupId">紐づく<see cref="CrewCustomFieldTemplateGroup.Id"/></param>
    /// <param name="CustomFieldTemplateGroup">紐づく<see cref="CrewCustomFieldTemplateGroup"/></param>
    public record FieldGroup(
        [property: JsonPropertyName("basic_field_group_type")] string? BasicFieldGroupType = null,
        [property: JsonPropertyName("hint")] string? Hint = null,
        [property: JsonPropertyName("position")] int? Position = default,
        [property: JsonPropertyName("fields")] IReadOnlyList<Field>? Fields = null,
        [property: JsonPropertyName("custom_field_template_group_id")] int? CustomFieldTemplateGroupId = default,
        [property: JsonPropertyName("custom_field_template_group")] CrewCustomFieldTemplateGroup? CustomFieldTemplateGroup = null
    );

    /// <summary>従業員情報項目</summary>
    /// <param name="AttributeName">紐づく従業員情報項目の物理名</param>
    /// <param name="DisplayType">項目の表示設定</param>
    /// <param name="CustomFieldTemplateId">紐づく<see cref="CrewCustomFieldTemplate.Id"/></param>
    /// <param name="CustomFieldTemplate">紐づく<see cref="CrewCustomFieldTemplate"/></param>
    public record Field(
        [property: JsonPropertyName("attribute_name")] string? AttributeName = null,
        [property: JsonPropertyName("display_type")] Field.Visibility? DisplayType = default,
        [property: JsonPropertyName("custom_field_template_id")] string? CustomFieldTemplateId = null,
        [property: JsonPropertyName("custom_field_template")] CrewCustomFieldTemplate? CustomFieldTemplate = null
    )
    {
        /// <summary><inheritdoc cref="Field" path="/param[@name='DisplayType']/text()"/></summary>
        [JsonConverter(typeof(JsonStringEnumConverterEx<Visibility>))]
        public enum Visibility
        {
            [EnumMember(Value = "hidden")] Hidden,
            [EnumMember(Value = "optional")] Optional,
            [EnumMember(Value = "required")] Required,
        }
    }

    /// <summary><inheritdoc cref="CrewInputForm" path="/param[@name='PresetType']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Preset>))]
    public enum Preset
    {
        [EnumMember(Value = "standard_invitation")] StandardInvitation,
        [EnumMember(Value = "minimal_invitation")] MinimalInvitation,
        [EnumMember(Value = "family_workflow")] FamilyWorkflow,
    }

    /// <summary><inheritdoc cref="CrewInputForm" path="/param[@name='FormType']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Kind>))]
    public enum Kind
    {
        [EnumMember(Value = "invitation")] Invitation,
    }
}
