using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="BizEstablishment"/>の単体テスト</summary>
public class BizEstablishmentTest
{
    /// <summary>
    /// <inheritdoc cref="BizEstablishment" path="/summary/text()"/>APIのサンプルレスポンスJSON
    /// </summary>
    internal const string Json = "{"
    + "\"id\": \"86b12705\","
    + "\"hel_ins_type\": \"kyokai_kenpo\","
    + "\"hel_ins_name\": \"\","
    + "\"name\": \"保険有限会社\","
    + "\"soc_ins_name\": \"株式会社 運輸\","
    + "\"soc_ins_owner_id\": \"0723a1df\","
    + "\"soc_ins_address\": {"
    + "  \"id\": \"0cf48a6b\","
    + "  \"country_number\": \"\","
    + "  \"zip_code\": \"000-0000\","
    + "  \"pref\": \"静岡県\","
    + "  \"city\": \"大野村\","
    + "  \"street\": \"大和 44939 結\","
    + "  \"building\": \"Suite 127\","
    + "  \"literal_yomi\": \"\""
    + "  },"
    + "\"soc_ins_tel_number\": \"000-0000-0000\","
    + "\"lab_ins_name\": \"株式会社 建設\","
    + "\"lab_ins_owner_id\": \"0723a1df\","
    + "\"lab_ins_address\": {"
    + "  \"id\": \"dbcce177\","
    + "  \"country_number\": \"\","
    + "  \"zip_code\": \"111-1111\","
    + "  \"pref\": \"東京都\","
    + "  \"city\": \"田村町\","
    + "  \"street\": \"結菜 31856 山本\","
    + "  \"building\": \"Apt. 367\","
    + "  \"literal_yomi\": \"\""
    + "  },"
    + "\"lab_ins_tel_number\": \"000-0000-0000\","
    + "\"jurisdiction_tax\": \"税務署\","
    + "\"salary_payer_address\": {"
    + "  \"id\": \"a581394c\","
    + "  \"country_number\": \"\","
    + "  \"zip_code\": \"222-2222\","
    + "  \"pref\": \"熊本県\","
    + "  \"city\": \"仁区\","
    + "  \"street\": \"智子 105 光\","
    + "  \"building\": \"Apt. 605\","
    + "  \"literal_yomi\": \"\""
    + "  },"
    + "\"updated_at\": \"2021-09-24T17:22:14.403Z\","
    + "\"created_at\": \"2021-09-24T17:22:14.403Z\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(BizEstablishment)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<BizEstablishment>(Json, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 9, 24, 17, 22, 14, 403, TimeSpan.Zero);
        entity.Should().Be(new BizEstablishment(
            Id: "86b12705",
            HelInsType: BizEstablishment.HealthInsurance.KyokaiKenpo,
            HelInsName: "",
            Name: "保険有限会社",
            SocInsName: "株式会社 運輸",
            SocInsOwnerId: "0723a1df",
            SocInsOwner: default,
            SocInsAddress: new("0cf48a6b", "", "000-0000", "静岡県", "大野村", "大和 44939 結", "Suite 127", ""),
            SocInsTelNumber: "000-0000-0000",
            LabInsName: "株式会社 建設",
            LabInsOwnerId: "0723a1df",
            LabInsAddress: new("dbcce177", "", "111-1111", "東京都", "田村町", "結菜 31856 山本", "Apt. 367", ""),
            LabInsTelNumber: "000-0000-0000",
            JurisdictionTax: "税務署",
            SalaryPayerAddress: new("a581394c", "", "222-2222", "熊本県", "仁区", "智子 105 光", "Apt. 605", ""),
            CreatedAt: date,
            UpdatedAt: date
        ));
    }
}
