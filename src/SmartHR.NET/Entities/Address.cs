using System.Text.Json.Serialization;

namespace SmartHR.NET.Entities;

/// <summary>住所</summary>
/// <param name="Id">住所ID</param>
/// <param name="CountryNumber">国コード</param>
/// <param name="ZipCode">郵便番号</param>
/// <param name="Pref">都道府県</param>
/// <param name="City">市区町村</param>
/// <param name="Street">丁目・番地</param>
/// <param name="Building">建物名・部屋番号</param>
/// <param name="LiteralYomi">ヨミガナ</param>
public record Address(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("country_number")] string? CountryNumber,
    [property: JsonPropertyName("zip_code")] string? ZipCode,
    [property: JsonPropertyName("pref")] string? Pref,
    [property: JsonPropertyName("city")] string City,
    [property: JsonPropertyName("street")] string? Street = null,
    [property: JsonPropertyName("building")] string? Building = null,
    [property: JsonPropertyName("literal_yomi")] string? LiteralYomi = null
);
