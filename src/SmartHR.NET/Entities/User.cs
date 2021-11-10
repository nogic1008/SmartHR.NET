using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>ユーザ情報</summary>
/// <remarks>
/// SmartHRにログインするアカウントを表します。
/// ユーザは管理者とそれ以外で分けられます。
/// 従業員単体ではSmartHRにログインできませんが、ユーザと紐づけることでログインできるようになります。
/// </remarks>
/// <param name="Id">ユーザID</param>
/// <param name="Email">ログインメールアドレス</param>
/// <param name="IsAdmin">管理者フラグ</param>
/// <param name="Role">権限</param>
/// <param name="AgreementForElectronicDelivery">「給与所得等明細書の電子交付に関する同意」に同意済みか否か</param>
/// <param name="CrewId">紐づく従業員のID。<see cref="Crew"/>が<c>null</c>の場合に出力されます。</param>
/// <param name="Crew">紐づく従業員。<see cref="CrewId"/>が<c>null</c>の場合に出力されます。</param>
/// <param name="Tenants">テナント</param>
/// <param name="InvitationCreatedAt">招待日時</param>
/// <param name="InvitationOpenedAt">招待開封日時</param>
/// <param name="InvitationAcceptedAt">招待承認日時</param>
/// <param name="InvitationAnsweredAt">招待回答日時</param>
/// <param name="SuppressedEmailLogs">メール配信状況</param>
/// <param name="HasPassword">パスワードが設定されているかどうか</param>
/// <param name="CreatedAt">作成日</param>
/// <param name="UpdatedAt">最終更新日</param>
public record User(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("admin")] bool IsAdmin,
    [property: JsonPropertyName("role")] User.UserRole? Role = default,
    [property: JsonPropertyName("agreement_for_electronic_delivery")] bool? AgreementForElectronicDelivery = default,
    [property: JsonPropertyName("crew_id")] string? CrewId = null,
    [property: JsonPropertyName("crew")] Crew? Crew = null,
    [property: JsonPropertyName("tenants")] IReadOnlyList<User.Tenant>? Tenants = null,
    [property: JsonPropertyName("invitation_created_at")] DateTimeOffset? InvitationCreatedAt = default,
    [property: JsonPropertyName("invitation_opened_at")] DateTimeOffset? InvitationOpenedAt = default,
    [property: JsonPropertyName("invitation_accepted_at")] DateTimeOffset? InvitationAcceptedAt = default,
    [property: JsonPropertyName("invitation_answered_at")] DateTimeOffset? InvitationAnsweredAt = default,
    [property: JsonPropertyName("suppressed_email_logs")] IReadOnlyList<User.SuppressedEmailLog>? SuppressedEmailLogs = default,
    [property: JsonPropertyName("has_password")] bool? HasPassword = default,
    [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
    [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
)
{
    /// <summary>
    /// <inheritdoc cref="User" path="/param[@name='Role']/text()"/>
    /// </summary>
    /// <param name="Id">権限ID</param>
    /// <param name="Name">名称</param>
    /// <param name="Description">説明</param>
    /// <param name="CrewsScope">従業員の参照可能範囲</param>
    /// <param name="CrewsScopeQuery">従業員の参照可能範囲の条件</param>
    /// <param name="SessionTimeoutIn">セッション有効期限(分)</param>
    /// <param name="PresetType">プリセット種別</param>
    public record UserRole(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("crews_scope")] CrewsScope CrewsScope,
        [property: JsonPropertyName("crews_scope_query")] JsonElement? CrewsScopeQuery,
        [property: JsonPropertyName("session_timeout_in")] int? SessionTimeoutIn,
        [property: JsonPropertyName("preset_type")] string PresetType
    );

    /// <summary>
    /// <inheritdoc cref="User" path="/param[@name='Tenants']/text()"/>
    /// </summary>
    /// <param name="Id">テナントID</param>
    /// <param name="Name">名前</param>
    /// <param name="Subdomains">利用しているサブドメインの一覧</param>
    /// <param name="CustomerId"></param>
    /// <param name="TrialStartAt">トライアル開始日</param>
    /// <param name="TrialEndAt">トライアル終了日</param>
    /// <param name="AddonSubscribable">アドオンへの課金ができるかどうか</param>
    /// <param name="SubscribedPlan">契約プラン</param>
    /// <param name="GeneralSetting">設定</param>
    /// <param name="CreatedAt">作成日</param>
    /// <param name="UpdatedAt">最終更新日</param>
    public record Tenant(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("subdomains")] IReadOnlyList<Tenant.Subdomain> Subdomains,
        [property: JsonPropertyName("customer_id")] string? CustomerId,
        [property: JsonPropertyName("trial_start_at")] DateTimeOffset? TrialStartAt,
        [property: JsonPropertyName("trial_end_at")] DateTimeOffset? TrialEndAt,
        [property: JsonPropertyName("addon_subscribable")] bool? AddonSubscribable,
        [property: JsonPropertyName("subscribed_plan")] Tenant.Plan? SubscribedPlan,
        [property: JsonPropertyName("general_setting")] Tenant.Setting? GeneralSetting,
        [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
        [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
    )
    {
        /// <summary>サブドメイン</summary>
        /// <param name="Name">サブドメイン名</param>
        /// <param name="Active">アクティブフラグ</param>
        public record Subdomain(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("active")] bool Active
        );

        /// <summary>
        /// <inheritdoc cref="Tenant" path="/param[@name='SubscribedPlan']/text()"/>
        /// </summary>
        /// <param name="Name">契約プラン名</param>
        public record Plan([property: JsonPropertyName("name")] string? Name = null);

        /// <summary>
        /// <inheritdoc cref="Tenant" path="/param[@name='GeneralSetting']/text()"/>
        /// </summary>
        /// <param name="ShowAgreementForElectronicDeliveryInInvitation">招待承認時に「電子交付に関する同意」を入力してもらう</param>
        /// <param name="Multilingualization">多言語化</param>
        public record Setting(
            [property: JsonPropertyName("show_agreement_for_electronic_delivery_in_invitation")] bool ShowAgreementForElectronicDeliveryInInvitation,
            [property: JsonPropertyName("multilingualization")] bool Multilingualization
        );
    }

    /// <summary>
    /// <inheritdoc cref="User" path="/param[@name='SuppressedEmailLogs']/text()"/>
    /// </summary>
    /// <param name="Id">メール配信状況ID</param>
    /// <param name="SuppressionType">種別</param>
    /// <param name="Reason">理由</param>
    /// <param name="SuppressedAt">配信日</param>
    /// <param name="CreatedAt">作成日</param>
    /// <param name="UpdatedAt">最終更新日</param>
    public record SuppressedEmailLog(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("suppression_type")] int? SuppressionType = default,
        [property: JsonPropertyName("reason")] string? Reason = null,
        [property: JsonPropertyName("suppressed_at")] DateTimeOffset? SuppressedAt = default,
        [property: JsonPropertyName("created_at")] DateTimeOffset? CreatedAt = default,
        [property: JsonPropertyName("updated_at")] DateTimeOffset? UpdatedAt = default
    );

    /// <summary>
    /// <inheritdoc cref="UserRole" path="/param[@name='CrewsScope']/text()"/>
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverterEx<CrewsScope>))]
    public enum CrewsScope
    {
        [EnumMember(Value = "crews_scope_all")] All,
        [EnumMember(Value = "crews_scope_own")] Own,
        [EnumMember(Value = "crews_scope_department")] Department,
        [EnumMember(Value = "crews_scope_department_subtree")] DepartmentSubtree,
        [EnumMember(Value = "crews_scope_department_descendant")] DepartmentDescendant,
        [EnumMember(Value = "crews_scope_condition")] Condition,
    }
}
