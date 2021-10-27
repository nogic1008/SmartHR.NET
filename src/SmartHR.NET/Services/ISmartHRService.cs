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
    #region 従業員カスタム項目グループ
    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="CrewCustomFieldTemplateGroup" path="/summary/text()"/>を削除します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE%E3%82%B0%E3%83%AB%E3%83%BC%E3%83%97/deleteV1CrewCustomFieldTemplateGroupsId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='Id']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask DeleteCrewCustomFieldTemplateGroupAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="CrewCustomFieldTemplateGroup" path="/summary/text()"/>を取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE%E3%82%B0%E3%83%AB%E3%83%BC%E3%83%97/getV1CrewCustomFieldTemplateGroupsId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='Id']"/></param>
    /// <param name="includeTemplates"><see cref="CrewCustomFieldTemplateGroup.Templates"/>を含めるか</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<CrewCustomFieldTemplateGroup> FetchCrewCustomFieldTemplateGroupAsync(string id, bool includeTemplates = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="CrewCustomFieldTemplateGroup" path="/summary/text()"/>を部分更新します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE%E3%82%B0%E3%83%AB%E3%83%BC%E3%83%97/patchV1CrewCustomFieldTemplateGroupsId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='Id']"/></param>
    /// <param name="name"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='Name']/text()"/></param>
    /// <param name="position"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='Position']/text()"/></param>
    /// <param name="accessType"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='AccessType']/text()"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="CrewCustomFieldTemplateGroup"/>オブジェクト。</returns>
    public ValueTask<CrewCustomFieldTemplateGroup> UpdateCrewCustomFieldTemplateGroupAsync(string id, string? name = null, int? position = default, CrewCustomFieldTemplateGroup.Accessibility? accessType = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="CrewCustomFieldTemplateGroup" path="/summary/text()"/>を更新します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE%E3%82%B0%E3%83%AB%E3%83%BC%E3%83%97/putV1CrewCustomFieldTemplateGroupsId"/>
    /// </summary>
    /// <remarks>
    /// 未指定の属性は情報が削除されます。
    /// 未指定の属性を消したくない場合は<see cref="UpdateCrewCustomFieldTemplateGroupAsync"/>をご利用ください。
    /// </remarks>
    /// <param name="id"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='Id']"/></param>
    /// <param name="name"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='Name']/text()"/></param>
    /// <param name="position"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='Position']/text()"/></param>
    /// <param name="accessType"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='AccessType']/text()"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="CrewCustomFieldTemplateGroup"/>オブジェクト。</returns>
    public ValueTask<CrewCustomFieldTemplateGroup> ReplaceCrewCustomFieldTemplateGroupAsync(string id, string name, int? position = default, CrewCustomFieldTemplateGroup.Accessibility? accessType = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="CrewCustomFieldTemplateGroup" path="/summary/text()"/>をリストで取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE%E3%82%B0%E3%83%AB%E3%83%BC%E3%83%97/getV1CrewCustomFieldTemplateGroups"/>
    /// </summary>
    /// <param name="includeTemplates"><see cref="CrewCustomFieldTemplateGroup.Templates"/>を含めるか</param>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/summary/text()"/>の一覧</returns>
    public ValueTask<IReadOnlyList<CrewCustomFieldTemplateGroup>> FetchCrewCustomFieldTemplateGroupListAsync(bool includeTemplates = false, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="CrewCustomFieldTemplateGroup" path="/summary/text()"/>を新規登録します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE%E3%82%B0%E3%83%AB%E3%83%BC%E3%83%97/postV1CrewCustomFieldTemplateGroups"/>
    /// </summary>
    /// <param name="name"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='Name']/text()"/></param>
    /// <param name="position"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='Position']/text()"/></param>
    /// <param name="accessType"><inheritdoc cref="CrewCustomFieldTemplateGroup" path="/param[@name='AccessType']/text()"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>登録処理後の<see cref="CrewCustomFieldTemplateGroup"/>オブジェクト。</returns>
    public ValueTask<CrewCustomFieldTemplateGroup> AddCrewCustomFieldTemplateGroupAsync(string name, int? position = default, CrewCustomFieldTemplateGroup.Accessibility? accessType = default, CancellationToken cancellationToken = default);
    #endregion

    #region 従業員カスタム項目テンプレート
    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/>を削除します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE%E3%83%86%E3%83%B3%E3%83%97%E3%83%AC%E3%83%BC%E3%83%88/deleteV1CrewCustomFieldTemplatesId"/>
    /// </summary>
    /// <remarks>テンプレートに対して設定されている値がすべて削除されます。</remarks>
    /// <param name="id"><inheritdoc cref="CrewCustomFieldTemplate" path="/param[@name='Id']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask DeleteCrewCustomFieldTemplateAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/>を取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE%E3%83%86%E3%83%B3%E3%83%97%E3%83%AC%E3%83%BC%E3%83%88/getV1CrewCustomFieldTemplatesId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="CrewCustomFieldTemplate" path="/param[@name='Id']"/></param>
    /// <param name="includeGroup"><see cref="CrewCustomFieldTemplate.Group"/>を含めるか</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<CrewCustomFieldTemplate> FetchCrewCustomFieldTemplateAsync(string id, bool includeGroup = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/>を部分更新します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE%E3%83%86%E3%83%B3%E3%83%97%E3%83%AC%E3%83%BC%E3%83%88/patchV1CrewCustomFieldTemplatesId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="CrewCustomFieldTemplate" path="/param[@name='Id']"/></param>
    /// <param name="payload"><inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="CrewCustomFieldTemplate"/>オブジェクト。</returns>
    public ValueTask<CrewCustomFieldTemplate> UpdateCrewCustomFieldTemplateAsync(string id, CrewCustomFieldTemplatePayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/>を更新します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE%E3%83%86%E3%83%B3%E3%83%97%E3%83%AC%E3%83%BC%E3%83%88/putV1CrewCustomFieldTemplatesId"/>
    /// </summary>
    /// <remarks>
    /// 未指定の属性は情報が削除されます。
    /// 未指定の属性を消したくない場合は<see cref="UpdateCrewCustomFieldTemplateAsync"/>をご利用ください。
    /// </remarks>
    /// <param name="id"><inheritdoc cref="CrewCustomFieldTemplate" path="/param[@name='Id']"/></param>
    /// <param name="payload"><inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="CrewCustomFieldTemplate"/>オブジェクト。</returns>
    public ValueTask<CrewCustomFieldTemplate> ReplaceCrewCustomFieldTemplateAsync(string id, CrewCustomFieldTemplatePayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/>をリストで取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE%E3%83%86%E3%83%B3%E3%83%97%E3%83%AC%E3%83%BC%E3%83%88/getV1CrewCustomFieldTemplates"/>
    /// </summary>
    /// <param name="includeGroup"><see cref="CrewCustomFieldTemplate.Group"/>を含めるか</param>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/>の一覧</returns>
    public ValueTask<IReadOnlyList<CrewCustomFieldTemplate>> FetchCrewCustomFieldTemplateListAsync(bool includeGroup = false, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/>を新規登録します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%A0%E9%A0%85%E7%9B%AE%E3%83%86%E3%83%B3%E3%83%97%E3%83%AC%E3%83%BC%E3%83%88/postV1CrewCustomFieldTemplates"/>
    /// </summary>
    /// <param name="payload"><inheritdoc cref="CrewCustomFieldTemplate" path="/summary/text()"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>登録処理後の<see cref="CrewCustomFieldTemplate"/>オブジェクト。</returns>
    public ValueTask<CrewCustomFieldTemplate> AddCrewCustomFieldTemplateAsync(CrewCustomFieldTemplatePayload payload, CancellationToken cancellationToken = default);
    #endregion

    #region 部署
    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="Department" path="/summary/text()"/>を削除します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E9%83%A8%E7%BD%B2/deleteV1DepartmentsId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="Department" path="/param[@name='Id']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask DeleteDepartmentAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="Department" path="/summary/text()"/>を取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E9%83%A8%E7%BD%B2/getV1DepartmentsId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="Department" path="/param[@name='Id']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<Department> FetchDepartmentAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="Department" path="/summary/text()"/>を部分更新します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E9%83%A8%E7%BD%B2/patchV1DepartmentsId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="Department" path="/param[@name='Id']"/></param>
    /// <param name="name">
    /// <inheritdoc cref="Department" path="/param[@name='Name']"/>
    /// <c>"/"</c>を含められません。
    /// </param>
    /// <param name="position"><inheritdoc cref="Department" path="/param[@name='Position']"/></param>
    /// <param name="code"><inheritdoc cref="Department" path="/param[@name='Code']"/></param>
    /// <param name="parentId"><inheritdoc cref="Department" path="/param[@name='Parent']"/>ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="Department"/>オブジェクト。</returns>
    public ValueTask<Department> UpdateDepartmentAsync(string id, string? name = null, int? position = default, string? code = null, string? parentId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="Department" path="/summary/text()"/>を更新します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E9%83%A8%E7%BD%B2/putV1DepartmentsId"/>
    /// </summary>
    /// <remarks>
    /// 未指定の属性は情報が削除されます。
    /// 未指定の属性を消したくない場合は<see cref="UpdateDepartmentAsync"/>をご利用ください。
    /// </remarks>
    /// <param name="id"><inheritdoc cref="Department" path="/param[@name='Id']"/></param>
    /// <param name="name">
    /// <inheritdoc cref="Department" path="/param[@name='Name']"/>
    /// <c>"/"</c>を含められません。
    /// </param>
    /// <param name="position"><inheritdoc cref="Department" path="/param[@name='Position']"/></param>
    /// <param name="code"><inheritdoc cref="Department" path="/param[@name='Code']"/></param>
    /// <param name="parentId"><inheritdoc cref="Department" path="/param[@name='Parent']"/>ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="Department"/>オブジェクト。</returns>
    public ValueTask<Department> ReplaceDepartmentAsync(string id, string name, int position, string? code = null, string? parentId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="Department" path="/summary/text()"/>をリストで取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E9%83%A8%E7%BD%B2/getV1Departments"/>
    /// </summary>
    /// <param name="code"><inheritdoc cref="Department" path="/param[@name='Code']"/></param>
    /// <param name="sortBy">
    /// 並び順
    /// <see href="https://developer.smarthr.jp/api/index.html#!/overview/sort"/>
    /// </param>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="Department" path="/summary/text()"/>の一覧</returns>
    public ValueTask<IReadOnlyList<Department>> FetchDepartmentListAsync(string? code = null, string? sortBy = null, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="Department" path="/summary/text()"/>を新規登録します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E9%83%A8%E7%BD%B2/postV1Departments"/>
    /// </summary>
    /// <param name="name">
    /// <inheritdoc cref="Department" path="/param[@name='Name']"/>
    /// <c>"/"</c>を含められません。
    /// </param>
    /// <param name="position">
    /// <inheritdoc cref="Department" path="/param[@name='Position']"/>
    /// 未指定時には、自動で採番されます。
    /// </param>
    /// <param name="code"><inheritdoc cref="Department" path="/param[@name='Code']"/></param>
    /// <param name="parentId"><inheritdoc cref="Department" path="/param[@name='Parent']"/>ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>登録処理後の<see cref="Department"/>オブジェクト。</returns>
    public ValueTask<Department> AddDepartmentAsync(string name, int? position = null, string? code = null, string? parentId = null, CancellationToken cancellationToken = default);
    #endregion

    #region 雇用形態
    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="EmploymentType" path="/summary/text()"/>を削除します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E9%9B%87%E7%94%A8%E5%BD%A2%E6%85%8B/deleteV1EmploymentTypesId"/>
    /// </summary>
    /// <remarks>従業員と紐付いている雇用形態の削除はできません。</remarks>
    /// <param name="id"><inheritdoc cref="EmploymentType" path="/param[@name='Id']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask DeleteEmploymentTypeAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="EmploymentType" path="/summary/text()"/>を取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E9%9B%87%E7%94%A8%E5%BD%A2%E6%85%8B/getV1EmploymentTypesId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="EmploymentType" path="/param[@name='Id']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<EmploymentType> FetchEmploymentTypeAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="EmploymentType" path="/summary/text()"/>を部分更新します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E9%9B%87%E7%94%A8%E5%BD%A2%E6%85%8B/patchV1EmploymentTypesId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="EmploymentType" path="/param[@name='Id']"/></param>
    /// <param name="name"><inheritdoc cref="EmploymentType" path="/param[@name='Name']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="EmploymentType"/>オブジェクト。</returns>
    public ValueTask<EmploymentType> UpdateEmploymentTypeAsync(string id, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="EmploymentType" path="/summary/text()"/>を更新します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E9%9B%87%E7%94%A8%E5%BD%A2%E6%85%8B/putV1EmploymentTypesId"/>
    /// </summary>
    /// <remarks>
    /// 未指定の属性は情報が削除されます。
    /// 未指定の属性を消したくない場合は<see cref="UpdateEmploymentTypeAsync"/>をご利用ください。
    /// </remarks>
    /// <param name="id"><inheritdoc cref="EmploymentType" path="/param[@name='Id']"/></param>
    /// <param name="name"><inheritdoc cref="EmploymentType" path="/param[@name='Name']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="EmploymentType"/>オブジェクト。</returns>
    public ValueTask<EmploymentType> ReplaceEmploymentTypeAsync(string id, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="EmploymentType" path="/summary/text()"/>をリストで取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E9%9B%87%E7%94%A8%E5%BD%A2%E6%85%8B/getV1EmploymentTypes"/>
    /// </summary>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="EmploymentType" path="/summary/text()"/>の一覧</returns>
    public ValueTask<IReadOnlyList<EmploymentType>> FetchEmploymentTypeListAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="EmploymentType" path="/summary/text()"/>を新規登録します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E9%9B%87%E7%94%A8%E5%BD%A2%E6%85%8B/postV1EmploymentTypes"/>
    /// </summary>
    /// <param name="name"><inheritdoc cref="EmploymentType" path="/param[@name='Name']"/></param>
    /// <param name="presetType"><inheritdoc cref="EmploymentType" path="/param[@name='PresetType']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>登録処理後の<see cref="EmploymentType"/>オブジェクト。</returns>
    public ValueTask<EmploymentType> AddEmploymentTypeAsync(string name, EmploymentType.Preset? presetType = default, CancellationToken cancellationToken = default);
    #endregion

    #region 従業員情報収集フォーム
    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="CrewInputForm" path="/summary/text()"/>を取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E6%83%85%E5%A0%B1%E5%8F%8E%E9%9B%86%E3%83%95%E3%82%A9%E3%83%BC%E3%83%A0/getV1CrewInputFormsId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="CrewInputForm" path="/param[@name='Id']"/></param>
    /// <param name="embed">埋め込むオブジェクト</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<CrewInputForm> FetchCrewInputFormAsync(string id, CrewInputFormEmbed embed = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="CrewInputForm" path="/summary/text()"/>をリストで取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E5%BE%93%E6%A5%AD%E5%93%A1%E6%83%85%E5%A0%B1%E5%8F%8E%E9%9B%86%E3%83%95%E3%82%A9%E3%83%BC%E3%83%A0/getV1CrewInputForms"/>
    /// </summary>
    /// <param name="embed">埋め込むオブジェクト</param>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="CrewInputForm" path="/summary/text()"/>の一覧</returns>
    public ValueTask<IReadOnlyList<CrewInputForm>> FetchCrewInputFormListAsync(CrewInputFormEmbed embed = 0, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);
    #endregion

    #region メールフォーマット
    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="MailFormat" path="/summary/text()"/>を取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E3%83%A1%E3%83%BC%E3%83%AB%E3%83%95%E3%82%A9%E3%83%BC%E3%83%9E%E3%83%83%E3%83%88/getV1MailFormatsId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="MailFormat" path="/param[@name='Id']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<MailFormat> FetchMailFormatAsync(string id, bool includeCrewInputForms = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="MailFormat" path="/summary/text()"/>をリストで取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/%E3%83%A1%E3%83%BC%E3%83%AB%E3%83%95%E3%82%A9%E3%83%BC%E3%83%9E%E3%83%83%E3%83%88/getV1MailFormats"/>
    /// </summary>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="MailFormat" path="/summary/text()"/>の一覧</returns>
    public ValueTask<IReadOnlyList<MailFormat>> FetchMailFormatListAsync(bool includeCrewInputForms = false, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);
    #endregion

    #region ユーザ
    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="User" path="/summary/text()"/>を取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E3%83%A6%E3%83%BC%E3%82%B6/getV1UsersId"/>
    /// </remarks>
    /// <param name="id"><inheritdoc cref="User" path="/param[@name='Id']"/></param>
    /// <param name="includeCrewInfo"><see cref="User.Crew"/>を含めるか</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<User> FetchUserAsync(string id, bool includeCrewInfo = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="User" path="/summary/text()"/>をリストで取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E3%83%A6%E3%83%BC%E3%82%B6/getV1Users"/>
    /// </remarks>
    /// <param name="includeCrewInfo"><see cref="User.Crew"/>を含めるか</param>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="User" path="/summary/text()"/>の一覧</returns>
    public ValueTask<IReadOnlyList<User>> FetchUserListAsync(bool includeCrewInfo = false, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);
    #endregion

    #region 事業所
    /// <summary>
    /// <inheritdoc cref="BizEstablishment" path="/summary/text()"/>をリストで取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E4%BA%8B%E6%A5%AD%E6%89%80/getV1BizEstablishments"/>
    /// </remarks>
    /// <param name="embed"><see cref="BizEstablishment.SocInsOwner"/>および<see cref="BizEstablishment.LabInsOwner"/>を取得するか</param>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="BizEstablishment" path="/summary/text()"/>の一覧</returns>
    public ValueTask<IReadOnlyList<BizEstablishment>> FetchBizEstablishmentListAsync(BizEstablishmentEmbed embed = 0, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);
    #endregion

    #region Webhook
    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="Webhook" path="/summary/text()"/>を削除します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/Webhook/deleteV1WebhooksId"/>
    /// </remarks>
    /// <param name="id"><inheritdoc cref="Webhook" path="/param[@name='Id']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask DeleteWebhookAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="Webhook" path="/summary/text()"/>を取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/Webhook/getV1WebhooksId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="Webhook" path="/param[@name='Id']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<Webhook> FetchWebhookAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="Webhook" path="/summary/text()"/>を部分更新します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/Webhook/patchV1WebhooksId"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="Webhook" path="/param[@name='Id']"/></param>
    /// <param name="payload"><inheritdoc cref="Webhook" path="/summary/text()"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="Webhook"/>オブジェクト。</returns>
    public ValueTask<Webhook> UpdateWebhookAsync(string id, WebhookPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="Webhook" path="/summary/text()"/>を更新します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/Webhook/putV1WebhooksId"/>
    /// </summary>
    /// <remarks>
    /// 未指定の属性は情報が削除されます。
    /// 未指定の属性を消したくない場合は<see cref="UpdateWebhookAsync"/>をご利用ください。
    /// </remarks>
    /// <param name="id"><inheritdoc cref="Webhook" path="/param[@name='Id']"/></param>
    /// <param name="payload"><inheritdoc cref="Webhook" path="/summary/text()"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="Webhook"/>オブジェクト。</returns>
    public ValueTask<Webhook> ReplaceWebhookAsync(string id, WebhookPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="Webhook" path="/summary/text()"/>をリストで取得します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/Webhook/getV1Webhooks"/>
    /// </summary>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="Webhook" path="/summary/text()"/>の一覧</returns>
    public ValueTask<IReadOnlyList<Webhook>> FetchWebhookListAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="Webhook" path="/summary/text()"/>を新規登録します。
    /// <see href="https://developer.smarthr.jp/api/index.html#!/Webhook/postV1Webhooks"/>
    /// </summary>
    /// <param name="payload"><inheritdoc cref="Webhook" path="/summary/text()"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>登録処理後の<see cref="Webhook"/>オブジェクト。</returns>
    public ValueTask<Webhook> AddWebhookAsync(WebhookPayload payload, CancellationToken cancellationToken = default);
    #endregion

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
    public ValueTask<IReadOnlyList<DependentRelation>> FetchDependentRelationListAsync(bool spouse = false, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);
    #endregion

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

    #region TaxWithholdings
    /// <summary>
    /// <paramref name="id"/>と一致する源泉徴収情報を削除します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E6%BA%90%E6%B3%89%E5%BE%B4%E5%8F%8E/deleteV1TaxWithholdingsId"/>
    /// </remarks>
    /// <param name="id">源泉徴収票ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask DeleteTaxWithholdingAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する源泉徴収情報を取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E6%BA%90%E6%B3%89%E5%BE%B4%E5%8F%8E/getV1TaxWithholdingsId"/>
    /// </remarks>
    /// <param name="id">源泉徴収票ID</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public ValueTask<TaxWithholding> FetchTaxWithholdingAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する源泉徴収情報を部分更新します。
    /// なお、確定後の源泉徴収の更新はできません。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E6%BA%90%E6%B3%89%E5%BE%B4%E5%8F%8E/patchV1TaxWithholdingsId"/>
    /// </remarks>
    /// <param name="id">源泉徴収票ID</param>
    /// <param name="name">名前</param>
    /// <param name="status">ステータス</param>
    /// <param name="year">源泉徴収票に印字される年</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="TaxWithholding"/>オブジェクト。</returns>
    public ValueTask<TaxWithholding> UpdateTaxWithholdingAsync(string id, string? name = null, TaxWithholding.FormStatus? status = default, string? year = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する源泉徴収情報を更新します。
    /// なお、確定後の源泉徴収の更新はできません。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E6%BA%90%E6%B3%89%E5%BE%B4%E5%8F%8E/putV1TaxWithholdingsId"/>
    /// </remarks>
    /// <param name="id">源泉徴収票ID</param>
    /// <param name="name">名前</param>
    /// <param name="status">ステータス</param>
    /// <param name="year">源泉徴収票に印字される年</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>更新処理後の<see cref="TaxWithholding"/>オブジェクト。</returns>
    public ValueTask<TaxWithholding> ReplaceTaxWithholdingAsync(string id, string name, TaxWithholding.FormStatus status, string year, CancellationToken cancellationToken = default);

    /// <summary>
    /// 源泉徴収情報をリストで取得します。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E6%BA%90%E6%B3%89%E5%BE%B4%E5%8F%8E/getV1TaxWithholdings"/>
    /// </remarks>
    /// <param name="page">1から始まるページ番号</param>
    /// <param name="perPage">1ページあたりに含まれる要素数</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>源泉徴収情報の一覧</returns>
    public ValueTask<IReadOnlyList<TaxWithholding>> FetchTaxWithholdingListAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// 源泉徴収情報を新規登録します。
    /// ステータスは未確定で登録されます。
    /// </summary>
    /// <remarks>
    /// <seealso href="https://developer.smarthr.jp/api/index.html#!/%E6%BA%90%E6%B3%89%E5%BE%B4%E5%8F%8E/postV1TaxWithholdings"/>
    /// </remarks>
    /// <param name="name">名前</param>
    /// <param name="year">源泉徴収票に印字される年</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns>登録処理後の<see cref="JobTitle"/>オブジェクト。</returns>
    public ValueTask<TaxWithholding> AddTaxWithholdingAsync(string name, string year, CancellationToken cancellationToken = default);
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
