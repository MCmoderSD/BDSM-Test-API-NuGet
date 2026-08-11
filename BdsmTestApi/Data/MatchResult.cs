namespace MCmoderSD.BdsmTestApi.Data;

public sealed record MatchResult(int Score, TestResult Result, TestResult Partner);