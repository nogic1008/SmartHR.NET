using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Services;

/// <summary>
/// <see cref="ISmartHRService"/>のHTTP Client実装
/// </summary>
public class SmartHRService : ISmartHRService
{
    /// <summary>HttpClient(DI)</summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// SmartHRServiceの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="httpClient">HttpClient(DI)</param>
    /// <param name="endpoint">
    /// エンドポイントURI
    /// <para>未指定時には、<see cref="HttpClient.BaseAddress"/>がそのまま利用されます。</para>
    /// </param>
    /// <param name="accessToken">
    /// アクセストークン
    /// <para>未指定時には、<see cref="HttpClient.DefaultRequestHeaders"/>がそのまま利用されます。</para>
    /// </param>
    public SmartHRService(HttpClient httpClient, string? endpoint = null, string? accessToken = null)
        : this(httpClient, endpoint is null ? null : new Uri(endpoint), accessToken) { }

    /// <summary>
    /// SmartHRServiceの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="httpClient">HttpClient(DI)</param>
    /// <param name="endpoint">
    /// エンドポイントURI
    /// <para>未指定時には、<see cref="HttpClient.BaseAddress"/>がそのまま利用されます。</para>
    /// </param>
    /// <param name="accessToken">
    /// アクセストークン
    /// <para>未指定時には、<see cref="HttpClient.DefaultRequestHeaders"/>がそのまま利用されます。</para>
    /// </param>
    public SmartHRService(HttpClient httpClient, Uri? endpoint, string? accessToken = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        if (endpoint is not null)
            _httpClient.BaseAddress = endpoint;
        if (!string.IsNullOrEmpty(accessToken))
            _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", accessToken);
    }

    #region PayRolls
    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public async ValueTask DeletePayRollAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"/v1/payrolls/{id}", cancellationToken).ConfigureAwait(false);
        await ValidateResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<PayRoll> FetchPayRollAsync(string id, CancellationToken cancellationToken = default)
        => CallApiAsync<PayRoll>(new(HttpMethod.Get, $"/v1/payrolls/{id}"), cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<PayRoll> UpdatePayRollAsync(string id, string nameForAdmin, string nameForCrew, CancellationToken cancellationToken = default)
        => CallApiAsync<PayRoll>(
            new(new("PATCH"), $"/v1/payrolls/{id}")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "name_for_admin", nameForAdmin },
                    { "name_for_crew", nameForCrew }
                })
            }, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<PayRoll> PublishPayRollAsync(string id, DateTimeOffset? publishedAt = default, bool? notifyWithPublish = default, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>();
        if (publishedAt is not null)
            parameters.Add("published_at", publishedAt.GetValueOrDefault().ToString("o", CultureInfo.InvariantCulture));
        if (notifyWithPublish is not null)
            parameters.Add("notify_with_publish", notifyWithPublish.ToString()!.ToLowerInvariant());

        var request = new HttpRequestMessage(new("PATCH"), $"/v1/payrolls/{id}/publish")
        {
            Content = new FormUrlEncodedContent(parameters)
        };
        return CallApiAsync<PayRoll>(request, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<PayRoll> UnconfirmPayRollAsync(
        string id,
        PaymentType paymentType,
        DateTime paidAt,
        DateTime periodStartAt,
        DateTime periodEndAt,
        PaymentStatus status,
        NumeralSystemType numeralSystemHandleType,
        string nameForAdmin,
        string nameForCrew,
        DateTimeOffset? publishedAt = default,
        bool? notifyWithPublish = default,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>()
        {
            { "payment_type", JsonStringEnumConverterEx<PaymentType>.EnumToString[paymentType] },
            { "paid_at", paidAt.ToString("yyyy-MM-dd", null) },
            { "period_start_at", periodStartAt.ToString("yyyy-MM-dd", null) },
            { "period_end_at", periodEndAt.ToString("yyyy-MM-dd", null) },
            { "status", JsonStringEnumConverterEx<PaymentStatus>.EnumToString[status] },
            { "numeral_system_handle_type", JsonStringEnumConverterEx<NumeralSystemType>.EnumToString[numeralSystemHandleType] },
            { "name_for_admin", nameForAdmin },
            { "name_for_crew", nameForCrew },
        };
        if (publishedAt is not null)
            parameters.Add("published_at", publishedAt.GetValueOrDefault().ToString("o", CultureInfo.InvariantCulture));
        if (notifyWithPublish is not null)
            parameters.Add("notify_with_publish", notifyWithPublish.ToString()!.ToLowerInvariant());

        var request = new HttpRequestMessage(new("PATCH"), $"/v1/payrolls/{id}/unfix")
        {
            Content = new FormUrlEncodedContent(parameters)
        };
        return CallApiAsync<PayRoll>(request, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public async ValueTask<PayRoll> ConfirmPayRollAsync(
        string id,
        PaymentType paymentType,
        DateTime paidAt,
        DateTime periodStartAt,
        DateTime periodEndAt,
        PaymentStatus status,
        NumeralSystemType numeralSystemHandleType,
        string nameForAdmin,
        string nameForCrew,
        DateTimeOffset? publishedAt = default,
        bool? notifyWithPublish = default,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>()
        {
            { "payment_type", JsonStringEnumConverterEx<PaymentType>.EnumToString[paymentType] },
            { "paid_at", paidAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
            { "period_start_at", periodStartAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
            { "period_end_at", periodEndAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
            { "status", JsonStringEnumConverterEx<PaymentStatus>.EnumToString[status] },
            { "numeral_system_handle_type", JsonStringEnumConverterEx<NumeralSystemType>.EnumToString[numeralSystemHandleType] },
            { "name_for_admin", nameForAdmin },
            { "name_for_crew", nameForCrew },
        };
        if (publishedAt is not null)
            parameters.Add("published_at", publishedAt.GetValueOrDefault().ToString("o", CultureInfo.InvariantCulture));
        if (notifyWithPublish is not null)
            parameters.Add("notify_with_publish", notifyWithPublish.ToString()!.ToLowerInvariant());

        var request = new HttpRequestMessage(new("PATCH"), $"/v1/payrolls/{id}/fix")
        {
            Content = new FormUrlEncodedContent(parameters)
        };
        return await CallApiAsync<PayRoll>(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="page"/>か<paramref name="perPage"/>が0以下です。
    /// もしくは<paramref name="perPage"/>が100を超えています。
    /// </exception>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<IReadOnlyList<PayRoll>> FetchPayRollListAsync(int page, int perPage = 10, CancellationToken cancellationToken = default)
    {
        if (page <= 0)
            throw new ArgumentOutOfRangeException(nameof(page));
        if (perPage is <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(perPage));

        var request = new HttpRequestMessage(HttpMethod.Get, $"/v1/payrolls?page={page}&per_page={perPage}");
        return CallApiAsync<IReadOnlyList<PayRoll>>(request, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<PayRoll> AddPayRollAsync(
        PaymentType paymentType,
        DateTime paidAt,
        DateTime periodStartAt,
        DateTime periodEndAt,
        NumeralSystemType numeralSystemHandleType,
        string nameForAdmin,
        string nameForCrew,
        CancellationToken cancellationToken = default)
            => CallApiAsync<PayRoll>(new(HttpMethod.Post, "/v1/payrolls")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "payment_type", JsonStringEnumConverterEx<PaymentType>.EnumToString[paymentType] },
                    { "paid_at", paidAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
                    { "period_start_at", periodStartAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
                    { "period_end_at", periodEndAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
                    { "numeral_system_handle_type", JsonStringEnumConverterEx<NumeralSystemType>.EnumToString[numeralSystemHandleType] },
                    { "name_for_admin", nameForAdmin },
                    { "name_for_crew", nameForCrew },
                })
            }, cancellationToken);
    #endregion

    #region Payslips
    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public async ValueTask DeletePayslipAsync(string payrollId, string id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"/v1/payrolls/{payrollId}/payslips/{id}", cancellationToken).ConfigureAwait(false);
        await ValidateResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<Payslip> FetchPayslipAsync(string payrollId, string id, CancellationToken cancellationToken = default)
        => CallApiAsync<Payslip>(new(HttpMethod.Get, $"/v1/payrolls/{payrollId}/payslips/{id}"), cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<Payslip> AddPayslipListAsync(string payrollId, IReadOnlyList<PayslipRequest> payload, CancellationToken cancellationToken = default)
        => CallApiAsync<Payslip>(new(HttpMethod.Post, $"/v1/payrolls/{payrollId}/payslips/bulk")
        {
            Content = JsonContent.Create(payload, options: _serializerOptions)
        }, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="page"/>か<paramref name="perPage"/>が0以下です。
    /// もしくは<paramref name="perPage"/>が100を超えています。
    /// </exception>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<IReadOnlyList<Payslip>> FetchPayslipListAsync(string payrollId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
    {
        if (page <= 0)
            throw new ArgumentOutOfRangeException(nameof(page));
        if (perPage is <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(perPage));

        var request = new HttpRequestMessage(HttpMethod.Get, $"/v1/payrolls/{payrollId}/payslips?page={page}&per_page={perPage}");
        return CallApiAsync<IReadOnlyList<Payslip>>(request, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<Payslip> AddPayslipAsync(string payrollId, PayslipRequest payload, CancellationToken cancellationToken = default)
        => CallApiAsync<Payslip>(new(HttpMethod.Post, $"/v1/payrolls/{payrollId}/payslips")
        {
            Content = JsonContent.Create(payload, options: _serializerOptions)
        }, cancellationToken);
    #endregion

    #region Common
    private static readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
    /// <summary>
    /// APIを呼び出し、受け取ったJSONを<typeparamref name="T"/>に変換して返します。
    /// </summary>
    /// <param name="request">APIに対するリクエスト</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <typeparam name="T">JSONの型</typeparam>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    private async ValueTask<T> CallApiAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
        await ValidateResponseAsync(response, cancellationToken).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false))!;
    }

    /// <summary>
    /// APIリクエストが正常に実行できたことを検証します。
    /// </summary>
    /// <param name="response">APIからのレスポンスオブジェクト</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    /// <exception cref="HttpRequestException">
    /// HTTPステータスコードが400以上かつ、APIがエラーレスポンスを返さなかった場合にスローされます。
    /// </exception>
    private static async ValueTask ValidateResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Error? error;
            try
            {
                error = await response.Content.ReadFromJsonAsync<Error>(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                error = null;
            }

            if (error is null)
                throw;
            throw new ApiFailedException(error.Message, ex, error.Type, error.Errors);
        }
    }
    /// <summary>
    /// エラーレスポンス
    /// </summary>
    /// <param name="Code">エラーの原因毎に一意に振られたコード</param>
    /// <param name="Type">エラーの型</param>
    /// <param name="Message">原因の説明</param>
    /// <param name="Errors">送られたリソースのプロパティにエラーがあった場合の説明</param>
    private record Error(
        int Code,
        ErrorType Type,
        string Message,
        IReadOnlyList<ErrorDetail>? Errors = null
    );
    #endregion
}
