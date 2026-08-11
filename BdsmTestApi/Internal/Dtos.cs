using System.Text.Json.Serialization;

namespace MCmoderSD.BdsmTestApi.Internal;

internal sealed class ResultDto
{
    [JsonPropertyName("version")]
    public int Version { get; init; }

    [JsonPropertyName("date")]
    public long Date { get; init; }

    [JsonPropertyName("gender")]
    public string? Gender { get; init; }

    [JsonPropertyName("ageGroup")]
    public int AgeGroup { get; init; }

    [JsonPropertyName("lang")]
    public string? Lang { get; init; }

    [JsonPropertyName("scores")]
    public ScoreDto[]? Scores { get; init; }
}

internal sealed class ScoreDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("pairdesc")]
    public string? PairDesc { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("score")]
    public int Score { get; init; }
}

internal sealed class MatchDto
{
    [JsonPropertyName("score")]
    public int Score { get; init; }
}

internal sealed class DocumentationDto
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("pairdesc")]
    public string? PairDesc { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}