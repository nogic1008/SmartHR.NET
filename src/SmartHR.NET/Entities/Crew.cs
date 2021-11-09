using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>従業員情報</summary>
/// <param name="Id">従業員ID</param>
/// <param name="UserId"><inheritdoc cref="User" path="/param[@name='Id']/text()"/></param>
/// <param name="BizEstablishmentId">
/// <inheritdoc cref="Entities.BizEstablishment" path="/param[@name='Id']/text()"/>
/// <paramref name="BizEstablishment"/>が<see langword="null"/>の場合に出力されます。
/// </param>
/// <param name="BizEstablishment">
/// <inheritdoc cref="Entities.BizEstablishment" path="/summary/text()"/>
/// <paramref name="BizEstablishmentId"/>が<see langword="null"/>の場合に出力されます。
/// </param>
/// <param name="EmpCode">社員番号</param>
/// <param name="EmpType">プリセット雇用形態(非推奨)</param>
/// <param name="EmploymentType">雇用形態</param>
/// <param name="EmpStatus">在籍状況</param>
/// <param name="LastName">姓</param>
/// <param name="FirstName">名</param>
/// <param name="LastNameYomi">姓 (カタカナ)</param>
/// <param name="FirstNameYomi">名 (カタカナ)</param>
/// <param name="BusinessLastName">ビジネスネーム：姓</param>
/// <param name="BusinessFirstName">ビジネスネーム：名</param>
/// <param name="BusinessLastNameYomi">ビジネスネーム：姓 (カタカナ)</param>
/// <param name="BusinessFirstNameYomi">ビジネスネーム：名 (カタカナ)</param>
/// <param name="BirthAt">生年月日</param>
/// <param name="Gender">戸籍上の性別</param>
/// <param name="IdentityCardImage1">本人確認書類1</param>
/// <param name="IdentityCardImage2">本人確認書類2</param>
/// <param name="TelNumber">電話番号</param>
/// <param name="Address">現住所</param>
/// <param name="AddressImage">現住所を確認できる書類</param>
/// <param name="AddressHeadOfFamily">世帯主</param>
/// <param name="AddressRelationName">世帯主の続柄</param>
/// <param name="Email">メールアドレス</param>
/// <param name="ProfileImages">プロフィール画像</param>
/// <param name="EmergencyRelationName">緊急連絡先の続柄</param>
/// <param name="EmergencyLastName">緊急連絡先の姓</param>
/// <param name="EmergencyFirstName">緊急連絡先の名</param>
/// <param name="EmergencyLastNameYomi">緊急連絡先の姓 (カタカナ)</param>
/// <param name="EmergencyFirstNameYomi">緊急連絡先の名 (カタカナ)</param>
/// <param name="EmergencyTelNumber">緊急連絡先の電話番号</param>
/// <param name="EmergencyAddress">緊急連絡先の住所</param>
/// <param name="ResidentCardAddress">住民票住所</param>
/// <param name="ResidentCardAddressHeadOfFamily">住民票住所の世帯主</param>
/// <param name="ResidentCardAddressRelationName">続柄 (住民票住所の世帯主)</param>
/// <param name="Position">役職</param>
/// <param name="Occupation">業務内容</param>
/// <param name="EnteredAt">入社年月日</param>
/// <param name="ResignedAt">退職年月日</param>
/// <param name="ResignedReason">退職事由</param>
/// <param name="Resume1">履歴書・職務経歴書1</param>
/// <param name="Resume2">履歴書・職務経歴書2</param>
/// <param name="EmpInsInsuredPersonNumber">雇用保険の被保険者番号</param>
/// <param name="EmpInsInsuredPersonNumberImage">雇用保険被保険者番号添付画像</param>
/// <param name="EmpInsInsuredPersonNumberUnknownReasonType">雇用保険被保険者番号未記載理由</param>
/// <param name="EmpInsQualifiedAt">雇用保険の資格取得年月日</param>
/// <param name="EmpInsDisqualifiedAt">雇用保険の離職等年月日</param>
/// <param name="PreviousWorkplace">雇用保険に加入していた会社名</param>
/// <param name="PreviousEmploymentStartOn">雇用保険に加入していた会社の在籍開始日</param>
/// <param name="PreviousEmploymentEndOn">雇用保険に加入していた会社の在籍終了日</param>
/// <param name="SocInsInsuredPersonNumber">厚生年金保険の被保険者整理番号</param>
/// <param name="HelInsInsuredPersonNumber">健康保険の被保険者整理番号</param>
/// <param name="BasicPensionNumber">基礎年金番号</param>
/// <param name="BasicPensionNumberImage">基礎年金番号添付画像</param>
/// <param name="FirstEnrollingInEmpPnsInsFlag">厚生年金初加入フラグ</param>
/// <param name="BasicPensionNumberUnknownReasonType">基礎年金番号未記載理由</param>
/// <param name="FirstWorkplace">最初に厚生年金へ加入した会社名</param>
/// <param name="FirstWorkplaceAddressText">最初に厚生年金へ加入した会社の住所文字列</param>
/// <param name="FirstEmploymentStartOn">最初に厚生年金へ加入した会社の在籍開始日</param>
/// <param name="FirstEmploymentEndOn">最初に厚生年金へ加入した会社の在籍終了日</param>
/// <param name="LastWorkplace">最後に厚生年金へ加入した会社名</param>
/// <param name="LastWorkplaceAddressText">最後に厚生年金へ加入した会社の住所文字列</param>
/// <param name="LastEmploymentStartOn">最後に厚生年金へ加入した会社の在籍開始日</param>
/// <param name="LastEmploymentEndOn">最後に厚生年金へ加入した会社の在籍終了日</param>
/// <param name="SocInsQualifiedAt">社会保険の資格取得年月日</param>
/// <param name="SocInsDisqualifiedAt">社会保険の資格喪失年月日</param>
/// <param name="HavingSpouse">配偶者の有無</param>
/// <param name="SpouseYearlyIncome">配偶者の年収</param>
/// <param name="MonthlyIncomeCurrency">報酬月額 (通貨)</param>
/// <param name="MonthlyIncomeGoods">報酬月額 (現物)</param>
/// <param name="PaymentPeriod">給与支給形態</param>
/// <param name="MonthlyStandardIncomeUpdatedAt">標準報酬月額の改定年月</param>
/// <param name="MonthlyStandardIncomeHel">健康保険の標準報酬月額</param>
/// <param name="MonthlyStandardIncomePns">厚生年金の標準報酬月額</param>
/// <param name="NearestStationAndLine">通勤経路</param>
/// <param name="Commutation_1Expenses">通勤手当1の定期券代</param>
/// <param name="Commutation_1Period">通勤手当1の期間</param>
/// <param name="Commutation_1SingleFare">通勤手当1の片道運賃</param>
/// <param name="Commutation_2Expenses">通勤手当2の定期券代</param>
/// <param name="Commutation_2Period">通勤手当2の期間</param>
/// <param name="Commutation_2SingleFare">通勤手当2の片道運賃</param>
/// <param name="ForeignResidentLastName">在留資格情報：姓</param>
/// <param name="ForeignResidentFirstName">在留資格情報：名</param>
/// <param name="ForeignResidentMiddleName">在留資格情報：ミドルネーム</param>
/// <param name="ForeignResidentCardNumber">在留カード番号</param>
/// <param name="ForeignResidentCardImage1">在留カードの画像1</param>
/// <param name="ForeignResidentCardImage2">在留カードの画像2</param>
/// <param name="NationalityCode">国籍 / 国籍コード</param>
/// <param name="ResidentStatusType">在留資格</param>
/// <param name="ResidentStatusOtherReason">在留資格不明理由</param>
/// <param name="ResidentEndAt">在留期日</param>
/// <param name="HavingExActivityPermission">資格外活動許可の有無</param>
/// <param name="OtherBeWorkableType">派遣・請負就労区分</param>
/// <param name="BankAccounts">給与振込口座</param>
/// <param name="Department">部署</param>
/// <param name="Departments">部署</param>
/// <param name="ContractType">契約種別</param>
/// <param name="ContractStartOn">契約開始日</param>
/// <param name="ContractEndOn">契約終了日</param>
/// <param name="ContractRenewalType">契約更新の有無</param>
/// <param name="HandicappedType">障害者区分</param>
/// <param name="HandicappedNoteType">障害者手帳の種類</param>
/// <param name="HandicappedNoteDeliveryAt">障害者手帳の交付年月日</param>
/// <param name="HandicappedImage">障害者手帳の画像</param>
/// <param name="WorkingStudentFlag">勤労学生フラグ</param>
/// <param name="SchoolName">勤労学生：学校名</param>
/// <param name="StudentCardImage">勤労学生：学生証の画像</param>
/// <param name="EnrolledAt">勤労学生：入学年月日</param>
/// <param name="WorkingStudentIncome">勤労学生：所得の見積額</param>
/// <param name="EmploymentIncomeFlag">給与所得フラグ</param>
/// <param name="BusinessIncomeFlag">事業所得フラグ</param>
/// <param name="DevidendIncomeFlag">配当所得フラグ</param>
/// <param name="EstateIncomeFlag">不動産所得フラグ</param>
/// <param name="WidowType">寡婦・ひとり親</param>
/// <param name="WidowReasonType">寡婦・ひとり親の理由</param>
/// <param name="WidowMemo">寡婦・ひとり親の備考</param>
/// <param name="CustomFields">カスタム項目</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record Crew(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("user_id")] string? UserId,
    [property: JsonPropertyName("biz_establishment_id")] string? BizEstablishmentId,
    [property: JsonPropertyName("biz_establishment")] BizEstablishment? BizEstablishment,
    [property: JsonPropertyName("emp_code")] string? EmpCode,
    [property: JsonPropertyName("emp_type")][property: Obsolete("雇用形態データの構造変更に伴い、このプロパティは非推奨となりました。今後はEmploymentTypeをご利用ください。")] EmploymentType.Preset? EmpType,
    [property: JsonPropertyName("employment_type")] EmploymentType? EmploymentType,
    [property: JsonPropertyName("emp_status")] Crew.EmploymentStatus EmpStatus,
    [property: JsonPropertyName("last_name")] string LastName,
    [property: JsonPropertyName("first_name")] string FirstName,
    [property: JsonPropertyName("last_name_yomi")] string LastNameYomi,
    [property: JsonPropertyName("first_name_yomi")] string FirstNameYomi,
    [property: JsonPropertyName("business_last_name")] string? BusinessLastName,
    [property: JsonPropertyName("business_first_name")] string? BusinessFirstName,
    [property: JsonPropertyName("business_last_name_yomi")] string? BusinessLastNameYomi,
    [property: JsonPropertyName("business_first_name_yomi")] string? BusinessFirstNameYomi,
    [property: JsonPropertyName("birth_at")] string? BirthAt,
    [property: JsonPropertyName("gender")] Crew.CrewGender Gender,
    [property: JsonPropertyName("identity_card_image1")] Attachment? IdentityCardImage1,
    [property: JsonPropertyName("identity_card_image2")] Attachment? IdentityCardImage2,
    [property: JsonPropertyName("tel_number")] string? TelNumber,
    [property: JsonPropertyName("address")] Address? Address,
    [property: JsonPropertyName("address_image")] Attachment? AddressImage,
    [property: JsonPropertyName("address_head_of_family")] string? AddressHeadOfFamily,
    [property: JsonPropertyName("address_relation_name")] string? AddressRelationName,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("profile_images")] IReadOnlyList<Crew.Image>? ProfileImages,
    [property: JsonPropertyName("emergency_relation_name")] string? EmergencyRelationName,
    [property: JsonPropertyName("emergency_last_name")] string? EmergencyLastName,
    [property: JsonPropertyName("emergency_first_name")] string? EmergencyFirstName,
    [property: JsonPropertyName("emergency_last_name_yomi")] string? EmergencyLastNameYomi,
    [property: JsonPropertyName("emergency_first_name_yomi")] string? EmergencyFirstNameYomi,
    [property: JsonPropertyName("emergency_tel_number")] string? EmergencyTelNumber,
    [property: JsonPropertyName("emergency_address")] Address? EmergencyAddress,
    [property: JsonPropertyName("resident_card_address")] Address? ResidentCardAddress,
    [property: JsonPropertyName("resident_card_address_head_of_family")] string? ResidentCardAddressHeadOfFamily,
    [property: JsonPropertyName("resident_card_address_relation_name")] string? ResidentCardAddressRelationName,
    [property: JsonPropertyName("position")] string? Position,
    [property: JsonPropertyName("occupation")] string? Occupation,
    [property: JsonPropertyName("entered_at")] string? EnteredAt,
    [property: JsonPropertyName("resigned_at")] string? ResignedAt,
    [property: JsonPropertyName("resigned_reason")] string? ResignedReason,
    [property: JsonPropertyName("resume1")] Attachment? Resume1,
    [property: JsonPropertyName("resume2")] Attachment? Resume2,
    [property: JsonPropertyName("emp_ins_insured_person_number")] string? EmpInsInsuredPersonNumber,
    [property: JsonPropertyName("emp_ins_insured_person_number_image")] Attachment? EmpInsInsuredPersonNumberImage,
    [property: JsonPropertyName("emp_ins_insured_person_number_unknown_reason_type")] Crew.InsuredPersonNumberUnknownReason? EmpInsInsuredPersonNumberUnknownReasonType,
    [property: JsonPropertyName("emp_ins_qualified_at")] string? EmpInsQualifiedAt,
    [property: JsonPropertyName("emp_ins_disqualified_at")] string? EmpInsDisqualifiedAt,
    [property: JsonPropertyName("previous_workplace")] string? PreviousWorkplace,
    [property: JsonPropertyName("previous_employment_start_on")] string? PreviousEmploymentStartOn,
    [property: JsonPropertyName("previous_employment_end_on")] string? PreviousEmploymentEndOn,
    [property: JsonPropertyName("soc_ins_insured_person_number")] int? SocInsInsuredPersonNumber,
    [property: JsonPropertyName("hel_ins_insured_person_number")] int? HelInsInsuredPersonNumber,
    [property: JsonPropertyName("basic_pension_number")] string? BasicPensionNumber,
    [property: JsonPropertyName("basic_pension_number_image")] Attachment? BasicPensionNumberImage,
    [property: JsonPropertyName("first_enrolling_in_emp_pns_ins_flag")] bool? FirstEnrollingInEmpPnsInsFlag,
    [property: JsonPropertyName("basic_pension_number_unknown_reason_type")] Crew.BasicPensionNumberUnknownReason? BasicPensionNumberUnknownReasonType,
    [property: JsonPropertyName("first_workplace")] string? FirstWorkplace,
    [property: JsonPropertyName("first_workplace_address_text")] string? FirstWorkplaceAddressText,
    [property: JsonPropertyName("first_employment_start_on")] string? FirstEmploymentStartOn,
    [property: JsonPropertyName("first_employment_end_on")] string? FirstEmploymentEndOn,
    [property: JsonPropertyName("last_workplace")] string? LastWorkplace,
    [property: JsonPropertyName("last_workplace_address_text")] string? LastWorkplaceAddressText,
    [property: JsonPropertyName("last_employment_start_on")] string? LastEmploymentStartOn,
    [property: JsonPropertyName("last_employment_end_on")] string? LastEmploymentEndOn,
    [property: JsonPropertyName("soc_ins_qualified_at")] string? SocInsQualifiedAt,
    [property: JsonPropertyName("soc_ins_disqualified_at")] string? SocInsDisqualifiedAt,
    [property: JsonPropertyName("having_spouse")] bool? HavingSpouse,
    [property: JsonPropertyName("spouse_yearly_income")] int? SpouseYearlyIncome,
    [property: JsonPropertyName("monthly_income_currency")] int? MonthlyIncomeCurrency,
    [property: JsonPropertyName("monthly_income_goods")] int? MonthlyIncomeGoods,
    [property: JsonPropertyName("payment_period")] PaymentPeriod? PaymentPeriod,
    [property: JsonPropertyName("monthly_standard_income_updated_at")] string? MonthlyStandardIncomeUpdatedAt,
    [property: JsonPropertyName("monthly_standard_income_hel")] int? MonthlyStandardIncomeHel,
    [property: JsonPropertyName("monthly_standard_income_pns")] int? MonthlyStandardIncomePns,
    [property: JsonPropertyName("nearest_station_and_line")] string? NearestStationAndLine,
    [property: JsonPropertyName("commutation_1_expenses")] int? Commutation_1Expenses,
    [property: JsonPropertyName("commutation_1_period")] Crew.CommutationPeriod? Commutation_1Period,
    [property: JsonPropertyName("commutation_1_single_fare")] int? Commutation_1SingleFare,
    [property: JsonPropertyName("commutation_2_expenses")] int? Commutation_2Expenses,
    [property: JsonPropertyName("commutation_2_period")] Crew.CommutationPeriod? Commutation_2Period,
    [property: JsonPropertyName("commutation_2_single_fare")] int? Commutation_2SingleFare,
    [property: JsonPropertyName("foreign_resident_last_name")] string? ForeignResidentLastName,
    [property: JsonPropertyName("foreign_resident_first_name")] string? ForeignResidentFirstName,
    [property: JsonPropertyName("foreign_resident_middle_name")] string? ForeignResidentMiddleName,
    [property: JsonPropertyName("foreign_resident_card_number")] string? ForeignResidentCardNumber,
    [property: JsonPropertyName("foreign_resident_card_image1")] Attachment? ForeignResidentCardImage1,
    [property: JsonPropertyName("foreign_resident_card_image2")] Attachment? ForeignResidentCardImage2,
    [property: JsonPropertyName("nationality_code")] string? NationalityCode,
    [property: JsonPropertyName("resident_status_type")] string? ResidentStatusType,
    [property: JsonPropertyName("resident_status_other_reason")] string? ResidentStatusOtherReason,
    [property: JsonPropertyName("resident_end_at")] string? ResidentEndAt,
    [property: JsonPropertyName("having_ex_activity_permission")] Crew.ExActivityPermission? HavingExActivityPermission,
    [property: JsonPropertyName("other_be_workable_type")] Crew.WorkableType? OtherBeWorkableType,
    [property: JsonPropertyName("bank_accounts")] IReadOnlyList<Crew.BankAccount>? BankAccounts,
    [property: JsonPropertyName("department")][property: Obsolete("部署データの構造変更に伴い、このプロパティは非推奨となりました。今後はDepartmentsをご利用ください。")] string? Department,
    [property: JsonPropertyName("departments")] IReadOnlyList<Department>? Departments,
    [property: JsonPropertyName("contract_type")] Crew.Contract ContractType,
    [property: JsonPropertyName("contract_start_on")] string? ContractStartOn,
    [property: JsonPropertyName("contract_end_on")] string? ContractEndOn,
    [property: JsonPropertyName("contract_renewal_type")] Crew.ContractRenewal? ContractRenewalType,
    [property: JsonPropertyName("handicapped_type")] Crew.Handicapped? HandicappedType,
    [property: JsonPropertyName("handicapped_note_type")] string? HandicappedNoteType,
    [property: JsonPropertyName("handicapped_note_delivery_at")] string? HandicappedNoteDeliveryAt,
    [property: JsonPropertyName("handicapped_image")] Attachment? HandicappedImage,
    [property: JsonPropertyName("working_student_flag")] bool? WorkingStudentFlag,
    [property: JsonPropertyName("school_name")] string? SchoolName,
    [property: JsonPropertyName("student_card_image")] Attachment? StudentCardImage,
    [property: JsonPropertyName("enrolled_at")] string? EnrolledAt,
    [property: JsonPropertyName("working_student_income")] int? WorkingStudentIncome,
    [property: JsonPropertyName("employment_income_flag")] bool? EmploymentIncomeFlag,
    [property: JsonPropertyName("business_income_flag")] bool? BusinessIncomeFlag,
    [property: JsonPropertyName("devidend_income_flag")] bool? DevidendIncomeFlag,
    [property: JsonPropertyName("estate_income_flag")] bool? EstateIncomeFlag,
    [property: JsonPropertyName("widow_type")] Crew.Widow? WidowType,
    [property: JsonPropertyName("widow_reason_type")] Crew.WidowReason WidowReasonType,
    [property: JsonPropertyName("widow_memo")] string? WidowMemo,
    [property: JsonPropertyName("custom_fields")] IReadOnlyList<Crew.CustomField>? CustomFields,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTimeOffset UpdatedAt
)
{
    /// <summary><inheritdoc cref="Crew" path="/param[@name='EmpStatus']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<EmploymentStatus>))]
    public enum EmploymentStatus
    {
        /// <summary>在籍中</summary>
        [EnumMember(Value = "employed")] Employed,
        /// <summary>休職中</summary>
        [EnumMember(Value = "absent")] Absent,
        /// <summary>退職済み</summary>
        [EnumMember(Value = "retired")] Retired,
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='Gender']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<CrewGender>))]
    public enum CrewGender
    {
        /// <summary>男性</summary>
        [EnumMember(Value = "male")] Male = 1,
        /// <summary>女性</summary>
        [EnumMember(Value = "female")] Female = 2,
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='EmpInsInsuredPersonNumberUnknownReasonType']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<InsuredPersonNumberUnknownReason>))]
    public enum InsuredPersonNumberUnknownReason
    {
        /// <summary>新卒など就業経験がない</summary>
        [EnumMember(Value = "no_work_experience")] NoWorkExperience,
        /// <summary>就業経験はあるが雇用保険に加入したことがない</summary>
        [EnumMember(Value = "never_joined")] NeverJoined,
        /// <summary>雇用保険に加入したことがあるが番号がわからない</summary>
        [EnumMember(Value = "unknown")] Unknown,
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='BasicPensionNumberUnknownReasonType']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<BasicPensionNumberUnknownReason>))]
    public enum BasicPensionNumberUnknownReason
    {
        /// <summary>基礎年金番号がない (20 歳未満、外国人など)</summary>
        [EnumMember(Value = "non_pensionable")] NonPensionable,
        /// <summary>基礎年金番号がわからない</summary>
        [EnumMember(Value = "unknown")] Unknown,
    }

    /// <summary>通勤手当の期間</summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<CommutationPeriod>))]
    public enum CommutationPeriod
    {
        /// <summary>1ヶ月</summary>
        [EnumMember(Value = "commutation_period_1_month")] Month,
        /// <summary>3ヶ月</summary>
        [EnumMember(Value = "commutation_period_3_month")] Quarter,
        /// <summary>6ヶ月</summary>
        [EnumMember(Value = "commutation_period_6_month")] HalfYear,
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='ResidentStatusType']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<ResidentStatus>))]
    public enum ResidentStatus
    {
        /// <summary>教授</summary>
        [EnumMember(Value = "resident_status_type_0001")] Type0001 = 0001,
        /// <summary>芸術</summary>
        [EnumMember(Value = "resident_status_type_0002")] Type0002 = 0002,
        /// <summary>宗教</summary>
        [EnumMember(Value = "resident_status_type_0003")] Type0003 = 0003,
        /// <summary>報道</summary>
        [EnumMember(Value = "resident_status_type_0004")] Type0004 = 0004,
        /// <summary>経営・管理</summary>
        [EnumMember(Value = "resident_status_type_0005")] Type0005 = 0005,
        /// <summary>法律・会計業務</summary>
        [EnumMember(Value = "resident_status_type_0006")] Type0006 = 0006,
        /// <summary>医療</summary>
        [EnumMember(Value = "resident_status_type_0007")] Type0007 = 0007,
        /// <summary>研究</summary>
        [EnumMember(Value = "resident_status_type_0008")] Type0008 = 0008,
        /// <summary>教育</summary>
        [EnumMember(Value = "resident_status_type_0009")] Type0009 = 0009,
        /// <summary>技術</summary>
        [EnumMember(Value = "resident_status_type_0010")] Type0010 = 0010,
        /// <summary>人文知識・国際業務</summary>
        [EnumMember(Value = "resident_status_type_0011")] Type0011 = 0011,
        /// <summary>企業内転勤</summary>
        [EnumMember(Value = "resident_status_type_0012")] Type0012 = 0012,
        /// <summary>興行</summary>
        [EnumMember(Value = "resident_status_type_0013")] Type0013 = 0013,
        /// <summary>技能</summary>
        [EnumMember(Value = "resident_status_type_0014")] Type0014 = 0014,
        /// <summary>技能実習</summary>
        [EnumMember(Value = "resident_status_type_0015")] Type0015 = 0015,
        /// <summary>文化活動</summary>
        [EnumMember(Value = "resident_status_type_0016")] Type0016 = 0016,
        /// <summary>短期滞在</summary>
        [EnumMember(Value = "resident_status_type_0017")] Type0017 = 0017,
        /// <summary>留学</summary>
        [EnumMember(Value = "resident_status_type_0018")] Type0018 = 0018,
        /// <summary>研修</summary>
        [EnumMember(Value = "resident_status_type_0019")] Type0019 = 0019,
        /// <summary>家族滞在</summary>
        [EnumMember(Value = "resident_status_type_0020")] Type0020 = 0020,
        /// <summary>永住者</summary>
        [EnumMember(Value = "resident_status_type_0021")] Type0021 = 0021,
        /// <summary>日本人の配偶者等</summary>
        [EnumMember(Value = "resident_status_type_0022")] Type0022 = 0022,
        /// <summary>永住者の配偶者等</summary>
        [EnumMember(Value = "resident_status_type_0023")] Type0023 = 0023,
        /// <summary>定住者</summary>
        [EnumMember(Value = "resident_status_type_0024")] Type0024 = 0024,
        /// <summary>技術・人文知識・国際業務</summary>
        [EnumMember(Value = "resident_status_type_0025")] Type0025 = 0025,
        /// <summary>介護</summary>
        [EnumMember(Value = "resident_status_type_0026")] Type0026 = 0026,
        /// <summary>特定活動 (ワーキングホリデー)</summary>
        [EnumMember(Value = "resident_status_type_1001")] Type1001 = 1001,
        /// <summary>特定活動 (EPA)</summary>
        [EnumMember(Value = "resident_status_type_1002")] Type1002 = 1002,
        /// <summary>特定活動 (ハラール牛肉生産)</summary>
        [EnumMember(Value = "resident_status_type_1003")] Type1003 = 1003,
        /// <summary>特定活動 (家事支援)</summary>
        [EnumMember(Value = "resident_status_type_1004")] Type1004 = 1004,
        /// <summary>特定活動 (外国人調理師)</summary>
        [EnumMember(Value = "resident_status_type_1005")] Type1005 = 1005,
        /// <summary>特定活動 (建設分野)</summary>
        [EnumMember(Value = "resident_status_type_1006")] Type1006 = 1006,
        /// <summary>特定活動 (高度学術研究活動)</summary>
        [EnumMember(Value = "resident_status_type_1007")] Type1007 = 1007,
        /// <summary>特定活動 (高度経営・管理活動)</summary>
        [EnumMember(Value = "resident_status_type_1008")] Type1008 = 1008,
        /// <summary>特定活動 (高度人材外国人の就労配偶者)</summary>
        [EnumMember(Value = "resident_status_type_1009")] Type1009 = 1009,
        /// <summary>特定活動 (高度専門・技術活動)</summary>
        [EnumMember(Value = "resident_status_type_1010")] Type1010 = 1010,
        /// <summary>特定活動 (就職活動)</summary>
        [EnumMember(Value = "resident_status_type_1011")] Type1011 = 1011,
        /// <summary>特定活動 (製造分野)</summary>
        [EnumMember(Value = "resident_status_type_1012")] Type1012 = 1012,
        /// <summary>特定活動 (造船分野)</summary>
        [EnumMember(Value = "resident_status_type_1013")] Type1013 = 1013,
        /// <summary>特定活動 (日系四世)</summary>
        [EnumMember(Value = "resident_status_type_1014")] Type1014 = 1014,
        /// <summary>特定活動 (農業)</summary>
        [EnumMember(Value = "resident_status_type_1015")] Type1015 = 1015,
        /// <summary>特定活動 (本邦大卒者)</summary>
        [EnumMember(Value = "resident_status_type_1016")] Type1016 = 1016,
        /// <summary>特定活動 (その他)</summary>
        [EnumMember(Value = "resident_status_type_1999")] Type1999 = 1999,
        /// <summary>特定技能1号 (ビルクリーニング)</summary>
        [EnumMember(Value = "resident_status_type_2001")] Type2001 = 2001,
        /// <summary>特定技能1号 (飲食料品製造業)</summary>
        [EnumMember(Value = "resident_status_type_2002")] Type2002 = 2002,
        /// <summary>特定技能1号 (介護)</summary>
        [EnumMember(Value = "resident_status_type_2003")] Type2003 = 2003,
        /// <summary>特定技能1号 (外食業)</summary>
        [EnumMember(Value = "resident_status_type_2004")] Type2004 = 2004,
        /// <summary>特定技能1号 (漁業)</summary>
        [EnumMember(Value = "resident_status_type_2005")] Type2005 = 2005,
        /// <summary>特定技能1号 (建設)</summary>
        [EnumMember(Value = "resident_status_type_2006")] Type2006 = 2006,
        /// <summary>特定技能1号 (航空)</summary>
        [EnumMember(Value = "resident_status_type_2007")] Type2007 = 2007,
        /// <summary>特定技能1号 (産業機械製造業)</summary>
        [EnumMember(Value = "resident_status_type_2008")] Type2008 = 2008,
        /// <summary>特定技能1号 (自動車整備)</summary>
        [EnumMember(Value = "resident_status_type_2009")] Type2009 = 2009,
        /// <summary>特定技能1号 (宿泊)</summary>
        [EnumMember(Value = "resident_status_type_2010")] Type2010 = 2010,
        /// <summary>特定技能1号 (素形材産業)</summary>
        [EnumMember(Value = "resident_status_type_2011")] Type2011 = 2011,
        /// <summary>特定技能1号 (造船・舶用工業)</summary>
        [EnumMember(Value = "resident_status_type_2012")] Type2012 = 2012,
        /// <summary>特定技能1号 (電気・電子情報関連産業)</summary>
        [EnumMember(Value = "resident_status_type_2013")] Type2013 = 2013,
        /// <summary>特定技能1号 (農業)</summary>
        [EnumMember(Value = "resident_status_type_2014")] Type2014 = 2014,
        /// <summary>特定技能2号 (建設)</summary>
        [EnumMember(Value = "resident_status_type_2015")] Type2015 = 2015,
        /// <summary>特定技能2号 (造船・舶用工業)</summary>
        [EnumMember(Value = "resident_status_type_2016")] Type2016 = 2016,
        /// <summary>高度専門職1号</summary>
        [EnumMember(Value = "resident_status_type_3001")] Type3001 = 3001,
        /// <summary>高度専門職2号</summary>
        [EnumMember(Value = "resident_status_type_3002")] Type3002 = 3002,
        /// <summary>不明</summary>
        [EnumMember(Value = "resident_status_type_9999")] Type9999 = 9999,
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='HavingExActivityPermission']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<ExActivityPermission>))]
    public enum ExActivityPermission
    {
        /// <summary>有</summary>
        [EnumMember(Value = "permitted")] Permitted,
        /// <summary>無</summary>
        [EnumMember(Value = "none")] None,
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='OtherBeWorkableType']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<WorkableType>))]
    public enum WorkableType
    {
        /// <summary>派遣・請負労働者として主として当該事業所以外で就労する</summary>
        [EnumMember(Value = "other_be_workable")] OtherBeWorkable,
        /// <summary>その他</summary>
        [EnumMember(Value = "others")] Others,
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='ContractType']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Contract>))]
    public enum Contract
    {
        /// <summary>無期雇用</summary>
        [EnumMember(Value = "unlimited")] Unlimited,
        /// <summary>有期雇用</summary>
        [EnumMember(Value = "fixed_term")] FixedTerm,
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='ContractRenewalType']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<ContractRenewal>))]
    public enum ContractRenewal
    {
        /// <summary>無</summary>
        [EnumMember(Value = "none")] None,
        /// <summary>有</summary>
        [EnumMember(Value = "renewal")] Renewal,
        /// <summary>有 (自動更新)</summary>
        [EnumMember(Value = "auto_renewal")] AutoRenewal,
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='HandicappedType']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Handicapped>))]
    public enum Handicapped
    {
        /// <summary>一般の障害者</summary>
        [EnumMember(Value = "ordinary_handicapped")] Ordinary,
        /// <summary>特別障害者</summary>
        [EnumMember(Value = "special_handicapped")] Special,
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='WidowType']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<Widow>))]
    public enum Widow
    {
        /// <summary>寡婦</summary>
        [EnumMember(Value = "widow")] Widow,
        /// <summary>寡婦 (特別)</summary>
        [EnumMember(Value = "special_widow")] SpecialWidow,
        /// <summary>寡夫</summary>
        [EnumMember(Value = "widower")] Widower,
        /// <summary>ひとり親</summary>
        [EnumMember(Value = "single_parent")] SingleParent,
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='WidowReasonType']/text()"/></summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<WidowReason>))]
    public enum WidowReason
    {
        /// <summary>離婚</summary>
        [EnumMember(Value = "divorce")] Divorce,
        /// <summary>死別</summary>
        [EnumMember(Value = "bereavement")] Bereavement,
        /// <summary>生死不明</summary>
        [EnumMember(Value = "missing")] Missing,
        /// <summary>未婚</summary>
        [EnumMember(Value = "unmarried")] Unmarried,
    }

    /// <summary>画像情報</summary>
    /// <param name="SizeType">サイズ種別</param>
    /// <param name="Height">高さ</param>
    /// <param name="Width">幅</param>
    /// <param name="Url">URL (有効期限付き)</param>
    public record Image(
        [property: JsonPropertyName("size_type")] Image.Size? SizeType,
        [property: JsonPropertyName("height")] int? Height,
        [property: JsonPropertyName("width")] int? Width,
        [property: JsonPropertyName("url")] string? Url
    )
    {
        /// <summary><inheritdoc cref="Image" path="/param[@name='SizeType']/text()"/></summary>
        [JsonConverter(typeof(JsonStringEnumConverterEx<Size>))]
        public enum Size
        {
            [EnumMember(Value = "thumb")] Thumb,
            [EnumMember(Value = "small")] Small,
            [EnumMember(Value = "medium")] Medium,
            [EnumMember(Value = "large")] Large,
            [EnumMember(Value = "original")] Original,
        }
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='BankAccounts']/text()"/></summary>
    public record BankAccount();

    /// <summary><inheritdoc cref="Crew" path="/param[@name='CustomFields']/text()"/></summary>
    public record CustomField();
}
