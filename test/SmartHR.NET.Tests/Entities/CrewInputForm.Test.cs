using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="CrewInputForm"/>の単体テスト</summary>
public class CrewInputFormTest
{
    /// <summary>
    /// <inheritdoc cref="CrewInputForm" path="/summary/text()"/>APIのサンプルレスポンスJSON
    /// </summary>
    internal const string Json = "{"
    + "\"id\": \"9189cb26\","
    + "\"preset_type\": \"minimal_invitation\","
    + "\"form_type\": \"invitation\","
    + "\"name\": \"SmartHR 入社手続き不要従業員向けフォーム\","
    + "\"with_my_number\": false,"
    + "\"crew_my_number_required\": false,"
    + "\"dependents_my_number_required\": false,"
    + "\"crew_my_number_card_required\": false,"
    + "\"crew_identification_required\": false,"
    + "\"crew_is_for_hel_ins\": false,"
    + "\"crew_is_for_emp_ins\": false,"
    + "\"crew_is_for_acc_ins\": false,"
    + "\"crew_is_for_tax_deduction\": false,"
    + "\"crew_is_for_shareholding\": false,"
    + "\"dependents_my_number_card_required\": false,"
    + "\"dependents_identification_required\": false,"
    + "\"dependents_is_for_hel_ins\": false,"
    + "\"dependents_is_for_emp_ins\": false,"
    + "\"dependents_is_for_acc_ins\": false,"
    + "\"dependents_is_for_tax_deduction\": false,"
    + "\"dependents_is_for_shareholding\": false,"
    + "\"dependents_is_for_cat3_ins\": false,"
    + "\"field_groups\": ["
    + "    {"
    + "    \"basic_field_group_type\": \"basic_info\","
    + "    \"hint\": null,"
    + "    \"position\": 1,"
    + "    \"fields\": ["
    + "        {"
    + "        \"attribute_name\": \"last_name\","
    + "        \"display_type\": \"required\","
    + "        \"custom_field_template_id\": null"
    + "        },"
    + "        {"
    + "        \"attribute_name\": \"first_name\","
    + "        \"display_type\": \"required\","
    + "        \"custom_field_template_id\": null"
    + "        }"
    + "      ],"
    + "    \"custom_field_template_group_id\": null"
    + "    },"
    + "    {"
    + "    \"basic_field_group_type\": \"business_name\","
    + "    \"hint\": \"旧姓など\","
    + "    \"position\": 2,"
    + "    \"fields\": ["
    + "        {"
    + "        \"attribute_name\": \"business_last_name\","
    + "        \"display_type\": \"hidden\","
    + "        \"custom_field_template_id\": null"
    + "        },"
    + "        {"
    + "        \"attribute_name\": \"business_first_name\","
    + "        \"display_type\": \"hidden\","
    + "        \"custom_field_template_id\": null"
    + "        }"
    + "      ],"
    + "    \"custom_field_template_group_id\": null"
    + "    }"
    + "  ],"
    + "\"mail_format_id\": \"7b5adb07\","
    + "\"family_enabled\": false,"
    + "\"updated_at\": \"2021-09-24T17:22:21.847Z\","
    + "\"created_at\": \"2021-09-24T17:22:21.847Z\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(CrewInputForm)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<CrewInputForm>(Json, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 9, 24, 17, 22, 21, 847, TimeSpan.Zero);
        entity.Should().NotBeNull();
        entity!.Id.Should().Be("9189cb26");
        entity.PresetType.Should().Be(CrewInputForm.Preset.MinimalInvitation);
        entity.FormType.Should().Be(CrewInputForm.Kind.Invitation);
        entity.Name.Should().Be("SmartHR 入社手続き不要従業員向けフォーム");
        entity.WithMyNumber.Should().BeFalse();
        entity.CrewMyNumberRequired.Should().BeFalse();
        entity.DependentsMyNumberRequired.Should().BeFalse();
        entity.CrewMyNumberCardRequired.Should().BeFalse();
        entity.CrewIdentificationRequired.Should().BeFalse();
        entity.CrewIsForHelIns.Should().BeFalse();
        entity.CrewIsForEmpIns.Should().BeFalse();
        entity.CrewIsForAccIns.Should().BeFalse();
        entity.CrewIsForTaxDeduction.Should().BeFalse();
        entity.CrewIsForShareholding.Should().BeFalse();
        entity.DependentsMyNumberCardRequired.Should().BeFalse();
        entity.DependentsIdentificationRequired.Should().BeFalse();
        entity.DependentsIsForHelIns.Should().BeFalse();
        entity.DependentsIsForEmpIns.Should().BeFalse();
        entity.DependentsIsForAccIns.Should().BeFalse();
        entity.DependentsIsForTaxDeduction.Should().BeFalse();
        entity.DependentsIsForShareholding.Should().BeFalse();
        entity.DependentsIsForCat3Ins.Should().BeFalse();

        entity.FieldGroups.Should().HaveCount(2);
        entity.FieldGroups![0].BasicFieldGroupType.Should().Be("basic_info");
        entity.FieldGroups[1].Hint.Should().Be("旧姓など");
        entity.FieldGroups[0].Position.Should().Be(1);
        entity.FieldGroups[1].Fields.Should().Equal(
            new CrewInputForm.Field("business_last_name", CrewInputForm.Field.Visibility.Hidden),
            new CrewInputForm.Field("business_first_name", CrewInputForm.Field.Visibility.Hidden)
        );

        entity.MailFormatId.Should().Be("7b5adb07");
        entity.MailFormat.Should().BeNull();
        entity.FamilyEnabled.Should().BeFalse();
        entity.CreatedAt.Should().Be(date);
        entity.UpdatedAt.Should().Be(date);
    }
}
