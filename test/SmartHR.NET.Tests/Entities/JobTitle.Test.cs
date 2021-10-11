using System.Text.Json;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Tests.Entities;

/// <summary><see cref="JobTitle"/>の単体テスト</summary>
public class JobTitleTest
{
    /// <summary>役職APIのサンプルレスポンスJSON</summary>
    internal const string Json = "{"
    + "\"id\":\"id\","
    + "\"name\":\"name\","
    + "\"rank\":1,"
    + "\"created_at\":\"2021-10-06T00:24:48.910Z\","
    + "\"updated_at\":\"2021-10-06T00:24:48.910Z\""
    + "}";

    /// <summary>JSONからデシリアライズできる。</summary>
    [Fact(DisplayName = $"{nameof(JobTitle)} > JSONからデシリアライズできる。")]
    public void CanDeserializeJSON()
    {
        // Arrange - Act
        var entity = JsonSerializer.Deserialize<JobTitle>(Json, JsonConfig.Default);

        // Assert
        var date = new DateTimeOffset(2021, 10, 6, 0, 24, 48, 910, TimeSpan.Zero);
        entity.Should().Be(new JobTitle("id", "name", 1, date, date));
    }
}
