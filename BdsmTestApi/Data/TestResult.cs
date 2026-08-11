using System.Collections.ObjectModel;

using MCmoderSD.BdsmTestApi.Enums;
using MCmoderSD.BdsmTestApi.Exceptions;
using MCmoderSD.BdsmTestApi.Internal;

namespace MCmoderSD.BdsmTestApi.Data;

public sealed class TestResult : IEquatable<TestResult>
{
    public string Id { get; }
    public int Version { get; }
    public DateTimeOffset Timestamp { get; }
    public string Gender { get; }
    public AgeGroup AgeGroup { get; }
    public Language Language { get; }
    public IReadOnlyList<Score> Scores { get; }
    public IReadOnlyDictionary<Kink, int> ScoreMap { get; }

    internal TestResult(string id, ResultDto result)
    {
        Id = id;
        Version = result.Version;
        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(result.Date);
        Gender = result.Gender ?? string.Empty;
        AgeGroup = AgeGroupExtensions.FromId(result.AgeGroup);
        Language = LanguageExtensions.FromCode(result.Lang ?? throw new BdsmTestApiException($"Result '{id}' is missing its language."));

        var raw = result.Scores ?? [];
        var scores = new List<Score>(raw.Length);
        var scoreMap = new Dictionary<Kink, int>(raw.Length);

        foreach (var score in raw)
        {
            var kink = KinkExtensions.FromId(score.Id);

            scores.Add(new Score(
                kink,
                score.Name ?? string.Empty,
                score.PairDesc ?? string.Empty,
                score.Description ?? string.Empty,
                score.Score));

            scoreMap[kink] = score.Score;
        }

        Scores = new ReadOnlyCollection<Score>(scores);
        ScoreMap = new ReadOnlyDictionary<Kink, int>(scoreMap);
    }

    public int GetScore(Kink kink) => ScoreMap.GetValueOrDefault(kink);

    public bool Equals(TestResult? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return Id == other.Id
               && Version == other.Version
               && Timestamp == other.Timestamp
               && Gender == other.Gender
               && AgeGroup == other.AgeGroup
               && Language == other.Language
               && Scores.SequenceEqual(other.Scores);
    }

    public override bool Equals(object? obj) => Equals(obj as TestResult);

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(Id);
        hash.Add(Version);
        hash.Add(Timestamp);
        hash.Add(Gender);
        hash.Add(AgeGroup);
        hash.Add(Language);
        foreach (var score in Scores) hash.Add(score);

        return hash.ToHashCode();
    }

    public override string ToString() => $"TestResult {{ Id = {Id}, Language = {Language}, Scores = {Scores.Count} }}";

    public static bool operator ==(TestResult? left, TestResult? right) => Equals(left, right);

    public static bool operator !=(TestResult? left, TestResult? right) => !Equals(left, right);
}