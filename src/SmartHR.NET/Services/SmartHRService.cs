using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
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

    #region DependentRelations
    /// <summary>
    /// 続柄をリストで取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B6%9A%E6%9F%84/getV1DependentRelations"/>
    /// </remarks>
    /// <param name="spouse">配偶者のみを抽出するかどうか</param>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>続柄の一覧</returns>
    public ValueTask<IReadOnlyList<DependentRelation>> FetchDependentRelationListAsync(bool spouse = false, int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
        => FetchListAsync<DependentRelation>(spouse ? "/v1/dependent_relations?filter=spouse&" : "/v1/dependent_relations?", page, perPage, cancellationToken);
    #endregion

    #region PaymentPeriods
    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<PaymentPeriod> FetchPaymentPeriodAsync(string id, CancellationToken cancellationToken = default)
        => CallApiAsync<PaymentPeriod>(new(HttpMethod.Get, $"/v1/payment_periods/{id}"), cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="page"/>か<paramref name="perPage"/>が0以下です。
    /// もしくは<paramref name="perPage"/>が100を超えています。
    /// </exception>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<IReadOnlyList<PaymentPeriod>> FetchPaymentPeriodListAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
        => FetchListAsync<PaymentPeriod>("/v1/payment_periods?", page, perPage, cancellationToken);
    #endregion

    #region JobTitles
    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public async ValueTask DeleteJobTitleAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"/v1/job_titles/{id}", cancellationToken).ConfigureAwait(false);
        await ValidateResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<JobTitle> FetchJobTitleAsync(string id, CancellationToken cancellationToken = default)
        => CallApiAsync<JobTitle>(new(HttpMethod.Get, $"/v1/job_titles/{id}"), cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="rank"/>が1未満か、99999を超えています。</exception>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<JobTitle> UpdateJobTitleAsync(string id, string? name = null, int? rank = default, CancellationToken cancellationToken = default)
    {
        if (rank is < 1 or > 99999)
            throw new ArgumentOutOfRangeException(nameof(rank));

        var parameters = new Dictionary<string, string>(2);
        if (name is not null)
            parameters.Add(nameof(name), name);
        if (rank is not null)
            parameters.Add(nameof(rank), rank.ToString()!);

        return CallApiAsync<JobTitle>(
            new(new("PATCH"), $"/v1/job_titles/{id}")
            {
                Content = new FormUrlEncodedContent(parameters)
            }, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="rank"/>が1未満か、99999を超えています。</exception>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<JobTitle> ReplaceJobTitleAsync(string id, string name, int? rank = default, CancellationToken cancellationToken = default)
    {
        if (rank is < 1 or > 99999)
            throw new ArgumentOutOfRangeException(nameof(rank));

        var parameters = new Dictionary<string, string>(2) { { nameof(name), name } };
        if (rank is not null)
            parameters.Add(nameof(rank), rank.ToString()!);

        return CallApiAsync<JobTitle>(
            new(HttpMethod.Put, $"/v1/job_titles/{id}")
            {
                Content = new FormUrlEncodedContent(parameters)
            }, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="page"/>か<paramref name="perPage"/>が0以下です。
    /// もしくは<paramref name="perPage"/>が100を超えています。
    /// </exception>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<IReadOnlyList<JobTitle>> FetchJobTitleListAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
        => FetchListAsync<JobTitle>("/v1/job_titles?", page, perPage, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="rank"/>が1未満か、99999を超えています。</exception>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<JobTitle> AddJobTitleAsync(string name, int? rank = default, CancellationToken cancellationToken = default)
    {
        if (rank is < 1 or > 99999)
            throw new ArgumentOutOfRangeException(nameof(rank));

        var parameters = new Dictionary<string, string>(2) { { nameof(name), name } };
        if (rank is not null)
            parameters.Add(nameof(rank), rank.ToString()!);

        return CallApiAsync<JobTitle>(
            new(HttpMethod.Post, "/v1/job_titles")
            {
                Content = new FormUrlEncodedContent(parameters)
            }, cancellationToken);
    }
    #endregion

    #region BankAccountSettings
    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="page"/>か<paramref name="perPage"/>が0以下です。
    /// もしくは<paramref name="perPage"/>が100を超えています。
    /// </exception>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<IReadOnlyList<BankAccountSetting>> FetchBankAccountSettingListAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
        => FetchListAsync<BankAccountSetting>("/v1/bank_account_settings?", page, perPage, cancellationToken);
    #endregion

    #region TaxWithholdings
    private record TaxWithholdingPayload(
        string? Name = null,
        TaxWithholding.FormStatus? Status = default,
        string? Year = null);

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public async ValueTask DeleteTaxWithholdingAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"/v1/tax_withholdings/{id}", cancellationToken).ConfigureAwait(false);
        await ValidateResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<TaxWithholding> FetchTaxWithholdingAsync(string id, CancellationToken cancellationToken = default)
        => CallApiAsync<TaxWithholding>(new(HttpMethod.Get, $"/v1/tax_withholdings/{id}"), cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<TaxWithholding> UpdateTaxWithholdingAsync(string id, string? name = null, TaxWithholding.FormStatus? status = default, string? year = null, CancellationToken cancellationToken = default)
        => CallApiAsync<TaxWithholding>(
            new(new("PATCH"), $"/v1/tax_withholdings/{id}")
            {
                Content = JsonContent.Create(new TaxWithholdingPayload(name, status, year), options: _serializerOptions)
            }, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<TaxWithholding> ReplaceTaxWithholdingAsync(string id, string name, TaxWithholding.FormStatus status, string year, CancellationToken cancellationToken = default)
        => CallApiAsync<TaxWithholding>(
            new(HttpMethod.Put, $"/v1/tax_withholdings/{id}")
            {
                Content = JsonContent.Create(new TaxWithholdingPayload(name, status, year), options: _serializerOptions)
            }, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="page"/>か<paramref name="perPage"/>が0以下です。
    /// もしくは<paramref name="perPage"/>が100を超えています。
    /// </exception>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<IReadOnlyList<TaxWithholding>> FetchTaxWithholdingListAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
        => FetchListAsync<TaxWithholding>("/v1/tax_withholdings?", page, perPage, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<TaxWithholding> AddTaxWithholdingAsync(string name, string year, CancellationToken cancellationToken = default)
        => CallApiAsync<TaxWithholding>(
            new(HttpMethod.Post, "/v1/tax_withholdings")
            {
                Content = JsonContent.Create(new TaxWithholdingPayload(name, null, year), options: _serializerOptions)
            }, cancellationToken);
    #endregion

    #region Payrolls
    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public async ValueTask DeletePayrollAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"/v1/payrolls/{id}", cancellationToken).ConfigureAwait(false);
        await ValidateResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<Payroll> FetchPayrollAsync(string id, CancellationToken cancellationToken = default)
        => CallApiAsync<Payroll>(new(HttpMethod.Get, $"/v1/payrolls/{id}"), cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<Payroll> UpdatePayrollAsync(string id, string nameForAdmin, string nameForCrew, CancellationToken cancellationToken = default)
        => CallApiAsync<Payroll>(
            new(new("PATCH"), $"/v1/payrolls/{id}")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>(2)
                {
                    { "name_for_admin", nameForAdmin },
                    { "name_for_crew", nameForCrew }
                })
            }, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<Payroll> PublishPayrollAsync(string id, DateTimeOffset? publishedAt = default, bool? notifyWithPublish = default, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>(2);
        if (publishedAt is not null)
            parameters.Add("published_at", publishedAt.GetValueOrDefault().ToString("o", CultureInfo.InvariantCulture));
        if (notifyWithPublish is not null)
            parameters.Add("notify_with_publish", notifyWithPublish.ToString()!.ToLowerInvariant());

        var request = new HttpRequestMessage(new("PATCH"), $"/v1/payrolls/{id}/publish")
        {
            Content = new FormUrlEncodedContent(parameters)
        };
        return CallApiAsync<Payroll>(request, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<Payroll> UnconfirmPayrollAsync(
        string id,
        Payroll.Payment paymentType,
        DateTime paidAt,
        DateTime periodStartAt,
        DateTime periodEndAt,
        Payroll.SalaryStatus status,
        Payroll.NumeralSystem numeralSystemHandleType,
        string nameForAdmin,
        string nameForCrew,
        DateTimeOffset? publishedAt = default,
        bool? notifyWithPublish = default,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>(10)
        {
            { "payment_type", JsonStringEnumConverterEx<Payroll.Payment>.EnumToString[paymentType] },
            { "paid_at", paidAt.ToString("yyyy-MM-dd", null) },
            { "period_start_at", periodStartAt.ToString("yyyy-MM-dd", null) },
            { "period_end_at", periodEndAt.ToString("yyyy-MM-dd", null) },
            { "status", JsonStringEnumConverterEx<Payroll.SalaryStatus>.EnumToString[status] },
            { "numeral_system_handle_type", JsonStringEnumConverterEx<Payroll.NumeralSystem>.EnumToString[numeralSystemHandleType] },
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
        return CallApiAsync<Payroll>(request, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public async ValueTask<Payroll> ConfirmPayrollAsync(
        string id,
        Payroll.Payment paymentType,
        DateTime paidAt,
        DateTime periodStartAt,
        DateTime periodEndAt,
        Payroll.SalaryStatus status,
        Payroll.NumeralSystem numeralSystemHandleType,
        string nameForAdmin,
        string nameForCrew,
        DateTimeOffset? publishedAt = default,
        bool? notifyWithPublish = default,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>(10)
        {
            { "payment_type", JsonStringEnumConverterEx<Payroll.Payment>.EnumToString[paymentType] },
            { "paid_at", paidAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
            { "period_start_at", periodStartAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
            { "period_end_at", periodEndAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
            { "status", JsonStringEnumConverterEx<Payroll.SalaryStatus>.EnumToString[status] },
            { "numeral_system_handle_type", JsonStringEnumConverterEx<Payroll.NumeralSystem>.EnumToString[numeralSystemHandleType] },
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
        return await CallApiAsync<Payroll>(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="page"/>か<paramref name="perPage"/>が0以下です。
    /// もしくは<paramref name="perPage"/>が100を超えています。
    /// </exception>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<IReadOnlyList<Payroll>> FetchPayrollListAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
        => FetchListAsync<Payroll>("/v1/payrolls?", page, perPage, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ApiFailedException">APIがエラーレスポンスを返した場合にスローされます。</exception>
    public ValueTask<Payroll> AddPayrollAsync(
        Payroll.Payment paymentType,
        DateTime paidAt,
        DateTime periodStartAt,
        DateTime periodEndAt,
        Payroll.NumeralSystem numeralSystemHandleType,
        string nameForAdmin,
        string nameForCrew,
        CancellationToken cancellationToken = default)
            => CallApiAsync<Payroll>(new(HttpMethod.Post, "/v1/payrolls")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>(7)
                {
                    { "payment_type", JsonStringEnumConverterEx<Payroll.Payment>.EnumToString[paymentType] },
                    { "paid_at", paidAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
                    { "period_start_at", periodStartAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
                    { "period_end_at", periodEndAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
                    { "numeral_system_handle_type", JsonStringEnumConverterEx<Payroll.NumeralSystem>.EnumToString[numeralSystemHandleType] },
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
        => FetchListAsync<Payslip>($"/v1/payrolls/{payrollId}/payslips?", page, perPage, cancellationToken);

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
    internal async ValueTask<T> CallApiAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
        await ValidateResponseAsync(response, cancellationToken).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false))!;
    }

    /// <summary>
    /// リストを返すAPIを呼び出し、結果を<typeparamref name="T"/>の配列に変換して返します。
    /// </summary>
    /// <typeparam name="T">JSONの型</typeparam>
    /// <param name="endpoint">APIエンドポイント</param>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="page"/>か<paramref name="perPage"/>が0以下です。
    /// もしくは<paramref name="perPage"/>が100を超えています。
    /// </exception>
    internal ValueTask<IReadOnlyList<T>> FetchListAsync<T>(string endpoint, int page, int perPage, CancellationToken cancellationToken)
    {
        if (page <= 0)
            throw new ArgumentOutOfRangeException(nameof(page));
        if (perPage is <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(perPage));
        var request = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}page={page}&per_page={perPage}");
        return CallApiAsync<IReadOnlyList<T>>(request, cancellationToken);
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
