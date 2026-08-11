using MCmoderSD.BdsmTestApi.Core;

var api = new BdsmTestApi();

Console.Write("Enter your result id: ");
var resultId = (Console.ReadLine() ?? string.Empty).Trim();

Console.Write("Enter your partner's result id: ");
var partnerId = (Console.ReadLine() ?? string.Empty).Trim();

var match = await api.FetchMatchAsync(resultId, partnerId);
Console.WriteLine($"Your compatibility score: {match.Score}%");