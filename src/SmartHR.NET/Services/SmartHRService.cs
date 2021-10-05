using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
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
    public async ValueTask DeletePayRollAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"/v1/payrolls/{id}", cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc/>
    public async ValueTask<PayRoll> FetchPayRollAsync(string id, CancellationToken cancellationToken = default)
        => (await _httpClient.GetFromJsonAsync<PayRoll>($"/v1/payrolls/{id}", cancellationToken: cancellationToken).ConfigureAwait(false))!;

    /// <inheritdoc/>
    public async ValueTask<PayRoll> UpdatePayRollAsync(string id, string nameForAdmin, string nameForCrew, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>()
        {
            { "name_for_admin", nameForAdmin },
            { "name_for_crew", nameForCrew }
        };
        var content = new FormUrlEncodedContent(parameters);

        var response = await _httpClient.SendAsync(new(new("PATCH"), $"/v1/payrolls/{id}") { Content = content }, cancellationToken).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync<PayRoll>(cancellationToken: cancellationToken).ConfigureAwait(false))!;
    }

    /// <inheritdoc/>
    public async ValueTask<PayRoll> PublishPayRollAsync(string id, DateTimeOffset? publishedAt = default, bool? notifyWithPublish = default, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>();
        if (publishedAt is not null)
            parameters.Add("published_at", publishedAt.GetValueOrDefault().ToString("o", CultureInfo.InvariantCulture));
        if (notifyWithPublish is not null)
            parameters.Add("notify_with_publish", notifyWithPublish.ToString()!.ToLowerInvariant());
        var content = new FormUrlEncodedContent(parameters);

        var response = await _httpClient.SendAsync(new(new("PATCH"), $"/v1/payrolls/{id}/publish") { Content = content }, cancellationToken).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync<PayRoll>(cancellationToken: cancellationToken).ConfigureAwait(false))!;
    }

    /// <inheritdoc/>
    public async ValueTask<PayRoll> UnconfirmPayRollAsync(
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
        var content = new FormUrlEncodedContent(parameters);

        var response = await _httpClient.SendAsync(new(new("PATCH"), $"/v1/payrolls/{id}/unfix") { Content = content }, cancellationToken).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync<PayRoll>(cancellationToken: cancellationToken).ConfigureAwait(false))!;
    }

    /// <inheritdoc/>
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
        var content = new FormUrlEncodedContent(parameters);

        var response = await _httpClient.SendAsync(new(new("PATCH"), $"/v1/payrolls/{id}/fix") { Content = content }, cancellationToken).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync<PayRoll>(cancellationToken: cancellationToken).ConfigureAwait(false))!;
    }

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<PayRoll>> FetchPayRollListAsync(int page, int perPage = 10, CancellationToken cancellationToken = default)
    {
        if (page <= 0)
            throw new ArgumentOutOfRangeException(nameof(page));
        if (perPage is <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(perPage));

        return (await _httpClient.GetFromJsonAsync<IReadOnlyList<PayRoll>>($"/v1/payrolls?page={page}&per_page={perPage}").ConfigureAwait(false))!;
    }

    /// <inheritdoc/>
    public async ValueTask<PayRoll> AddPayRollAsync(
        PaymentType paymentType,
        DateTime paidAt,
        DateTime periodStartAt,
        DateTime periodEndAt,
        NumeralSystemType numeralSystemHandleType,
        string nameForAdmin,
        string nameForCrew,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>()
        {
            { "payment_type", JsonStringEnumConverterEx<PaymentType>.EnumToString[paymentType] },
            { "paid_at", paidAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
            { "period_start_at", periodStartAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
            { "period_end_at", periodEndAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
            { "numeral_system_handle_type", JsonStringEnumConverterEx<NumeralSystemType>.EnumToString[numeralSystemHandleType] },
            { "name_for_admin", nameForAdmin },
            { "name_for_crew", nameForCrew },
        };
        var content = new FormUrlEncodedContent(parameters);

        var response = await _httpClient.PostAsync("/v1/payrolls", content, cancellationToken).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync<PayRoll>(cancellationToken: cancellationToken).ConfigureAwait(false))!;
    }
    #endregion
}
