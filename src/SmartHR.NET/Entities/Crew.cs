using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>従業員情報</summary>
/// <param name="Id">従業員ID</param>
/// <param name="EmpStatus">在籍状況</param>
/// <param name="LastName">姓</param>
/// <param name="FirstName">名</param>
/// <param name="LastNameYomi">姓 (カタカナ)</param>
/// <param name="FirstNameYomi">名 (カタカナ)</param>
/// <param name="Gender">戸籍上の性別</param>
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
/// <param name="BusinessLastName">ビジネスネーム：姓</param>
/// <param name="BusinessFirstName">ビジネスネーム：名</param>
/// <param name="BusinessLastNameYomi">ビジネスネーム：姓 (カタカナ)</param>
/// <param name="BusinessFirstNameYomi">ビジネスネーム：名 (カタカナ)</param>
/// <param name="BirthAt">生年月日</param>
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
[JsonConverter(typeof(Crew.CrewJsonConverter))]
public record Crew(
    string Id,
    Crew.EmploymentStatus EmpStatus,
    string LastName,
    string FirstName,
    string LastNameYomi,
    string FirstNameYomi,
    Crew.CrewGender Gender,
    string? UserId = null,
    string? BizEstablishmentId = null,
    BizEstablishment? BizEstablishment = null,
    string? EmpCode = null,
    [property: Obsolete("雇用形態データの構造変更に伴い、このプロパティは非推奨となりました。今後はEmploymentTypeをご利用ください。")] EmploymentType.Preset? EmpType = default,
    EmploymentType? EmploymentType = null,
    string? BusinessLastName = null,
    string? BusinessFirstName = null,
    string? BusinessLastNameYomi = null,
    string? BusinessFirstNameYomi = null,
    // TODO: DateOnly型に変更
    string? BirthAt = null,
    Attachment? IdentityCardImage1 = null,
    Attachment? IdentityCardImage2 = null,
    string? TelNumber = null,
    Address? Address = null,
    Attachment? AddressImage = null,
    string? AddressHeadOfFamily = null,
    string? AddressRelationName = null,
    string? Email = null,
    IReadOnlyList<Crew.Image>? ProfileImages = null,
    string? EmergencyRelationName = null,
    string? EmergencyLastName = null,
    string? EmergencyFirstName = null,
    string? EmergencyLastNameYomi = null,
    string? EmergencyFirstNameYomi = null,
    string? EmergencyTelNumber = null,
    Address? EmergencyAddress = null,
    Address? ResidentCardAddress = null,
    string? ResidentCardAddressHeadOfFamily = null,
    string? ResidentCardAddressRelationName = null,
    string? Position = null,
    string? Occupation = null,
    // TODO: DateOnly型に変更
    string? EnteredAt = null,
    string? ResignedAt = null,
    string? ResignedReason = null,
    Attachment? Resume1 = null,
    Attachment? Resume2 = null,
    string? EmpInsInsuredPersonNumber = null,
    Attachment? EmpInsInsuredPersonNumberImage = null,
    Crew.InsuredPersonNumberUnknownReason? EmpInsInsuredPersonNumberUnknownReasonType = default,
    // TODO: DateOnly型に変更
    string? EmpInsQualifiedAt = null,
    // TODO: DateOnly型に変更
    string? EmpInsDisqualifiedAt = null,
    string? PreviousWorkplace = null,
    string? PreviousEmploymentStartOn = null,
    string? PreviousEmploymentEndOn = null,
    int? SocInsInsuredPersonNumber = default,
    int? HelInsInsuredPersonNumber = default,
    string? BasicPensionNumber = null,
    Attachment? BasicPensionNumberImage = null,
    bool? FirstEnrollingInEmpPnsInsFlag = default,
    Crew.BasicPensionNumberUnknownReason? BasicPensionNumberUnknownReasonType = default,
    string? FirstWorkplace = null,
    string? FirstWorkplaceAddressText = null,
    // TODO: DateOnly型に変更
    string? FirstEmploymentStartOn = null,
    // TODO: DateOnly型に変更
    string? FirstEmploymentEndOn = null,
    string? LastWorkplace = null,
    string? LastWorkplaceAddressText = null,
    // TODO: DateOnly型に変更
    string? LastEmploymentStartOn = null,
    // TODO: DateOnly型に変更
    string? LastEmploymentEndOn = null,
    // TODO: DateOnly型に変更
    string? SocInsQualifiedAt = null,
    // TODO: DateOnly型に変更
    string? SocInsDisqualifiedAt = null,
    bool? HavingSpouse = default,
    int? SpouseYearlyIncome = default,
    int? MonthlyIncomeCurrency = default,
    int? MonthlyIncomeGoods = default,
    PaymentPeriod? PaymentPeriod = null,
    // TODO: DateOnly型に変更
    string? MonthlyStandardIncomeUpdatedAt = null,
    int? MonthlyStandardIncomeHel = default,
    int? MonthlyStandardIncomePns = default,
    string? NearestStationAndLine = null,
    int? Commutation_1Expenses = default,
    Crew.CommutationPeriod? Commutation_1Period = default,
    int? Commutation_1SingleFare = default,
    int? Commutation_2Expenses = default,
    Crew.CommutationPeriod? Commutation_2Period = default,
    int? Commutation_2SingleFare = default,
    string? ForeignResidentLastName = null,
    string? ForeignResidentFirstName = null,
    string? ForeignResidentMiddleName = null,
    string? ForeignResidentCardNumber = null,
    Attachment? ForeignResidentCardImage1 = null,
    Attachment? ForeignResidentCardImage2 = null,
    string? NationalityCode = null,
    string? ResidentStatusType = null,
    string? ResidentStatusOtherReason = null,
    // TODO: DateOnly型に変更
    string? ResidentEndAt = null,
    Crew.ExActivityPermission? HavingExActivityPermission = default,
    Crew.WorkableType? OtherBeWorkableType = default,
    IReadOnlyList<Crew.BankAccount>? BankAccounts = null,
    [property: Obsolete("部署データの構造変更に伴い、このプロパティは非推奨となりました。今後はDepartmentsをご利用ください。")] string? Department = null,
    IReadOnlyList<Department>? Departments = null,
    Crew.Contract? ContractType = default,
    // TODO: DateOnly型に変更
    string? ContractStartOn = null,
    // TODO: DateOnly型に変更
    string? ContractEndOn = null,
    Crew.ContractRenewal? ContractRenewalType = default,
    Crew.Handicapped? HandicappedType = default,
    string? HandicappedNoteType = null,
    // TODO: DateOnly型に変更
    string? HandicappedNoteDeliveryAt = null,
    Attachment? HandicappedImage = null,
    bool? WorkingStudentFlag = default,
    string? SchoolName = null,
    Attachment? StudentCardImage = null,
    // TODO: DateOnly型に変更
    string? EnrolledAt = null,
    int? WorkingStudentIncome = default,
    bool? EmploymentIncomeFlag = default,
    bool? BusinessIncomeFlag = default,
    bool? DevidendIncomeFlag = default,
    bool? EstateIncomeFlag = default,
    Crew.Widow? WidowType = default,
    Crew.WidowReason? WidowReasonType = default,
    string? WidowMemo = null,
    IReadOnlyList<Crew.CustomField>? CustomFields = null,
    DateTimeOffset? CreatedAt = default,
    DateTimeOffset? UpdatedAt = default
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
    /// <param name="Code">銀行コード</param>
    /// <param name="BranchCode">支店コード</param>
    /// <param name="AccountType">預金種別</param>
    /// <param name="AccountNumber">口座番号</param>
    /// <param name="AccountHolderName">名義 (カタカナ)</param>
    /// <param name="BookImage">口座情報を確認できる画像</param>
    /// <param name="BankAccountSettingId"><inheritdoc cref="BankAccountSetting" path="/param[@name='Id']/text()"/></param>
    public record BankAccount(
        [property: JsonPropertyName("bank_code")] string Code,
        [property: JsonPropertyName("bank_branch_code")] string BranchCode,
        [property: JsonPropertyName("account_type")] BankAccount.Type? AccountType,
        [property: JsonPropertyName("account_number")] string AccountNumber,
        [property: JsonPropertyName("account_holder_nam")] string AccountHolderName,
        [property: JsonPropertyName("bankbook_image")] AttachmentParams? BookImage = default,
        [property: JsonPropertyName("bank_account_setting_id")] string? BankAccountSettingId = null
    )
    {
        /// <summary><inheritdoc cref="BankAccount" path="/param[@name='AccountType']/text()"/></summary>
        [JsonConverter(typeof(JsonStringEnumConverterEx<BankAccount.Type>))]
        public enum Type
        {
            [EnumMember(Value = "saving")] Saving,
            [EnumMember(Value = "checking")] Checking,
            [EnumMember(Value = "deposit")] Deposit,
        }
    }

    /// <summary><inheritdoc cref="Crew" path="/param[@name='CustomFields']/text()"/></summary>
    /// <param name="Value">設定値。テンプレート種別に応じた形式になります。</param>
    /// <param name="Template">カスタム項目テンプレート</param>
    public record CustomField(
        [property: JsonPropertyName("value")] JsonElement? Value,
        [property: JsonPropertyName("template")] CrewCustomFieldTemplate Template
    );

    [ExcludeFromCodeCoverage]
    private class CrewJsonConverter : JsonConverter<Crew>
    {
        public override Crew? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();
            var crew = new Crew(null!, (EmploymentStatus)(-1), null!, null!, null!, null!, (CrewGender)(-1));

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    switch (reader.GetString())
                    {
                        case "id":
                            reader.Read();
                            crew = crew with { Id = reader.GetString()! };
                            break;
                        case "user_id":
                            reader.Read();
                            crew = crew with { UserId = reader.GetString() };
                            break;
                        case "biz_establishment_id":
                            reader.Read();
                            crew = crew with { BizEstablishmentId = reader.GetString() };
                            break;
                        case "biz_establishment":
                            reader.Read();
                            crew = crew with { BizEstablishment = JsonSerializer.Deserialize<BizEstablishment?>(ref reader, options) };
                            break;
                        case "emp_code":
                            reader.Read();
                            crew = crew with { EmpCode = reader.GetString() };
                            break;
                        case "emp_type":
                            reader.Read();
#pragma warning disable CS0618
                            crew = crew with { EmpType = JsonSerializer.Deserialize<EmploymentType.Preset?>(ref reader, options) };
#pragma warning restore
                            break;
                        case "employment_type":
                            reader.Read();
                            crew = crew with { EmploymentType = JsonSerializer.Deserialize<EmploymentType?>(ref reader, options) };
                            break;
                        case "emp_status":
                            reader.Read();
                            crew = crew with { EmpStatus = JsonSerializer.Deserialize<EmploymentStatus>(ref reader, options) };
                            break;
                        case "last_name":
                            reader.Read();
                            crew = crew with { LastName = reader.GetString()! };
                            break;
                        case "first_name":
                            reader.Read();
                            crew = crew with { FirstName = reader.GetString()! };
                            break;
                        case "last_name_yomi":
                            reader.Read();
                            crew = crew with { LastNameYomi = reader.GetString()! };
                            break;
                        case "first_name_yomi":
                            reader.Read();
                            crew = crew with { FirstNameYomi = reader.GetString()! };
                            break;
                        case "business_last_name":
                            reader.Read();
                            crew = crew with { BusinessLastName = reader.GetString() };
                            break;
                        case "business_first_name":
                            reader.Read();
                            crew = crew with { BusinessFirstName = reader.GetString() };
                            break;
                        case "business_last_name_yomi":
                            reader.Read();
                            crew = crew with { BusinessLastNameYomi = reader.GetString() };
                            break;
                        case "business_first_name_yomi":
                            reader.Read();
                            crew = crew with { BusinessFirstNameYomi = reader.GetString() };
                            break;
                        case "birth_at":
                            reader.Read();
                            crew = crew with { BirthAt = reader.GetString() };
                            break;
                        case "gender":
                            reader.Read();
                            crew = crew with { Gender = JsonSerializer.Deserialize<CrewGender>(ref reader, options) };
                            break;
                        case "identity_card_image1":
                            reader.Read();
                            crew = crew with { IdentityCardImage1 = JsonSerializer.Deserialize<Attachment?>(ref reader, options) };
                            break;
                        case "identity_card_image2":
                            reader.Read();
                            crew = crew with { IdentityCardImage2 = JsonSerializer.Deserialize<Attachment?>(ref reader, options) };
                            break;
                        case "tel_number":
                            reader.Read();
                            crew = crew with { TelNumber = reader.GetString() };
                            break;
                        case "address":
                            reader.Read();
                            crew = crew with { Address = JsonSerializer.Deserialize<Address?>(ref reader, options) };
                            break;
                        case "address_image":
                            reader.Read();
                            crew = crew with { AddressImage = JsonSerializer.Deserialize<Attachment?>(ref reader, options) };
                            break;
                        case "address_head_of_family":
                            reader.Read();
                            crew = crew with { AddressHeadOfFamily = reader.GetString() };
                            break;
                        case "address_relation_name":
                            reader.Read();
                            crew = crew with { AddressRelationName = reader.GetString() };
                            break;
                        case "email":
                            reader.Read();
                            crew = crew with { Email = reader.GetString() };
                            break;
                        case "profile_images":
                            reader.Read();
                            crew = crew with { ProfileImages = JsonSerializer.Deserialize<IReadOnlyList<Crew.Image>?>(ref reader, options) };
                            break;
                        case "emergency_relation_name":
                            reader.Read();
                            crew = crew with { EmergencyRelationName = reader.GetString() };
                            break;
                        case "emergency_last_name":
                            reader.Read();
                            crew = crew with { EmergencyLastName = reader.GetString() };
                            break;
                        case "emergency_first_name":
                            reader.Read();
                            crew = crew with { EmergencyFirstName = reader.GetString() };
                            break;
                        case "emergency_last_name_yomi":
                            reader.Read();
                            crew = crew with { EmergencyLastNameYomi = reader.GetString() };
                            break;
                        case "emergency_first_name_yomi":
                            reader.Read();
                            crew = crew with { EmergencyFirstNameYomi = reader.GetString() };
                            break;
                        case "emergency_tel_number":
                            reader.Read();
                            crew = crew with { EmergencyTelNumber = reader.GetString() };
                            break;
                        case "emergency_address":
                            reader.Read();
                            crew = crew with { EmergencyAddress = JsonSerializer.Deserialize<Address?>(ref reader, options) };
                            break;
                        case "resident_card_address":
                            reader.Read();
                            crew = crew with { ResidentCardAddress = JsonSerializer.Deserialize<Address?>(ref reader, options) };
                            break;
                        case "resident_card_address_head_of_family":
                            reader.Read();
                            crew = crew with { ResidentCardAddressHeadOfFamily = reader.GetString() };
                            break;
                        case "resident_card_address_relation_name":
                            reader.Read();
                            crew = crew with { ResidentCardAddressRelationName = reader.GetString() };
                            break;
                        case "position":
                            reader.Read();
                            crew = crew with { Position = reader.GetString() };
                            break;
                        case "occupation":
                            reader.Read();
                            crew = crew with { Occupation = reader.GetString() };
                            break;
                        case "entered_at":
                            reader.Read();
                            crew = crew with { EnteredAt = reader.GetString() };
                            break;
                        case "resigned_at":
                            reader.Read();
                            crew = crew with { ResignedAt = reader.GetString() };
                            break;
                        case "resigned_reason":
                            reader.Read();
                            crew = crew with { ResignedReason = reader.GetString() };
                            break;
                        case "resume1":
                            reader.Read();
                            crew = crew with { Resume1 = JsonSerializer.Deserialize<Attachment?>(ref reader, options) };
                            break;
                        case "resume2":
                            reader.Read();
                            crew = crew with { Resume2 = JsonSerializer.Deserialize<Attachment?>(ref reader, options) };
                            break;
                        case "emp_ins_insured_person_number":
                            reader.Read();
                            crew = crew with { EmpInsInsuredPersonNumber = reader.GetString() };
                            break;
                        case "emp_ins_insured_person_number_image":
                            reader.Read();
                            crew = crew with { EmpInsInsuredPersonNumberImage = JsonSerializer.Deserialize<Attachment?>(ref reader, options) };
                            break;
                        case "emp_ins_insured_person_number_unknown_reason_type":
                            reader.Read();
                            crew = crew with { EmpInsInsuredPersonNumberUnknownReasonType = JsonSerializer.Deserialize<InsuredPersonNumberUnknownReason?>(ref reader, options) };
                            break;
                        case "emp_ins_qualified_at":
                            reader.Read();
                            crew = crew with { EmpInsQualifiedAt = reader.GetString() };
                            break;
                        case "emp_ins_disqualified_at":
                            reader.Read();
                            crew = crew with { EmpInsDisqualifiedAt = reader.GetString() };
                            break;
                        case "previous_workplace":
                            reader.Read();
                            crew = crew with { PreviousWorkplace = reader.GetString() };
                            break;
                        case "previous_employment_start_on":
                            reader.Read();
                            crew = crew with { PreviousEmploymentStartOn = reader.GetString() };
                            break;
                        case "previous_employment_end_on":
                            reader.Read();
                            crew = crew with { PreviousEmploymentEndOn = reader.GetString() };
                            break;
                        case "soc_ins_insured_person_number":
                            reader.Read();
                            crew = crew with { SocInsInsuredPersonNumber = JsonSerializer.Deserialize<int?>(ref reader, options) };
                            break;
                        case "hel_ins_insured_person_number":
                            reader.Read();
                            crew = crew with { HelInsInsuredPersonNumber = JsonSerializer.Deserialize<int?>(ref reader, options) };
                            break;
                        case "basic_pension_number":
                            reader.Read();
                            crew = crew with { BasicPensionNumber = reader.GetString() };
                            break;
                        case "basic_pension_number_image":
                            reader.Read();
                            crew = crew with { BasicPensionNumberImage = JsonSerializer.Deserialize<Attachment?>(ref reader, options) };
                            break;
                        case "first_enrolling_in_emp_pns_ins_flag":
                            reader.Read();
                            crew = crew with { FirstEnrollingInEmpPnsInsFlag = JsonSerializer.Deserialize<bool?>(ref reader, options) };
                            break;
                        case "basic_pension_number_unknown_reason_type":
                            reader.Read();
                            crew = crew with { BasicPensionNumberUnknownReasonType = JsonSerializer.Deserialize<Crew.BasicPensionNumberUnknownReason?>(ref reader, options) };
                            break;
                        case "first_workplace":
                            reader.Read();
                            crew = crew with { FirstWorkplace = reader.GetString() };
                            break;
                        case "first_workplace_address_text":
                            reader.Read();
                            crew = crew with { FirstWorkplaceAddressText = reader.GetString() };
                            break;
                        case "first_employment_start_on":
                            reader.Read();
                            crew = crew with { FirstEmploymentStartOn = reader.GetString() };
                            break;
                        case "first_employment_end_on":
                            reader.Read();
                            crew = crew with { FirstEmploymentEndOn = reader.GetString() };
                            break;
                        case "last_workplace":
                            reader.Read();
                            crew = crew with { LastWorkplace = reader.GetString() };
                            break;
                        case "last_workplace_address_text":
                            reader.Read();
                            crew = crew with { LastWorkplaceAddressText = reader.GetString() };
                            break;
                        case "last_employment_start_on":
                            reader.Read();
                            crew = crew with { LastEmploymentStartOn = reader.GetString() };
                            break;
                        case "last_employment_end_on":
                            reader.Read();
                            crew = crew with { LastEmploymentEndOn = reader.GetString() };
                            break;
                        case "soc_ins_qualified_at":
                            reader.Read();
                            crew = crew with { SocInsQualifiedAt = reader.GetString() };
                            break;
                        case "soc_ins_disqualified_at":
                            reader.Read();
                            crew = crew with { SocInsDisqualifiedAt = reader.GetString() };
                            break;
                        case "having_spouse":
                            reader.Read();
                            crew = crew with { HavingSpouse = JsonSerializer.Deserialize<bool?>(ref reader, options) };
                            break;
                        case "spouse_yearly_income":
                            reader.Read();
                            crew = crew with { SpouseYearlyIncome = JsonSerializer.Deserialize<int?>(ref reader, options) };
                            break;
                        case "monthly_income_currency":
                            reader.Read();
                            crew = crew with { MonthlyIncomeCurrency = JsonSerializer.Deserialize<int?>(ref reader, options) };
                            break;
                        case "monthly_income_goods":
                            reader.Read();
                            crew = crew with { MonthlyIncomeGoods = JsonSerializer.Deserialize<int?>(ref reader, options) };
                            break;
                        case "payment_period":
                            reader.Read();
                            crew = crew with { PaymentPeriod = JsonSerializer.Deserialize<PaymentPeriod?>(ref reader, options) };
                            break;
                        case "monthly_standard_income_updated_at":
                            reader.Read();
                            crew = crew with { MonthlyStandardIncomeUpdatedAt = reader.GetString() };
                            break;
                        case "monthly_standard_income_hel":
                            reader.Read();
                            crew = crew with { MonthlyStandardIncomeHel = JsonSerializer.Deserialize<int?>(ref reader, options) };
                            break;
                        case "monthly_standard_income_pns":
                            reader.Read();
                            crew = crew with { MonthlyStandardIncomePns = JsonSerializer.Deserialize<int?>(ref reader, options) };
                            break;
                        case "nearest_station_and_line":
                            reader.Read();
                            crew = crew with { NearestStationAndLine = reader.GetString() };
                            break;
                        case "commutation_1_expenses":
                            reader.Read();
                            crew = crew with { Commutation_1Expenses = JsonSerializer.Deserialize<int?>(ref reader, options) };
                            break;
                        case "commutation_1_period":
                            reader.Read();
                            crew = crew with { Commutation_1Period = JsonSerializer.Deserialize<Crew.CommutationPeriod?>(ref reader, options) };
                            break;
                        case "commutation_1_single_fare":
                            reader.Read();
                            crew = crew with { Commutation_1SingleFare = JsonSerializer.Deserialize<int?>(ref reader, options) };
                            break;
                        case "commutation_2_expenses":
                            reader.Read();
                            crew = crew with { Commutation_2Expenses = JsonSerializer.Deserialize<int?>(ref reader, options) };
                            break;
                        case "commutation_2_period":
                            reader.Read();
                            crew = crew with { Commutation_2Period = JsonSerializer.Deserialize<Crew.CommutationPeriod?>(ref reader, options) };
                            break;
                        case "commutation_2_single_fare":
                            reader.Read();
                            crew = crew with { Commutation_2SingleFare = JsonSerializer.Deserialize<int?>(ref reader, options) };
                            break;
                        case "foreign_resident_last_name":
                            reader.Read();
                            crew = crew with { ForeignResidentLastName = reader.GetString() };
                            break;
                        case "foreign_resident_first_name":
                            reader.Read();
                            crew = crew with { ForeignResidentFirstName = reader.GetString() };
                            break;
                        case "foreign_resident_middle_name":
                            reader.Read();
                            crew = crew with { ForeignResidentMiddleName = reader.GetString() };
                            break;
                        case "foreign_resident_card_number":
                            reader.Read();
                            crew = crew with { ForeignResidentCardNumber = reader.GetString() };
                            break;
                        case "foreign_resident_card_image1":
                            reader.Read();
                            crew = crew with { ForeignResidentCardImage1 = JsonSerializer.Deserialize<Attachment?>(ref reader, options) };
                            break;
                        case "foreign_resident_card_image2":
                            reader.Read();
                            crew = crew with { ForeignResidentCardImage2 = JsonSerializer.Deserialize<Attachment?>(ref reader, options) };
                            break;
                        case "nationality_code":
                            reader.Read();
                            crew = crew with { NationalityCode = reader.GetString() };
                            break;
                        case "resident_status_type":
                            reader.Read();
                            crew = crew with { ResidentStatusType = reader.GetString() };
                            break;
                        case "resident_status_other_reason":
                            reader.Read();
                            crew = crew with { ResidentStatusOtherReason = reader.GetString() };
                            break;
                        case "resident_end_at":
                            reader.Read();
                            crew = crew with { ResidentEndAt = reader.GetString() };
                            break;
                        case "having_ex_activity_permission":
                            reader.Read();
                            crew = crew with { HavingExActivityPermission = JsonSerializer.Deserialize<Crew.ExActivityPermission?>(ref reader, options) };
                            break;
                        case "other_be_workable_type":
                            reader.Read();
                            crew = crew with { OtherBeWorkableType = JsonSerializer.Deserialize<Crew.WorkableType?>(ref reader, options) };
                            break;
                        case "bank_accounts":
                            reader.Read();
                            crew = crew with { BankAccounts = JsonSerializer.Deserialize<IReadOnlyList<Crew.BankAccount>?>(ref reader, options) };
                            break;
                        case "department":
                            reader.Read();
#pragma warning disable CS0618
                            crew = crew with { Department = reader.GetString() };
#pragma warning restore
                            break;
                        case "departments":
                            reader.Read();
                            crew = crew with { Departments = JsonSerializer.Deserialize<IReadOnlyList<Department>?>(ref reader, options) };
                            break;
                        case "contract_type":
                            reader.Read();
                            crew = crew with { ContractType = JsonSerializer.Deserialize<Crew.Contract?>(ref reader, options) };
                            break;
                        case "contract_start_on":
                            reader.Read();
                            crew = crew with { ContractStartOn = reader.GetString() };
                            break;
                        case "contract_end_on":
                            reader.Read();
                            crew = crew with { ContractEndOn = reader.GetString() };
                            break;
                        case "contract_renewal_type":
                            reader.Read();
                            crew = crew with { ContractRenewalType = JsonSerializer.Deserialize<Crew.ContractRenewal?>(ref reader, options) };
                            break;
                        case "handicapped_type":
                            reader.Read();
                            crew = crew with { HandicappedType = JsonSerializer.Deserialize<Crew.Handicapped?>(ref reader, options) };
                            break;
                        case "handicapped_note_type":
                            reader.Read();
                            crew = crew with { HandicappedNoteType = reader.GetString() };
                            break;
                        case "handicapped_note_delivery_at":
                            reader.Read();
                            crew = crew with { HandicappedNoteDeliveryAt = reader.GetString() };
                            break;
                        case "handicapped_image":
                            reader.Read();
                            crew = crew with { HandicappedImage = JsonSerializer.Deserialize<Attachment?>(ref reader, options) };
                            break;
                        case "working_student_flag":
                            reader.Read();
                            crew = crew with { WorkingStudentFlag = JsonSerializer.Deserialize<bool?>(ref reader, options) };
                            break;
                        case "school_name":
                            reader.Read();
                            crew = crew with { SchoolName = reader.GetString() };
                            break;
                        case "student_card_image":
                            reader.Read();
                            crew = crew with { StudentCardImage = JsonSerializer.Deserialize<Attachment?>(ref reader, options) };
                            break;
                        case "enrolled_at":
                            reader.Read();
                            crew = crew with { EnrolledAt = reader.GetString() };
                            break;
                        case "working_student_income":
                            reader.Read();
                            crew = crew with { WorkingStudentIncome = JsonSerializer.Deserialize<int?>(ref reader, options) };
                            break;
                        case "employment_income_flag":
                            reader.Read();
                            crew = crew with { EmploymentIncomeFlag = JsonSerializer.Deserialize<bool?>(ref reader, options) };
                            break;
                        case "business_income_flag":
                            reader.Read();
                            crew = crew with { BusinessIncomeFlag = JsonSerializer.Deserialize<bool?>(ref reader, options) };
                            break;
                        case "devidend_income_flag":
                            reader.Read();
                            crew = crew with { DevidendIncomeFlag = JsonSerializer.Deserialize<bool?>(ref reader, options) };
                            break;
                        case "estate_income_flag":
                            reader.Read();
                            crew = crew with { EstateIncomeFlag = JsonSerializer.Deserialize<bool?>(ref reader, options) };
                            break;
                        case "widow_type":
                            reader.Read();
                            crew = crew with { WidowType = JsonSerializer.Deserialize<Crew.Widow?>(ref reader, options) };
                            break;
                        case "widow_reason_type":
                            reader.Read();
                            crew = crew with { WidowReasonType = JsonSerializer.Deserialize<WidowReason?>(ref reader, options) };
                            break;
                        case "widow_memo":
                            reader.Read();
                            crew = crew with { WidowMemo = reader.GetString() };
                            break;
                        case "custom_fields":
                            reader.Read();
                            crew = crew with { CustomFields = JsonSerializer.Deserialize<IReadOnlyList<CustomField>?>(ref reader, options) };
                            break;
                        case "created_at":
                            reader.Read();
                            crew = crew with { CreatedAt = JsonSerializer.Deserialize<DateTimeOffset?>(ref reader, options) };
                            break;
                        case "updated_at":
                            reader.Read();
                            crew = crew with { UpdatedAt = JsonSerializer.Deserialize<DateTimeOffset?>(ref reader, options) };
                            break;
                        default:
                            throw new JsonException();
                    }
                    continue;
                }

                throw new JsonException();
            }

            if (crew.Id is null)
                throw new JsonException("\"id\"プロパティは必須です");
            if ((int)crew.EmpStatus == -1)
                throw new JsonException("\"emp_status\"プロパティは必須です");
            if (crew.LastName is null)
                throw new JsonException("\"last_name\"プロパティは必須です");
            if (crew.FirstName is null)
                throw new JsonException("\"first_name\"プロパティは必須です");
            if (crew.LastNameYomi is null)
                throw new JsonException("\"last_name_yomi\"プロパティは必須です");
            if (crew.FirstNameYomi is null)
                throw new JsonException("\"first_name_yomi\"プロパティは必須です");
            if ((int)crew.Gender == -1)
                throw new JsonException("\"gender\"プロパティは必須です");

            return crew;
        }
        public override void Write(Utf8JsonWriter writer, Crew value, JsonSerializerOptions options)
        {

        }
    }
}
