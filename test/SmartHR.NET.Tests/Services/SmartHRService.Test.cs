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

    #region Common
    /// <summary>APIのサンプルエラーレスポンスJSON(アクセストークンが無効)</summary>
    private const string UnauthorizedTokenJson = "{"
        + "\"code\": 2,"
        + "\"type\": \"unauthorized_token\","
        + "\"message\": \"無効なアクセストークンです\","
        + "\"errors\": null"
        + "}";
    /// <summary>APIのサンプルエラーレスポンスJSON(リクエストパラメータが不正)</summary>
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
        var request = new HttpRequestMessage(HttpMethod.Get, "/v1");

        // Act
        var sut = CreateSut(handler, "");
        var action = async () => await sut.CallApiAsync<string>(request, default).ConfigureAwait(false);

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
        var request = new HttpRequestMessage(HttpMethod.Get, "/v1");

        // Act
        var sut = CreateSut(handler, "");
        var action = async () => await sut.CallApiAsync<string>(request, default).ConfigureAwait(false);

        // Assert
        await action.Should().ThrowExactlyAsync<HttpRequestException>().ConfigureAwait(false);
    }

    /// <summary>
    /// リストを返すAPIで不正なページ数/要素数を指定したとき、ArgumentOutOfRangeExceptionの例外をスローする。
    /// </summary>
    /// <param name="page">ページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > List API Caller > ArgumentOutOfRangeExceptionをスローする。")]
    public async Task ListApiCaller_Throws_ArgumentOutOfRangeException(int page, int perPage)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler, "");
        var action = async () => await sut.FetchListAsync<string>("/v1", page, perPage, default).ConfigureAwait(false);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>().ConfigureAwait(false);
        handler.VerifyAnyRequest(Times.Never());
    }
    #endregion

    #region DependentRelations
    private const string DependentRelationResponseJson = "{"
    + "\"id\":\"9ba6128d-9a8a-4b6a-9464-03907dc37e71\","
    + "\"name\":\"妻\","
    + "\"preset_type\":\"wife\","
    + "\"is_child\":false,"
    + "\"position\":1,"
    + "\"created_at\":\"2021-09-24T17:22:12.426Z\","
    + "\"updated_at\":\"2021-09-24T17:22:12.426Z\""
    + "}";

    /// <summary>
    /// <see cref="SmartHRService.FetchDependentRelationListAsync"/>は、"/v1/dependent_relations"にGETリクエストを行う。
    /// </summary>
    [InlineData(true, "/v1/dependent_relations?filter=spouse&page=1&per_page=10")]
    [InlineData(false, "/v1/dependent_relations?page=1&per_page=10")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchDependentRelationListAsync)} > GET /v1/dependent_relations をコールする。")]
    public async Task FetchDependentRelationListAsync_Calls_GetApi(bool spouse, string expected)
    {
        // Arrange
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{DependentRelationResponseJson}]", "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var entities = await sut.FetchDependentRelationListAsync(spouse, 1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.PathAndQuery.Should().Be(expected);
            req.Method.Should().Be(HttpMethod.Get);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }
    #endregion

    #region PaymentPeriods
    /// <summary>給与支給形態APIのサンプルレスポンスJSON</summary>
    private const string PaymentPeriodResponseJson = "{"
    + "\"id\":\"id\","
    + "\"name\":\"name\","
    + "\"period_type\":\"monthly\","
    + "\"updated_at\":\"2021-10-06T00:24:48.897Z\","
    + "\"created_at\":\"2021-10-06T00:24:48.897Z\""
    + "}";

    /// <summary>
    /// <see cref="SmartHRService.FetchPaymentPeriodAsync"/>は、"/v1/payment_periods/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPaymentPeriodAsync)} > GET /v1/payment_periods/:id をコールする。")]
    public async Task FetchPaymentPeriodAsync_Calls_GetApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PaymentPeriodResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var jobTitle = await sut.FetchPaymentPeriodAsync(id).ConfigureAwait(false);

        // Assert
        jobTitle.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/payment_periods/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchPaymentPeriodListAsync"/>は、"/v1/payment_periods"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPaymentPeriodListAsync)} > GET /v1/payment_periods をコールする。")]
    public async Task FetchPaymentPeriodListAsync_Calls_GetApi()
    {
        // Arrange
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{PaymentPeriodResponseJson}]", "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var jobTitles = await sut.FetchPaymentPeriodListAsync(1, 10).ConfigureAwait(false);

        // Assert
        jobTitles.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be("/v1/payment_periods");
            req.Method.Should().Be(HttpMethod.Get);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }
    #endregion

    #region JobTitles
    /// <summary>役職APIのサンプルレスポンスJSON</summary>
    private const string JobTitleResponseJson = "{"
        + "\"id\":\"id\","
        + "\"name\":\"name\","
        + "\"rank\":1,"
        + "\"created_at\":\"2021-10-06T00:24:48.910Z\","
        + "\"updated_at\":\"2021-10-06T00:24:48.910Z\""
        + "}";

    /// <summary>
    /// <see cref="SmartHRService.DeleteJobTitleAsync"/>は、"/v1/job_titles/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeleteJobTitleAsync)} > DELETE /v1/job_titles/:id をコールする。")]
    public async Task DeleteJobTitleAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler, accessToken);
        await sut.DeleteJobTitleAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/job_titles/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchJobTitleAsync"/>は、"/v1/job_titles/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchJobTitleAsync)} > GET /v1/job_titles/:id をコールする。")]
    public async Task FetchJobTitleAsync_Calls_GetApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(JobTitleResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var jobTitle = await sut.FetchJobTitleAsync(id).ConfigureAwait(false);

        // Assert
        jobTitle.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/job_titles/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UpdateJobTitleAsync"/>は、"/v1/job_titles/{id}"にPATCHリクエストを行う。
    /// </summary>
    /// <param name="name">役職名</param>
    /// <param name="rank">役職のランク</param>
    /// <param name="expected">サーバー側が受け取るパラメータ</param>
    [InlineData(null, null, "")]
    [InlineData("name1", null, "name=name1")]
    [InlineData(null, 1, "rank=1")]
    [InlineData("name1", 1, "name=name1&rank=1")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UpdateJobTitleAsync)} > PATCH /v1/job_titles/:id/publish をコールする。")]
    public async Task UpdateJobTitleAsync_Calls_PatchApi(string? name, int? rank, string expected)
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(JobTitleResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.UpdateJobTitleAsync(id, name, rank).ConfigureAwait(false);

        // Assert
        payRoll.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/job_titles/{id}");
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
    /// <see cref="SmartHRService.ReplaceJobTitleAsync"/>は、"/v1/job_titles/{id}"にPUTリクエストを行う。
    /// </summary>
    /// <param name="rank">役職のランク</param>
    /// <param name="expected">サーバー側が受け取るパラメータ</param>
    [InlineData(null, "")]
    [InlineData(1, "&rank=1")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.ReplaceJobTitleAsync)} > PUT /v1/job_titles/:id をコールする。")]
    public async Task ReplaceJobTitleAsync_Calls_PutApi(int? rank, string expected)
    {
        // Arrange
        string id = GenerateRandomString();
        string name = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(JobTitleResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.ReplaceJobTitleAsync(id, name, rank).ConfigureAwait(false);

        // Assert
        payRoll.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/job_titles/{id}");
            req.Method.Should().Be(HttpMethod.Put);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);

            string receivedParameters = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedParameters.Should().Be($"name={name}{expected}");
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchJobTitleListAsync"/>は、"/v1/job_titles"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchJobTitleListAsync)} > GET /v1/job_titles をコールする。")]
    public async Task FetchJobTitleListAsync_Calls_GetApi()
    {
        // Arrange
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{JobTitleResponseJson}]", "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var jobTitles = await sut.FetchJobTitleListAsync(1, 10).ConfigureAwait(false);

        // Assert
        jobTitles.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be("/v1/job_titles");
            req.Method.Should().Be(HttpMethod.Get);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.AddJobTitleAsync"/>は、"/v1/job_titles"にPOSTリクエストを行う。
    /// </summary>
    [InlineData(null, "")]
    [InlineData(1, "&rank=1")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.AddJobTitleAsync)} > POST /v1/job_titles をコールする。")]
    public async Task AddJobTitleAsync_Calls_PostApi(int? rank, string expected)
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();
        string name = GenerateRandomString();
        string nameForCrew = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(JobTitleResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var jobTitle = await sut.AddJobTitleAsync(name, rank).ConfigureAwait(false);

        // Assert
        jobTitle.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be("/v1/job_titles");
            req.Method.Should().Be(HttpMethod.Post);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);

            string receivedParameters = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedParameters.Should().Be($"name={name}{expected}");
            return true;
        }, Times.Once());
    }
    #endregion

    #region TaxWithholdings
    /// <summary>源泉徴収APIのサンプルレスポンスJSON</summary>
    private const string TaxWithholdingResponseJson = "{"
    + "\"id\":\"id\","
    + "\"name\":\"name\","
    + "\"status\":\"wip\","
    + "\"year\":\"H23\""
    + "}";

    /// <summary>
    /// <see cref="SmartHRService.DeleteTaxWithholdingAsync"/>は、"/v1/tax_withholdings/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeleteTaxWithholdingAsync)} > DELETE /v1/tax_withholdings/:id をコールする。")]
    public async Task DeleteTaxWithholdingAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler, accessToken);
        await sut.DeleteTaxWithholdingAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/tax_withholdings/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchTaxWithholdingAsync"/>は、"/v1/tax_withholdings/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchTaxWithholdingAsync)} > GET /v1/tax_withholdings/:id をコールする。")]
    public async Task FetchTaxWithholdingAsync_Calls_GetApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(TaxWithholdingResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var taxWithholding = await sut.FetchTaxWithholdingAsync(id).ConfigureAwait(false);

        // Assert
        taxWithholding.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/tax_withholdings/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UpdateTaxWithholdingAsync"/>は、"/v1/tax_withholdings/{id}"にPATCHリクエストを行う。
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="status">ステータス</param>
    /// <param name="year">源泉徴収票に印字される年</param>
    /// <param name="expected">サーバー側が受け取るパラメータ</param>
    [InlineData(null, null, null, "{}")]
    [InlineData("name", null, null, "{\"name\":\"name\"}")]
    [InlineData(null, TaxWithholding.FormStatus.Wip, null, "{\"status\":\"wip\"}")]
    [InlineData(null, null, "R04", "{\"year\":\"R04\"}")]
    [InlineData("name", TaxWithholding.FormStatus.Fixed, "R04", "{\"name\":\"name\",\"status\":\"fixed\",\"year\":\"R04\"}")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UpdateTaxWithholdingAsync)} > PATCH /v1/tax_withholdings/:id をコールする。")]
    public async Task UpdateTaxWithholdingAsync_Calls_PatchApi(string? name, TaxWithholding.FormStatus? status, string? year, string expected)
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(TaxWithholdingResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.UpdateTaxWithholdingAsync(id, name, status, year).ConfigureAwait(false);

        // Assert
        payRoll.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/tax_withholdings/{id}");
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
    /// <see cref="SmartHRService.ReplaceTaxWithholdingAsync"/>は、"/v1/tax_withholdings/{id}"にPUTリクエストを行う。
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="status">ステータス</param>
    /// <param name="year">源泉徴収票に印字される年</param>
    /// <param name="expected">サーバー側が受け取るパラメータ</param>
    [InlineData("name", TaxWithholding.FormStatus.Wip, "R04", "{\"name\":\"name\",\"status\":\"wip\",\"year\":\"R04\"}")]
    [InlineData("name", TaxWithholding.FormStatus.Fixed, "H24", "{\"name\":\"name\",\"status\":\"fixed\",\"year\":\"H24\"}")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.ReplaceTaxWithholdingAsync)} > PUT /v1/tax_withholdings/:id をコールする。")]
    public async Task ReplaceTaxWithholdingAsync_Calls_PutApi(string name, TaxWithholding.FormStatus status, string year, string expected)
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(TaxWithholdingResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.ReplaceTaxWithholdingAsync(id, name, status, year).ConfigureAwait(false);

        // Assert
        payRoll.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be($"/v1/tax_withholdings/{id}");
            req.Method.Should().Be(HttpMethod.Put);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);

            string receivedParameters = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedParameters.Should().Be(expected);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchTaxWithholdingListAsync"/>は、"/v1/tax_withholdings"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchTaxWithholdingListAsync)} > GET /v1/tax_withholdings をコールする。")]
    public async Task FetchTaxWithholdingListAsync_Calls_GetApi()
    {
        // Arrange
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{TaxWithholdingResponseJson}]", "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var jobTitles = await sut.FetchTaxWithholdingListAsync(1, 10).ConfigureAwait(false);

        // Assert
        jobTitles.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be("/v1/tax_withholdings");
            req.Method.Should().Be(HttpMethod.Get);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.AddTaxWithholdingAsync"/>は、"/v1/tax_withholdings"にPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.AddTaxWithholdingAsync)} > POST /v1/tax_withholdings をコールする。")]
    public async Task AddTaxWithholdingAsync_Calls_PostApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();
        string name = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(TaxWithholdingResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var jobTitle = await sut.AddTaxWithholdingAsync(name, "R03").ConfigureAwait(false);

        // Assert
        jobTitle.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.AbsolutePath.Should().Be("/v1/tax_withholdings");
            req.Method.Should().Be(HttpMethod.Post);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);

            string receivedParameters = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedParameters.Should().Be($"{{\"name\":\"{name}\",\"year\":\"R03\"}}");
            return true;
        }, Times.Once());
    }
    #endregion

    #region Payrolls
    /// <summary>給与APIのサンプルレスポンスJSON</summary>
    private const string PayrollResponseJson = "{"
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
    /// <see cref="SmartHRService.DeletePayrollAsync"/>は、"/v1/payrolls/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeletePayrollAsync)} > DELETE /v1/payrolls/:id をコールする。")]
    public async Task DeletePayrollAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler, accessToken);
        await sut.DeletePayrollAsync(id).ConfigureAwait(false);

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
    /// <see cref="SmartHRService.FetchPayrollAsync"/>は、"/v1/payrolls/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPayrollAsync)} > GET /v1/payrolls/:id をコールする。")]
    public async Task FetchPayrollAsync_Calls_GetApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayrollResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.FetchPayrollAsync(id).ConfigureAwait(false);

        // Assert
        payRoll.Should().Be(new Payroll(
            "string",
            Payroll.Payment.Salary,
            new(2021, 9, 30),
            new(2021, 9, 30),
            new(2021, 9, 30),
            "api",
            Payroll.SalaryStatus.Wip,
            default,
            true,
            Payroll.NumeralSystem.AsIs,
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
    /// <see cref="SmartHRService.UpdatePayrollAsync"/>は、"/v1/payrolls/{id}"にPATCHリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.PublishPayrollAsync)} > PATCH /v1/payrolls/:id/publish をコールする。")]
    public async Task UpdatePayrollAsync_Calls_PatchApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();
        string nameForAdmin = GenerateRandomString();
        string nameForCrew = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayrollResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.UpdatePayrollAsync(id, nameForAdmin, nameForCrew).ConfigureAwait(false);

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
    /// <see cref="SmartHRService.PublishPayrollAsync"/>は、"/v1/payrolls/{id}/publish"にPATCHリクエストを行う。
    /// </summary>
    /// <param name="publishedAt">公開時刻</param>
    /// <param name="notifyWithPublish">公開と同時に通知を行う</param>
    /// <param name="expected">サーバー側が受け取るパラメータ</param>
    [InlineData(null, null, "")]
    [InlineData(null, false, "notify_with_publish=false")]
    [InlineData("2021-09-30T00:00:00Z", true, "published_at=2021-09-30T00%3A00%3A00.0000000%2B00%3A00&notify_with_publish=true")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.PublishPayrollAsync)} > PATCH /v1/payrolls/:id/publish をコールする。")]
    public async Task PublishPayrollAsync_Calls_PatchApi(string publishedAt, bool? notifyWithPublish, string expected)
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayrollResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.PublishPayrollAsync(
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
    /// <see cref="SmartHRService.UnconfirmPayrollAsync"/>は、"/v1/payrolls/{id}/unfix"にPATCHリクエストを行う。
    /// </summary>
    /// <param name="publishedAt">公開時刻</param>
    /// <param name="notifyWithPublish">公開と同時に通知を行う</param>
    /// <param name="expected">サーバー側が受け取るパラメータ</param>
    [InlineData(null, null, "")]
    [InlineData(null, false, "&notify_with_publish=false")]
    [InlineData("2021-09-30T00:00:00Z", true, "&published_at=2021-09-30T00%3A00%3A00.0000000%2B00%3A00&notify_with_publish=true")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UnconfirmPayrollAsync)} > PATCH /v1/payrolls/:id/unfix をコールする。")]
    public async Task UnconfirmPayrollAsync_Calls_PatchApi(string publishedAt, bool? notifyWithPublish, string expected)
    {
        // Arrange
        string id = GenerateRandomString();
        string nameForAdmin = GenerateRandomString();
        string nameForCrew = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayrollResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.UnconfirmPayrollAsync(
            id,
            Payroll.Payment.Salary,
            new(2021, 11, 1),
            new(2021, 10, 1),
            new(2021, 10, 31),
            Payroll.SalaryStatus.Wip,
            Payroll.NumeralSystem.AsIs,
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
    /// <see cref="SmartHRService.ConfirmPayrollAsync"/>は、"/v1/payrolls/{id}/fix"にPATCHリクエストを行う。
    /// </summary>
    /// <param name="publishedAt">公開時刻</param>
    /// <param name="notifyWithPublish">公開と同時に通知を行う</param>
    /// <param name="expected">サーバー側が受け取るパラメータ</param>
    [InlineData(null, null, "")]
    [InlineData(null, false, "&notify_with_publish=false")]
    [InlineData("2021-09-30T00:00:00Z", true, "&published_at=2021-09-30T00%3A00%3A00.0000000%2B00%3A00&notify_with_publish=true")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.ConfirmPayrollAsync)} > PATCH /v1/payrolls/:id/fix をコールする。")]
    public async Task ConfirmPayrollAsync_Calls_PatchApi(string publishedAt, bool? notifyWithPublish, string expected)
    {
        // Arrange
        string id = GenerateRandomString();
        string nameForAdmin = GenerateRandomString();
        string nameForCrew = GenerateRandomString();
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayrollResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.ConfirmPayrollAsync(
            id,
            Payroll.Payment.Salary,
            new(2021, 11, 1),
            new(2021, 10, 1),
            new(2021, 10, 31),
            Payroll.SalaryStatus.Wip,
            Payroll.NumeralSystem.AsIs,
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
    /// <see cref="SmartHRService.FetchPayrollListAsync"/>は、"/v1/payrolls"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPayrollListAsync)} > GET /v1/payrolls をコールする。")]
    public async Task FetchPayrollListAsync_Calls_GetApi()
    {
        // Arrange
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{PayrollResponseJson}]", "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRolls = await sut.FetchPayrollListAsync(1, 10).ConfigureAwait(false);

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
    /// <see cref="SmartHRService.AddPayrollAsync"/>は、"/v1/payrolls"にPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.AddPayrollAsync)} > POST /v1/payrolls をコールする。")]
    public async Task AddPayrollAsync_Calls_PostApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string accessToken = GenerateRandomString();
        string nameForAdmin = GenerateRandomString();
        string nameForCrew = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayrollResponseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken);
        var payRoll = await sut.AddPayrollAsync(
            Payroll.Payment.Salary,
            new(2021, 11, 1),
            new(2021, 10, 1),
            new(2021, 10, 31),
            Payroll.NumeralSystem.AsIs,
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
    /// <summary>給与明細APIのサンプルレスポンスJSON</summary>
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
