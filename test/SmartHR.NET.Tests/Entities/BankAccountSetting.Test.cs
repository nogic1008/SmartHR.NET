using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary>
/// <see cref="BankAccountSetting"/>の単体テスト
/// </summary>
public class BankAccountSettingTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    [Fact(DisplayName = $"{nameof(BankAccountSetting)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange
        const string jsonString = "{"
        + "\"id\": \"4bcc14d0-a7fb-4a90-acd0-5d92472d27df\","
        + "\"name\": \"給与振込口座\","
        + "\"enabled\": true,"
        + "\"number\": 1,"
        + "\"preset_type\": \"salary_bank_account\","
        + "\"created_at\": \"2021-09-24T17:22:13.143Z\","
        + "\"updated_at\": \"2021-09-24T17:22:13.143Z\""
        + "}";

        // Act
        var entity = JsonSerializer.Deserialize<BankAccountSetting>(jsonString, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 9, 24, 17, 22, 13, 143, TimeSpan.Zero);
        entity.Should().Be(new BankAccountSetting(
            "4bcc14d0-a7fb-4a90-acd0-5d92472d27df",
            "給与振込口座",
            true,
            1,
            "salary_bank_account",
            date,
            date
        ));
    }
}
