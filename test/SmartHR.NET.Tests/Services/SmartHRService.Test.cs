using System.Net.Http;
using SmartHR.NET.Services;

namespace SmartHR.NET.Tests.Services;

/// <summary>
/// <see cref="SmartHRService"/>の単体テスト
/// </summary>
public class SmartHRServiceTest
{
    /// <summary>テスト用の<see cref="HttpClient.BaseAddress"/></summary>
    private static readonly Uri _baseUri = new("https://example.com/");

    /// <summary>ランダムな文字列を生成します。</summary>
    private static string GenerateRandomString() => Guid.NewGuid().ToString();

    #region Constractor
    /// <summary>
    /// コンストラクターを呼び出す<see cref="Action"/>を生成します。
    /// </summary>
    private static Action Constractor(HttpClient? client, string? endpoint)
        => () => _ = new SmartHRService(client!, endpoint);

    /// <summary>
    /// HttpClientが<c>null</c>のとき、<see cref="ArgumentNullException"/>をスローする。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > Constractor > ArgumentNullExceptionをスローする。")]
    public void Constractor_Throws_ArgumentNullException_WhenClientIsNull()
        => Constractor(null, _baseUri.ToString()).Should().ThrowExactly<ArgumentNullException>();

    /// <summary>
    /// 不正なURIが渡されたとき、<see cref="UriFormatException"/>をスローする。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > Constractor > UriFormatExceptionをスローする。")]
    public void Constractor_Throws_UriFormatException()
        => Constractor(null, "foo").Should().ThrowExactly<UriFormatException>();

    /// <summary>
    /// コンストラクターで指定された引数をHttpClientにセットする。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > Constractor > 指定された引数をHttpClientにセットする。")]
    public void Constractor_Sets_Values_WhenNotNull()
    {
        // Arrange
        var client = new HttpClient();
        string accessToken = GenerateRandomString();

        // Act
        _ = new SmartHRService(client, _baseUri, accessToken);

        // Assert
        client.BaseAddress.Should().Be(_baseUri);
        client.DefaultRequestHeaders.Authorization.Should().NotBeNull();
        client.DefaultRequestHeaders.Authorization!.Scheme.Should().Be("Bearer");
        client.DefaultRequestHeaders.Authorization.Parameter.Should().Be(accessToken);
    }

    /// <summary>
    /// コンストラクターで指定された引数が<c>null</c>の場合、HttpClientに値をセットしない。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > Constractor > nullの場合、HttpClientに値をセットしない。")]
    public void Constractor_DoesNot_Set_Values_WhenNull()
    {
        // Arrange
        var client = new HttpClient();
        string accessToken = GenerateRandomString();
        client.BaseAddress = _baseUri;
        client.DefaultRequestHeaders.Authorization = new("Bearer", accessToken);

        // Act
        _ = new SmartHRService(client);

        // Assert
        client.BaseAddress.Should().Be(_baseUri);
        client.DefaultRequestHeaders.Authorization.Should().NotBeNull();
        client.DefaultRequestHeaders.Authorization!.Scheme.Should().Be("Bearer");
        client.DefaultRequestHeaders.Authorization.Parameter.Should().Be(accessToken);
    }
    #endregion
}
