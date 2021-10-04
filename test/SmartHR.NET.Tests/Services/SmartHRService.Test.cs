using SmartHR.NET.Entities;
using SmartHR.NET.Services;

namespace SmartHR.NET.Tests.Services;

/// <summary>
/// <see cref="SmartHRService"/>の単体テスト
/// </summary>
public class SmartHRServiceTest
{
    /// <summary>テスト用の<see cref="HttpClient.BaseAddress"/></summary>
    private const string BaseUri = "https://example.com";

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
        => Constractor(null, BaseUri).Should().ThrowExactly<ArgumentNullException>();

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
        _ = new SmartHRService(client, BaseUri, accessToken);

        // Assert
        client.BaseAddress.Should().Be(BaseUri);
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
        client.BaseAddress = new(BaseUri);
        client.DefaultRequestHeaders.Authorization = new("Bearer", accessToken);

        // Act
        _ = new SmartHRService(client);

        // Assert
        client.BaseAddress.Should().Be(BaseUri);
        client.DefaultRequestHeaders.Authorization.Should().NotBeNull();
        client.DefaultRequestHeaders.Authorization!.Scheme.Should().Be("Bearer");
        client.DefaultRequestHeaders.Authorization.Parameter.Should().Be(accessToken);
    }
    #endregion

    /// <summary>
    /// テスト対象(System Under Test)となる<see cref="SmartHRService"/>のインスタンスを生成します。
    /// </summary>
    /// <param name="handler">HttpClientをモックするためのHandlerオブジェクト</param>
    /// <param name="accessToken">アクセストークン</param>
    private static SmartHRService CreateSut(Mock<HttpMessageHandler> handler, string accessToken)
    {
        var client = handler.CreateClient();
        client.BaseAddress = new(BaseUri);
        client.DefaultRequestHeaders.Authorization = new("Bearer", accessToken);
        return new(client);
    }

    #region PayRolls
    private const string PayRollResponseJson = "{"
        + "\"id\": \"string\","
        + "\"payment_type\": \"salary\","
        + "\"paid_at\": \"2021-09-30\","
        + "\"period_start_at\": \"2021-09-30\","
        + "\"period_end_at\": \"2021-09-30\","
        + "\"source_type\": \"api\","
        + "\"status\": \"wip\","
        + "\"published_at\": \"string\","
        + "\"notify_with_publish\": true,"
        + "\"numeral_system_handle_type\": \"as_is\","
        + "\"name_for_admin\": \"string\","
        + "\"name_for_crew\": \"string\""
        + "}";

    /// <summary>
    /// <see cref="SmartHRService.DeletePayRollAsync"/>は、"/v1/payrolls/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeletePayRollAsync)} > DELETE /v1/payrolls/:id をコールする。")]
    public async Task DeletePayRollAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler, accessToken);
        await sut.DeletePayRollAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/payrolls/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchPayRollAsync"/>は、"/v1/payrolls/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPayRollAsync)} > GET /v1/payrolls/:id をコールする。")]
    public async Task FetchPayRollAsync_Calls_GetApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayRollResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.FetchPayRollAsync(id).ConfigureAwait(false);

        // Assert
        payRoll.Should().Be(new PayRoll(
            "string",
            PaymentType.Salary,
            new(2021, 9, 30),
            new(2021, 9, 30),
            new(2021, 9, 30),
            "api",
            PaymentStatus.Wip,
            default,
            true,
            NumeralSystemType.AsIs,
            "string",
            "string"
        ));
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/payrolls/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UpdatePayRollAsync"/>は、"/v1/payrolls/{id}"にPATCHリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.PublishPayRollAsync)} > PATCH /v1/payrolls/:id/publish をコールする。")]
    public async Task UpdatePayRollAsync_Calls_PatchApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();
        string nameForAdmin = GenerateRandomString();
        string nameForCrew = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayRollResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.UpdatePayRollAsync(id, nameForAdmin, nameForCrew).ConfigureAwait(false);

        // Assert
        payRoll.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/payrolls/{id}");
            req.Method.Should().Be(HttpMethod.Patch);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);

            string receivedParameters = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedParameters.Should().Be($"name_for_admin={nameForAdmin}&name_for_crew={nameForCrew}");
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.PublishPayRollAsync"/>は、"/v1/payrolls/{id}/publish"にPATCHリクエストを行う。
    /// </summary>
    /// <param name="publishedAt">公開時刻</param>
    /// <param name="notifyWithPublish">公開と同時に通知を行う</param>
    /// <param name="expected">サーバー側が受け取るパラメータ</param>
    [InlineData(null, null, "")]
    [InlineData(null, false, "notify_with_publish=false")]
    [InlineData("2021-09-30T00:00:00Z", true, "published_at=2021-09-30T09%3A00%3A00.0000000%2B09%3A00&notify_with_publish=true")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.PublishPayRollAsync)} > PATCH /v1/payrolls/:id/publish をコールする。")]
    public async Task PublishPayRollAsync_Calls_PatchApi(string publishedAt, bool? notifyWithPublish, string expected)
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayRollResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.PublishPayRollAsync(
            id,
            publishedAt is not null ? DateTime.Parse(publishedAt) : null,
            notifyWithPublish
        ).ConfigureAwait(false);

        // Assert
        payRoll.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/payrolls/{id}/publish");
            req.Method.Should().Be(HttpMethod.Patch);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);

            string receivedParameters = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedParameters.Should().Be(expected);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UnconfirmPayRollAsync"/>は、"/v1/payrolls/{id}/unfix"にPATCHリクエストを行う。
    /// </summary>
    /// <param name="publishedAt">公開時刻</param>
    /// <param name="notifyWithPublish">公開と同時に通知を行う</param>
    /// <param name="expected">サーバー側が受け取るパラメータ</param>
    [InlineData(null, null, "")]
    [InlineData(null, false, "&notify_with_publish=false")]
    [InlineData("2021-09-30T00:00:00Z", true, "&published_at=2021-09-30T09%3A00%3A00.0000000%2B09%3A00&notify_with_publish=true")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UnconfirmPayRollAsync)} > PATCH /v1/payrolls/:id/unfix をコールする。")]
    public async Task UnconfirmPayRollAsync_Calls_PatchApi(string publishedAt, bool? notifyWithPublish, string expected)
    {
        // Arrange
        string id = GenerateRandomString();
        string nameForAdmin = GenerateRandomString();
        string nameForCrew = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayRollResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.UnconfirmPayRollAsync(
            id,
            PaymentType.Salary,
            new(2021, 11, 1),
            new(2021, 10, 1),
            new(2021, 10, 31),
            PaymentStatus.Wip,
            NumeralSystemType.AsIs,
            nameForAdmin,
            nameForCrew,
            publishedAt is not null ? DateTime.Parse(publishedAt) : null,
            notifyWithPublish
        ).ConfigureAwait(false);

        // Assert
        payRoll.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/payrolls/{id}/unfix");
            req.Method.Should().Be(HttpMethod.Patch);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);

            string receivedParameters = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedParameters.Should().Be(
                "payment_type=salary"
                + "&paid_at=2021-11-01"
                + "&period_start_at=2021-10-01"
                + "&period_end_at=2021-10-31"
                + "&status=wip"
                + "&numeral_system_handle_type=as_is"
                + $"&name_for_admin={nameForAdmin}"
                + $"&name_for_crew={nameForCrew}"
                + expected);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.ConfirmPayRollAsync"/>は、"/v1/payrolls/{id}/fix"にPATCHリクエストを行う。
    /// </summary>
    /// <param name="publishedAt">公開時刻</param>
    /// <param name="notifyWithPublish">公開と同時に通知を行う</param>
    /// <param name="expected">サーバー側が受け取るパラメータ</param>
    [InlineData(null, null, "")]
    [InlineData(null, false, "&notify_with_publish=false")]
    [InlineData("2021-09-30T00:00:00Z", true, "&published_at=2021-09-30T09%3A00%3A00.0000000%2B09%3A00&notify_with_publish=true")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.ConfirmPayRollAsync)} > PATCH /v1/payrolls/:id/fix をコールする。")]
    public async Task ConfirmPayRollAsync_Calls_PatchApi(string publishedAt, bool? notifyWithPublish, string expected)
    {
        // Arrange
        string id = GenerateRandomString();
        string nameForAdmin = GenerateRandomString();
        string nameForCrew = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayRollResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.ConfirmPayRollAsync(
            id,
            PaymentType.Salary,
            new(2021, 11, 1),
            new(2021, 10, 1),
            new(2021, 10, 31),
            PaymentStatus.Wip,
            NumeralSystemType.AsIs,
            nameForAdmin,
            nameForCrew,
            publishedAt is not null ? DateTime.Parse(publishedAt) : null,
            notifyWithPublish
        ).ConfigureAwait(false);

        // Assert
        payRoll.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/payrolls/{id}/fix");
            req.Method.Should().Be(HttpMethod.Patch);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);

            string receivedParameters = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedParameters.Should().Be(
                "payment_type=salary"
                + "&paid_at=2021-11-01"
                + "&period_start_at=2021-10-01"
                + "&period_end_at=2021-10-31"
                + "&status=wip"
                + "&numeral_system_handle_type=as_is"
                + $"&name_for_admin={nameForAdmin}"
                + $"&name_for_crew={nameForCrew}"
                + expected);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// ページ数が不正なとき、<see cref="SmartHRService.FetchPayRollListAsync"/>は、ArgumentOutOfRangeExceptionをスローする。
    /// </summary>
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPayRollListAsync)} > ArgumentOutOfRangeException をスローする。")]
    public async Task FetchPayRollListAsync_Throws_ArgumentOutOfRangeException(int page, int perPage)
    {
        // Arrange
        string accessToken = GenerateRandomString();
        string nameForAdmin = GenerateRandomString();
        string nameForCrew = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest((_) => true)
            .ReturnsResponse($"[{PayRollResponseJson}]", "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var action = async () => _ = await sut.FetchPayRollListAsync(page, perPage).ConfigureAwait(false);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>().ConfigureAwait(false);
        handler.VerifyRequest((_) => true, Times.Never());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchPayRollListAsync"/>は、"/v1/payrolls"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPayRollListAsync)} > GET /v1/payrolls をコールする。")]
    public async Task FetchPayRollListAsync_Calls_GetApi()
    {
        // Arrange
        string accessToken = GenerateRandomString();
        string nameForAdmin = GenerateRandomString();
        string nameForCrew = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{PayRollResponseJson}]", "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRolls = await sut.FetchPayRollListAsync(1, 10).ConfigureAwait(false);

        // Assert
        payRolls.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be("/v1/payrolls");
            req.Method.Should().Be(HttpMethod.Get);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.AddPayRollAsync"/>は、"/v1/payrolls"にPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.AddPayRollAsync)} > POST /v1/payrolls をコールする。")]
    public async Task AddPayRollAsync_Calls_PostApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();
        string nameForAdmin = GenerateRandomString();
        string nameForCrew = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayRollResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.AddPayRollAsync(
            PaymentType.Salary,
            new(2021, 11, 1),
            new(2021, 10, 1),
            new(2021, 10, 31),
            NumeralSystemType.AsIs,
            nameForAdmin,
            nameForCrew
        ).ConfigureAwait(false);

        // Assert
        payRoll.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be("/v1/payrolls");
            req.Method.Should().Be(HttpMethod.Post);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);

            string receivedParameters = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedParameters.Should().Be(
                "payment_type=salary"
                + "&paid_at=2021-11-01"
                + "&period_start_at=2021-10-01"
                + "&period_end_at=2021-10-31"
                + "&numeral_system_handle_type=as_is"
                + $"&name_for_admin={nameForAdmin}"
                + $"&name_for_crew={nameForCrew}"
            );
            return true;
        }, Times.Once());
    }
    #endregion
}
