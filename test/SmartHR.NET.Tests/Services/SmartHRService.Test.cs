using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using SmartHR.NET.Entities;
using SmartHR.NET.Services;
using SmartHR.NET.Tests.Entities;

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
    private static SmartHRService CreateSut(Mock<HttpMessageHandler> handler, string accessToken = "")
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
        string accessToken = GenerateRandomString();
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.BadRequest, json, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Get, "/v1");

        // Act
        var sut = CreateSut(handler, accessToken);
        var action = async () => await sut.CallApiAsync<string>(request, default).ConfigureAwait(false);

        // Assert
        (await action.Should().ThrowExactlyAsync<ApiFailedException>().ConfigureAwait(false))
            .WithMessage(message)
            .WithInnerExceptionExactly<HttpRequestException>();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
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
        string accessToken = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.BadRequest, body);
        var request = new HttpRequestMessage(HttpMethod.Get, "/v1");

        // Act
        var sut = CreateSut(handler, accessToken);
        var action = async () => await sut.CallApiAsync<string>(request, default).ConfigureAwait(false);

        // Assert
        await action.Should().ThrowExactlyAsync<HttpRequestException>().ConfigureAwait(false);
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.Headers.Authorization.Should().NotBeNull();
            req.Headers.Authorization!.Scheme.Should().Be("Bearer");
            req.Headers.Authorization!.Parameter.Should().Be(accessToken);
            return true;
        }, Times.Once());
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

    #region Crews
    /// <summary>
    /// <see cref="SmartHRService.InviteCrewAsync"/>は、"/v1/crews/{id}"にPUTリクエストを行う。
    /// </summary>
    [InlineData(null, "")]
    [InlineData("foo", "&crew_input_form_id=foo")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.InviteCrewAsync)} > PUT /v1/crews/:id をコールする。")]
    public async Task InviteCrewAsync_Calls_PutApi(string? crewInputFormId, string expected)
    {
        // Arrange
        string id = GenerateRandomString();
        string inviterCrewId = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.InviteCrewAsync(id, inviterCrewId, crewInputFormId).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crews/{id}/invite");
            req.Method.Should().Be(HttpMethod.Put);

            string receivedParameters = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedParameters.Should().Be($"inviter_crew_id={inviterCrewId}{expected}");
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.DeleteCrewAsync"/>は、"/v1/crews/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeleteCrewAsync)} > DELETE /v1/crews/:id をコールする。")]
    public async Task DeleteCrewAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.DeleteCrewAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crews/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchCrewAsync"/>は、"/v1/crews/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchCrewAsync)} > GET /v1/crews/:id をコールする。")]
    public async Task FetchCrewAsync_Calls_GetApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(CrewTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchCrewAsync(id).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crews/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UpdateCrewAsync"/>は、"/v1/crews/{id}"にPATCHリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UpdateCrewAsync)} > PATCH /v1/crews/:id をコールする。")]
    public async Task UpdateCrewAsync_Calls_PatchApi()
    {
        // Arrange
        string id = GenerateRandomString();
        var element = JsonSerializer.Deserialize<JsonElement>(CrewTest.Json);

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(CrewTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.UpdateCrewAsync(id, element).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crews/{id}");
            req.Method.Should().Be(HttpMethod.Patch);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().NotBeNullOrEmpty();
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchCrewListAsync"/>は、"/v1/crews"にGETリクエストを行う。
    /// </summary>
    [InlineData(null, null, null, null, null, null, null, null, null, "page=1&per_page=10")]
    [InlineData(
        "0000",
        "board_member",
        "employed",
        "male",
        "emp_code",
        "2021-04-01",
        "2021-09-01",
        "query",
        "field1,field2",
        "emp_code=0000&emp_type=board_member&emp_status=employed&gender=male&sort=emp_code&entered_at_from=2021-04-01&entered_at_to=2021-09-01&q=query&fields=field1%2cfield2&page=1&per_page=10")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchCrewListAsync)} > GET /v1/crews をコールする。")]
    public async Task FetchCrewListAsync_Calls_GetApi(
        string? empCode,
        string? empType,
        string? empStatus,
        string? gender,
        string? sort,
        string? from,
        string? to,
        string? query,
        string? field,
        string expected
    )
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{CrewTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchCrewListAsync(
            1,
            10,
            empCode,
            empType,
            empStatus,
            gender,
            sort,
            from is null ? null : DateTime.Parse(from),
            to is null ? null : DateTime.Parse(to),
            query,
            field?.Split(',')
        ).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crews?{expected}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.AddCrewAsync"/>は、"/v1/crews"にPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.AddCrewAsync)} > POST /v1/crews をコールする。")]
    public async Task AddCrewAsync_Calls_PostApi()
    {
        // Arrange
        var element = JsonSerializer.Deserialize<JsonElement>(CrewTest.Json);

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(CrewTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.AddCrewAsync(element).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/crews");
            req.Method.Should().Be(HttpMethod.Post);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().NotBeNullOrEmpty();
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.DeleteCrewDepartmentsAsync"/>は、"/v1/crews/{id}/departments"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeleteCrewDepartmentsAsync)} > DELETE /v1/crews/:id/departments をコールする。")]
    public async Task DeleteCrewDepartmentsAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.DeleteCrewDepartmentsAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crews/{id}/departments");
            req.Method.Should().Be(HttpMethod.Delete);
            return true;
        }, Times.Once());
    }
    #endregion

    #region Dependents
    /// <summary>
    /// <see cref="SmartHRService.DeleteDependentAsync"/>は、"/v1/crews/{crewId}/dependents/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeleteDependentAsync)} > DELETE /v1/crews/:crewId/dependents/:id をコールする。")]
    public async Task DeleteDependentAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string crewId = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.DeleteDependentAsync(crewId, id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crews/{crewId}/dependents/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchDependentAsync"/>は、"/v1/crews/{crewId}/dependents/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchDependentAsync)} > GET /v1/crews/:crewId/dependents/:id をコールする。")]
    public async Task FetchDependentAsync_Calls_GetApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string crewId = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(DependentTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchDependentAsync(crewId, id).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crews/{crewId}/dependents/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UpdateDependentAsync"/>は、"/v1/crews/{crewId}/dependents/{id}"にPATCHリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UpdateDependentAsync)} > PATCH /v1/crews/:crewId/dependents/:id をコールする。")]
    public async Task UpdateDependentAsync_Calls_PatchApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string crewId = GenerateRandomString();
        var element = JsonSerializer.Deserialize<JsonElement>(CrewTest.Json);

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(DependentTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.UpdateDependentAsync(crewId, id, element).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crews/{crewId}/dependents/{id}");
            req.Method.Should().Be(HttpMethod.Patch);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().NotBeNullOrEmpty();
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchDependentListAsync"/>は、"/v1/crews/{crewId}/dependents"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchDependentListAsync)} > GET /v1/crews/:crewId/dependents をコールする。")]
    public async Task FetchDependentListAsync_Calls_GetApi()
    {
        // Arrange
        string crewId = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{DependentTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchDependentListAsync(crewId, 1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crews/{crewId}/dependents?page=1&per_page=10");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.AddDependentAsync"/>は、"/v1/crews/{crewId}/dependents"にPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.AddDependentAsync)} > POST /v1/crews/:crewId/dependents をコールする。")]
    public async Task AddDependentAsync_Calls_PostApi()
    {
        // Arrange
        string crewId = GenerateRandomString();
        var element = JsonSerializer.Deserialize<JsonElement>(CrewTest.Json);

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(DependentTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.AddDependentAsync(crewId ,element).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crews/{crewId}/dependents");
            req.Method.Should().Be(HttpMethod.Post);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().NotBeNullOrEmpty();
            return true;
        }, Times.Once());
    }
    #endregion

    #region Users
    /// <summary>
    /// <see cref="SmartHRService.FetchUserAsync"/>は、"/v1/users/{id}"にGETリクエストを行う。
    /// </summary>
    /// <param name="includeCrewInfo">従業員情報を含めるか</param>
    /// <param name="expectedQuery">パラメータクエリ</param>
    [InlineData(true, "?embed=crew")]
    [InlineData(false, "")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchUserAsync)} > GET /v1/users/:id をコールする。")]
    public async Task FetchUserAsync_Calls_GetApi(bool includeCrewInfo, string expectedQuery)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(UserTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchUserAsync(id, includeCrewInfo).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/users/{id}{expectedQuery}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchUserListAsync"/>は、"/v1/users"にGETリクエストを行う。
    /// </summary>
    /// <param name="includeCrewInfo">従業員情報を含めるか</param>
    /// <param name="expectedQuery">パラメータクエリ</param>
    [InlineData(true, "embed=crew&page=1&per_page=10")]
    [InlineData(false, "page=1&per_page=10")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchUserListAsync)} > GET /v1/users をコールする。")]
    public async Task FetchUserListAsync_Calls_GetApi(bool includeCrewInfo, string expectedQuery)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{UserTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchUserListAsync(includeCrewInfo, 1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.PathAndQuery.Should().Be($"/v1/users?{expectedQuery}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }
    #endregion

    #region DependentRelations
    /// <summary>
    /// <see cref="SmartHRService.FetchDependentRelationListAsync"/>は、"/v1/dependent_relations"にGETリクエストを行う。
    /// </summary>
    [InlineData(true, "/v1/dependent_relations?filter=spouse&page=1&per_page=10")]
    [InlineData(false, "/v1/dependent_relations?page=1&per_page=10")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchDependentRelationListAsync)} > GET /v1/dependent_relations をコールする。")]
    public async Task FetchDependentRelationListAsync_Calls_GetApi(bool spouse, string expected)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{DependentRelationTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchDependentRelationListAsync(spouse, 1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.PathAndQuery.Should().Be(expected);
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }
    #endregion

    #region PaymentPeriods
    /// <summary>
    /// <see cref="SmartHRService.FetchPaymentPeriodAsync"/>は、"/v1/payment_periods/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchPaymentPeriodAsync)} > GET /v1/payment_periods/:id をコールする。")]
    public async Task FetchPaymentPeriodAsync_Calls_GetApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PaymentPeriodTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchPaymentPeriodAsync(id).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/payment_periods/{id}");
            req.Method.Should().Be(HttpMethod.Get);
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
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{PaymentPeriodTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchPaymentPeriodListAsync(1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/payment_periods?page=1&per_page=10");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }
    #endregion

    #region JobTitles
    /// <summary>
    /// <see cref="SmartHRService.DeleteJobTitleAsync"/>は、"/v1/job_titles/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeleteJobTitleAsync)} > DELETE /v1/job_titles/:id をコールする。")]
    public async Task DeleteJobTitleAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.DeleteJobTitleAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/job_titles/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
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

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(JobTitleTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchJobTitleAsync(id).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/job_titles/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <paramref name="rank"/>が範囲外のとき、<see cref="SmartHRService.UpdateJobTitleAsync"/>は、ArgumentOutOfRangeExceptionをスローする。
    /// </summary>
    /// <param name="rank">役職のランク</param>
    [InlineData(0)]
    [InlineData(100000)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UpdateJobTitleAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
    public async Task UpdateJobTitleAsync_Throws_ArgumentOutOfRangeException(int rank)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(JobTitleTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var action = async () => await sut.UpdateJobTitleAsync(id, rank: rank).ConfigureAwait(false);

        // Assert
        (await action.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>().ConfigureAwait(false))
            .WithParameterName(nameof(rank));
        handler.VerifyAnyRequest(Times.Never());
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
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UpdateJobTitleAsync)} > PATCH /v1/job_titles/:id をコールする。")]
    public async Task UpdateJobTitleAsync_Calls_PatchApi(string? name, int? rank, string expected)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(JobTitleTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.UpdateJobTitleAsync(id, name, rank).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/job_titles/{id}");
            req.Method.Should().Be(HttpMethod.Patch);

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

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(JobTitleTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.ReplaceJobTitleAsync(id, name, rank).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/job_titles/{id}");
            req.Method.Should().Be(HttpMethod.Put);

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
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{JobTitleTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchJobTitleListAsync(1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/job_titles?page=1&per_page=10");
            req.Method.Should().Be(HttpMethod.Get);
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
        string name = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(JobTitleTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.AddJobTitleAsync(name, rank).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/job_titles");
            req.Method.Should().Be(HttpMethod.Post);

            string receivedParameters = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedParameters.Should().Be($"name={name}{expected}");
            return true;
        }, Times.Once());
    }
    #endregion

    #region BankAccountSettings
    /// <summary>
    /// <see cref="SmartHRService.FetchBankAccountSettingListAsync"/>は、"/v1/bank_account_settings"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchBankAccountSettingListAsync)} > GET /v1/bank_account_settings をコールする。")]
    public async Task FetchBankAccountSettingListAsync_Calls_GetApi()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{BankAccountSettingTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchBankAccountSettingListAsync(1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.PathAndQuery.Should().Be("/v1/bank_account_settings?page=1&per_page=10");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }
    #endregion

    #region TaxWithholdings
    /// <summary>
    /// <see cref="SmartHRService.DeleteTaxWithholdingAsync"/>は、"/v1/tax_withholdings/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeleteTaxWithholdingAsync)} > DELETE /v1/tax_withholdings/:id をコールする。")]
    public async Task DeleteTaxWithholdingAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.DeleteTaxWithholdingAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/tax_withholdings/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
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

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(TaxWithholdingTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchTaxWithholdingAsync(id).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/tax_withholdings/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UpdateTaxWithholdingAsync"/>は、"/v1/tax_withholdings/{id}"にPATCHリクエストを行う。
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="status">ステータス</param>
    /// <param name="year">源泉徴収票に印字される年</param>
    /// <param name="expected">サーバー側が受け取るJSON</param>
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

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(TaxWithholdingTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.UpdateTaxWithholdingAsync(id, name, status, year).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.PathAndQuery.Should().Be($"/v1/tax_withholdings/{id}");
            req.Method.Should().Be(HttpMethod.Patch);

            string jsonString = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            jsonString.Should().Be(expected);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.ReplaceTaxWithholdingAsync"/>は、"/v1/tax_withholdings/{id}"にPUTリクエストを行う。
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="status">ステータス</param>
    /// <param name="year">源泉徴収票に印字される年</param>
    /// <param name="expected">サーバー側が受け取るJSON</param>
    [InlineData("name", TaxWithholding.FormStatus.Wip, "R04", "{\"name\":\"name\",\"status\":\"wip\",\"year\":\"R04\"}")]
    [InlineData("name", TaxWithholding.FormStatus.Fixed, "H24", "{\"name\":\"name\",\"status\":\"fixed\",\"year\":\"H24\"}")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.ReplaceTaxWithholdingAsync)} > PUT /v1/tax_withholdings/:id をコールする。")]
    public async Task ReplaceTaxWithholdingAsync_Calls_PutApi(string name, TaxWithholding.FormStatus status, string year, string expected)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(TaxWithholdingTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.ReplaceTaxWithholdingAsync(id, name, status, year).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/tax_withholdings/{id}");
            req.Method.Should().Be(HttpMethod.Put);

            string jsonString = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            jsonString.Should().Be(expected);
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
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{TaxWithholdingTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchTaxWithholdingListAsync(1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/tax_withholdings?page=1&per_page=10");
            req.Method.Should().Be(HttpMethod.Get);
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
        string name = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(TaxWithholdingTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.AddTaxWithholdingAsync(name, "R03").ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/tax_withholdings");
            req.Method.Should().Be(HttpMethod.Post);

            string jsonString = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            jsonString.Should().Be($"{{\"name\":\"{name}\",\"year\":\"R03\"}}");
            return true;
        }, Times.Once());
    }
    #endregion

    #region Payrolls
    /// <summary>
    /// <see cref="SmartHRService.DeletePayrollAsync"/>は、"/v1/payrolls/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeletePayrollAsync)} > DELETE /v1/payrolls/:id をコールする。")]
    public async Task DeletePayrollAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.DeletePayrollAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/payrolls/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
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

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayrollTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchPayrollAsync(id).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/payrolls/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UpdatePayrollAsync"/>は、"/v1/payrolls/{id}"にPATCHリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.PublishPayrollAsync)} > PATCH /v1/payrolls/:id をコールする。")]
    public async Task UpdatePayrollAsync_Calls_PatchApi()
    {
        // Arrange
        string id = GenerateRandomString();
        string nameForAdmin = GenerateRandomString();
        string nameForCrew = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayrollTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.UpdatePayrollAsync(id, nameForAdmin, nameForCrew).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/payrolls/{id}");
            req.Method.Should().Be(HttpMethod.Patch);

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

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayrollTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.PublishPayrollAsync(
            id,
            publishedAt is not null ? DateTimeOffset.Parse(publishedAt, CultureInfo.InvariantCulture) : null,
            notifyWithPublish
        ).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/payrolls/{id}/publish");
            req.Method.Should().Be(HttpMethod.Patch);

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

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayrollTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.UnconfirmPayrollAsync(
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
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/payrolls/{id}/unfix");
            req.Method.Should().Be(HttpMethod.Patch);

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

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayrollTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.ConfirmPayrollAsync(
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
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/payrolls/{id}/fix");
            req.Method.Should().Be(HttpMethod.Patch);

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
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{PayrollTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchPayrollListAsync(1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/payrolls?page=1&per_page=10");
            req.Method.Should().Be(HttpMethod.Get);
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
        string nameForAdmin = GenerateRandomString();
        string nameForCrew = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayrollTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.AddPayrollAsync(
            Payroll.Payment.Salary,
            new(2021, 11, 1),
            new(2021, 10, 1),
            new(2021, 10, 31),
            Payroll.NumeralSystem.AsIs,
            nameForAdmin,
            nameForCrew
        ).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/payrolls");
            req.Method.Should().Be(HttpMethod.Post);

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
    /// <summary>
    /// <see cref="SmartHRService.DeletePayslipAsync"/>は、"/v1/payrolls/{payrollId}/payslip/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeletePayslipAsync)} > DELETE /v1/payrolls/:payrollId/payslips/:id をコールする。")]
    public async Task DeletePayslipAsync_Calls_DeleteApi()
    {
        // Arrange
        string payrollId = GenerateRandomString();
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.DeletePayslipAsync(payrollId, id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/payrolls/{payrollId}/payslips/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
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

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(PayslipTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchPayslipAsync(payrollId, id).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/payrolls/{payrollId}/payslips/{id}");
            req.Method.Should().Be(HttpMethod.Get);
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
        string payrollId = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{PayslipTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchPayslipListAsync(payrollId).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/payrolls/{payrollId}/payslips?page=1&per_page=10");
            req.Method.Should().Be(HttpMethod.Get);
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
            .ReturnsResponse(PayslipTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.AddPayslipListAsync(payrollId, new[] { request }).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/payrolls/{payrollId}/payslips/bulk");
            req.Method.Should().Be(HttpMethod.Post);

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
            .ReturnsResponse(PayslipTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.AddPayslipAsync(payrollId, request).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/payrolls/{payrollId}/payslips");
            req.Method.Should().Be(HttpMethod.Post);

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
