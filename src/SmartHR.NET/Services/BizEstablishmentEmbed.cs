namespace SmartHR.NET.Services;

/// <summary>
/// <see cref="ISmartHRService.FetchBizEstablishmentListAsync"/>のリクエストパラメータ
/// </summary>
public enum BizEstablishmentEmbed
{
    /// <summary>ユーザー情報を埋め込まない</summary>
    None = 0,
    /// <summary>社会保険代表者のユーザー情報を埋め込む</summary>
    SocInsOwner,
    /// <summary>労働保険代表者のユーザー情報を埋め込む</summary>
    LabInsOwner,
}
