using System.Collections.Generic;
using System.Globalization;
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

    #region 従業員カスタム項目グループ
    /// <summary>
    /// <see cref="SmartHRService.DeleteCrewCustomFieldTemplateGroupAsync"/>は、"/v1/crew_custom_field_template_groups/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeleteCrewCustomFieldTemplateGroupAsync)} > DELETE /v1/crew_custom_field_template_groups/:id をコールする。")]
    public async Task DeleteCrewCustomFieldTemplateGroupAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.DeleteCrewCustomFieldTemplateGroupAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crew_custom_field_template_groups/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchCrewCustomFieldTemplateGroupAsync"/>は、"/v1/crew_custom_field_template_groups/{id}"にGETリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.FetchCrewCustomFieldTemplateGroupAsync" path="/param"/>
    /// <param name="expectedQuery">サーバー側が受け取るパラメータクエリ</param>
    [InlineData(true, "?embed=templates")]
    [InlineData(false, "")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchCrewCustomFieldTemplateGroupAsync)} > GET /v1/crew_custom_field_template_groups/:id をコールする。")]
    public async Task FetchCrewCustomFieldTemplateGroupAsync_Calls_GetApi(bool includeTemplates, string expectedQuery)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(CrewCustomFieldTemplateGroupTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchCrewCustomFieldTemplateGroupAsync(id, includeTemplates).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crew_custom_field_template_groups/{id}{expectedQuery}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UpdateCrewCustomFieldTemplateGroupAsync"/>は、"/v1/crew_custom_field_template_groups/{id}"にPATCHリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.UpdateCrewCustomFieldTemplateGroupAsync" path="/param"/>
    /// <param name="expectedJson">サーバー側が受け取るリクエストボディ</param>
    [InlineData(null, null, null, "{}")]
    [InlineData("foo", null, null, "{\"name\":\"foo\"}")]
    [InlineData(null, 1, null, "{\"position\":1}")]
    [InlineData(null, null, CrewCustomFieldTemplateGroup.Accessibility.Hidden, "{\"access_type\":\"hidden\"}")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UpdateCrewCustomFieldTemplateGroupAsync)} > PATCH /v1/crew_custom_field_template_groups/:id をコールする。")]
    public async Task UpdateCrewCustomFieldTemplateGroupAsync_Calls_PatchApi(string? name, int? position, CrewCustomFieldTemplateGroup.Accessibility? accessType, string expectedJson)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(CrewCustomFieldTemplateGroupTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.UpdateCrewCustomFieldTemplateGroupAsync(id, name, position, accessType).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crew_custom_field_template_groups/{id}");
            req.Method.Should().Be(HttpMethod.Patch);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be(expectedJson);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.ReplaceCrewCustomFieldTemplateGroupAsync"/>は、"/v1/crew_custom_field_template_groups/{id}"にPUTリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.ReplaceCrewCustomFieldTemplateGroupAsync" path="/param"/>
    /// <param name="expectedJson">サーバー側が受け取るリクエストボディ</param>
    [InlineData(null, null, "{\"name\":\"test\"}")]
    [InlineData(1, null, "{\"name\":\"test\",\"position\":1}")]
    [InlineData(null, CrewCustomFieldTemplateGroup.Accessibility.Hidden, "{\"name\":\"test\",\"access_type\":\"hidden\"}")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.ReplaceCrewCustomFieldTemplateGroupAsync)} > PUT /v1/crew_custom_field_template_groups/:id をコールする。")]
    public async Task ReplaceCrewCustomFieldTemplateGroupAsync_Calls_PutApi(int? position, CrewCustomFieldTemplateGroup.Accessibility? accessType, string expectedJson)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(CrewCustomFieldTemplateGroupTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.ReplaceCrewCustomFieldTemplateGroupAsync(id, "test", position, accessType).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crew_custom_field_template_groups/{id}");
            req.Method.Should().Be(HttpMethod.Put);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be(expectedJson);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchCrewCustomFieldTemplateGroupListAsync"/>は、"/v1/crew_custom_field_template_groups"にGETリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.FetchCrewCustomFieldTemplateGroupListAsync" path="/param"/>
    /// <param name="expectedQuery">サーバー側が受け取るパラメータクエリ</param>
    [InlineData(true, "embed=templates&page=1&per_page=10")]
    [InlineData(false, "page=1&per_page=10")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchCrewCustomFieldTemplateGroupListAsync)} > GET /v1/crew_custom_field_template_groups をコールする。")]
    public async Task FetchCrewCustomFieldTemplateGroupListAsync_Calls_GetApi(bool includeTemplates, string expectedQuery)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{CrewCustomFieldTemplateGroupTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchCrewCustomFieldTemplateGroupListAsync(includeTemplates, 1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crew_custom_field_template_groups?{expectedQuery}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.AddCrewCustomFieldTemplateGroupAsync"/>は、"/v1/crew_custom_field_template_groups"にPOSTリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.AddCrewCustomFieldTemplateGroupAsync" path="/param"/>
    /// <param name="expectedJson">サーバー側が受け取るリクエストボディ</param>
    [InlineData(null, null, "{\"name\":\"test\"}")]
    [InlineData(1, null, "{\"name\":\"test\",\"position\":1}")]
    [InlineData(null, CrewCustomFieldTemplateGroup.Accessibility.Hidden, "{\"name\":\"test\",\"access_type\":\"hidden\"}")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.AddCrewCustomFieldTemplateGroupAsync)} > POST /v1/crew_custom_field_template_groups をコールする。")]
    public async Task AddCrewCustomFieldTemplateGroupAsync_Calls_PostApi(int? position, CrewCustomFieldTemplateGroup.Accessibility? accessType, string expectedJson)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(CrewCustomFieldTemplateGroupTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.AddCrewCustomFieldTemplateGroupAsync("test", position, accessType).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/crew_custom_field_template_groups");
            req.Method.Should().Be(HttpMethod.Post);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be(expectedJson);
            return true;
        }, Times.Once());
    }
    #endregion

    #region 従業員カスタム項目テンプレート
    /// <summary><inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/>APIのテストデータ</summary>
    public static object[][] CrewCustomFieldTemplateTestData => new[]
    {
        new object[]{ new CrewCustomFieldTemplatePayload(), "{}" },
        new object[]
        {
            new CrewCustomFieldTemplatePayload(
                Name: "身長",
                Type: CrewCustomFieldTemplate.Input.Decimal,
                GroupId: "group_1",
                Hint: "cm",
                Scale: 4,
                SeparatedByCommas: false
            ),
            "{\"name\":\"身長\","
            + "\"type\":\"decimal\","
            + "\"group_id\":\"group_1\","
            + "\"hint\":\"cm\","
            + "\"scale\":4,"
            + "\"separated_by_commas\":false"
            + "}"
        },
        new object[]
        {
            new CrewCustomFieldTemplatePayload(
                Name: "性別",
                Type: CrewCustomFieldTemplate.Input.Enum,
                Elements: new CrewCustomFieldTemplatePayload.Element[]
                {
                    new("element_1", "男性", Position: 1),
                    new(Name: "女性", Position: 2),
                },
                GroupId: "group_1",
                Position: 2
            ),
            "{\"name\":\"性別\","
            + "\"type\":\"enum\","
            + "\"elements\":["
            + "{\"id\":\"element_1\",\"name\":\"男性\",\"position\":1},"
            + "{\"name\":\"女性\",\"position\":2}"
            + "],"
            + "\"group_id\":\"group_1\","
            + "\"position\":2"
            + "}" },
        };

    /// <summary>
    /// <see cref="SmartHRService.DeleteCrewCustomFieldTemplateAsync"/>は、"/v1/crew_custom_field_templates/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeleteCrewCustomFieldTemplateAsync)} > DELETE /v1/crew_custom_field_templates/:id をコールする。")]
    public async Task DeleteCrewCustomFieldTemplateAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.DeleteCrewCustomFieldTemplateAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crew_custom_field_templates/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchCrewCustomFieldTemplateAsync"/>は、"/v1/crew_custom_field_templates/{id}"にGETリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.FetchCrewCustomFieldTemplateAsync" path="/param[@name='includeGroup']"/>
    /// <param name="expectedQuery">サーバー側が受け取るパラメータクエリ</param>
    [InlineData(false, "")]
    [InlineData(true, "?embed=group")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchCrewCustomFieldTemplateAsync)} > GET /v1/crew_custom_field_templates/:id をコールする。")]
    public async Task FetchCrewCustomFieldTemplateAsync_Calls_GetApi(bool includeGroup, string expectedQuery)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(CrewCustomFieldTemplateTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchCrewCustomFieldTemplateAsync(id, includeGroup).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crew_custom_field_templates/{id}{expectedQuery}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UpdateCrewCustomFieldTemplateAsync"/>は、"/v1/crew_custom_field_templates/{id}"にPATCHリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.UpdateCrewCustomFieldTemplateAsync" path="/param[@name='payload']"/>
    /// <param name="expectedJson">サーバー側が受け取るリクエストボディ</param>
    [MemberData(nameof(CrewCustomFieldTemplateTestData))]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UpdateCrewCustomFieldTemplateAsync)} > PATCH /v1/crew_custom_field_templates/:id をコールする。")]
    public async Task UpdateCrewCustomFieldTemplateAsync_Calls_PatchApi(CrewCustomFieldTemplatePayload payload, string expectedJson)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(CrewCustomFieldTemplateTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.UpdateCrewCustomFieldTemplateAsync(id, payload).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crew_custom_field_templates/{id}");
            req.Method.Should().Be(HttpMethod.Patch);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be(expectedJson);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.ReplaceCrewCustomFieldTemplateAsync"/>は、"/v1/crew_custom_field_templates/{id}"にPUTリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.ReplaceCrewCustomFieldTemplateAsync" path="/param[@name='payload']"/>
    /// <param name="expectedJson">サーバー側が受け取るリクエストボディ</param>
    [MemberData(nameof(CrewCustomFieldTemplateTestData))]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.ReplaceCrewCustomFieldTemplateAsync)} > PUT /v1/crew_custom_field_templates/:id をコールする。")]
    public async Task ReplaceCrewCustomFieldTemplateAsync_Calls_PutApi(CrewCustomFieldTemplatePayload payload, string expectedJson)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(CrewCustomFieldTemplateTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.ReplaceCrewCustomFieldTemplateAsync(id, payload).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crew_custom_field_templates/{id}");
            req.Method.Should().Be(HttpMethod.Put);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be(expectedJson);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchCrewCustomFieldTemplateListAsync"/>は、"/v1/crew_custom_field_templates"にGETリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.FetchCrewCustomFieldTemplateListAsync" path="/param"/>
    /// <param name="expectedQuery">サーバー側が受け取るパラメータクエリ</param>
    [InlineData(false, "page=1&per_page=10")]
    [InlineData(true, "embed=group&page=1&per_page=10")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchCrewCustomFieldTemplateListAsync)} > GET /v1/crew_custom_field_templates をコールする。")]
    public async Task FetchCrewCustomFieldTemplateListAsync_Calls_GetApi(bool includeGroup, string expectedQuery)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{CrewCustomFieldTemplateTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchCrewCustomFieldTemplateListAsync(includeGroup, 1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/crew_custom_field_templates?{expectedQuery}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.AddCrewCustomFieldTemplateAsync"/>は、"/v1/crew_custom_field_templates"にPOSTリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.AddCrewCustomFieldTemplateAsync" path="/param[@name='payload']"/>
    /// <param name="expectedJson">サーバー側が受け取るリクエストボディ</param>
    [MemberData(nameof(CrewCustomFieldTemplateTestData))]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.AddCrewCustomFieldTemplateAsync)} > POST /v1/crew_custom_field_templates をコールする。")]
    public async Task AddCrewCustomFieldTemplateAsync_Calls_PostApi(CrewCustomFieldTemplatePayload payload, string expectedJson)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(CrewCustomFieldTemplateTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.AddCrewCustomFieldTemplateAsync(payload).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/crew_custom_field_templates");
            req.Method.Should().Be(HttpMethod.Post);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be(expectedJson);
            return true;
        }, Times.Once());
    }
    #endregion

    #region 部署
    /// <summary>
    /// <see cref="SmartHRService.DeleteDepartmentAsync"/>は、"/v1/departments/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeleteDepartmentAsync)} > DELETE /v1/departments/:id をコールする。")]
    public async Task DeleteDepartmentAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.DeleteDepartmentAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/departments/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchDepartmentAsync"/>は、"/v1/departments/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchDepartmentAsync)} > GET /v1/departments/:id をコールする。")]
    public async Task FetchDepartmentAsync_Calls_GetApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(DepartmentTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchDepartmentAsync(id).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/departments/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UpdateDepartmentAsync"/>は、"/v1/departments/{id}"にPATCHリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.UpdateDepartmentAsync" path="/param"/>
    /// <param name="expectedJson">サーバー側が受け取るリクエストボディ</param>
    [InlineData(null, null, null, null, "{}")]
    [InlineData(null, 1, null, null, "{\"position\":1}")]
    [InlineData(null, null, "foo", null, "{\"code\":\"foo\"}")]
    [InlineData(null, null, null, "foo", "{\"parent_id\":\"foo\"}")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UpdateDepartmentAsync)} > PATCH /v1/departments/:id をコールする。")]
    public async Task UpdateDepartmentAsync_Calls_PatchApi(string? name, int? position, string? code, string? parentId, string expectedJson)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(DepartmentTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.UpdateDepartmentAsync(id, name, position, code, parentId).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/departments/{id}");
            req.Method.Should().Be(HttpMethod.Patch);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be(expectedJson);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.ReplaceDepartmentAsync"/>は、"/v1/departments/{id}"にPUTリクエストを行う。
    /// </summary>
    /// <param name="code"><inheritdoc cref="SmartHRService.UpdateDepartmentAsync" path="/param[@name='code']"/></param>
    /// <param name="parentId"><inheritdoc cref="SmartHRService.UpdateDepartmentAsync" path="/param[@name='parentId']"/></param>
    /// <param name="expectedJson">サーバー側が受け取るリクエストボディ</param>
    [InlineData(null, null, "{\"name\":\"test\",\"position\":1}")]
    [InlineData("foo", null, "{\"name\":\"test\",\"position\":1,\"code\":\"foo\"}")]
    [InlineData(null, "foo", "{\"name\":\"test\",\"position\":1,\"parent_id\":\"foo\"}")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.ReplaceDepartmentAsync)} > PUT /v1/departments/:id をコールする。")]
    public async Task ReplaceDepartmentAsync_Calls_PutApi(string? code, string? parentId, string expectedJson)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(DepartmentTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.ReplaceDepartmentAsync(id, "test", 1, code, parentId).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/departments/{id}");
            req.Method.Should().Be(HttpMethod.Put);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be(expectedJson);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchDepartmentListAsync"/>は、"/v1/departments"にGETリクエストを行う。
    /// </summary>
    [InlineData(null, null, "page=1&per_page=10")]
    [InlineData("foo", null, "code=foo&page=1&per_page=10")]
    [InlineData(null, "name,-updated_at", "sort=name,-updated_at&page=1&per_page=10")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchDepartmentListAsync)} > GET /v1/departments をコールする。")]
    public async Task FetchDepartmentListAsync_Calls_GetApi(string? code, string? sortBy, string expectedQuery)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{DepartmentTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchDepartmentListAsync(code, sortBy, 1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/departments?{expectedQuery}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.AddDepartmentAsync"/>は、"/v1/departments"にPOSTリクエストを行う。
    /// </summary>
    [InlineData(null, null, null, "{\"name\":\"test\"}")]
    [InlineData(3, null, null, "{\"name\":\"test\",\"position\":3}")]
    [InlineData(null, "foo", null, "{\"name\":\"test\",\"code\":\"foo\"}")]
    [InlineData(null, null, "foo", "{\"name\":\"test\",\"parent_id\":\"foo\"}")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.AddDepartmentAsync)} > POST /v1/departments をコールする。")]
    public async Task AddDepartmentAsync_Calls_PostApi(int? position, string? code, string? parentId, string expectedJson)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(DepartmentTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.AddDepartmentAsync("test", position, code, parentId).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/departments");
            req.Method.Should().Be(HttpMethod.Post);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be(expectedJson);
            return true;
        }, Times.Once());
    }
    #endregion

    #region 雇用形態
    /// <summary>
    /// <see cref="SmartHRService.DeleteEmploymentTypeAsync"/>は、"/v1/departments/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeleteEmploymentTypeAsync)} > DELETE /v1/departments/:id をコールする。")]
    public async Task DeleteEmploymentTypeAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.DeleteEmploymentTypeAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/employment_types/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchEmploymentTypeAsync"/>は、"/v1/employment_types/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchEmploymentTypeAsync)} > GET /v1/employment_types/:id をコールする。")]
    public async Task FetchEmploymentTypeAsync_Calls_GetApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(EmploymentTypeTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchEmploymentTypeAsync(id).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/employment_types/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UpdateEmploymentTypeAsync"/>は、"/v1/employment_types/{id}"にPATCHリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UpdateEmploymentTypeAsync)} > PATCH /v1/employment_types/:id をコールする。")]
    public async Task UpdateEmploymentTypeAsync_Calls_PatchApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(EmploymentTypeTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.UpdateEmploymentTypeAsync(id, "foo").ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/employment_types/{id}");
            req.Method.Should().Be(HttpMethod.Patch);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be("{\"name\":\"foo\"}");
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.ReplaceEmploymentTypeAsync"/>は、"/v1/employment_types/{id}"にPUTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.ReplaceEmploymentTypeAsync)} > PUT /v1/employment_types/:id をコールする。")]
    public async Task ReplaceEmploymentTypeAsync_Calls_PutApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(EmploymentTypeTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.ReplaceEmploymentTypeAsync(id, "test").ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/employment_types/{id}");
            req.Method.Should().Be(HttpMethod.Put);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be("{\"name\":\"test\"}");
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchEmploymentTypeListAsync"/>は、"/v1/employment_types"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchEmploymentTypeListAsync)} > GET /v1/employment_types をコールする。")]
    public async Task FetchEmploymentTypeListAsync_Calls_GetApi()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{EmploymentTypeTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchEmploymentTypeListAsync(1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/employment_types?page=1&per_page=10");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.AddEmploymentTypeAsync"/>は、"/v1/employment_types"にPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.AddEmploymentTypeAsync)} > POST /v1/employment_types をコールする。")]
    public async Task AddEmploymentTypeAsync_Calls_PostApi()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(EmploymentTypeTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.AddEmploymentTypeAsync("test", EmploymentType.Preset.BoardMember).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/employment_types");
            req.Method.Should().Be(HttpMethod.Post);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be("{\"name\":\"test\",\"preset_type\":\"board_member\"}");
            return true;
        }, Times.Once());
    }
    #endregion

    #region メールフォーマット
    /// <summary>
    /// <see cref="SmartHRService.FetchMailFormatAsync"/>は、"/v1/mail_formats/{id}"にGETリクエストを行う。
    /// </summary>
    [InlineData(false, "")]
    [InlineData(true, "?embed=crew_input_forms")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchMailFormatAsync)} > GET /v1/mail_formats/:id をコールする。")]
    public async Task FetchMailFormatAsync_Calls_GetApi(bool includeCrewInputForms, string expectedQuery)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(MailFormatTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchMailFormatAsync(id, includeCrewInputForms).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/mail_formats/{id}{expectedQuery}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchMailFormatListAsync"/>は、"/v1/mail_formats"にGETリクエストを行う。
    /// </summary>
    [InlineData(false, "page=1&per_page=10")]
    [InlineData(true, "embed=crew_input_forms&page=1&per_page=10")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchMailFormatListAsync)} > GET /v1/mail_formats をコールする。")]
    public async Task FetchMailFormatListAsync_Calls_GetApi(bool includeCrewInputForms, string expectedQuery)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{MailFormatTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchMailFormatListAsync(includeCrewInputForms, 1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/mail_formats?{expectedQuery}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }
    #endregion

    #region ユーザ
    /// <summary>
    /// <see cref="SmartHRService.FetchUserAsync"/>は、"/v1/users/{id}"にGETリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.FetchUserAsync" path="/param[@name='includeCrewInfo']"/>
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
    /// <inheritdoc cref="SmartHRService.FetchUserListAsync" path="/param[@name='includeCrewInfo']"/>
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

    #region 事業所
    /// <summary>
    /// <see cref="SmartHRService.FetchBizEstablishmentListAsync"/>は、"/v1/users"にGETリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.FetchBizEstablishmentListAsync" path="/param[@name='includeCrewInfo']"/>
    /// <param name="expectedQuery">パラメータクエリ</param>
    [InlineData(BizEstablishmentEmbed.None, "page=1&per_page=10")]
    [InlineData(BizEstablishmentEmbed.SocInsOwner, "embed=soc_ins_owner&page=1&per_page=10")]
    [InlineData(BizEstablishmentEmbed.LabInsOwner, "embed=lab_ins_owner&page=1&per_page=10")]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchBizEstablishmentListAsync)} > GET /v1/users をコールする。")]
    public async Task FetchBizEstablishmentListAsync_Calls_GetApi(BizEstablishmentEmbed embed, string expectedQuery)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{BizEstablishmentTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchBizEstablishmentListAsync(embed, 1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri!.PathAndQuery.Should().Be($"/v1/biz_establishments?{expectedQuery}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }
    #endregion

    #region Webhook
    /// <summary><inheritdoc cref="Webhook" path="/summary/text()"/>APIのテストデータ</summary>
    public static object[][] WebhookTestData => new[]
    {
        new object[]{ new WebhookPayload("example.com"), "{\"url\":\"example.com\"}" },
        new object[]{ new WebhookPayload(
            Url: "example.com",
            Description: "テスト用",
            SecretToken: "token",
            CrewCreated: true,
            CrewUpdated: true,
            CrewDeleted: true,
            CrewImported: true,
            DependentCreated: false,
            DependentUpdated: false,
            DependentDeleted: false,
            DependentImported: false,
            DisabledAt: new(2021, 10, 26, 13, 0, 0, 0, TimeSpan.Zero)),
            "{"
            + "\"url\":\"example.com\","
            + "\"description\":\"テスト用\","
            + "\"secret_token\":\"token\","
            + "\"crew_created\":true,"
            + "\"crew_updated\":true,"
            + "\"crew_deleted\":true,"
            + "\"crew_imported\":true,"
            + "\"dependent_created\":false,"
            + "\"dependent_updated\":false,"
            + "\"dependent_deleted\":false,"
            + "\"dependent_imported\":false,"
            + "\"disabled_at\":\"2021-10-26T13:00:00+00:00\""
            + "}" },
    };

    /// <summary>
    /// <see cref="SmartHRService.DeleteWebhookAsync"/>は、"/v1/webhooks/{id}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.DeleteWebhookAsync)} > DELETE /v1/webhooks/:id をコールする。")]
    public async Task DeleteWebhookAsync_Calls_DeleteApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler);
        await sut.DeleteWebhookAsync(id).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/webhooks/{id}");
            req.Method.Should().Be(HttpMethod.Delete);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchWebhookAsync"/>は、"/v1/webhooks/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchWebhookAsync)} > GET /v1/webhooks/:id をコールする。")]
    public async Task FetchWebhookAsync_Calls_GetApi()
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(WebhookTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.FetchWebhookAsync(id).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(req =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/webhooks/{id}");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.UpdateWebhookAsync"/>は、"/v1/webhooks/{id}"にPATCHリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.UpdateWebhookAsync" path="/param"/>
    /// <param name="expectedJson">サーバー側が受け取るリクエストボディ</param>
    [MemberData(nameof(WebhookTestData))]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.UpdateWebhookAsync)} > PATCH /v1/webhooks/:id をコールする。")]
    public async Task UpdateWebhookAsync_Calls_PatchApi(WebhookPayload payload, string expectedJson)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(WebhookTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.UpdateWebhookAsync(id, payload).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/webhooks/{id}");
            req.Method.Should().Be(HttpMethod.Patch);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be(expectedJson);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.ReplaceWebhookAsync"/>は、"/v1/webhooks/{id}"にPUTリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.ReplaceWebhookAsync" path="/param"/>
    /// <param name="expectedJson">サーバー側が受け取るリクエストボディ</param>
    [MemberData(nameof(WebhookTestData))]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.ReplaceWebhookAsync)} > PUT /v1/webhooks/:id をコールする。")]
    public async Task ReplaceWebhookAsync_Calls_PutApi(WebhookPayload payload, string expectedJson)
    {
        // Arrange
        string id = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(WebhookTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.ReplaceWebhookAsync(id, payload).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be($"/v1/webhooks/{id}");
            req.Method.Should().Be(HttpMethod.Put);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be(expectedJson);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.FetchWebhookListAsync"/>は、"/v1/webhooks"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.FetchWebhookListAsync)} > GET /v1/webhooks をコールする。")]
    public async Task FetchWebhookListAsync_Calls_GetApi()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse($"[{WebhookTest.Json}]", "application/json");

        // Act
        var sut = CreateSut(handler);
        var entities = await sut.FetchWebhookListAsync(1, 10).ConfigureAwait(false);

        // Assert
        entities.Should().NotBeNullOrEmpty();
        handler.VerifyRequest((req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/webhooks?page=1&per_page=10");
            req.Method.Should().Be(HttpMethod.Get);
            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="SmartHRService.AddWebhookAsync"/>は、"/v1/webhooks"にPOSTリクエストを行う。
    /// </summary>
    /// <inheritdoc cref="SmartHRService.AddWebhookAsync" path="/param"/>
    /// <param name="expectedJson">サーバー側が受け取るリクエストボディ</param>
    [MemberData(nameof(WebhookTestData))]
    [Theory(DisplayName = $"{nameof(SmartHRService)} > {nameof(SmartHRService.AddWebhookAsync)} > POST /v1/webhooks をコールする。")]
    public async Task AddWebhookAsync_Calls_PostApi(WebhookPayload payload, string expectedJson)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(req => req.RequestUri?.GetLeftPart(UriPartial.Authority) == BaseUri)
            .ReturnsResponse(WebhookTest.Json, "application/json");

        // Act
        var sut = CreateSut(handler);
        var entity = await sut.AddWebhookAsync(payload).ConfigureAwait(false);

        // Assert
        entity.Should().NotBeNull();
        handler.VerifyRequest(async (req) =>
        {
            req.RequestUri.Should().NotBeNull();
            req.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be(BaseUri);
            req.RequestUri.PathAndQuery.Should().Be("/v1/webhooks");
            req.Method.Should().Be(HttpMethod.Post);

            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            receivedJson.Should().Be(expectedJson);
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
