using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities.Webhook;

/// <summary>
/// SmartHR サーバのWebhookよりPOSTされるデータの構造
/// </summary>
public interface IWebhookObject
{
    /// <summary>発動したイベントのID</summary>
    public WebhookEvent Event { get; init; }

    /// <summary>イベントが発動した日時</summary>
    public DateTimeOffset TriggeredAt { get; init; }

    /// <summary>
    /// イベントを発動させた従業員。
    /// 従業員データと紐付かないアカウントの場合は<c>null</c>となります。
    /// </summary>
    public Crew? Sender { get; init; }
}

[JsonConverter(typeof(JsonStringEnumConverterEx<WebhookEvent>))]
public enum WebhookEvent
{
    None,
    /// <summary>従業員データが作成されたタイミングで発動</summary>
    [EnumMember(Value = "crew_created")] CrewCreated,
    /// <summary>従業員データが更新されたタイミングで発動</summary>
    [EnumMember(Value = "crew_updated")] CrewUpdated,
    /// <summary>従業員データが完全に削除されたタイミングで発動</summary>
    [EnumMember(Value = "crew_deleted")] CrewDeleted,
    /// <summary>従業員データがファイルを使って取り込まれたタイミングで発動</summary>
    [EnumMember(Value = "crew_imported")] CrewImported,
    /// <summary>家族データが作成されたタイミングで発動</summary>
    [EnumMember(Value = "dependent_created")] DependentCreated,
    /// <summary>家族データが更新されたタイミングで発動</summary>
    [EnumMember(Value = "dependent_updated")] DependentUpdated,
    /// <summary>家族データが完全に削除されたタイミングで発動</summary>
    [EnumMember(Value = "dependent_deleted")] DependentDeleted,
    /// <summary>家族データがファイルを使って取り込まれたタイミングで発動</summary>
    [EnumMember(Value = "dependent_imported")] DependentImported,
}
