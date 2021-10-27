using System;
using SmartHR.NET.Entities;

namespace SmartHR.NET.Services;

/// <summary>
/// <inheritdoc cref="CrewInputForm" path="/summary/text()"/>APIの取得オプション
/// </summary>
[Flags]
public enum CrewInputFormEmbed
{
    /// <summary>取得しない</summary>
    None,
    /// <summary><see cref="CrewInputForm.FieldGroup.CustomFieldTemplateGroup"/>を取得する</summary>
    CustomFieldTemplateGroup = 0b1,
    /// <summary><see cref="CrewInputForm.Field.CustomFieldTemplate"/>を取得する</summary>
    CustomFieldTemplate = 0b10,
    /// <summary><see cref="CrewInputForm.MailFormat"/>を取得する</summary>
    MailFormat = 0b100,
}
