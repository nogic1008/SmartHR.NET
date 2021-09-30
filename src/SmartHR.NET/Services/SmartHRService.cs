using System;
using System.Net.Http;
using System.Net.Http.Json;
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
}
