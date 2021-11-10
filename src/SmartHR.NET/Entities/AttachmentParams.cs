using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>添付ファイル情報</summary>
/// <param name="FileName">ファイル名</param>
/// <param name="Content">
/// 登録したいデータ
/// <para>
/// ファイル（エンコード前）の最大サイズは 10MB で、次の拡張子のファイルを受け付けます:
/// png, jpeg (jpg), pdf, text (txt), csv, conf, log, docx (doc), xlsx (xls), gif
/// </para>
/// </param>
public record AttachmentParams(
    [property: JsonPropertyName("file_name")] string? FileName,
    [property: JsonPropertyName("url")] IReadOnlyList<byte>? Content
);
