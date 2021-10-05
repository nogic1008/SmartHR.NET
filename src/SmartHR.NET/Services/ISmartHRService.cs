using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Services;

/// <summary>
/// SmartHR APIを呼び出す機能を提供します。
/// </summary>
public interface ISmartHRService
{
    #region PayRolls
    /// <summary>
    /// <paramref name="id"/>と一致する給与情報を削除します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/deleteV1PayrollsId"/>
    /// </remarks>
    /// <param name="id">給与ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask DeletePayRollAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する給与情報を取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/getV1PayrollsId"/>
    /// </remarks>
    /// <param name="id">給与ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<PayRoll> FetchPayRollAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する給与情報を部分更新します。
    /// </summary>
    /// <param name="id">給与ID</param>
    /// <param name="nameForAdmin">給与明細の名前 (管理者向け)</param>
    /// <param name="nameForCrew">給与明細の名前 (従業員向け)</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="PayRoll"/>オブジェクト。</returns>
    public ValueTask<PayRoll> UpdatePayRollAsync(string id, string nameForAdmin, string nameForCrew, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する給与情報を公開処理します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/patchV1PayrollsIdPublish"/>
    /// </remarks>
    /// <param name="id">給与ID</param>
    /// <param name="publishedAt">
    /// 公開時刻
    /// <list type="bullet">
    /// <item>未指定の場合は即時公開されます。</item>
    /// <item>過去の日時は登録できません。</item>
    /// <item>予約可能な期間は、1ヶ月以内です。</item>
    /// <item>未来の公開時刻が設定されている場合は、更新可能です。</item>
    /// </list>
    /// </param>
    /// <param name="notifyWithPublish">公開と同時に通知を行う</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>公開処理後の<see cref="PayRoll"/>オブジェクト。</returns>
    public ValueTask<PayRoll> PublishPayRollAsync(string id, DateTimeOffset? publishedAt = default, bool? notifyWithPublish = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する給与情報を未確定処理します。
    /// 公開済みの場合は、未公開になります。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/patchV1PayrollsIdUnfix"/>
    /// </remarks>
    /// <param name="id"><see cref="PayRoll.Id"/></param>
    /// <param name="paymentType"><see cref="PayRoll.PaymentType"/></param>
    /// <param name="paidAt"><see cref="PayRoll.PaidAt"/></param>
    /// <param name="periodStartAt"><see cref="PayRoll.PeriodStartAt"/></param>
    /// <param name="periodEndAt"><see cref="PayRoll.PeriodEndAt"/></param>
    /// <param name="status"><see cref="PayRoll.Status"/></param>
    /// <param name="numeralSystemHandleType"><see cref="PayRoll.NumeralSystemHandleType"/></param>
    /// <param name="nameForAdmin"><see cref="PayRoll.NameForAdmin"/></param>
    /// <param name="nameForCrew"><see cref="PayRoll.NameForCrew"/></param>
    /// <param name="publishedAt"><see cref="PayRoll.PublishedAt"/></param>
    /// <param name="notifyWithPublish"><see cref="PayRoll.NotifyWithPublish"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns>未確定処理後の<see cref="PayRoll"/>オブジェクト。</returns>
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する給与情報を確定処理します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/patchV1PayrollsIdFix"/>
    /// </remarks>
    /// <param name="id"><see cref="PayRoll.Id"/></param>
    /// <param name="paymentType"><see cref="PayRoll.PaymentType"/></param>
    /// <param name="paidAt"><see cref="PayRoll.PaidAt"/></param>
    /// <param name="periodStartAt"><see cref="PayRoll.PeriodStartAt"/></param>
    /// <param name="periodEndAt"><see cref="PayRoll.PeriodEndAt"/></param>
    /// <param name="status"><see cref="PayRoll.Status"/></param>
    /// <param name="numeralSystemHandleType"><see cref="PayRoll.NumeralSystemHandleType"/></param>
    /// <param name="nameForAdmin"><see cref="PayRoll.NameForAdmin"/></param>
    /// <param name="nameForCrew"><see cref="PayRoll.NameForCrew"/></param>
    /// <param name="publishedAt"><see cref="PayRoll.PublishedAt"/></param>
    /// <param name="notifyWithPublish"><see cref="PayRoll.NotifyWithPublish"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns>確定処理後の<see cref="PayRoll"/>オブジェクト。</returns>
    public ValueTask<PayRoll> ConfirmPayRollAsync(
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 給与情報をリストで取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/getV1Payrolls"/>
    /// </remarks>
    /// <param name="page">Page of results to fetch.</param>
    /// <param name="perPage">Number of results to return per page.</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>給与情報の一覧</returns>
    public ValueTask<IReadOnlyList<PayRoll>> FetchPayRollListAsync(int page, int perPage, CancellationToken cancellationToken = default);

    /// <summary>
    /// 給与情報を新規登録します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/postV1Payrolls"/>
    /// </remarks>
    /// <param name="paymentType"><see cref="PayRoll.PaymentType"/></param>
    /// <param name="paidAt"><see cref="PayRoll.PaidAt"/></param>
    /// <param name="periodStartAt"><see cref="PayRoll.PeriodStartAt"/></param>
    /// <param name="periodEndAt"><see cref="PayRoll.PeriodEndAt"/></param>
    /// <param name="numeralSystemHandleType"><see cref="PayRoll.NumeralSystemHandleType"/></param>
    /// <param name="nameForAdmin"><see cref="PayRoll.NameForAdmin"/></param>
    /// <param name="nameForCrew"><see cref="PayRoll.NameForCrew"/></param>
    /// <returns>登録処理後の<see cref="PayRoll"/>オブジェクト。</returns>
    public ValueTask<PayRoll> AddPayRollAsync(
        PaymentType paymentType,
        DateTime paidAt,
        DateTime periodStartAt,
        DateTime periodEndAt,
        NumeralSystemType numeralSystemHandleType,
        string nameForAdmin,
        string nameForCrew,
        CancellationToken cancellationToken = default);
    #endregion
}
