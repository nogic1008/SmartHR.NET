using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="MailFormat"/>の単体テスト</summary>
public class MailFormatTest
{
    /// <summary>
    /// <inheritdoc cref="MailFormat" path="/summary/text()"/>APIのサンプルレスポンスJSON
    /// </summary>
    internal const string Json = "{"
    + "\"id\": \"7b5adb07-db38-49d3-b9f6-0dc17af9e517\","
    + "\"mail_type\": \"invitation\","
    + "\"name\": \"標準招待メール\","
    + "\"crew_input_forms\": ["
    + "  {"
    + "    \"id\": \"9189cb26-786a-404d-a4ff-bdcc1225ae51\","
    + "    \"preset_type\": \"minimal_invitation\","
    + "    \"form_type\": \"invitation\","
    + "    \"name\": \"SmartHR 入社手続き不要従業員向けフォーム\","
    + "    \"with_my_number\": false,"
    + "    \"crew_my_number_required\": false,"
    + "    \"dependents_my_number_required\": false,"
    + "    \"crew_my_number_card_required\": false,"
    + "    \"crew_identification_required\": false,"
    + "    \"crew_is_for_hel_ins\": false,"
    + "    \"crew_is_for_emp_ins\": false,"
    + "    \"crew_is_for_acc_ins\": false,"
    + "    \"crew_is_for_tax_deduction\": false,"
    + "    \"crew_is_for_shareholding\": false,"
    + "    \"dependents_my_number_card_required\": false,"
    + "    \"dependents_identification_required\": false,"
    + "    \"dependents_is_for_hel_ins\": false,"
    + "    \"dependents_is_for_emp_ins\": false,"
    + "    \"dependents_is_for_acc_ins\": false,"
    + "    \"dependents_is_for_tax_deduction\": false,"
    + "    \"dependents_is_for_shareholding\": false,"
    + "    \"dependents_is_for_cat3_ins\": false,"
    + "    \"field_groups\": ["
    + "      {"
    + "        \"basic_field_group_type\": \"basic_info\","
    + "        \"hint\": null,"
    + "        \"position\": 1,"
    + "        \"fields\": ["
    + "          {"
    + "            \"attribute_name\": \"last_name\","
    + "            \"display_type\": \"required\","
    + "            \"custom_field_template_id\": null"
    + "          }"
    + "        ],"
    + "        \"custom_field_template_group_id\": null"
    + "      }"
    + "    ],"
    + "    \"mail_format_id\": \"7b5adb07-db38-49d3-b9f6-0dc17af9e517\","
    + "    \"family_enabled\": false,"
    + "    \"updated_at\": \"2021-09-24T17:22:21.847+09:00\","
    + "    \"created_at\": \"2021-09-24T17:22:21.847+09:00\""
    + "  }"
    + "],"
    + "\"updated_at\": \"2021-09-24T17:22:21.314+09:00\","
    + "\"created_at\": \"2021-09-24T17:22:21.314+09:00\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(MailFormat)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<MailFormat>(Json, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 9, 24, 17, 22, 21, 314, new TimeSpan(9, 0, 0));
        entity.Should().BeAssignableTo<MailFormat>();
        entity!.Id.Should().Be("7b5adb07-db38-49d3-b9f6-0dc17af9e517");
        entity.MailType.Should().Be(MailFormat.Kind.Invitation);
        entity.Name.Should().Be("標準招待メール");
        entity.CrewInputForms.Should().HaveCount(1);
        entity.CreatedAt.Should().Be(date);
        entity.UpdatedAt.Should().Be(date);
    }
}
