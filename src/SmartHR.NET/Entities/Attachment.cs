using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>添付ファイル情報</summary>
/// <param name="FileName">ファイル名</param>
/// <param name="Url">URL</param>
public record Attachment(
    [property: JsonPropertyName("file_name")] string? FileName,
    [property: JsonPropertyName("url")] string? Url
);
