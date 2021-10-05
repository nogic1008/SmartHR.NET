using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHR.NET;

/// <summary>
/// SmartHRのAPIがエラーを返したことを表す例外。
/// </summary>
[Serializable]
public class ApiFailedException : Exception
{
    /// <summary>
    /// ApiFailedExceptionの新しいインスタンスを生成します。
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    public ApiFailedException()
    { }

    /// <summary>
    /// ApiFailedExceptionの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    public ApiFailedException(string message) : base(message)
    { }

    /// <summary>
    /// ApiFailedExceptionの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="innerException">内部例外</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    public ApiFailedException(string message, Exception innerException) : base(message, innerException)
    { }

    /// <summary>
    /// ApiFailedExceptionの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="innerException">内部例外</param>
    /// <param name="errorType">エラーの原因</param>
    /// <param name="errors">送られたリソースのプロパティにエラーがあった場合の説明</param>
    public ApiFailedException(string message, Exception innerException, ErrorType errorType, IReadOnlyList<ErrorDetail>? errors) : base(message, innerException)
        => (ErrorType, Errors) = (errorType, errors);

    /// <summary>エラーの原因</summary>
    public ErrorType ErrorType { get; }

    /// <summary>
    /// 送られたリソースのプロパティにエラーがあった場合の説明
    /// </summary>
    public IReadOnlyList<ErrorDetail>? Errors { get; }
}

/// <summary>エラー詳細</summary>
public record ErrorDetail
{
    /// <summary>
    /// ErrorDetailの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="message"><see cref="Message"/></param>
    /// <param name="resource"><see cref="Resource"/></param>
    /// <param name="field"><see cref="Field"/></param>
    public ErrorDetail(string message, JsonElement? resource = null, string? field = null)
        => (Message, Resource, Field) = (message, resource, field);

    /// <summary>メッセージ</summary>
    public string Message { get; }

    /// <summary>リソース項目</summary>
    public JsonElement? Resource { get; }

    /// <summary>フィールド名</summary>
    public string? Field { get; }
}

/// <summary>エラーの原因を表すコード</summary>
[JsonConverter(typeof(JsonStringEnumConverterEx<ErrorType>))]
public enum ErrorType
{
    None = 0,
    /// <summary><see cref="HttpStatusCode.BadRequest"/>: リクエストパラメータの内容が不正</summary>
    [EnumMember(Value = "bad_request")] BadRequest = 1,
    /// <summary><see cref="HttpStatusCode.Unauthorized"/>: 指定したアクセストークンが無効</summary>
    [EnumMember(Value = "unauthorized_token")] UnauthorizedToken = 2,
    /// <summary><see cref="HttpStatusCode.Forbidden"/>: リソースへのアクセスが禁止されている</summary>
    [EnumMember(Value = "forbidden")] Forbidden = 3,
    /// <summary><see cref="HttpStatusCode.NotFound"/>: 指定したリソースが存在しない</summary>
    [EnumMember(Value = "not_found")] NotFound = 4,
    /// <summary><see cref="HttpStatusCode.InternalServerError"/>: システム内部で予期せぬエラーが発生した</summary>
    [EnumMember(Value = "internal_server_error")] InternalServerError = 5,
    /// <summary><see cref="HttpStatusCode"/>429: リクエスト制限の上限を超えた</summary>
    [EnumMember(Value = "too_many_requests")] TooManyRequests = 6,
    /// <summary><see cref="HttpStatusCode.Forbidden"/>: 上限人数に達しているため従業員の追加は出来ません</summary>
    [EnumMember(Value = "plan_limit_exceeded")] PlanLimitExceeded = 7,
    /// <summary><see cref="HttpStatusCode.BadRequest"/>: 削除できないリソースです</summary>
    [EnumMember(Value = "non_deletable_resource")] NonDeletableResource = 8,
    /// <summary><see cref="HttpStatusCode.ServiceUnavailable"/>: メンテナンスを行っています</summary>
    [EnumMember(Value = "service_maintenance")] ServiceMaintenance = 9,
}
