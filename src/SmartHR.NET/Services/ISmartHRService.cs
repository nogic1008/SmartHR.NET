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
    #region PaymentPeriods
    /// <summary>
    /// <paramref name="id"/>と一致する給与支給形態情報を取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E%E6%94%AF%E7%B5%A6%E5%BD%A2%E6%85%8B/getV1PaymentPeriodsId"/>
    /// </remarks>
    /// <param name="id">給与支給形態のID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<PaymentPeriod> FetchPaymentPeriodAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 給与支給形態をリストで取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E%E6%94%AF%E7%B5%A6%E5%BD%A2%E6%85%8B/getV1PaymentPeriods"/>
    /// </remarks>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>給与支給形態の一覧</returns>
    public ValueTask<IReadOnlyList<PaymentPeriod>> FetchPaymentPeriodListAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);
    #endregion

    #region JobTitles
    /// <summary>
    /// <paramref name="id"/>と一致する役職情報を削除します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E5%BD%B9%E8%81%B7/deleteV1JobTitlesId"/>
    /// </remarks>
    /// <param name="id">役職ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask DeleteJobTitleAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する役職情報を取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E5%BD%B9%E8%81%B7/getV1JobTitlesId"/>
    /// </remarks>
    /// <param name="id">役職ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<JobTitle> FetchJobTitleAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する役職情報を部分更新します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E5%BD%B9%E8%81%B7/patchV1JobTitlesId"/>
    /// </remarks>
    /// <param name="id">役職ID</param>
    /// <param name="name">役職の名前</param>
    /// <param name="rank">役職のランク (1 ~ 99999)</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="JobTitle"/>オブジェクト。</returns>
    public ValueTask<JobTitle> UpdateJobTitleAsync(string id, string? name = null, int? rank = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する役職情報を更新します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E5%BD%B9%E8%81%B7/putV1JobTitlesId"/>
    /// </remarks>
    /// <param name="id">役職ID</param>
    /// <param name="name">役職の名前</param>
    /// <param name="rank">役職のランク (1 ~ 99999)</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="JobTitle"/>オブジェクト。</returns>
    public ValueTask<JobTitle> ReplaceJobTitleAsync(string id, string name, int? rank = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// 役職情報をリストで取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E5%BD%B9%E8%81%B7/getV1JobTitles"/>
    /// </remarks>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>役職情報の一覧</returns>
    public ValueTask<IReadOnlyList<JobTitle>> FetchJobTitleListAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// 役職情報を新規登録します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E5%BD%B9%E8%81%B7/postV1JobTitles"/>
    /// </remarks>
    /// <param name="name">役職の名前</param>
    /// <param name="rank">役職のランク (1 ~ 99999)</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>登録処理後の<see cref="JobTitle"/>オブジェクト。</returns>
    public ValueTask<JobTitle> AddJobTitleAsync(string name, int? rank = default, CancellationToken cancellationToken = default);
    #endregion

    #region BankAccountSettings
    /// <summary>
    /// 口座情報をリストで取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E5%8F%A3%E5%BA%A7%E6%83%85%E5%A0%B1/getV1BankAccountSettings"/>
    /// </remarks>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>口座情報の一覧</returns>
    public ValueTask<IReadOnlyList<BankAccountSetting>> FetchBankAccountSettingListAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);
    #endregion

    #region Payrolls
    /// <summary>
    /// <paramref name="id"/>と一致する給与情報を削除します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/deleteV1PayrollsId"/>
    /// </remarks>
    /// <param name="id">給与ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask DeletePayrollAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する給与情報を取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/getV1PayrollsId"/>
    /// </remarks>
    /// <param name="id">給与ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<Payroll> FetchPayrollAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する給与情報を部分更新します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/patchV1PayrollsId"/>
    /// </remarks>
    /// <param name="id">給与ID</param>
    /// <param name="nameForAdmin">給与明細の名前 (管理者向け)</param>
    /// <param name="nameForCrew">給与明細の名前 (従業員向け)</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="Payroll"/>オブジェクト。</returns>
    public ValueTask<Payroll> UpdatePayrollAsync(string id, string nameForAdmin, string nameForCrew, CancellationToken cancellationToken = default);

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
    /// <returns>公開処理後の<see cref="Payroll"/>オブジェクト。</returns>
    public ValueTask<Payroll> PublishPayrollAsync(string id, DateTimeOffset? publishedAt = default, bool? notifyWithPublish = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する給与情報を未確定処理します。
    /// 公開済みの場合は、未公開になります。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/patchV1PayrollsIdUnfix"/>
    /// </remarks>
    /// <param name="id"><see cref="Payroll.Id"/></param>
    /// <param name="paymentType"><see cref="Payroll.PaymentType"/></param>
    /// <param name="paidAt"><see cref="Payroll.PaidAt"/></param>
    /// <param name="periodStartAt"><see cref="Payroll.PeriodStartAt"/></param>
    /// <param name="periodEndAt"><see cref="Payroll.PeriodEndAt"/></param>
    /// <param name="status"><see cref="Payroll.Status"/></param>
    /// <param name="numeralSystemHandleType"><see cref="Payroll.NumeralSystemHandleType"/></param>
    /// <param name="nameForAdmin"><see cref="Payroll.NameForAdmin"/></param>
    /// <param name="nameForCrew"><see cref="Payroll.NameForCrew"/></param>
    /// <param name="publishedAt"><see cref="Payroll.PublishedAt"/></param>
    /// <param name="notifyWithPublish"><see cref="Payroll.NotifyWithPublish"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns>未確定処理後の<see cref="Payroll"/>オブジェクト。</returns>
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する給与情報を確定処理します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/patchV1PayrollsIdFix"/>
    /// </remarks>
    /// <param name="id"><see cref="Payroll.Id"/></param>
    /// <param name="paymentType"><see cref="Payroll.PaymentType"/></param>
    /// <param name="paidAt"><see cref="Payroll.PaidAt"/></param>
    /// <param name="periodStartAt"><see cref="Payroll.PeriodStartAt"/></param>
    /// <param name="periodEndAt"><see cref="Payroll.PeriodEndAt"/></param>
    /// <param name="status"><see cref="Payroll.Status"/></param>
    /// <param name="numeralSystemHandleType"><see cref="Payroll.NumeralSystemHandleType"/></param>
    /// <param name="nameForAdmin"><see cref="Payroll.NameForAdmin"/></param>
    /// <param name="nameForCrew"><see cref="Payroll.NameForCrew"/></param>
    /// <param name="publishedAt"><see cref="Payroll.PublishedAt"/></param>
    /// <param name="notifyWithPublish"><see cref="Payroll.NotifyWithPublish"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns>確定処理後の<see cref="Payroll"/>オブジェクト。</returns>
    public ValueTask<Payroll> ConfirmPayrollAsync(
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 給与情報をリストで取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/getV1Payrolls"/>
    /// </remarks>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>給与情報の一覧</returns>
    public ValueTask<IReadOnlyList<Payroll>> FetchPayrollListAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// 給与情報を新規登録します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E/postV1Payrolls"/>
    /// </remarks>
    /// <param name="paymentType"><see cref="Payroll.PaymentType"/></param>
    /// <param name="paidAt"><see cref="Payroll.PaidAt"/></param>
    /// <param name="periodStartAt"><see cref="Payroll.PeriodStartAt"/></param>
    /// <param name="periodEndAt"><see cref="Payroll.PeriodEndAt"/></param>
    /// <param name="numeralSystemHandleType"><see cref="Payroll.NumeralSystemHandleType"/></param>
    /// <param name="nameForAdmin"><see cref="Payroll.NameForAdmin"/></param>
    /// <param name="nameForCrew"><see cref="Payroll.NameForCrew"/></param>
    /// <returns>登録処理後の<see cref="Payroll"/>オブジェクト。</returns>
    public ValueTask<Payroll> AddPayrollAsync(
        Payroll.Payment paymentType,
        DateTime paidAt,
        DateTime periodStartAt,
        DateTime periodEndAt,
        Payroll.NumeralSystem numeralSystemHandleType,
        string nameForAdmin,
        string nameForCrew,
        CancellationToken cancellationToken = default);
    #endregion

    #region Payslips
    /// <summary>
    /// <paramref name="id"/>に一致する給与明細情報を削除します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E%E6%98%8E%E7%B4%B0/deleteV1PayrollsPayrollIdPayslipsId"/>
    /// </remarks>
    /// <param name="payrollId">給与ID</param>
    /// <param name="id">給与明細ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask DeletePayslipAsync(string payrollId, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>に一致する給与明細情報を取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E%E6%98%8E%E7%B4%B0/getV1PayrollsPayrollIdPayslipsId"/>
    /// </remarks>
    /// <param name="payrollId">給与ID</param>
    /// <param name="id">給与明細ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<Payslip> FetchPayslipAsync(string payrollId, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 給与明細情報を一括登録します。
    /// <para>最大100件まで一括登録可能です。</para>
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E%E6%98%8E%E7%B4%B0/postV1PayrollsPayrollIdPayslipsBulk"/>
    /// </remarks>
    /// <param name="payrollId">給与ID</param>
    /// <param name="payload">給与明細情報の一覧</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>一括登録後の<see cref="Payslip"/>オブジェクト</returns>
    public ValueTask<Payslip> AddPayslipListAsync(string payrollId, IReadOnlyList<PayslipRequest> payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// 給与明細情報をリストで取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E%E6%98%8E%E7%B4%B0/getV1PayrollsPayrollIdPayslips"/>
    /// </remarks>
    /// <param name="payrollId">給与ID</param>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<IReadOnlyList<Payslip>> FetchPayslipListAsync(string payrollId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// 給与明細情報を登録します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E7%B5%A6%E4%B8%8E%E6%98%8E%E7%B4%B0/postV1PayrollsPayrollIdPayslips"/>
    /// </remarks>
    /// <param name="payrollId">給与ID</param>
    /// <param name="payload">給与明細情報</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>登録後の<see cref="Payslip"/>オブジェクト</returns>
    public ValueTask<Payslip> AddPayslipAsync(string payrollId, PayslipRequest payload, CancellationToken cancellationToken = default);
    #endregion
}
