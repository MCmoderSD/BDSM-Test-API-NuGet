using MCmoderSD.BdsmTestApi.Core;
using MCmoderSD.BdsmTestApi.Enums;

var api = new BdsmTestApi();
const Language language = Language.English;

Console.Write("Enter your result id: ");
var resultId = (Console.ReadLine() ?? string.Empty).Trim();

// ReSharper disable once RedundantArgumentDefaultValue
var result = await api.FetchResultAsync(resultId, language);

Console.WriteLine();
Console.WriteLine($"Result ID: {result.Id}");
Console.WriteLine($"Version:   {result.Version}");
Console.WriteLine($"Gender:    {result.Gender}");
Console.WriteLine($"Age Group: {result.AgeGroup} ({result.AgeGroup.GetMinAge()} to {result.AgeGroup.GetMaxAge()})");
Console.WriteLine($"Timestamp: {result.Timestamp:u}");
Console.WriteLine($"Language:  {result.Language} ({result.Language.GetCode()})");
Console.WriteLine();

foreach (var score in result.Scores)
{
    Console.WriteLine($"{score.Name}: {score.Value}%");
    Console.WriteLine($"- {score.PairDescription}");
    Console.WriteLine($"- {score.Description}");
    Console.WriteLine();
}

var top = result.Scores[0];
Console.WriteLine($"Top kink in German: {top.GetName(Language.German)}");