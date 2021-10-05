using System.Collections.Generic;
using System.Globalization;
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

    #region API Error
    private const string UnauthorizedTokenJson = "{"
        + "\"code\": 2,"
        + "\"type\": \"unauthorized_token\","
        + "\"message\": \"無効なアクセストークンです\","
        + "\"errors\": null"
        + "}";
    private const string BadRequestJson = "{"
        + "\"code\": 1,"
        + "\"type\": \"bad_request\","
        + "\"message\": \"不正なリクエストパラメータです\","
        + "\"errors\": ["
        + "  {"
        + "    \"message\": \"給与が確定されていません\","
        + "    \"resource\": null,"
        + "    \"field\": null"
        + "  }"
        + "]"
        + "}";

    /// <summary>
    /// APIがエラーレスポンスを返したとき、APIを呼び出すメソッドは<see cref="ApiFailedException"/>をスローする。
    /// </summary>
    [InlineData(UnauthorizedTokenJson, "無効なアクセストークンです")]
    [InlineData(BadRequestJson, "不正なリクエストパラメータです")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > API Caller > {nameof(ApiFailedException)}をスローする。")]
    public async Task ApiCaller_Throws_ApiFailedException(string json, string message)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.BadRequest, json, "application/json");

        // Act
        var sut = CreateSut(handler, "");
        var action = async () => await sut.DeletePayRollAsync("").ConfigureAwait(false);

        // Assert
        (await action.Should().ThrowExactlyAsync<ApiFailedException>().ConfigureAwait(false))
            .WithMessage(message)
            .WithInnerExceptionExactly<HttpRequestException>();
    }

    /// <summary>
    /// APIが不明なエラーを返したとき、APIを呼び出すメソッドは<see cref="HttpRequestException"/>をスローする。
    /// </summary>
    /// <param name="body">エラーメッセージ</param>
    [InlineData("")]
    [InlineData("null")]
    [InlineData("Not Found")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > API Caller > HttpRequestExceptionをスローする。")]
    public async Task ApiCaller_Throws_HttpRequestException(string body)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.BadRequest, body);

        // Act
        var sut = CreateSut(handler, "");
        var action = async () => await sut.DeletePayRollAsync("").ConfigureAwait(false);

        // Assert
        await action.Should().ThrowExactlyAsync<HttpRequestException>().ConfigureAwait(false);
    }
    #endregion

    #region PayRolls
    private const string PayRollResponseJson = "{"
        + "\"id\": \"string\","
        + "\"payment_type\": \"salary\","
        + "\"paid_at\": \"2021-09-30\","
        + "\"period_start_at\": \"2021-09-30\","
        + "\"period_end_at\": \"2021-09-30\","
        + "\"source_type\": \"api\","
        + "\"status\": \"wip\","
        + "\"published_at\": null,"
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
    [InlineData("2021-09-30T00:00:00Z", true, "published_at=2021-09-30T00%3A00%3A00.0000000%2B00%3A00&notify_with_publish=true")]
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
            publishedAt is not null ? DateTimeOffset.Parse(publishedAt, CultureInfo.InvariantCulture) : null,
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
    [InlineData("2021-09-30T00:00:00Z", true, "&published_at=2021-09-30T00%3A00%3A00.0000000%2B00%3A00&notify_with_publish=true")]
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
            publishedAt is not null ? DateTimeOffset.Parse(publishedAt, CultureInfo.InvariantCulture) : null,
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
    [InlineData("2021-09-30T00:00:00Z", true, "&published_at=2021-09-30T00%3A00%3A00.0000000%2B00%3A00&notify_with_publish=true")]
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
            publishedAt is not null ? DateTimeOffset.Parse(publishedAt, CultureInfo.InvariantCulture) : null,
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

    #region Payslips
    private const string PayslipResponseJson = "{"
        + "  \"id\": \"id\","
        + "  \"crew_id\": \"crew_id\","
        + "  \"payroll_id\": \"payroll_id\","
        + "  \"memo\": \"memo\","
        + "  \"mismatch\": true,"
        + "  \"last_notified_at\": \"2021-10-01T00:00:00Z\","
        + "  \"allowances\": ["
        + "    {"
        + "      \"name\": \"allowances\","
        + "      \"amount\": 200000"
        + "    }"
        + "  ],"
        + "  \"deductions\": ["
        + "    {"
        + "      \"name\": \"deductions\","
        + "      \"amount\": 50000"
        + "    }"
        + "  ],"
        + "  \"attendances\": ["
        + "    {"
        + "      \"name\": \"attendances\","
        + "      \"amount\": 160,"
        + "      \"unit_type\": \"hours\","
        + "      \"numeral_system_type\": \"decimal\","
        + "      \"delimiter_type\": \"period\""
        + "    }"
        + "  ],"
        + "  \"payroll_aggregates\": ["
        + "    {"
        + "      \"name\": \"payroll_aggregates\","
        + "      \"aggregate_type\": \"net_payment\","
        + "      \"amount\": 100,"
        + "      \"value\": \"string\""
        + "    }"
        + "  ]"
        + "}";

    /// <summary>
    /// <see cref="SmartHRService.DeletePayslipAsync"/>は、"/v1/payrolls/{payrollId}/payslip/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeletePayslipAsync)} > DELETE /v1/payrolls/:payrollId/payslips/:id をコールする。")]
    public async Task DeletePayslipAsync_Calls_DeleteApi()
    {
        // Arrange
        string payrollId = GenerateRandomString();
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler, accessToken);
        await sut.DeletePayslipAsync(payrollId, id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/payrolls/{payrollId}/payslips/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchPayslipAsync"/>は、"/v1/payrolls/{payrollId}/payslips/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPayslipAsync)} > GET /v1/payrolls/:payrollId/payslips/:id をコールする。")]
    public async Task FetchPayslipAsync_Calls_GetApi()
    {
        // Arrange
        string payrollId = GenerateRandomString();
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayslipResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payslip = await sut.FetchPayslipAsync(payrollId, id).ConfigureAwait(false);

        // Assert
        payslip.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/payrolls/{payrollId}/payslips/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchPayslipListAsync"/>は、"/v1/payrolls/{payrollId}/payslips"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPayslipListAsync)} > GET /v1/payrolls/:payrollId/payslips をコールする。")]
    public async Task FetchPayslipListAsync_Calls_GetApi()
    {
        // Arrange
        string accessToken = GenerateRandomString();
        string payrollId = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{PayslipResponseJson}]", "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payslips = await sut.FetchPayslipListAsync(payrollId).ConfigureAwait(false);

        // Assert
        payslips.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/payrolls/{payrollId}/payslips");
            req.Method.Should().Be(HttpMethod.Get);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.AddPayslipListAsync"/>は、"/v1/payrolls/{payrollId}/payslips/bulk"にPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPayslipListAsync)} > POST /v1/payrolls/:payrollId/payslips/bulk をコールする。")]
    public async Task AddPayslipListAsync_Calls_PostApi()
    {
        // Arrange
        string accessToken = GenerateRandomString();
        string payrollId = GenerateRandomString();
        var request = new PayslipRequest("従業員ID", new Dictionary<string, string>()
        {
            { "支給項目1", "10000" },
            { "支給項目2", "100000" },
            { "支給項目3", "200000" },
            { "控除項目1", "3000" },
            { "控除項目2", "30000" },
            { "控除項目3", "4000" },
            { "勤怠項目1", "160" },
            { "勤怠項目2", "20" },
            { "勤怠項目3", "10" },
            { "合計項目1", "9000000" },
            { "合計項目2", "5000000" },
            { "合計項目3", "4000000" }
        }.ToArray(), "string");

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayslipResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payslip = await sut.AddPayslipListAsync(payrollId, new[] { request }).ConfigureAwait(false);

        // Assert
        payslip.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/payrolls/{payrollId}/payslips/bulk");
            req.Method.Should().Be(HttpMethod.Post);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            string jsonString = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            jsonString.Should().Be("[{"
            + "\"crew_id\":\"従業員ID\","
            + "\"values\":["
            + "{\"key\":\"支給項目1\",\"value\":\"10000\"},"
            + "{\"key\":\"支給項目2\",\"value\":\"100000\"},"
            + "{\"key\":\"支給項目3\",\"value\":\"200000\"},"
            + "{\"key\":\"控除項目1\",\"value\":\"3000\"},"
            + "{\"key\":\"控除項目2\",\"value\":\"30000\"},"
            + "{\"key\":\"控除項目3\",\"value\":\"4000\"},"
            + "{\"key\":\"勤怠項目1\",\"value\":\"160\"},"
            + "{\"key\":\"勤怠項目2\",\"value\":\"20\"},"
            + "{\"key\":\"勤怠項目3\",\"value\":\"10\"},"
            + "{\"key\":\"合計項目1\",\"value\":\"9000000\"},"
            + "{\"key\":\"合計項目2\",\"value\":\"5000000\"},"
            + "{\"key\":\"合計項目3\",\"value\":\"4000000\"}],"
            + "\"memo\":\"string\"}]");
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// ページ数が不正なとき、<see cref="SmartHRService.FetchPayslipListAsync"/>は、ArgumentOutOfRangeExceptionをスローする。
    /// </summary>
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPayslipListAsync)} > ArgumentOutOfRangeException をスローする。")]
    public async Task FetchPayslipListAsync_Throws_ArgumentOutOfRangeException(int page, int perPage)
    {
        // Arrange
        string payrollId = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest((_) => true)
            .ReturnsResponse($"[{PayslipResponseJson}]", "application/json");

        // Act
        var sut = CreateSut(handler, "");
        var action = async () => _ = await sut.FetchPayslipListAsync(payrollId, page, perPage).ConfigureAwait(false);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>().ConfigureAwait(false);
        handler.VerifyRequest((_) => true, Times.Never());
    }

    /// <summary>
    /// <see cref="SmartHRService.AddPayslipAsync"/>は、"/v1/payrolls/{payrollId}/payslips"にPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPayslipListAsync)} > POST /v1/payrolls/:payrollId/payslips をコールする。")]
    public async Task AddPayslipAsync_Calls_PostApi()
    {
        // Arrange
        string accessToken = GenerateRandomString();
        string payrollId = GenerateRandomString();
        var request = new PayslipRequest("従業員ID", new Dictionary<string, string>()
        {
            { "支給項目1", "10000" },
            { "支給項目2", "100000" },
            { "支給項目3", "200000" },
            { "控除項目1", "3000" },
            { "控除項目2", "30000" },
            { "控除項目3", "4000" },
            { "勤怠項目1", "160" },
            { "勤怠項目2", "20" },
            { "勤怠項目3", "10" },
            { "合計項目1", "9000000" },
            { "合計項目2", "5000000" },
            { "合計項目3", "4000000" }
        }.ToArray(), "string");

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayslipResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payslip = await sut.AddPayslipAsync(payrollId, request).ConfigureAwait(false);

        // Assert
        payslip.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/payrolls/{payrollId}/payslips");
            req.Method.Should().Be(HttpMethod.Post);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            string jsonString = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            jsonString.Should().Be("{"
            + "\"crew_id\":\"従業員ID\","
            + "\"values\":["
            + "{\"key\":\"支給項目1\",\"value\":\"10000\"},"
            + "{\"key\":\"支給項目2\",\"value\":\"100000\"},"
            + "{\"key\":\"支給項目3\",\"value\":\"200000\"},"
            + "{\"key\":\"控除項目1\",\"value\":\"3000\"},"
            + "{\"key\":\"控除項目2\",\"value\":\"30000\"},"
            + "{\"key\":\"控除項目3\",\"value\":\"4000\"},"
            + "{\"key\":\"勤怠項目1\",\"value\":\"160\"},"
            + "{\"key\":\"勤怠項目2\",\"value\":\"20\"},"
            + "{\"key\":\"勤怠項目3\",\"value\":\"10\"},"
            + "{\"key\":\"合計項目1\",\"value\":\"9000000\"},"
            + "{\"key\":\"合計項目2\",\"value\":\"5000000\"},"
            + "{\"key\":\"合計項目3\",\"value\":\"4000000\"}],"
            + "\"memo\":\"string\"}");
            return true;
        }, Times.Once());
    }
    #endregion
}
