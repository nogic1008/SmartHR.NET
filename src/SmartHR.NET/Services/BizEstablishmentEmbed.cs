using SmartHR.NET.Entities;

namespace SmartHR.NET.Services;

/// <summary>
/// <see cref="ISmartHRService.FetchBizEstablishmentListAsync"/>の取得オプション
/// </summary>
public enum BizEstablishmentEmbed
{
    /// <summary>
    /// <list type="bullet">
    /// <item><see cref="BizEstablishment.SocInsOwner"/>を取得しない</item>
    /// <item><see cref="BizEstablishment.LabInsOwner"/>を取得しない</item>
    /// </list>
    /// </summary>
    None,
    /// <summary>
    /// <list type="bullet">
    /// <item><see cref="BizEstablishment.SocInsOwner"/>を取得する(<see cref="BizEstablishment.SocInsOwnerId"/>を含めない)</item>
    /// <item><see cref="BizEstablishment.LabInsOwner"/>を取得しない</item>
    /// </list>
    /// </summary>
    SocInsOwner,
    /// <summary>
    /// <list type="bullet">
    /// <item><see cref="BizEstablishment.SocInsOwner"/>を取得しない</item>
    /// <item><see cref="BizEstablishment.LabInsOwner"/>を取得する(<see cref="BizEstablishment.LabInsOwnerId"/>を含めない)</item>
    /// </list>
    /// </summary>
    LabInsOwner,
}
