using System.Text.Json.Serialization;

namespace MCmoderSD.BdsmTestApi.Internal;

[JsonSourceGenerationOptions(NumberHandling = JsonNumberHandling.AllowReadingFromString)]
[JsonSerializable(typeof(ResultDto))]
[JsonSerializable(typeof(MatchDto))]
[JsonSerializable(typeof(Dictionary<string, Dictionary<string, DocumentationDto>>), TypeInfoPropertyName = "DocumentationData")]
internal sealed partial class BdsmJsonContext : JsonSerializerContext;