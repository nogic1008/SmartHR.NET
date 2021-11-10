using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="Crew"/>の単体テスト</summary>
public class CrewTest
{
    /// <summary>
    /// <inheritdoc cref="Crew" path="/summary/text()"/>APIのサンプルレスポンスJSON
    /// </summary>
    internal const string Json = "{"
    + "\"id\": \"0723a1df\","
    + "\"user_id\": \"70eb1bbb\","
    + "\"biz_establishment_id\": \"86b12705\","
    + "\"emp_code\": \"00027\","
    + "\"emp_type\": \"full_timer\","
    + "\"employment_type\": {"
    + "  \"id\": \"3a54c46e\","
    + "  \"name\": \"正社員\","
    + "  \"preset_type\": \"full_timer\""
    + "},"
    + "\"emp_status\": \"employed\","
    + "\"last_name\": \"苗字\","
    + "\"first_name\": \"名前\","
    + "\"last_name_yomi\": \"ミョウジ\","
    + "\"first_name_yomi\": \"ナマエ\","
    + "\"business_last_name\": \"ビジネス苗字\","
    + "\"business_first_name\": \"ビジネス名前\","
    + "\"business_last_name_yomi\": \"ビジネスミョウジ\","
    + "\"business_first_name_yomi\": \"ビジネスナマエ\","
    + "\"birth_at\": \"1937-02-06\","
    + "\"gender\": \"female\","
    + "\"identity_card_image1\": null,"
    + "\"identity_card_image2\": null,"
    + "\"tel_number\": \"000-0000-0000\","
    + "\"address\": {"
    + "  \"id\": \"816d627e\","
    + "  \"country_number\": \"\","
    + "  \"zip_code\": \"615-8216\","
    + "  \"pref\": \"北海道\","
    + "  \"city\": \"西原田市\","
    + "  \"street\": \"2-5-10\","
    + "  \"building\": \"\","
    + "  \"literal_yomi\": \"\""
    + "},"
    + "\"address_image\": {"
    + "  \"file_name\": \"menkyo.jpg\","
    + "  \"url\": \"https://example.com/menkyo.jpg\""
    + "},"
    + "\"address_head_of_family\": \"川本 永遠\","
    + "\"address_relation_name\": \"父\","
    + "\"email\": \"dylan@example.com\","
    + "\"profile_images\": null,"
    + "\"emergency_relation_name\": null,"
    + "\"emergency_last_name\": null,"
    + "\"emergency_first_name\": null,"
    + "\"emergency_last_name_yomi\": null,"
    + "\"emergency_first_name_yomi\": null,"
    + "\"emergency_tel_number\": null,"
    + "\"emergency_address\": null,"
    + "\"resident_card_address\": {"
    + "  \"id\": \"9340a62e\","
    + "  \"country_number\": \"\","
    + "  \"zip_code\": \"154-0011\","
    + "  \"pref\": \"東京都\","
    + "  \"city\": \"世田谷区上馬\","
    + "  \"street\": \"8-4-5\","
    + "  \"building\": \"フォレスター２１　１０１\","
    + "  \"literal_yomi\": \"\""
    + "},"
    + "\"resident_card_address_head_of_family\": null,"
    + "\"resident_card_address_relation_name\": null,"
    + "\"position\": null,"
    + "\"occupation\": \"物流戦略\","
    + "\"entered_at\": \"2011-11-01\","
    + "\"resigned_at\": null,"
    + "\"resigned_reason\": null,"
    + "\"resume1\": null,"
    + "\"resume2\": null,"
    + "\"emp_ins_insured_person_number\": \"0000-000000-1\","
    + "\"emp_ins_insured_person_number_image\": null,"
    + "\"emp_ins_insured_person_number_unknown_reason_type\": null,"
    + "\"emp_ins_qualified_at\": null,"
    + "\"emp_ins_disqualified_at\": null,"
    + "\"previous_workplace\": null,"
    + "\"previous_employment_start_on\": null,"
    + "\"previous_employment_end_on\": null,"
    + "\"soc_ins_insured_person_number\": null,"
    + "\"hel_ins_insured_person_number\": null,"
    + "\"basic_pension_number\": \"0000-000000\","
    + "\"basic_pension_number_image\": null,"
    + "\"first_enrolling_in_emp_pns_ins_flag\": false,"
    + "\"basic_pension_number_unknown_reason_type\": null,"
    + "\"first_workplace\": null,"
    + "\"first_workplace_address_text\": null,"
    + "\"first_employment_start_on\": null,"
    + "\"first_employment_end_on\": null,"
    + "\"last_workplace\": null,"
    + "\"last_workplace_address_text\": null,"
    + "\"last_employment_start_on\": null,"
    + "\"last_employment_end_on\": null,"
    + "\"soc_ins_qualified_at\": null,"
    + "\"soc_ins_disqualified_at\": null,"
    + "\"having_spouse\": false,"
    + "\"spouse_yearly_income\": null,"
    + "\"monthly_income_currency\": 771772,"
    + "\"monthly_income_goods\": 927629,"
    + "\"payment_period\": null,"
    + "\"monthly_standard_income_updated_at\": \"2020-12-14\","
    + "\"monthly_standard_income_hel\": null,"
    + "\"monthly_standard_income_pns\": null,"
    + "\"nearest_station_and_line\": \"東急田園都市線 三軒茶屋駅\","
    + "\"commutation_1_expenses\": 50000,"
    + "\"commutation_1_period\": \"commutation_period_1_month\","
    + "\"commutation_1_single_fare\": 500,"
    + "\"commutation_2_expenses\": 60000,"
    + "\"commutation_2_period\": \"commutation_period_3_month\","
    + "\"commutation_2_single_fare\": 600,"
    + "\"foreign_resident_last_name\": null,"
    + "\"foreign_resident_first_name\": null,"
    + "\"foreign_resident_middle_name\": null,"
    + "\"foreign_resident_card_number\": null,"
    + "\"foreign_resident_card_image1\": null,"
    + "\"foreign_resident_card_image2\": null,"
    + "\"nationality_code\": null,"
    + "\"resident_status_type\": null,"
    + "\"resident_status_other_reason\": null,"
    + "\"resident_end_at\": null,"
    + "\"having_ex_activity_permission\": null,"
    + "\"other_be_workable_type\": null,"
    + "\"bank_accounts\": null,"
    + "\"department\": null,"
    + "\"departments\": null,"
    + "\"contract_type\": \"unlimited\","
    + "\"contract_start_on\": null,"
    + "\"contract_end_on\": null,"
    + "\"contract_renewal_type\": null,"
    + "\"handicapped_type\": null,"
    + "\"handicapped_image\": null,"
    + "\"working_student_flag\": false,"
    + "\"school_name\": null,"
    + "\"student_card_image\": null,"
    + "\"enrolled_at\": null,"
    + "\"working_student_income\": null,"
    + "\"employment_income_flag\": false,"
    + "\"business_income_flag\": false,"
    + "\"devidend_income_flag\": false,"
    + "\"estate_income_flag\": false,"
    + "\"widow_type\": null,"
    + "\"widow_reason_type\": null,"
    + "\"widow_memo\": null,"
    + "\"created_at\": \"2021-09-24T17:00:00.000Z\","
    + "\"updated_at\": \"2021-09-24T17:00:00.000Z\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(Crew)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<Crew>(Json, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 9, 24, 17, 0, 0, 0, TimeSpan.Zero);
        entity.Should().Be(new Crew(
            Id: "0723a1df",
            UserId: "70eb1bbb",
            BizEstablishmentId: "86b12705",
            EmpCode: "00027",
            EmpType: EmploymentType.Preset.FullTimer,
            EmploymentType: new("3a54c46e", "正社員", EmploymentType.Preset.FullTimer),
            EmpStatus: Crew.EmploymentStatus.Employed,
            LastName: "苗字",
            FirstName: "名前",
            LastNameYomi: "ミョウジ",
            FirstNameYomi: "ナマエ",
            BusinessLastName: "ビジネス苗字",
            BusinessFirstName: "ビジネス名前",
            BusinessLastNameYomi: "ビジネスミョウジ",
            BusinessFirstNameYomi: "ビジネスナマエ",
            BirthAt: "1937-02-06",
            Gender: Crew.CrewGender.Female,
            TelNumber: "000-0000-0000",
            Address: new("816d627e", "", "615-8216", "北海道", "西原田市", "2-5-10", "", ""),
            AddressImage: new("menkyo.jpg", "https://example.com/menkyo.jpg"),
            AddressHeadOfFamily: "川本 永遠",
            AddressRelationName: "父",
            Email: "dylan@example.com",
            ResidentCardAddress: new("9340a62e", "", "154-0011", "東京都", "世田谷区上馬", "8-4-5", "フォレスター２１　１０１", ""),
            Occupation: "物流戦略",
            EnteredAt: "2011-11-01",
            EmpInsInsuredPersonNumber: "0000-000000-1",
            BasicPensionNumber: "0000-000000",
            FirstEnrollingInEmpPnsInsFlag: false,
            HavingSpouse: false,
            MonthlyIncomeCurrency: 771772,
            MonthlyIncomeGoods: 927629,
            MonthlyStandardIncomeUpdatedAt: "2020-12-14",
            NearestStationAndLine: "東急田園都市線 三軒茶屋駅",
            Commutation_1Expenses: 50000,
            Commutation_1Period: Crew.CommutationPeriod.Month,
            Commutation_1SingleFare: 500,
            Commutation_2Expenses: 60000,
            Commutation_2Period: Crew.CommutationPeriod.Quarter,
            Commutation_2SingleFare: 600,
            ContractType: Crew.Contract.Unlimited,
            WorkingStudentFlag: false,
            EmploymentIncomeFlag: false,
            BusinessIncomeFlag: false,
            DevidendIncomeFlag: false,
            EstateIncomeFlag: false,
            CreatedAt: date,
            UpdatedAt: date
        ));
    }
}
